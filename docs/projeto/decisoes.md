# Decisões

## Kubernetes local

A pasta `k8s/` permanece dedicada exclusivamente à execução local do projeto.

Motivo:

A execução local utilizando Kubernetes é um dos entregáveis da Fase 2 do Tech Challenge.

---

## Achados aceitos do SonarQube

A análise de SonarQube (ver [`docs/evidencias/sonarqube.md`](../evidencias/sonarqube.md)) terminou com 0 issues abertas e nota A em todas as categorias. Quatro achados foram resolvidos via `Won't Fix`/`Safe` (com justificativa registrada na própria issue do SonarQube), por serem intencionais e não representarem risco real:

- `k8s/api-secret.yaml`: contém uma senha de banco e segredos de demonstração em texto plano, claramente identificados como `-local-2026` no valor. Aceito porque a pasta `k8s/` é só para execução local (ver seção acima); o ambiente AWS usa segredos via Terraform/GitHub Secrets, nunca commitados.
- `k8s/api-configmap.yaml`: usa `ASPNETCORE_URLS=http://+:8080` (protocolo em texto plano). Aceito porque a terminação TLS acontece fora do container — no Load Balancer da AWS em produção, e não é necessária na máquina do próprio desenvolvedor em execução local.
- `k8s/api-deployment.yaml`: usa `image: oficina_mecanica_api-api:latest`, tag não fixada em uma versão especifica. Aceito porque a imagem é construída localmente pelo próprio desenvolvedor (nunca publicada em um registry), sem um esquema de versionamento semântico no projeto — fixar uma versão arbitrária e nunca atualizada seria mais enganoso do que usar `latest` de forma explícita e consciente.
- `tests/OficinaMecanica.API.IntegrationTests/GestaoOrdemServico/Builders/OrdemServicoRequestBuilder.cs`: o método `BuildNotificacaoOrcamento` não acessa dados de instância e poderia ser `static`, mas foi mantido como método de instância de propósito, para preservar a consistência do padrão Builder fluente (`Novo().Build...()`) usado pelos demais métodos `Build` da classe.

Motivo (para os três achados em `k8s/`):

Manter a simplicidade de um único `kubectl apply -R -f k8s/` para o ambiente local, sem introduzir gerenciamento externo de segredos (ex.: `kubectl create secret` fora do versionamento) que os requisitos da Fase 2 não exigem para este cenário.

---

## Deploy AWS

O deploy na AWS utiliza recursos específicos gerenciados pelo Terraform em `infra/terraform/environments/dev/`. Não há manifests YAML AWS duplicados no repositório: o workload Kubernetes da API na AWS fica em arquivos Terraform separados por responsabilidade (`namespace.tf`, `api-configmap.tf`, `api-secret.tf`, `api-deployment.tf`, `api-service.tf`, `api-hpa.tf`), no mesmo padrão adotado em `k8s/`.

Motivo:

Evitar impacto na execução local.

---

## AWS temporária

Todo provisionamento AWS deve considerar o orçamento limitado do Learner Lab.

Motivo:

Evitar consumo indevido de créditos. Nenhum `terraform apply` manual ou via CD deve ocorrer sem aprovação explícita; no Git Flow, essa aprovação é o merge revisado para `develop`, sempre com plano de `terraform destroy` ao final da sessão.

---

## Trade-offs AWS Academy

O ambiente AWS foi desenhado para demonstração acadêmica e para caber nas limitações do AWS Academy Learner Lab. Por isso, algumas escolhas reduzem custo e risco operacional, mas não representam um desenho produtivo completo:

- O node group do EKS permanece com 1 nó para reduzir consumo de EC2.
- O RDS roda sem Multi-AZ, sem retenção de backup e com `skip_final_snapshot=true`.
- A rede utiliza um único NAT Gateway para equilibrar custo e simplicidade.
- As migrations e o seed inicial continuam no startup da API para simplificar a entrega.

Motivo:

Essas decisões deixam a solução executável no laboratório, mantêm o desenho rastreável por Terraform e evitam recursos permanentes caros. Em produção real, o baseline deveria evoluir para alta disponibilidade, backups, snapshots finais, rotação de segredos e execução controlada de migrations fora do ciclo de inicialização da API.

---

## Ambiente AWS de demonstração

O ambiente publicado na AWS roda com `ASPNETCORE_ENVIRONMENT=Staging`. Swagger e usuários demo são habilitados explicitamente via `appsettings.Staging.json` para permitir a avaliação do Tech Challenge.

Motivo:

Evitar que a exposição de Swagger e credenciais demo pareça vazamento acidental de `Development`. Este padrão é aceitável apenas para demonstração acadêmica e não deve ser replicado em produção real.

---

## Ajuste de `requests.memory` da API (128Mi -> 256Mi)

Observado em ambiente real (AWS): o HPA escalava de 1 para múltiplas réplicas pouco depois do deploy, mesmo sem carga de usuário. Um pod recém-criado, sem nenhuma requisição recebida, já consumia ~77Mi e subia rápido para ~100-128Mi em poucos minutos - o "custo de largada" normal de uma aplicação .NET/ASP.NET Core (JIT, Entity Framework, geração do Swagger, pipeline de middlewares) somado ao valor de `requests.memory=128Mi`, que era pequeno demais para esse baseline.

Como o HPA calcula a porcentagem de uso em cima do `requests` (não do `limits`), o baseline sozinho já chegava perto/no target de 80%, fazendo o autoscaler reagir a "ruído" de inicialização em vez de carga real. Ajustado `requests.memory` para `256Mi` em `infra/terraform/environments/dev/api-deployment.tf` e `k8s/api-deployment.yaml` (mantendo `limits.memory=512Mi`), baseado nos valores reais medidos, dando margem confortável (baseline ~128Mi vira ~50% de 256Mi) para o HPA só escalar quando houver carga de verdade.

Motivo:

Fazer o HPA refletir carga real, não o custo de inicialização do runtime .NET. `requests.cpu` não foi alterado porque o uso de CPU observado (1-6m de 100m) estava bem abaixo do target de 70%, sem indício de problema.

---

## Workstation GC em vez de Server GC na API

Além do ajuste de `requests.memory` acima, identificado que a API não configurava explicitamente o modo do Garbage Collector do .NET, usando o padrão (Server GC). O Server GC cria um heap de memória separado por núcleo de CPU visível ao processo, otimizado para aplicações de alto throughput com vários núcleos - mas o container roda com `limits.cpu=500m` (meio núcleo), um cenário onde o Server GC reserva memória para paralelismo que o container nem tem disponível.

Adicionado `<ServerGarbageCollection>false</ServerGarbageCollection>` em `src/OficinaMecanica.API/OficinaMecanica.API.csproj`, ativando o Workstation GC. Confirmado localmente que a opção é aplicada de verdade no artefato publicado (`"System.GC.Server": false` no `.runtimeconfig.json` gerado pelo build).

Motivo:

Reduzir o consumo real de memória da aplicação (não só aumentar a margem do `requests.memory`), alinhando o modo de GC ao perfil real do container: baixo paralelismo de CPU, baixo tráfego, típico de um ambiente de demonstração acadêmica.
