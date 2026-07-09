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
7. Gera relatorio com `ReportGenerator`.
8. Publica o artifact `test-and-coverage-results`.

O artifact contem:

- arquivos `.trx` dos testes;
- `coverage.cobertura.xml`;
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

### `deploy-kubernetes`

Job manual e protegido por GitHub Environment.

Condicoes:

- roda apenas em `workflow_dispatch`;
- exige aprovacao no environment `local-kubernetes`;
- precisa do secret `KUBE_CONFIG`.

Depois da aprovacao:

1. configura o kubeconfig a partir do secret;
2. executa dry-run contra os manifests locais;
3. aplica namespace, banco SQL Server, API, service, HPA e ingress;
4. aguarda rollout de `sqlserver` e `oficina-api`.

## Secrets e variables esperados

### Obrigatorios para deploy manual

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

### Opcionais para registry externo

| Nome | Tipo | Uso |
| --- | --- | --- |
| `REGISTRY_USERNAME` | Repository secret | Usuario de registry externo. |
| `REGISTRY_TOKEN` | Repository secret | Token de registry externo. |

## Como trocar GHCR por ECR

O workflow usa GHCR por padrao para evitar custo e dependencia da AWS Academy. Para usar ECR na demonstracao:

1. provisionar o ECR pelo modulo `infra/modules/registry`;
2. executar `terraform output ecr_repository_url`;
3. trocar `IMAGE_NAME` no workflow pelo repository URL do ECR;
4. substituir o login GHCR por `aws-actions/configure-aws-credentials` e `aws-actions/amazon-ecr-login`;
5. garantir que a imagem enviada ao ECR seja a mesma usada nos manifests AWS.

Nao executar `terraform apply` apenas para testar CI/CD sem aprovacao explicita e sem plano de `terraform destroy`.

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
2. criar o environment `local-kubernetes`;
3. adicionar reviewer obrigatorio;
4. adicionar o secret `KUBE_CONFIG`;
5. executar o workflow via `workflow_dispatch`.

Sem aprovacao no environment, o deploy real nao e executado.
