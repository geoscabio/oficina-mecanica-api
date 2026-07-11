# Evidencia: Kubernetes HPA

Este arquivo reserva o registro do teste de escalonamento horizontal em Kubernetes local.

## Pre-requisitos

- Docker Desktop com Kubernetes habilitado.
- Metrics Server instalado.
- Manifests aplicados em `k8s/`.

## Comandos sugeridos

Aplicar ambiente:

```powershell
kubectl apply -R -f k8s/
kubectl rollout status deployment/sqlserver -n oficina-mecanica --timeout=180s
kubectl rollout status deployment/oficina-mecanica-api -n oficina-mecanica --timeout=180s
```

Monitorar HPA:

```powershell
kubectl get hpa -n oficina-mecanica -w
```

Gerar carga simples dentro do cluster:

```powershell
kubectl run load-generator `
  -n oficina-mecanica `
  --image=busybox:1.36 `
  --restart=Never `
  -- /bin/sh -c "while true; do wget -q -O- http://oficina-mecanica-api:8080/api/health; done"
```

Consultar pods e metricas:

```powershell
kubectl get pods -n oficina-mecanica
kubectl top pods -n oficina-mecanica
kubectl get hpa oficina-mecanica-api-hpa -n oficina-mecanica
```

Limpeza do gerador de carga:

```powershell
kubectl delete pod load-generator -n oficina-mecanica --ignore-not-found
```

## Resultado real

> Colar aqui o print do HPA durante o teste de carga antes da entrega final.

| Campo | Valor |
| --- | --- |
| HPA | `oficina-mecanica-api-hpa` |
| Namespace | `oficina-mecanica` |
| Replicas minimas | 1 |
| Replicas maximas | 5 |
| CPU target | 70% |
| Memory target | 80% |
| Evidencia de escala | Pendente de print real |

## Evidencia visual

Adicionar o print abaixo:

```text
[INSERIR PRINT DO kubectl get hpa -n oficina-mecanica AQUI]
```
