# Evidencia: build, testes e cobertura

Data da execucao local: 09/07/2026

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
  --no-build `
  --collect:"XPlat Code Coverage" `
  --results-directory TestResults `
  --logger "trx;LogFileName=test-results.trx"
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
| Suite com cobertura | 396 aprovados, 28 ignorados, 0 falhas |
| Coverlet collector | `XPlat Code Coverage` |
| ReportGenerator | `5.5.10` |

## Cobertura real

Resumo gerado por ReportGenerator a partir dos arquivos `coverage.cobertura.xml`:

| Metrica | Resultado |
| --- | --- |
| Line coverage | 42.6% (3100 de 7275 linhas) |
| Branch coverage | 67.8% (602 de 887 branches) |
| Assemblies | 4 |
| Classes | 285 |
| Arquivos | 276 |

## Observacoes

- A suite completa sem cobertura foi usada como referencia principal de qualidade funcional.
- Durante a coleta de cobertura, parte dos testes de integracao foi marcada como ignorada pelo runner; por isso o numero com cobertura ficou em 396 aprovados e 28 ignorados.
- Os arquivos brutos ficam em `TestResults/`, pasta ignorada pelo Git.
- No GitHub Actions, o artifact `test-and-coverage-results` guarda `.trx`, Cobertura XML e relatorio HTML/Markdown.
