# GitHub Actions CI/CD

Este workflow fica em `.github/workflows/ci-cd.yml` e cobre build, testes, cobertura, imagem Docker, validacao de manifests Kubernetes e deploy manual protegido.

## Gatilhos

- `push` em `main`, `develop` e `feature/**`.
- `pull_request` com destino `develop`.
- `workflow_dispatch` para execucao manual, incluindo o job de deploy protegido.

## Jobs

### `build-test`

Executa a validacao principal do backend:

1. Checkout do codigo.
2. Setup do .NET SDK `10.0.x`.
3. `dotnet restore OficinaMecanica.sln`.
4. `dotnet build OficinaMecanica.sln --configuration Release --no-restore`.
5. `dotnet format OficinaMecanica.sln --verify-no-changes --no-restore`.
6. `dotnet test` com Coverlet collector (`XPlat Code Coverage`).
7. Valida que nenhum teste foi ignorado antes de consolidar cobertura.
8. Gera relatorio com `ReportGenerator`.
9. Falha o pipeline se a cobertura global de linhas ficar abaixo de `90%`.
10. Publica o artifact `test-and-coverage-results`.

O artifact contem:

- arquivos `.trx` dos testes;
- `coverage.cobertura.xml`;
- `coverage.opencover.xml`;
- relatorio HTML e resumo Markdown da cobertura.

### `docker-image`

Executa apos `build-test`:

1. Checkout do codigo.
2. Setup do Docker Buildx.
3. Gera tags com `docker/metadata-action`.
4. Faz login no GitHub Container Registry em eventos que nao sejam `pull_request`.
5. Build da imagem usando o `Dockerfile` da raiz.
6. Push para GHCR apenas em `push` ou `workflow_dispatch`.

Imagem padrao:

```text
ghcr.io/geoscabio/oficina_mecanica_api
```

### `kubernetes-dry-run`

Executa apos `docker-image` e nao aplica nada em cluster real:

```powershell
kubectl apply --dry-run=client -R -f k8s/
kubectl apply --dry-run=client -R -f infra/k8s/aws/
```

Para evitar dependencia de um cluster externo, o job cria um cluster KinD efemero no runner. Esse cluster existe apenas durante o workflow e serve para o `kubectl` descobrir os tipos de API durante o dry-run.

### `deploy-local-kubernetes`

Job manual para cluster Kubernetes local.

Condicoes:

- roda apenas em `workflow_dispatch`;
- exige selecionar `deployment_target=local-kubernetes`;
- exige aprovacao no environment `local-kubernetes`;
- precisa do secret `KUBE_CONFIG`.

Depois da aprovacao:

1. configura o kubeconfig a partir do secret;
2. executa dry-run contra os manifests locais;
3. aplica namespace, banco SQL Server, API, service, HPA e ingress;
4. aguarda rollout de `sqlserver` e `oficina-api`.

### `deploy-aws-academy`

Job manual para demonstracao real no AWS Academy.

Condicoes:

- roda apenas em `workflow_dispatch`;
- exige selecionar `deployment_target=aws-academy-deploy`;
- exige aprovacao no environment `aws-academy`;
- exige que a infraestrutura AWS ja exista via Terraform;
- exige secrets temporarios da sessao AWS Academy atual.

Depois da aprovacao:

1. autentica na AWS Academy;
2. faz login no ECR;
3. faz build da imagem Docker;
4. envia a imagem para o ECR criado pelo Terraform;
5. configura o kubeconfig do EKS;
6. cria ou atualiza o Secret da API no cluster;
7. aplica os manifests de `infra/k8s/aws/`;
8. aguarda rollout da API;
9. imprime o endpoint do Service `LoadBalancer`.

### `destroy-aws-academy-kubernetes`

Job manual para remover somente os recursos Kubernetes da demonstracao AWS.

Condicoes:

- roda apenas em `workflow_dispatch`;
- exige selecionar `deployment_target=aws-academy-destroy-k8s`;
- exige aprovacao no environment `aws-academy`;
- precisa das mesmas credenciais AWS temporarias usadas no deploy.

Esse job remove Service, Deployment, Secret, ConfigMap e Namespace. Ele ajuda a apagar o Load Balancer criado pelo Kubernetes, mas nao substitui `terraform destroy`.

## Secrets e variables esperados

### Obrigatorios para deploy local

| Nome | Tipo | Uso |
| --- | --- | --- |
| `KUBE_CONFIG` | Environment secret | Kubeconfig em Base64 do cluster alvo. |

Para gerar o valor em Base64 no PowerShell:

```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes("$HOME\.kube\config"))
```

### Usados automaticamente

| Nome | Tipo | Uso |
| --- | --- | --- |
| `GITHUB_TOKEN` | Secret automatico | Publicacao no GitHub Container Registry. |

### Obrigatorios para deploy AWS Academy

