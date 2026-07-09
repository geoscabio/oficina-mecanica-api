# Deploy na AWS

Este guia descreve como preparar a infraestrutura AWS real do environment `development` e como a esteira Git Flow executa deploy lógico para `homologation` e `production`.

## Regra de ouro

Ambientes temporários devem ser destruídos após a demonstração. Antes de qualquer criação de recurso, tenha um plano claro de cleanup Kubernetes e `terraform destroy`.

## Provisionamento da infraestrutura

- [ ] Configurar credenciais AWS fora do repositório.
- [ ] Configurar `TF_VAR_db_password` fora do repositório.
- [ ] Configurar `TF_VAR_eks_cluster_role_name` e `TF_VAR_eks_node_role_name` quando o ambiente exigir roles existentes.
- [ ] Executar `terraform plan`.
- [ ] Executar `terraform apply` somente com aprovação explícita.

```powershell
terraform -chdir=infra/terraform/environments/dev init
terraform -chdir=infra/terraform/environments/dev validate
terraform -chdir=infra/terraform/environments/dev plan
terraform -chdir=infra/terraform/environments/dev apply
```

## Outputs necessários

Após o `terraform apply`, copie estes outputs:

```powershell
terraform -chdir=infra/terraform/environments/dev output ecr_repository_url
terraform -chdir=infra/terraform/environments/dev output eks_cluster_name
terraform -chdir=infra/terraform/environments/dev output rds_address
```

Use os valores nos GitHub Environments correspondentes.

## GitHub Environments

Criar para o deploy real:

- `development`

O environment `development` precisa conter:

| Nome | Tipo | Uso |
| --- | --- | --- |
| `AWS_ACCESS_KEY_ID` | Secret | Access key do ambiente. |
| `AWS_SECRET_ACCESS_KEY` | Secret | Secret key do ambiente. |
| `AWS_SESSION_TOKEN` | Secret | Session token quando aplicável. |
| `JWT_SECRET` | Secret | Chave JWT da API. |
| `WEBHOOK_TOKEN` | Secret | Token do webhook de orçamento. |
| `RDS_CONNECTION_STRING` | Secret | Connection string completa do RDS. |
| `ECR_REPOSITORY_URL` | Variable | URL do ECR gerado pelo Terraform. |
| `EKS_CLUSTER_NAME` | Variable | Nome do EKS gerado pelo Terraform. |

Repository variables:

| Nome | Valor |
| --- | --- |
| `AWS_DEPLOY_ENABLED` | `true` apenas durante a janela de deploy/demonstração em `development`. |
| `AUTO_PR_ENABLED` | `true` apenas depois de habilitar o GitHub Actions a criar PRs. |
| `RELEASE_BRANCH` | Opcional. Default: `release`. |

## Deploy pela esteira

| Branch | Ambiente | Próximo passo automático |
| --- | --- | --- |
| `develop` | `development` | Faz deploy AWS real e abre PR para `release`. |
| `release` ou `release/**` | `homologation` | Registra deploy lógico e abre PR para `main`. |
| `main` | `production` | Registra deploy lógico final após PR aprovado. |

O deploy AWS real aplica os manifests de `infra/aws/k8s/`, aguarda rollout e imprime o endpoint do Load Balancer. Os estágios `homologation` e `production` não provisionam AWS enquanto não existirem ambientes físicos separados.

## Validação

- [ ] Validar rollout da API.
- [ ] Validar `kubectl get pods -n oficina`.
- [ ] Validar `kubectl get svc oficina-api -n oficina`.
- [ ] Validar `/api/health`.
- [ ] Validar Swagger.

## Encerramento obrigatório

1. Rodar `Actions > AWS Cleanup > Run workflow`.
2. Selecionar o `target_environment` correto.
3. Confirmar a execução.
4. Confirmar que o Service `LoadBalancer` foi removido.
5. Executar:

```powershell
terraform -chdir=infra/terraform/environments/dev destroy
```

6. Conferir que não restaram EKS, EC2, RDS, NAT Gateway, ECR com imagens ou Load Balancer ativos.
