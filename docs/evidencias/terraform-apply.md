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

PR de evidencia: [#162](https://github.com/geoscabio/oficina_mecanica_api/pull/162).

| Etapa | Resultado |
| --- | --- |
| `terraform init` | Sucesso |
| `terraform validate` | Sucesso |
| `terraform plan` | `Plan: 26 to add, 0 to change, 0 to destroy` |
| `terraform apply` | Sucesso — `Apply complete! Resources: 25 added, 0 changed, 0 destroyed.` (ECR criado antes, via apply direcionado em `module.ecr`, nao contabilizado de novo no apply principal) |
| `terraform output` | `eks_cluster_name = "oficina-mecanica-eks-dev"`, `rds_endpoint`, `ecr_repository_url`, `api_service_hostname` (Load Balancer) todos presentes |
| Rollout no EKS | Pod da API subiu (`1/1 Running`), HPA e Service criados, endpoint do Load Balancer respondendo |
| Conferencia funcional pos-deploy | `/api/health` respondeu `Healthy`; Swagger carregou; login retornou JWT valido; listagem de clientes retornou dados reais |

### Destroy (encerramento oficial da sessao)

PR de evidencia: [#159](https://github.com/geoscabio/oficina_mecanica_api/pull/159).

| Etapa | Resultado |
| --- | --- |
| `terraform plan -destroy` | `Plan: 0 to add, 0 to change, 26 to destroy` |
| `terraform destroy` (via `apply` do plano de destroy) | Sucesso — `Apply complete! Resources: 0 added, 0 changed, 26 destroyed.` |
| Conferencia pos-destroy (AWS Console) | EKS, RDS, VPC e ECR confirmados vazios de forma independente (prints abaixo) |

## Evidencia visual

### Apply — log real do `terraform apply`, com outputs

![Log do apply: Apply complete, 25 added, outputs de eks/rds/ecr/lb](terraform-apply/apply-log-apply-complete.png)

### Apply — PR oficial mergeada

![PR #162 mergeada](terraform-apply/apply-pr-162-merged.png)

### Apply — resumo da esteira CD Development

![Resumo do CD Development](terraform-apply/apply-cd-development-summary.png)

### Apply — resumo do CI

![Resumo do CI](terraform-apply/apply-ci-summary.png)

![Detalhe do build da imagem Docker](terraform-apply/apply-ci-docker-build-detail.png)

### Apply — rollout confirmado via kubectl (log real da esteira)

![kubectl get pods/svc/hpa mostrando o pod Running](terraform-apply/apply-rollout-kubectl-log.png)

### Apply — recursos criados no AWS Console

![VPC criada com 4 subnets](terraform-apply/apply-aws-vpc.png)

![Cluster EKS ativo com node group](terraform-apply/apply-aws-eks.png)

![RDS disponivel](terraform-apply/apply-aws-rds-1.png)

![RDS - grupos de seguranca e replicacao](terraform-apply/apply-aws-rds-2.png)

![Repositorio ECR com a imagem publicada](terraform-apply/apply-aws-ecr.png)

![NAT Gateway disponivel](terraform-apply/apply-aws-nat-gateway.png)

### Apply — validacao funcional da API publicada

![Healthcheck respondendo Healthy](terraform-apply/apply-api-healthcheck.png)

![Swagger UI carregado com todos os endpoints](terraform-apply/apply-swagger-ui.png)

![Login retornando JWT valido](terraform-apply/apply-swagger-login-success.png)

![Listagem de clientes retornando dados reais](terraform-apply/apply-swagger-list-clients-success.png)

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
