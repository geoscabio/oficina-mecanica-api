# Kubernetes local

Este diretório contém somente manifests para execução local com Docker Desktop Kubernetes.

## Aplicar manifests

Execute a partir da raiz do repositório:

```powershell
kubectl apply -R -f local/kubernetes/
kubectl rollout status deployment/sqlserver -n oficina --timeout=180s
kubectl rollout status deployment/oficina-api -n oficina --timeout=180s
```

## Acessar API

```powershell
kubectl port-forward service/oficina-api 5093:8080 -n oficina
```

Depois acesse:

```text
http://localhost:5093/swagger
http://localhost:5093/api/health
```

## Diferença para AWS

O Kubernetes local inclui SQL Server em container. Na AWS, a API roda no EKS e usa Amazon RDS; por isso a fonte de verdade do workload AWS fica em Terraform, em `infra/terraform/environments/dev/api-workload.tf`.
