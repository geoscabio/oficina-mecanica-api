# Kubernetes

Esta pasta separa os manifests por finalidade para evitar mistura entre execução local e referência AWS.

| Pasta | Uso | Conteúdo |
| --- | --- | --- |
| `local/` | Execução local com Docker Desktop Kubernetes. | API, SQL Server, Service, Ingress, HPA, Secrets e ConfigMaps locais. |
| `aws-reference/` | Referência operacional e validação client-side no CI. | API para EKS usando RDS externo e imagem publicada no ECR pela esteira. |

## Execução local

```powershell
kubectl apply -R -f deploy/kubernetes/local/
kubectl rollout status deployment/sqlserver -n oficina --timeout=180s
kubectl rollout status deployment/oficina-api -n oficina --timeout=180s
```

Depois, acesse a API com port-forward:

```powershell
kubectl port-forward service/oficina-api 5093:8080 -n oficina
```

## AWS

No deploy real, os recursos Kubernetes da API são gerenciados pelo Terraform em `infra/terraform/environments/dev/api-workload.tf`.

Os manifests em `aws-reference/` continuam versionados para leitura, troubleshooting e validação no CI, mas não são a fonte principal do deploy AWS.

## Regras

- Não colocar secrets reais em YAML.
- Manter `local/` voltado ao ambiente `Development`.
- Manter `aws-reference/` alinhado ao ambiente `Staging` de demonstração.
- Usar `terraform destroy` para remover recursos AWS ao final da demonstração.
