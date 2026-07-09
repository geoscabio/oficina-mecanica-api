# Secrets Kubernetes - AWS

Os Secrets da API nao sao versionados neste repositorio. Crie-os diretamente no cluster EKS antes de aplicar o Deployment.

## Secret da API

```powershell
kubectl create secret generic oficina-api-secret `
  -n oficina `
  --from-literal=Jwt__Secret="<jwt-com-pelo-menos-32-caracteres>" `
  --from-literal=Integracoes__Orcamento__WebhookToken="<webhook-com-pelo-menos-32-caracteres>" `
  --from-literal=ConnectionStrings__DefaultConnection="Server=<endpoint-rds>,1433;Database=OficinaMecanicaDb;User Id=adminoficina;Password=<senha-rds>;TrustServerCertificate=True;"
```

## Observacoes

- Nao commitar valores reais de senha, token ou connection string.
- A senha do RDS deve ser a mesma informada ao Terraform por `TF_VAR_db_password`.
- O endpoint do RDS pode ser obtido com `terraform output rds_address`.
- Ao finalizar a demonstracao, remover os manifests com `kubectl delete -f infra/k8s/aws/`.
