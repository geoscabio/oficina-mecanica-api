# Deploy na AWS

Este guia descreve como preparar a infraestrutura AWS real do environment `development` e como a esteira Git Flow executa deploy lógico para `homologation` e `production`.

## Regra de ouro

Ambientes temporários devem ser destruídos após a demonstração. Antes de qualquer criação de recurso, tenha um plano claro de `terraform destroy`.

## Provisionamento da infraestrutura

O provisionamento do ambiente `development` acontece no workflow `CD Development`, após merge/push na branch `develop`.

O CD executa:

1. cria ou reutiliza o bucket S3 do Terraform state;
2. executa `terraform init`, `validate`, `plan` e `apply`;
3. cria VPC, ECR, RDS, EKS e recursos Kubernetes da API;
4. publica a imagem Docker no ECR;
5. reinicia o Deployment no EKS e valida o rollout.

Execução local equivalente para diagnóstico:

```powershell
terraform -chdir=infra/terraform/environments/dev init `
  -backend-config="bucket=<tf-state-bucket>" `
  -backend-config="key=oficina-mecanica/development/terraform.tfstate" `
  -backend-config="region=us-east-1" `
  -backend-config="encrypt=true" `
  -backend-config="use_lockfile=true" `
  -reconfigure

terraform -chdir=infra/terraform/environments/dev validate
terraform -chdir=infra/terraform/environments/dev plan
terraform -chdir=infra/terraform/environments/dev apply
```

## GitHub Environments

Criar para o deploy real:

- `development`

O environment `development` precisa conter:

| Nome | Tipo | Uso |
| --- | --- | --- |
| `AWS_ACCESS_KEY_ID` | Secret | Access key do ambiente. |
| `AWS_SECRET_ACCESS_KEY` | Secret | Secret key do ambiente. |
| `AWS_SESSION_TOKEN` | Secret | Session token quando aplicável. |
| `DB_PASSWORD` | Secret | Senha do usuário administrador do RDS. |
| `JWT_SECRET` | Secret | Chave JWT da API. |
| `WEBHOOK_TOKEN` | Secret | Token do webhook de orçamento. |

Repository variables:

| Nome | Valor |
| --- | --- |
| `AUTO_PR_ENABLED` | `true` para abrir PR automático após deploy: `develop -> release` e `release -> main`. |
| `RELEASE_BRANCH` | Opcional. Default: `release`. |
| `AWS_REGION` | Opcional. Default: `us-east-1`. |
| `TF_STATE_BUCKET` | Obrigatório para CD com Terraform. Deve apontar para um bucket S3 preexistente; a esteira não cria bucket automaticamente. |
| `TF_STATE_KEY` | Opcional. Default: `oficina-mecanica/development/terraform.tfstate`. |
| `EKS_CLUSTER_ROLE_NAME` | Opcional. Default: `LabRole`. |
| `EKS_NODE_ROLE_NAME` | Opcional. Default: `LabRole`. |

## Deploy pela esteira

| Branch | Ambiente | Próximo passo automático |
| --- | --- | --- |
| `develop` | `development` | Executa Terraform apply, faz deploy AWS real e abre PR para `release`. |
| `release` ou `release/**` | `homologation` | Registra deploy lógico e abre PR para `main`. |
| `main` | `production` | Registra deploy lógico final após PR aprovado. |

O deploy AWS real gerencia os recursos Kubernetes da API pelo Terraform, aguarda rollout e imprime o endpoint do Load Balancer. Os estágios `homologation` e `production` não provisionam AWS enquanto não existirem ambientes físicos separados.

## Validação

- [ ] Validar rollout da API.
- [ ] Validar `kubectl get pods -n oficina`.
- [ ] Validar `kubectl get svc oficina-api -n oficina`.
- [ ] Validar `/api/health`.
- [ ] Validar Swagger.

## Encerramento obrigatório

Ao final da demonstração, executar o destroy explícito do Terraform usando o mesmo backend/state da esteira:

Antes do comando, manter disponíveis as mesmas variáveis usadas no apply, principalmente `TF_VAR_db_password`, `TF_VAR_eks_cluster_role_name` e `TF_VAR_eks_node_role_name`.

```powershell
terraform -chdir=infra/terraform/environments/dev init `
  -backend-config="bucket=<tf-state-bucket>" `
  -backend-config="key=oficina-mecanica/development/terraform.tfstate" `
  -backend-config="region=us-east-1" `
  -backend-config="encrypt=true" `
  -backend-config="use_lockfile=true" `
  -reconfigure

terraform -chdir=infra/terraform/environments/dev destroy
```

Como os recursos Kubernetes da API também estão no state, o `terraform destroy` remove Service/Load Balancer, Deployment, Secret, ConfigMap, Namespace, EKS, RDS, ECR, NAT Gateway e VPC.

Por fim, conferir que não restaram EKS, EC2, RDS, NAT Gateway, ECR ou Load Balancer ativos. Se o bucket de state foi criado apenas para a demonstração, remover também após confirmar que o destroy terminou com sucesso.
