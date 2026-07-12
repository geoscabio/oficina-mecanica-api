# Evidência: build, testes e cobertura

Data da execução local: 09/07/2026

## Comandos executados

Build:

```powershell
dotnet build --no-restore
```

Suite completa sem cobertura:

```powershell
dotnet test --no-build
```

Coleta de cobertura com Coverlet:

```powershell
dotnet test OficinaMecanica.sln `
  --configuration Release `
  --no-build `
  --collect:"XPlat Code Coverage" `
  --settings tests/sonarqube.runsettings `
  --results-directory TestResults `
  --logger "trx"
```

Geração de relatório com ReportGenerator:

```powershell
dotnet tool install dotnet-reportgenerator-globaltool --tool-path "$env:TEMP\dotnet-reportgenerator"
& "$env:TEMP\dotnet-reportgenerator\reportgenerator.exe" `
  -reports:"TestResults/**/coverage.cobertura.xml" `
  -targetdir:"TestResults/CoverageReport" `
  -reporttypes:"MarkdownSummaryGithub;Cobertura;HtmlInline_AzurePipelines"
```

## Resultado validado

| Item | Resultado |
| --- | --- |
| Build | Sucesso, 0 erros, 0 warnings |
| Suite completa sem cobertura | 424 testes aprovados, 0 falhas |
| Suite com cobertura | 424 aprovados, 0 ignorados, 0 falhas |
| Coverlet collector | `XPlat Code Coverage` |
| ReportGenerator | `5.5.10` |

## Cobertura real

Resumo validado a partir dos arquivos `coverage.cobertura.xml` com Docker acessível para executar os testes de integração:

| Métrica | Resultado |
| --- | --- |
| Line coverage global | 91.2% (3679 de 4036 linhas) |
| Testes ignorados | 0 |
| Assemblies | 4 |
| Projetos de teste | 3 |

## Observações

- A suite completa sem cobertura foi usada como referência principal de qualidade funcional.
- O número anterior de 42.6% foi descartado porque a coleta havia ignorado testes de integração quando o runner não enxergou o Docker.
- A evidência válida exige Docker acessível, 424 testes aprovados e 0 testes ignorados.
- A geração simultânea de Cobertura e OpenCover depende do ajuste de CI/CD que configura `tests/sonarqube.runsettings` com os dois formatos.
- O pipeline do PR de CI/CD falha se houver teste ignorado ou cobertura global de linhas abaixo de 90%.
- Os arquivos brutos ficam em `TestResults/`, pasta ignorada pelo Git.
- No GitHub Actions, o artifact `test-and-coverage-results` guarda `.trx`, Cobertura XML e relatório HTML/Markdown.
