# Prompt para atualizar o contexto no ChatGPT

Use este prompt para continuar o trabalho do Tech Challenge Fase 2 no ChatGPT, sem perder o contexto do que foi feito via Codex.

---

Voce esta ajudando no projeto `oficina_mecanica_api`, uma API .NET com arquitetura em camadas para oficina mecanica. O objetivo e deixar a entrega da Fase 2 do Tech Challenge pronta ate 14/07/2026, cobrindo codigo, infraestrutura, deploy, documentacao, diagramas de arquitetura e roteiro de demonstracao.

## Regras obrigatorias

1. O ambiente AWS usado e AWS Academy Learner Lab, com credito muito limitado.
2. Nunca criar recursos AWS sem aprovacao explicita.
3. Todo `terraform apply` precisa ter `terraform destroy` planejado e executado ao fim do teste.
4. Nunca versionar credenciais, tokens, senhas ou secrets reais.
5. Nao colar nem repetir credenciais AWS em respostas, commits, PRs, docs ou arquivos.
6. Branches devem seguir o padrao `feature/...`.
7. PRs devem ser abertos contra `develop`.

## Estado atual do trabalho

Branch local usada:

```text
feature/pacote-correcoes-infra-codigo
```

Objetivo do pacote:

- consolidar correcoes de codigo e infraestrutura apos o merge do PR 110 em `develop`;
- preparar o projeto para execucao local com Docker e Kubernetes;
- reduzir riscos de custo na AWS Academy;
- deixar documentado o fluxo seguro de deploy e destroy;
- corrigir problemas potenciais de concorrencia/persistencia;
- entregar um PR revisavel contra `develop`.

## O que foi implementado

### Healthcheck e Kubernetes

- O endpoint `/api/health` ja existe e foi adotado como healthcheck operacional.
- Probes de `startup`, `readiness` e `liveness` foram ajustadas para `/api/health`.
- Manifestos locais em `k8s/` usam imagem local `oficina_mecanica_api-api:latest` com `imagePullPolicy: IfNotPresent`, facilitando testes no `docker-desktop`.
- Manifestos AWS em `infra/k8s/aws/` tambem usam `/api/health`.
- O HPA local foi validado com metrics server ativo.

### AWS Academy e Terraform

- Criado `docs/deploy/aws-academy-guardrails.md` com regras obrigatorias de custo e destroy.
- Atualizado `docs/deploy/deploy-aws.md` com checklist seguro de provisionamento, deploy e encerramento.
- Removida senha hardcoded de `infra/environments/dev/terraform.tfvars`.
- `db_password` deve ser informado por `TF_VAR_db_password`.
- Roles EKS hardcoded foram removidas do ambiente dev.
- Agora as roles devem ser informadas por:

```powershell
$env:TF_VAR_eks_cluster_role_name = "<LabEksClusterRole-...>"
$env:TF_VAR_eks_node_role_name = "<LabEksNodeRole-...>"
```

- Foi documentado que roles EKS podem mudar entre sessoes/labs da AWS Academy.
- O node group do ambiente dev foi limitado para `desired_size = 1`, `min_size = 1`, `max_size = 1`.
- RDS ficou mais seguro para ambiente efemero:
  - `backup_retention_period = 0`;
  - `deletion_protection = false`;
  - `skip_final_snapshot = true`.
- Outputs Terraform relevantes foram adicionados:
  - ECR repository URL;
  - EKS cluster name;
  - EKS cluster endpoint;
  - RDS endpoint/address.
- Importante: nenhum recurso AWS foi criado durante esse pacote. Foram feitas apenas validacoes e consultas read-only.

### Codigo da API

- Swagger deixou de ser assumido como endpoint operacional.
- Log de startup foi ajustado para nao prometer Swagger sempre ativo.
- Middleware de pipeline agora:
  - habilita Swagger em `Development`, `Testing` ou quando `Swagger:Enabled=true`;
  - redireciona `/` para `/swagger` quando Swagger esta ativo;
  - redireciona `/` para `/api/health` quando Swagger nao esta ativo.
