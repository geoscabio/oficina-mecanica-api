# Evidencia: SonarQube

Este arquivo reserva o registro versionado da analise estatica. Nao versionar tokens reais.

## Comando sugerido

```powershell
dotnet tool install --global dotnet-sonarscanner

dotnet sonarscanner begin `
  /k:"oficina_mecanica_api" `
  /n:"Oficina Mecanica API" `
  /d:sonar.host.url="<url-do-sonarqube>" `
  /d:sonar.token="<token-local-ou-secret>" `
  /d:sonar.cs.cobertura.reportsPaths="TestResults/CoverageReport/Cobertura.xml"

dotnet build --no-restore
dotnet test --no-build --collect:"XPlat Code Coverage" --results-directory TestResults

dotnet sonarscanner end /d:sonar.token="<token-local-ou-secret>"
```

## Resultado real

> Colar aqui o print do Quality Gate antes da entrega final.

| Campo | Valor |
| --- | --- |
| Quality Gate | Pendente de print real |
| Bugs | Pendente de print real |
| Vulnerabilities | Pendente de print real |
| Security Hotspots | Pendente de print real |
| Code Smells | Pendente de print real |
| Coverage report usado | `TestResults/CoverageReport/Cobertura.xml` |

## Evidencia visual

Adicionar o print abaixo:

```text
[INSERIR PRINT DO QUALITY GATE AQUI]
```
