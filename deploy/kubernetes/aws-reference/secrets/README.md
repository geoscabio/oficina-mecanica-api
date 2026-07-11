# Secrets Kubernetes - AWS

Os Secrets da API nao sao versionados neste repositorio. No deploy AWS real, eles sao criados pelo Terraform a partir dos secrets do GitHub Environment.

## Secret da API para teste manual

```powershell
kubectl create secret generic oficina-api-secret `
  -n oficina `
  --from-literal=Jwt__Secret="<jwt-com-pelo-menos-32-caracteres>" `
  --from-literal=Integracoes__Orcamento__WebhookToken="<webhook-com-pelo-menos-32-caracteres>" `
  --from-literal=ConnectionStrings__DefaultConnection="Server=<endpoint-rds>,1433;Database=OficinaMecanicaDb;User Id=adminoficina;Password=<senha-rds>;TrustServerCertificate=True;"
```

## Observacoes

- Nao commitar valores reais de senha, token ou connection string.
- A senha do RDS deve ser a mesma informada ao Terraform por `DB_PASSWORD` no GitHub Environment ou por variável local segura.
- O endpoint do RDS pode ser obtido com `terraform output rds_address`.
- Ao finalizar a demonstracao, executar `terraform destroy` usando o mesmo backend/state da esteira.
