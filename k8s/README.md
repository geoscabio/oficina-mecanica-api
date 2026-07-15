# ☸️ Kubernetes local

Este diretório contém somente manifests para execução local com Docker Desktop Kubernetes.

## 📦 Aplicar manifests

Execute a partir da raiz do repositório:

```powershell
kubectl apply -R -f k8s/
kubectl rollout status deployment/sqlserver -n oficina-mecanica --timeout=180s
kubectl rollout status deployment/oficina-mecanica-api -n oficina-mecanica --timeout=180s
```

## 🌐 Acessar API

```powershell
kubectl port-forward service/oficina-mecanica-api 5093:8080 -n oficina-mecanica
```

Depois acesse:

```text
http://localhost:5093/swagger
http://localhost:5093/api/health
```

## 🔄 Diferença para AWS

O Kubernetes local inclui SQL Server em container. Na AWS, a API roda no EKS e usa Amazon RDS; por isso a fonte de verdade do workload AWS fica em Terraform, em `infra/terraform/environments/dev/` (arquivos `namespace.tf`, `api-configmap.tf`, `api-secret.tf`, `api-deployment.tf`, `api-service.tf`, `api-hpa.tf`).
