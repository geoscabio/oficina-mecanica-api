# Evidencia De Validacao Final - 2026-05-20

## Resumo

Validacao executada na branch `feature/backlog-tecnico-ajustes` para consolidar build, testes automatizados, Dockerfile e execucao via Docker Compose antes da entrega.

## Build

```powershell
dotnet build --nologo
```

Resultado:

| Item | Resultado |
| --- | --- |
| Status | Sucesso |
| Avisos | `0` |
| Erros | `0` |

## Testes Automatizados

```powershell
dotnet test --nologo --no-build
```

Resultado:

| Projeto | Aprovados | Ignorados | Falhas |
| --- | ---: | ---: | ---: |
| `OficinaMecanica.Domain.UnitTests` | `156` | `0` | `0` |
| `OficinaMecanica.Application.UnitTests` | `210` | `0` | `0` |
| `OficinaMecanica.API.IntegrationTests` | `25` | `0` | `0` |
| Total | `391` | `0` | `0` |

## Dockerfile

```powershell
docker --context default build -t oficina-mecanica-api:validation .
```

Resultado:

| Item | Resultado |
| --- | --- |
| Status | Sucesso |
| Imagem local | `oficina-mecanica-api:validation` |

## Docker Compose

Como o `.env` local nao existia, ele foi criado a partir do `.env.example`, seguindo o fluxo documentado no README:

```powershell
Copy-Item .env.example .env
docker --context default compose up --build -d
```

Resultado:

| Container | Status | Porta |
| --- | --- | --- |
| `oficina-mecanica-api` | `Up` | `5093 -> 8080` |
| `oficina-mecanica-sqlserver` | `Up (healthy)` | `14333 -> 1433` |

## Swagger

```powershell
Invoke-WebRequest -Uri 'http://localhost:5093/swagger/index.html' -UseBasicParsing
```

Resultado:

| Item | Resultado |
| --- | --- |
| HTTP Status | `200` |
| Conteudo Swagger | Encontrado |

## Logs Relevantes Da API

```text
Inicializando OficinaMecanica API...
Aplicando migrations do banco de dados...
Banco de dados atualizado.
Carregando dados demo...
Dados demo prontos.
API pronta. Swagger disponivel em /swagger.
Now listening on: http://[::]:8080
```

## Observacoes

- O arquivo `.env` foi criado apenas localmente e esta ignorado pelo Git.
- Os containers foram mantidos em execucao para facilitar a gravacao do video de entrega.
- Os avisos de Data Protection do ASP.NET Core sao esperados em ambiente local/container sem persistencia especifica de chaves.
