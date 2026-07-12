# 🛡️ Guardrails AWS Academy

## ⚠️ Regra obrigatória

O ambiente AWS Academy possui orçamento limitado. Nenhum recurso AWS deve ser criado sem aprovação explícita antes do comando ou merge que dispara CD, e todo `terraform apply` deve ter um `terraform destroy` planejado para o fim da sessão.

## 💰 Recursos com maior risco de custo

- EKS Cluster
- EC2/Managed Node Group
- NAT Gateway
- RDS SQL Server
- Load Balancer criado indiretamente por `kubernetes_service_v1.oficina_mecanica_api` com `type = "LoadBalancer"` em `infra/terraform/environments/dev/api-service.tf`
- ECR com imagens armazenadas

## ✅ Antes de aplicar

1. Confirmar que o Learner Lab está iniciado.
2. Configurar credenciais temporárias da AWS Academy.
3. Configurar a senha fora do repositório:

```powershell
$env:TF_VAR_db_password = "<senha-forte>"
```

4. Configurar as roles EKS existentes do lab fora do repositório:

```powershell
$env:TF_VAR_eks_cluster_role_name = "<LabEksClusterRole-...>"
$env:TF_VAR_eks_node_role_name = "<LabEksNodeRole-...>"
```

5. Rodar somente validações locais:

```powershell
terraform fmt -check -recursive infra
terraform -chdir=infra/terraform/environments/dev init -backend=false
terraform -chdir=infra/terraform/environments/dev validate
```

6. Para planejar/aplicar, inicializar normalmente (backend `local`, mesmo state file `terraform.tfstate` usado pela esteira via cache do GitHub Actions):

```powershell
terraform -chdir=infra/terraform/environments/dev init
terraform -chdir=infra/terraform/environments/dev plan
```

7. Conferir no plano se serão criados apenas os recursos esperados.

## 🧹 Depois de testar

Destruir a infraestrutura e os recursos Kubernetes gerenciados pelo Terraform pela esteira:

1. Criar uma branch a partir da `develop`.
2. Alterar `infra/terraform/environments/dev/terraform-action.env` para `TERRAFORM_ACTION=destroy`.
3. Abrir PR para `develop`.
4. Fazer merge do PR.
5. Acompanhar o workflow `CD Development` até a etapa `Terraform destroy`.
6. Depois do destroy, abrir outro PR voltando para `TERRAFORM_ACTION=apply`.

Conferir se não restou recurso cobrável:

```powershell
aws eks list-clusters --region us-east-1
aws rds describe-db-instances --region us-east-1
aws elbv2 describe-load-balancers --region us-east-1
aws ec2 describe-nat-gateways --region us-east-1
```

## 📌 Observação importante

O fim da sessão do Learner Lab pode encerrar instâncias EC2, mas outros recursos como RDS, Load Balancer e NAT Gateway podem continuar existindo e consumindo crédito. Por isso, o destroy via esteira é obrigatório.
