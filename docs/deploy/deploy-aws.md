# Deploy na AWS Academy

> Obrigatório: antes de qualquer criação de recurso, ler `docs/deploy/aws-academy-guardrails.md`.

## Regra de ouro

AWS Academy Learner Lab tem crédito limitado. Só execute `terraform apply` com aprovação explícita e com plano de `terraform destroy` ao final.

## Provisionamento da infraestrutura

- [ ] Iniciar o Learner Lab no AWS Academy.
- [ ] Configurar credenciais temporárias da AWS Academy.
- [ ] Configurar `TF_VAR_db_password` fora do repositório.
- [ ] Configurar `TF_VAR_eks_cluster_role_name` e `TF_VAR_eks_node_role_name` com roles existentes no lab.
- [ ] Executar `terraform plan`.
- [ ] Executar `terraform apply` somente com aprovação explícita.

As roles EKS variam entre labs/sessões. Se o lab permitir consulta de IAM, liste candidatas com:

```powershell
aws iam list-roles --profile academy --query "Roles[?contains(RoleName, 'LabEks')].[RoleName]" --output table
```

Se nenhuma role EKS existir, não executar `terraform apply` até validar a estratégia de IAM do ambiente.

## Outputs necessários

Após o `terraform apply`, copie estes outputs:

```powershell
terraform -chdir=infra/environments/dev output ecr_repository_url
terraform -chdir=infra/environments/dev output eks_cluster_name
terraform -chdir=infra/environments/dev output rds_address
```

Use os valores em `Settings > Environments > aws-academy` no GitHub.

## Secrets e variables do environment `aws-academy`

| Nome | Tipo | Uso |
| --- | --- | --- |
| `AWS_ACCESS_KEY_ID` | Secret | Access key temporária da sessão AWS Academy. |
| `AWS_SECRET_ACCESS_KEY` | Secret | Secret key temporária da sessão AWS Academy. |
| `AWS_SESSION_TOKEN` | Secret | Session token temporário da sessão AWS Academy. |
| `JWT_SECRET` | Secret | Chave JWT da API. |
| `WEBHOOK_TOKEN` | Secret | Token do webhook de orçamento. |
| `RDS_CONNECTION_STRING` | Secret | Connection string completa do RDS. |
| `ECR_REPOSITORY_URL` | Variable | URL do ECR gerado pelo Terraform. |
| `EKS_CLUSTER_NAME` | Variable | Nome do EKS gerado pelo Terraform. |

Exemplo de connection string:

```text
Server=<rds-address>,1433;Database=OficinaMecanicaDb;User Id=adminoficina;Password=<senha-rds>;TrustServerCertificate=True;
```

## Deploy manual com aprovação

1. Abrir `Actions > CI/CD`.
2. Clicar em `Run workflow`.
3. Escolher a branch.
4. Selecionar `deployment_target=aws-academy-deploy`.
5. Abrir o run criado.
6. Clicar em `Review deployments`.
7. Selecionar `aws-academy`.
8. Clicar em `Approve and deploy`.

O workflow faz build da imagem, envia para o ECR, configura kubeconfig do EKS, cria/atualiza o Secret da API, aplica os manifests AWS e imprime o endpoint do Load Balancer.

## Validação

- [ ] Validar rollout da API.
- [ ] Validar `kubectl get pods -n oficina`.
- [ ] Validar `kubectl get svc oficina-api -n oficina`.
- [ ] Validar `/api/health`.
- [ ] Validar Swagger.

## Encerramento obrigatório

1. Rodar o workflow manual com `deployment_target=aws-academy-destroy-k8s`.
2. Confirmar que o Service `LoadBalancer` foi removido.
3. Executar:

```powershell
terraform -chdir=infra/environments/dev destroy
```

4. Conferir que não restaram EKS, EC2, RDS, NAT Gateway, ECR com imagens ou Load Balancer ativos.
