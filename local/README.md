# Runtime local

Esta pasta concentra apenas recursos para executar a aplicação na máquina do desenvolvedor.

| Pasta | Uso |
| --- | --- |
| `docker/` | Docker Compose local com API e SQL Server. |
| `kubernetes/` | Manifests locais para Docker Desktop Kubernetes. |

## Convenção adotada

- `Dockerfile` permanece na raiz porque é usado pelo CI/CD e pelo build da imagem da API.
- `.dockerignore` permanece na raiz porque controla o contexto de build do `Dockerfile`.
- `local/docker/docker-compose.yml` fica separado porque é runtime local, não infraestrutura AWS.
- `local/kubernetes/` contém somente manifests locais com banco SQL Server em container.

## AWS não fica aqui

O deploy AWS real é descrito em Terraform:

- `infra/terraform/environments/dev/vpc.tf`
- `infra/terraform/environments/dev/ecr.tf`
- `infra/terraform/environments/dev/rds.tf`
- `infra/terraform/environments/dev/eks.tf`
- `infra/terraform/environments/dev/api-workload.tf`