Environment `aws-academy`:

| Nome | Tipo | Uso |
| --- | --- | --- |
| `AWS_ACCESS_KEY_ID` | Environment secret | Access key temporaria da sessao AWS Academy. |
| `AWS_SECRET_ACCESS_KEY` | Environment secret | Secret key temporaria da sessao AWS Academy. |
| `AWS_SESSION_TOKEN` | Environment secret | Session token temporario da sessao AWS Academy. |
| `JWT_SECRET` | Environment secret | Chave JWT da API, com pelo menos 32 caracteres. |
| `WEBHOOK_TOKEN` | Environment secret | Token do webhook de orcamento, com pelo menos 32 caracteres. |
| `RDS_CONNECTION_STRING` | Environment secret | Connection string completa do RDS. |
| `ECR_REPOSITORY_URL` | Environment variable | Output `terraform output ecr_repository_url`. |
| `EKS_CLUSTER_NAME` | Environment variable | Output `terraform output eks_cluster_name`. |

Exemplo de `RDS_CONNECTION_STRING`:

```text
Server=<rds-address>,1433;Database=OficinaMecanicaDb;User Id=adminoficina;Password=<senha-rds>;TrustServerCertificate=True;
```

Os secrets da AWS Academy expiram quando a sessao do lab expira. Atualize os secrets do environment antes de rodar o deploy real.

### Opcionais para registry externo

| Nome | Tipo | Uso |
| --- | --- | --- |
| `REGISTRY_USERNAME` | Repository secret | Usuario de registry externo. |
| `REGISTRY_TOKEN` | Repository secret | Token de registry externo. |

## Como aprovar o deploy AWS Academy

O botao de aprovacao nao aparece em runs de `pull_request`. Em PR, o job de deploy fica cinza/skipped porque deploy real nao deve rodar em validacao de PR.

Para abrir a aprovacao:

1. Abra `Actions > CI/CD`.
2. Clique em `Run workflow`.
3. Escolha a branch do PR, por exemplo `feature/fase2-ci-cd`.
4. Em `deployment_target`, selecione `aws-academy-deploy`.
5. Clique em `Run workflow`.
6. Abra o run criado.
7. O job `Manual AWS Academy deploy` ficara aguardando aprovacao no environment `aws-academy`.
8. Clique em `Review deployments`.
9. Selecione `aws-academy`.
10. Clique em `Approve and deploy`.

Depois da aprovacao, o job faz build/push no ECR, aplica os manifests no EKS e imprime o endpoint da API.

Nao executar `terraform apply` apenas para testar CI/CD sem aprovacao explicita e sem plano de `terraform destroy`.

## Pre-requisitos antes do deploy AWS Academy

1. Iniciar a sessao do AWS Academy Learner Lab.
2. Configurar credenciais temporarias no environment `aws-academy`.
3. Rodar Terraform localmente com aprovacao explicita:

```powershell
terraform -chdir=infra/environments/dev init
terraform -chdir=infra/environments/dev plan
terraform -chdir=infra/environments/dev apply
```

4. Copiar os outputs para as variables/secrets do environment:

```powershell
terraform -chdir=infra/environments/dev output ecr_repository_url
terraform -chdir=infra/environments/dev output eks_cluster_name
terraform -chdir=infra/environments/dev output rds_address
```

5. Rodar o workflow manual com `deployment_target=aws-academy-deploy`.

## Cleanup obrigatorio AWS Academy

Depois da demonstracao:

1. Rode o workflow manual com `deployment_target=aws-academy-destroy-k8s`.
2. Aguarde a remocao do Service `LoadBalancer`.
3. Rode localmente:

```powershell
terraform -chdir=infra/environments/dev destroy
```

Nao deixar recursos ativos no AWS Academy apos a demonstracao.

## Execucao manual sem acesso ao cluster

Se o runner nao tiver kubeconfig do cluster de avaliacao:

1. rode normalmente o workflow em `push` ou `pull_request`;
2. use o artifact `test-and-coverage-results` como evidencia de build/testes/cobertura;
3. use o job `kubernetes-dry-run` como evidencia de validacao sintatica dos manifests;
4. execute localmente o deploy em Docker Desktop:

```powershell
docker compose up -d --build
kubectl apply -R -f k8s/
kubectl rollout status deployment/oficina-api -n oficina --timeout=180s
```

## Protecao do environment

No GitHub:

1. abrir `Settings > Environments`;
2. criar o environment `local-kubernetes`, se for usar deploy local remoto;
3. criar o environment `aws-academy`, se for usar deploy real na AWS Academy;
4. adicionar reviewer obrigatorio nos environments;
5. adicionar os secrets e variables descritos nesta documentacao;
6. executar o workflow via `workflow_dispatch`.

Sem aprovacao no environment, o deploy real nao e executado.
