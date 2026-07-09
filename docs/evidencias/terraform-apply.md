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

## Planejamento com backend da esteira

```powershell
terraform -chdir=infra/terraform/environments/dev init `
  -backend-config="bucket=<tf-state-bucket>" `
  -backend-config="key=oficina-mecanica/development/terraform.tfstate" `
  -backend-config="region=us-east-1" `
  -backend-config="encrypt=true" `
  -backend-config="use_lockfile=true" `
  -reconfigure

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

> Colar aqui os outputs reais quando a execucao for feita.

| Etapa | Resultado |
| --- | --- |
| `terraform init` | Pendente de execucao real |
| `terraform validate` | Pendente de execucao real |
| `terraform plan` | Pendente de execucao real |
| `terraform apply` | Pendente de aprovacao |
| `terraform output` | Pendente de execucao real |
| `terraform destroy` | Pendente de execucao real |
| Conferencia pos-destroy | Pendente de execucao real |

## Evidencia visual

Adicionar prints abaixo:

```text
[INSERIR PRINT DO PLAN/APPLY/OUTPUT/DESTROY AQUI]
```
