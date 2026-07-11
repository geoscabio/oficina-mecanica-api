# Guardrails AWS Academy

## Regra obrigatoria

O ambiente AWS Academy possui orcamento limitado. Nenhum recurso AWS deve ser criado sem aprovacao explicita antes do comando ou merge que dispara CD, e todo `terraform apply` deve ter um `terraform destroy` planejado para o fim da sessao.

## Recursos com maior risco de custo

- EKS Cluster
- EC2/Managed Node Group
- NAT Gateway
- RDS SQL Server
- Load Balancer criado indiretamente por `kubernetes_service_v1.oficina_api` com `type = "LoadBalancer"` em `infra/terraform/environments/dev/api-service.tf`
- ECR com imagens armazenadas

## Antes de aplicar

1. Confirmar que o Learner Lab esta iniciado.
2. Configurar credenciais temporarias da AWS Academy.
3. Configurar a senha fora do repositorio:

```powershell
$env:TF_VAR_db_password = "<senha-forte>"
```

4. Configurar as roles EKS existentes do lab fora do repositorio:

```powershell
$env:TF_VAR_eks_cluster_role_name = "<LabEksClusterRole-...>"
$env:TF_VAR_eks_node_role_name = "<LabEksNodeRole-...>"
```

5. Rodar somente validacoes locais:

```powershell
terraform fmt -check -recursive infra
terraform -chdir=infra/terraform/environments/dev init -backend=false
terraform -chdir=infra/terraform/environments/dev validate
```

6. Para planejar/aplicar, inicializar com o mesmo backend S3 da esteira:

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

7. Conferir no plano se serao criados apenas os recursos esperados.

## Depois de testar

Destruir a infraestrutura e os recursos Kubernetes gerenciados pelo Terraform pela esteira:

1. Criar uma branch a partir da `develop`.
2. Alterar `infra/terraform/environments/dev/terraform-action.env` para `TERRAFORM_ACTION=destroy`.
3. Abrir PR para `develop`.
4. Fazer merge do PR.
5. Acompanhar o workflow `CD Development` ate a etapa `Terraform destroy`.
6. Depois do destroy, abrir outro PR voltando para `TERRAFORM_ACTION=apply`.

Conferir se nao restou recurso cobravel:

```powershell
aws eks list-clusters --region us-east-1
aws rds describe-db-instances --region us-east-1
aws elbv2 describe-load-balancers --region us-east-1
aws ec2 describe-nat-gateways --region us-east-1
```

## Observacao importante

O fim da sessao do Learner Lab pode encerrar instancias EC2, mas outros recursos como RDS, Load Balancer e NAT Gateway podem continuar existindo e consumindo credito. Por isso, o destroy via esteira e obrigatorio.
