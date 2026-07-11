# Runtime local

Esta pasta concentra apenas recursos para executar a aplicação na máquina do desenvolvedor.

| Pasta | Uso |
| --- | --- |
| `docker/` | Docker Compose local com API e SQL Server. |

Os manifests Kubernetes ficam em [`/k8s`](../k8s/README.md), na raiz do repositório.

## Convenção adotada

- `Dockerfile` permanece na raiz porque é usado pelo CI/CD e pelo build da imagem da API.
- `.dockerignore` permanece na raiz porque controla o contexto de build do `Dockerfile`.
- `local/docker/docker-compose.yml` fica separado porque é runtime local, não infraestrutura AWS.
- `k8s/` contém os manifests Kubernetes (Deployment, Service, ConfigMap, Secret, HPA).

## AWS não fica aqui

O deploy AWS real é descrito em Terraform:

- `infra/terraform/environments/dev/vpc.tf`
- `infra/terraform/environments/dev/ecr.tf`
- `infra/terraform/environments/dev/rds.tf`
- `infra/terraform/environments/dev/eks.tf`
- `infra/terraform/environments/dev/api-workload.tf`
