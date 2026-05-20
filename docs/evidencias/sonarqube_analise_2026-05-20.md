# Evidencia SonarQube - 2026-05-20

## Resumo

Analise SonarQube executada localmente para o projeto `oficina-mecanica-api`.

| Item | Resultado |
| --- | --- |
| SonarQube | `26.5.0.122743` |
| Scanner | `dotnet-sonarscanner 11.2.1` |
| Projeto | `oficina-mecanica-api` |
| Dashboard | `http://localhost:9000/dashboard?id=oficina-mecanica-api` |
| Compute Engine Task | `SUCCESS` |
| Quality Gate | `OK` |
| Bugs | `0` |
| Vulnerabilidades | `0` |
| Security Hotspots | `1` |
| Code Smells | `17` |
| Cobertura | `89.9%` |
| Duplicacao | `0.2%` |
| Linhas de codigo | `7083` |

## Evidencia De Testes Na Analise

Durante a analise do SonarQube, os testes foram executados com coleta de cobertura em formato OpenCover:

| Projeto | Resultado |
| --- | --- |
| `OficinaMecanica.Domain.UnitTests` | `156` testes aprovados |
| `OficinaMecanica.Application.UnitTests` | `210` testes aprovados |
| `OficinaMecanica.API.IntegrationTests` | `25` testes aprovados |
| Total | `391` testes aprovados |

## Comandos Executados

O token do SonarQube nao deve ser versionado. Ele foi informado apenas por variavel de ambiente durante a execucao.

```powershell
dotnet tool install --global dotnet-sonarscanner

dotnet sonarscanner begin `
  /k:"oficina-mecanica-api" `
  /n:"Oficina Mecanica API" `
  /d:sonar.host.url="http://localhost:9000" `
  /d:sonar.token="$env:SONAR_TOKEN" `
  /d:sonar.cs.opencover.reportsPaths="TestResults/SonarCoverage/**/coverage.opencover.xml" `
  /d:sonar.exclusions="**/Migrations/**,**/bin/**,**/obj/**" `
  /d:sonar.coverage.exclusions="**/Migrations/**"

dotnet build --nologo

dotnet test --nologo --no-build `
  --settings tests\sonarqube.runsettings `
  --collect:"XPlat Code Coverage" `
  --results-directory TestResults\SonarCoverage

dotnet sonarscanner end /d:sonar.token="$env:SONAR_TOKEN"
```

## Issues Apontadas Pelo Sonar

| Tipo | Severidade | Regra | Local | Observacao |
| --- | --- | --- | --- | --- |
| Code Smell | Major | `csharpsquid:S107` | `OrdemServicoSeedData.cs` linhas `56`, `78`, `102`, `125`, `141` | Metodos de seed com 8 parametros |
| Code Smell | Major | `csharpsquid:S1118` | `Program.cs` linha `28` | Classe utilitaria sem construtor protegido ou `static` |
| Code Smell | Major | `csharpsquid:S6966` | `Program.cs` linha `26` | Preferir `await RunAsync` |
| Code Smell | Minor | `csharpsquid:S3267` | `ReservarPecaInsumoUseCase.cs` linha `110` | Loop pode ser simplificado com LINQ |
| Code Smell | Info | `external_roslyn:CA2263` | Validadores e entidades com `Enum.IsDefined` | Preferir overload generica |
| Code Smell | Info | `external_roslyn:CA1861` | `CpfCnpj.cs` linhas `61`, `62` | Preferir campos `static readonly` para arrays constantes |
| Code Smell | Info | `external_roslyn:CA1859` | `OficinaMecanicaApiFixture.cs` linha `119` | Tipo concreto pode melhorar performance |
| Code Smell | Info | `external_roslyn:ASP0027` | `Program.cs` linha `28` | `partial Program` publico nao e mais obrigatorio |

## Security Hotspot

| Status | Probabilidade | Local | Observacao |
| --- | --- | --- | --- |
| `TO_REVIEW` | `MEDIUM` | `Dockerfile` linha `19` | Imagem roda com usuario padrao `root`; avaliar seguranca do container no contexto de entrega |

## Como Gerar O PDF Para Entrega

1. Acesse `http://localhost:9000/dashboard?id=oficina-mecanica-api`.
2. Abra o projeto `Oficina Mecanica API`.
3. Confirme visualmente o `Quality Gate`, bugs, vulnerabilidades, hotspots, code smells, cobertura e duplicacao.
4. No navegador, use `Ctrl + P`.
5. Escolha `Salvar como PDF`.
6. Salve o arquivo como evidencia da entrega.

## Observacao

Os arquivos temporarios da analise ficam em `.sonarqube/` e `TestResults/`. Ambos devem permanecer fora do versionamento.
