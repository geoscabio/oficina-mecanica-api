# Evidencia: build, testes e cobertura

Data da execucao local: 12/07/2026 (atualizado apos refatoracao do AbrirOrdemServicoUseCase)

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

Geracao de relatorio com ReportGenerator:

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

Resumo validado a partir dos arquivos `coverage.cobertura.xml` com Docker acessivel para executar os testes de integracao:

| Metrica | Resultado |
| --- | --- |
| Line coverage global | 92.47% (3462 de 3744 linhas) |
| Branch coverage global | 78.80% |
| Testes ignorados | 0 |
| Assemblies | 4 |
| Projetos de teste | 3 |

> O gate da esteira (`CI`, job `test_application`) usa exclusivamente **line coverage** (92.47% acima), com minimo de 90%. O SonarQube reporta uma metrica combinada de line + branch coverage (87.4%, ver [`sonarqube.md`](sonarqube.md#por-que-o-coverage-do-sonarqube-874-e-diferente-do-gate-de-90-da-esteira)) — sao dois criterios diferentes sobre os mesmos testes, nao um erro de configuracao.

## Observacoes

- A suite completa sem cobertura foi usada como referencia principal de qualidade funcional.
- O numero anterior de 42.6% foi descartado porque a coleta havia ignorado testes de integracao quando o runner nao enxergou o Docker.
- A evidencia valida exige Docker acessivel, 424 testes aprovados e 0 testes ignorados.
- A geracao simultanea de Cobertura e OpenCover depende do ajuste de CI/CD que configura `tests/sonarqube.runsettings` com os dois formatos.
- O pipeline do PR de CI/CD falha se houver teste ignorado ou cobertura global de linhas abaixo de 90%.
- Os arquivos brutos ficam em `TestResults/`, pasta ignorada pelo Git.
- No GitHub Actions, o artifact `test-and-coverage-results` guarda `.trx`, Cobertura XML e relatorio HTML/Markdown.
