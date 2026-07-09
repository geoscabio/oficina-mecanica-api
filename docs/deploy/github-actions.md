# GitHub Actions CI/CD

O workflow principal fica em `.github/workflows/ci-cd.yml` e implementa um Git Flow automatizado com validação contínua, entrega progressiva e promoção por PR.

## Fluxo

```text
feature/*, bugfix/*, hotfix/* ...
  -> CI
  -> PR automático para develop
  -> merge manual/revisado
  -> deploy development
  -> PR automático para release
  -> merge manual/revisado
  -> deploy homologation
  -> PR automático para main
  -> aprovação obrigatória
  -> deploy production
```

## Gatilhos

| Evento | Resultado |
| --- | --- |
| `push` em branches de trabalho | Valida build/testes/cobertura, build da imagem Docker, manifests Kubernetes e abre PR para `develop`. |
| `pull_request` para `develop`, `release` ou `main` | Validação completa antes do merge. |
| `push` em `develop` | Validação completa, deploy em `development` quando habilitado e PR automático para `release`. |
| `push` em `release` ou `release/**` | Validação completa, deploy em `homologation` quando habilitado e PR automático para `main`. |
| `push` em `main` | Validação completa e deploy em `production` protegido por regras do repositório/environment. |
| `workflow_dispatch` | Execução complementar de CI ou cleanup Kubernetes. |

Branches de trabalho cobertas:

- `feature/**`
- `bugfix/**`
- `hotfix/**`
- `fix/**`
- `refactor/**`
- `chore/**`
- `docs/**`
- `test/**`
- `ci/**`

## Jobs

### `build-test`

Executa:

1. `dotnet restore OficinaMecanica.sln`
2. `dotnet build OficinaMecanica.sln --configuration Release --no-restore`
3. `dotnet format OficinaMecanica.sln --verify-no-changes --no-restore`
4. `dotnet test` com Coverlet collector
5. validação de zero testes ignorados
6. relatório com `ReportGenerator`
7. bloqueio se cobertura global de linhas ficar abaixo de `90%`
8. publicação do artifact `test-and-coverage-results`

### `docker-image`

Constrói a imagem a partir do `Dockerfile` da raiz. Em branches de trabalho e PRs, apenas valida o build da imagem; em `develop`, `release`/`release/**` e `main`, também publica no GitHub Container Registry.

Imagem padrão:

```text
ghcr.io/geoscabio/oficina_mecanica_api
```

### `kubernetes-dry-run`

Valida manifests locais e AWS sem aplicar em cluster real:

```powershell
kubectl apply --dry-run=client -R -f k8s/
kubectl apply --dry-run=client -R -f infra/aws/k8s/
```

O job cria um KinD efêmero apenas para validar os manifests no runner.

### `open-pr-to-develop`

Depois que uma branch de trabalho passa no CI, abre automaticamente um PR para `develop`, sem fazer merge automático.

### `deploy-aws`

Executa deploy da aplicação no EKS conforme a branch:

| Branch | Environment |
| --- | --- |
| `develop` | `development` |
| `release` ou `release/**` | `homologation` |
| `main` | `production` |

Esse job só roda quando a variável do repositório `AWS_DEPLOY_ENABLED=true` estiver configurada. Isso evita deploy acidental enquanto o ambiente AWS não estiver preparado.

O deploy:

1. autentica na AWS;
2. faz login no ECR;
3. faz build/push da imagem Docker;
4. configura kubeconfig do EKS;
5. cria/atualiza o Secret da API;
6. aplica manifests em `infra/aws/k8s/`;
7. aguarda rollout;
8. imprime o endpoint do Service `LoadBalancer`.

### `open-pr-to-release`

Depois do deploy bem-sucedido da `develop`, abre PR automático de `develop` para a branch definida em `RELEASE_BRANCH`, ou `release` por padrão.

Se a branch `release` ainda não existir, o workflow cria a branch a partir da `main` antes de abrir o PR.

### `open-pr-to-main`

Depois do deploy bem-sucedido em homologação, abre PR automático de `release` para `main`.

Esse PR deve ser protegido por branch protection com aprovação obrigatória antes do merge.

### `cleanup-aws-kubernetes`

Job manual via `workflow_dispatch` para remover recursos Kubernetes do ambiente escolhido.

Inputs:

| Input | Valor |
| --- | --- |
| `operation` | `cleanup-kubernetes` |
| `target_environment` | `development`, `homologation` ou `production` |

Esse cleanup remove Service, Deployment, Secret, ConfigMap e Namespace. Ele não substitui `terraform destroy`.

## Secrets e variables

### Repository variables

| Nome | Tipo | Uso |
| --- | --- | --- |
| `AWS_DEPLOY_ENABLED` | Repository variable | Habilita deploy automático quando `true`. |
| `RELEASE_BRANCH` | Repository variable opcional | Nome da branch de release. Default: `release`. |

### Environments

Criar os environments:

- `development`
- `homologation`
- `production`

Cada environment precisa conter:

| Nome | Tipo | Uso |
| --- | --- | --- |
| `AWS_ACCESS_KEY_ID` | Secret | Access key temporária ou credencial do ambiente. |
| `AWS_SECRET_ACCESS_KEY` | Secret | Secret key temporária ou credencial do ambiente. |
| `AWS_SESSION_TOKEN` | Secret | Session token quando aplicável. |
| `JWT_SECRET` | Secret | Chave JWT da API, com pelo menos 32 caracteres. |
| `WEBHOOK_TOKEN` | Secret | Token do webhook de orçamento, com pelo menos 32 caracteres. |
| `RDS_CONNECTION_STRING` | Secret | Connection string completa do banco. |
| `ECR_REPOSITORY_URL` | Variable | Output `terraform output ecr_repository_url`. |
| `EKS_CLUSTER_NAME` | Variable | Output `terraform output eks_cluster_name`. |

Exemplo de `RDS_CONNECTION_STRING`:

```text
Server=<rds-address>,1433;Database=OficinaMecanicaDb;User Id=adminoficina;Password=<senha-rds>;TrustServerCertificate=True;
```

## Proteções recomendadas

Para a entrega final:

1. proteger `develop`, `release` e `main`;
2. exigir status check do workflow `CI/CD`;
3. exigir pelo menos um reviewer em PR para `main`;
4. configurar reviewer obrigatório no environment `production`;
5. manter `AWS_DEPLOY_ENABLED=false` quando não houver janela de demonstração;
6. após qualquer demonstração em AWS temporária, executar `cleanup-kubernetes` e `terraform destroy`.
