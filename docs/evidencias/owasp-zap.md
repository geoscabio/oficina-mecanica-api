# Evidencia: OWASP ZAP Baseline

Este arquivo reserva o registro versionado da analise dinamica de seguranca.

## Pre-requisito

A API precisa estar executando localmente:

```powershell
docker compose --env-file .env -f docker-compose.yml up -d --build
curl http://localhost:5093/api/health
```

Resultado esperado:

```text
Healthy
```

## Comando sugerido

PowerShell:

```powershell
New-Item -ItemType Directory -Force docs/evidencias/zap | Out-Null

docker run --rm -t `
  -v "${PWD}/docs/evidencias/zap:/zap/wrk" `
  ghcr.io/zaproxy/zaproxy:stable `
  zap-baseline.py `
  -t http://host.docker.internal:5093 `
  -r zap-baseline.html `
  -J zap-baseline.json
```

## Resultado real

> Colar aqui o resultado real do baseline scan antes da entrega final.

| Campo | Valor |
| --- | --- |
| Target | `http://localhost:5093` |
| Falhas criticas | Pendente de execucao real |
| Alertas medios/baixos | Pendente de execucao real |
| Relatorio HTML | `docs/evidencias/zap/zap-baseline.html` |
| Relatorio JSON | `docs/evidencias/zap/zap-baseline.json` |

## Evidencia visual

Adicionar o print abaixo:

```text
[INSERIR PRINT DO RESULTADO OWASP ZAP AQUI]
```
