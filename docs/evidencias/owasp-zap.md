# Evidencia: OWASP ZAP Baseline

Este arquivo documenta o resultado real da analise dinamica de seguranca (DAST) contra a API rodando localmente.

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

## Comando usado

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

Scan executado em `2026-07-12`, contra a API local (`docker compose`, commit `4d53f5b`).

| Campo | Valor |
| --- | --- |
| Target | `http://host.docker.internal:5093` |
| URLs analisadas | 12 |
| Regras verificadas | 67 |
| **Falhas (FAIL)** | **0** |
| Avisos (WARN) | 4 |
| Aprovados (PASS) | 63 |
| Relatorio HTML | [`zap/zap-baseline.html`](zap/zap-baseline.html) |
| Relatorio JSON | [`zap/zap-baseline.json`](zap/zap-baseline.json) |

Nenhuma falha de seguranca real (FAIL) foi encontrada. Os 4 avisos existentes sao todos originados pela interface do **Swagger UI** — ferramenta de terceiros (Swashbuckle), intencionalmente exposta neste ambiente para fins de avaliacao/demonstracao (ja documentado em [`docs/projeto/decisoes.md`](../projeto/decisoes.md), seção "Ambiente AWS de demonstração"), nao pela API de negocio em si:

| Regra | Alertas | Motivo aceito |
| --- | --- | --- |
| Non-Storable Content [10049] | 5 | Respostas dinamicas da API/Swagger nao sao cacheaveis por padrao — comportamento esperado e seguro para uma API, nao uma falha. |
| CSP: script-src unsafe-inline [10055] | 4 | O HTML padrao do Swagger UI (gerado pelo Swashbuckle) usa scripts inline. Restringir exigiria customizar/hospedar uma versao propria da UI do Swagger so para satisfazer o scanner, sem ganho real de seguranca no cenario de demonstracao academica. |
| Timestamp Disclosure - Unix [10096] | 5 | Encontrado dentro de `swagger-ui-standalone-preset.js`, arquivo de biblioteca de terceiro (bundle do Swashbuckle), fora do controle do codigo da aplicacao. |
| Modern Web Application [10109] | 2 | Alerta puramente informativo do ZAP (reconhece a Swagger UI como aplicacao moderna) — nao representa risco. |

Nenhuma acao adicional e necessaria: 0 falhas reais, e os 4 avisos remanescentes sao inerentes ao Swagger UI (dependencia de terceiro exposta de proposito), nao ao codigo da API.

## Evidencia visual

Relatorio HTML completo gerado pelo scan: [`docs/evidencias/zap/zap-baseline.html`](zap/zap-baseline.html) (abrir localmente no navegador).