- `DatabaseInitializer` deixou de ser default-on:
  - migrations/seed so rodam se `Database:InitializeOnStartup=true`;
  - isso reduz risco de producao rodar migrations/seed acidentalmente.
- Controller de webhook de pagamento ficou mais simples, sem null-forgiving desnecessario.

### Concorrencia e persistencia

- Adicionado tipo de erro `Conflito`.
- `TipoErro.Conflito` agora mapeia para HTTP `409 Conflict`.
- Middleware global captura:
  - `DbUpdateConcurrencyException` como `409 Conflict`;
  - violacoes SQL Server de unique constraint (`2601`, `2627`) como `409 Conflict`;
  - `DomainException` segue como `422 Unprocessable Entity`;
  - excecoes inesperadas seguem como `500 Internal Server Error`.
- Adicionados tokens de concorrencia `RowVersion` em:
  - `GestaoEstoque.ItensEstoque`;
  - `GestaoOrdemServico.OrdensServico`.
- Criada migration:
  - `20260709060823_AddConcurrencyTokens`.

### Testes

- Teste novo para `DbUpdateConcurrencyException` retornando `409 Conflict`.
- Teste atualizado para `TipoErro.Conflito` no mapeamento de status code.
- Suite completa passou localmente.

### Documentacao de projeto

- `README.md` foi atualizado para refletir `/api/health` e o nome correto do HPA local.
- `docs/projeto/backlog.md`, `docs/projeto/decisoes.md` e `docs/projeto/pendencias.md` foram atualizados com decisoes e pendencias atuais.
- `infra/k8s/aws/secrets/README.md` documenta como criar secrets manualmente sem versionar valores reais.
- `infra/modules/kubernetes/README.md` documenta premissas do modulo EKS e variaveis de roles.

## Validacoes executadas

### .NET

```powershell
dotnet build --no-restore
dotnet test --no-build
dotnet format --verify-no-changes --no-restore
```

Resultado:

- build OK;
- 0 warnings;
- 424 testes aprovados;
- formatacao OK.

### Entity Framework

```powershell
dotnet ef migrations has-pending-model-changes `
  --project src/OficinaMecanica.Infrastructure/OficinaMecanica.Infrastructure.csproj `
  --startup-project src/OficinaMecanica.API/OficinaMecanica.API.csproj `
  --no-build
