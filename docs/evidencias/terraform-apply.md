# Evidencia: Terraform plan/apply/destroy

Este arquivo documenta o procedimento seguro para AWS Academy. Nao executar `terraform apply` manual ou via CD sem aprovacao explicita e sem janela reservada para `terraform destroy`.

## Guardrail obrigatorio

Ambiente AWS Academy possui credito limitado. Todo recurso criado deve ser destruido ao final da validacao.

## Pre-requisitos

Configurar credenciais temporarias fora do repositorio:

```powershell
aws configure --profile academy
```

Configurar variaveis sensiveis fora do Git:

```powershell
$env:AWS_PROFILE = "academy"
$env:AWS_REGION = "us-east-1"
$env:TF_VAR_db_password = "<senha-forte>"
$env:TF_VAR_eks_cluster_role_name = "<LabEksClusterRole-...>"
$env:TF_VAR_eks_node_role_name = "<LabEksNodeRole-...>"
```

## Validacao sem criar recursos

```powershell
terraform fmt -check -recursive infra
terraform -chdir=infra/terraform/environments/dev init -backend=false
terraform -chdir=infra/terraform/environments/dev validate
```

## Planejamento

```powershell
terraform -chdir=infra/terraform/environments/dev init
terraform -chdir=infra/terraform/environments/dev plan
```

## Aplicacao real

> Executar somente com aprovacao explicita. No CD, a aprovacao e o merge revisado para `develop`.

```powershell
terraform -chdir=infra/terraform/environments/dev apply
terraform -chdir=infra/terraform/environments/dev output
```

## Destroy obrigatorio

Manter disponíveis as mesmas variáveis usadas no apply, principalmente `TF_VAR_db_password`, `TF_VAR_eks_cluster_role_name` e `TF_VAR_eks_node_role_name`.

```powershell
terraform -chdir=infra/terraform/environments/dev destroy
```

## Conferencia pos-destroy

```powershell
aws eks list-clusters --region us-east-1
aws rds describe-db-instances --region us-east-1
aws elbv2 describe-load-balancers --region us-east-1
aws ec2 describe-nat-gateways --region us-east-1
aws ec2 describe-instances --region us-east-1
aws ecr describe-repositories --region us-east-1
```

## Resultado real

Execucao real via esteira `CD Development` (nao manual), em `infra/terraform/environments/dev`.

### Apply (recriacao do ambiente)

| Etapa | Resultado |
| --- | --- |
| `terraform init` | Sucesso |
| `terraform validate` | Sucesso |
| `terraform plan` | `Plan: 26 to add, 0 to change, 0 to destroy` |
| `terraform apply` | Sucesso — `Apply complete! Resources: 25 added, 0 changed, 0 destroyed.` (ECR criado antes, via apply direcionado em `module.ecr`, nao contabilizado de novo no apply principal) |
| `terraform output` | `eks_cluster_name = "oficina-mecanica-eks-dev"`, `rds_endpoint`, `ecr_repository_url`, `api_service_hostname` (Load Balancer) todos presentes |
| Rollout no EKS | Pod da API subiu, HPA e Service criados, endpoint do Load Balancer respondendo |

### Destroy (encerramento oficial da sessao)

PR de evidencia: [#159](https://github.com/geoscabio/oficina_mecanica_api/pull/159).

| Etapa | Resultado |
| --- | --- |
| `terraform plan -destroy` | `Plan: 0 to add, 0 to change, 26 to destroy` |
| `terraform destroy` (via `apply` do plano de destroy) | Sucesso — `Apply complete! Resources: 0 added, 0 changed, 26 destroyed.` |
| Conferencia pos-destroy (AWS Console) | EKS, RDS, VPC e ECR confirmados vazios de forma independente (prints abaixo) |

## Evidencia visual

### Destroy — log real do `terraform apply` (plano de destroy)

![Log do destroy: Apply complete, 26 destroyed](terraform-apply/destroy-log-apply-complete.png)

### Destroy — PR oficial mergeada

![PR #159 mergeada](terraform-apply/destroy-pr-159-merged.png)

### Destroy — resumo da esteira CD Development

![Resumo do CD Development](terraform-apply/destroy-cd-development-summary.png)

### Destroy — resumo do CI

![Resumo do CI](terraform-apply/destroy-ci-summary.png)

![Detalhe do build da imagem Docker](terraform-apply/destroy-ci-docker-build-detail.png)

### Destroy — conferencia independente no AWS Console

![EKS sem clusters](terraform-apply/destroy-aws-eks-empty.png)

![RDS sem bancos de dados](terraform-apply/destroy-aws-rds-empty.png)

![VPC sem recursos](terraform-apply/destroy-aws-vpc-empty.png)

![ECR sem repositorios](terraform-apply/destroy-aws-ecr-empty.png)
