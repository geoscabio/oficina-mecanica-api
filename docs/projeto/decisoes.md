# Decisões

## Kubernetes local

A pasta `k8s/` permanece dedicada exclusivamente à execução local do projeto.

Motivo:

A execução local utilizando Kubernetes é um dos entregáveis da Fase 2 do Tech Challenge.

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

Observado em ambiente real (AWS): o HPA escalava de 1 para múltiplas réplicas pouco depois do deploy, mesmo sem carga de usuário. Um pod recém-criado, sem nenhuma requisição recebida, já consumia ~77Mi e subia rapido para ~100-128Mi em poucos minutos - o "custo de largada" normal de uma aplicação .NET/ASP.NET Core (JIT, Entity Framework, geração do Swagger, pipeline de middlewares) somado ao valor de `requests.memory=128Mi`, que era pequeno demais para esse baseline.

Como o HPA calcula a porcentagem de uso em cima do `requests` (nao do `limits`), o baseline sozinho ja chegava perto/no target de 80%, fazendo o autoscaler reagir a "ruido" de inicializacao em vez de carga real. Ajustado `requests.memory` para `256Mi` em `infra/terraform/environments/dev/api-deployment.tf` e `k8s/api-deployment.yaml` (mantendo `limits.memory=512Mi`), baseado nos valores reais medidos, dando margem confortavel (baseline ~128Mi vira ~50% de 256Mi) para o HPA so escalar quando houver carga de verdade.

Motivo:

Fazer o HPA refletir carga real, nao o custo de inicializacao do runtime .NET. `requests.cpu` nao foi alterado porque o uso de CPU observado (1-6m de 100m) estava bem abaixo do target de 70%, sem indicio de problema.