```

Resultado:

- sem mudancas pendentes no modelo apos a migration criada.

### Terraform

```powershell
terraform fmt -check -recursive infra
terraform -chdir=infra/environments/dev validate
terraform -chdir=infra/environments/dev plan -refresh=false -input=false -detailed-exitcode
```

Resultado:

- formatacao OK;
- validate OK;
- plan OK com as roles EKS atuais informadas por variavel;
- nenhum recurso criado.

### Kubernetes

```powershell
kubectl apply --dry-run=client -R -f k8s
kubectl apply --dry-run=client -R -f infra/k8s/aws
kubectl apply -R -f k8s
kubectl rollout status deployment/sqlserver -n oficina
kubectl rollout status deployment/oficina-api -n oficina
kubectl get pods,svc,hpa -n oficina
```

Resultado:

- dry-run local OK;
- dry-run AWS manifests OK;
- rollout local no `docker-desktop` OK;
- pods rodando;
- HPA com metricas.

Healthcheck no Kubernetes local:

```powershell
kubectl port-forward --address 127.0.0.1 service/oficina-api 5095:8080 -n oficina
curl http://127.0.0.1:5095/api/health
```

Resultado:

```text
Healthy
```

### Docker Compose

```powershell
docker compose up -d --build
docker compose ps
curl http://localhost:5093/api/health
docker compose logs --tail 80 api
```

Resultado:

- API e SQL Server subiram;
- endpoint `/api/health` retornou `200 Healthy`;
- logs sem erro critico.

### AWS Academy read-only

Foram feitas consultas read-only para verificar recursos cobraveis:

```powershell
aws eks list-clusters
aws rds describe-db-instances
aws elbv2 describe-load-balancers
aws ec2 describe-nat-gateways
aws ec2 describe-instances
aws ecr describe-repositories
```

Resultado:

- nao havia EKS, RDS, Load Balancer, NAT Gateway, EC2 ou ECR ativos.

## Pontos de atencao

1. O Terraform ainda esta desenhado para criar recursos caros para AWS Academy:
   - EKS;
   - node group EC2;
   - RDS SQL Server;
   - Load Balancer quando aplicar manifests AWS.
2. O PR melhora a seguranca operacional, mas nao elimina custo se alguem executar `apply`.
3. Antes de qualquer `terraform apply`, revisar o plano e combinar horario para destruir tudo depois.
4. As roles EKS da AWS Academy mudam entre sessoes; nunca deixar nome fixo no codigo.
5. `RowVersion` melhora concorrencia, mas fluxos que geram numero sequencial por consulta ainda podem precisar de uma estrategia mais robusta no futuro, como sequence de banco ou indice unico com retry controlado.
6. O ambiente local Docker/Kubernetes ficou pronto para demonstracao, mas a revisao final amanha deve passar por toda a jornada da aplicacao.

## Proximos passos recomendados

### Para revisar este PR

1. Conferir se todos os arquivos alterados pertencem ao pacote.
2. Revisar especialmente:
   - `docs/deploy/aws-academy-guardrails.md`;
   - `docs/deploy/deploy-aws.md`;
   - `infra/environments/dev/*.tf`;
   - `infra/modules/database/*.tf`;
   - `infra/modules/kubernetes/*.tf`;
   - `src/OficinaMecanica.API/Middlewares/GlobalExceptionMiddleware.cs`;
   - migration `20260709060823_AddConcurrencyTokens`.
3. Rodar novamente:

```powershell
dotnet build --no-restore
dotnet test --no-build
terraform fmt -check -recursive infra
terraform -chdir=infra/environments/dev validate
kubectl apply --dry-run=client -R -f k8s
kubectl apply --dry-run=client -R -f infra/k8s/aws
```

### Para AWS Academy

1. Nao executar `terraform apply` automaticamente.
2. Iniciar lab e configurar credenciais temporarias.
3. Listar roles EKS disponiveis:

```powershell
aws iam list-roles --profile academy --query "Roles[?contains(RoleName, 'LabEks')].[RoleName]" --output table
```

4. Exportar variaveis:

```powershell
$env:TF_VAR_db_password = "<senha-forte>"
$env:TF_VAR_eks_cluster_role_name = "<LabEksClusterRole-...>"
$env:TF_VAR_eks_node_role_name = "<LabEksNodeRole-...>"
```

5. Rodar `terraform plan`.
6. So aplicar se houver tempo e compromisso de destruir no mesmo bloco de trabalho.
7. Apos teste:

```powershell
kubectl delete -f infra/k8s/aws/
terraform -chdir=infra/environments/dev destroy
```

8. Confirmar que nao restaram recursos cobraveis.

### Para entrega final do Tech Challenge

1. Fechar revisao do PR e merge em `develop`.
2. Rodar teste ponta a ponta da API:
   - cliente;
   - veiculo;
   - ordem de servico;
   - orcamento;
   - pagamento/webhook;
   - estoque;
   - relatorios, se aplicavel.
3. Gerar diagramas finais:
   - C4 Context;
   - C4 Container;
   - C4 Component;
   - Deployment local;
   - Deployment AWS;
   - ERD/banco;
   - fluxo de ordem de servico;
   - fluxo de pagamento.
4. Preparar roteiro do video:
   - problema e objetivo;
   - arquitetura;
   - API e Swagger;
   - Docker local;
   - Kubernetes local;
   - estrategia AWS Academy segura;
   - testes automatizados;
   - pontos de evolucao.
5. Garantir que README e docs finais batem com o que sera demonstrado.

## Pedido para o ChatGPT continuar

Com base nesse contexto, revise o PR e o estado do projeto buscando problemas que possam comprometer a entrega da Fase 2. Priorize:

1. riscos de custo AWS Academy;
2. bugs graves em runtime;
3. inconsistencias entre docs, Docker, Kubernetes e Terraform;
4. gaps de teste relevantes;
5. melhorias que aumentem clareza da entrega sem aumentar complexidade.

Nao sugira criar recursos AWS sem plano de destroy. Nao use credenciais reais em nenhum exemplo.
