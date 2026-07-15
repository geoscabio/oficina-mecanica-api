# Evidencia: SonarQube

Este arquivo documenta a analise estatica real do codigo, rodada contra um SonarQube Community Edition local (Docker), ja que o projeto nao depende de uma conta externa (SonarCloud) para essa evidencia.

## Comando usado

```powershell
docker run -d --name sonarqube -p 9000:9000 sonarqube:community

dotnet tool install --global dotnet-sonarscanner

dotnet sonarscanner begin `
  /k:"oficina_mecanica_api" `
  /n:"Oficina Mecanica API" `
  /d:sonar.host.url="http://localhost:9000" `
  /d:sonar.token="<token-local>" `
  /d:sonar.cs.cobertura.reportsPaths="TestResults/**/coverage.cobertura.xml"

dotnet build OficinaMecanica.sln --configuration Release
dotnet test OficinaMecanica.sln --configuration Release --no-build --collect:"XPlat Code Coverage" --settings tests/sonarqube.runsettings --results-directory ./TestResults

dotnet sonarscanner end /d:sonar.token="<token-local>"
```

## Resultado real (final)

Analise rodada em `2026-07-12`, contra o commit `1b8de662`.

| Campo | Valor |
| --- | --- |
| Quality Gate | ✅ **Passed** |
| Linhas de codigo | 9.152 |
| Testes executados | 424 (160 Domain + 226 Application + 38 Integration) — 100% aprovados |
| Coverage (linha + branch combinados) | 87,4% — ver nota abaixo sobre a diferença para o gate de 90% da esteira |
| Bugs | **0** |
| Reliability Rating | **A** |
| Code Smells | **0** |
| Maintainability Rating | **A** |
| Duplicated Lines Density | 1,9% |
| Vulnerabilities | **0** |
| Security Hotspots | **0** (revisados) |
| Security Rating | **A** |

## Por que o Coverage do SonarQube (87,4%) e diferente do gate de 90% da esteira

Nao e uma inconsistencia: sao duas metricas diferentes, medidas de formas diferentes, a partir dos mesmos relatorios de teste.

- **A esteira (`CI`, job `test_application`)** mede exclusivamente **cobertura de linha** (`line-rate` do Cobertura.xml apos merge com ReportGenerator) e exige minimo de 90%. No commit desta evidencia, esse numero real foi **92,47%** (log real da esteira, run `29192181661`), acima do minimo.
- **O SonarQube** reporta "Coverage" como uma metrica combinada de **cobertura de linha + cobertura de branch/condicao** (`if`/`else`, curto-circuito, etc). Nos relatorios reais deste projeto, o `branch-rate` e sempre mais baixo que o `line-rate` (ex.: `line-rate=0.90` vs `branch-rate=0.79` em um dos assemblies) — cobrir toda linha nao garante cobrir todo caminho condicional. Isso puxa a media combinada do Sonar para baixo, resultando em 87,4%.
- O proprio Quality Gate do SonarQube usado aqui ("Sonar way") so exige 80% de cobertura em **codigo novo** (`new_coverage`) para essa metrica combinada — que tambem foi atendido (`new_coverage actualValue=95.6`, ver Quality Gate acima).

Resumindo: a esteira aplica um criterio (linha, 90%) e o SonarQube aplica outro, mais rigoroso (linha+branch, 80% no gate padrao) — os dois passam, cada um no seu proprio criterio, sobre os mesmos testes reais.

## Trajetoria ate o resultado zerado

A primeira execucao encontrou 5 vulnerabilidades e 7 code smells, todos em arquivos de infraestrutura (Terraform/Kubernetes) ou pontos legitimos de refino no codigo C# — nenhum bug real, nenhuma falha funcional.

### Corrigidos de verdade, sem trade-off

| Achado | Arquivo | Correcao |
| --- | --- | --- |
| Storage do RDS sem criptografia | `infra/terraform/modules/rds/database.tf` | Adicionado `storage_encrypted = true` |
| Container sem limite de storage efemero (API) | `k8s/api-deployment.yaml` | Adicionado `ephemeral-storage` em requests/limits |
| Container sem limite de storage efemero (SQL Server) | `k8s/sqlserver-deployment.yaml` | Adicionado `ephemeral-storage` em requests/limits |
| ServiceAccount token automontado sem necessidade (API) | `k8s/api-deployment.yaml` | Adicionado `automountServiceAccountToken: false` |
| ServiceAccount token automontado sem necessidade (SQL Server) | `k8s/sqlserver-deployment.yaml` | Adicionado `automountServiceAccountToken: false` |
| Construtor com 10 parametros | `AbrirOrdemServicoUseCase.cs` | Extraidos os 7 repositorios para o record `AbrirOrdemServicoRepositorios`, reduzindo o construtor para 4 parametros |
| Complexidade cognitiva 18 (limite 15) | `AbrirOrdemServicoUseCase.cs` | Extraido o bloco de reserva de pecas/insumos para o metodo privado `ReservarPecasInsumosAsync` |
| Array literal repetido em teste | `OrdensServicoControllerTests.cs` | Extraido para o campo `static readonly string[] StatusHistoricoEsperados` |

Todas as 424 testes continuam passando apos essas mudancas (nenhum comportamento alterado, apenas estrutura interna).

### Aceitos e documentados (marcados "Won't Fix" no SonarQube, com justificativa)

Estes 4 achados sao intencionais e nao representam risco real:

| Achado | Arquivo | Motivo aceito |
| --- | --- | --- |
| Senha de banco em texto plano (era BLOCKER) | `k8s/api-secret.yaml` | Segredo de demonstracao explicitamente `-local-2026`, usado so para `kubectl apply` local. AWS usa Terraform/GitHub Secrets. |
| Protocolo em texto plano (era security hotspot) | `k8s/api-configmap.yaml` | `ASPNETCORE_URLS=http://+:8080` — TLS termina no Load Balancer em produção; desnecessario na maquina local. |
| Tag de imagem `:latest` nao fixada | `k8s/api-deployment.yaml` | Imagem construida localmente pelo proprio desenvolvedor, nunca publicada em registry; sem esquema de versionamento semantico no projeto. |
| Metodo de instancia que nao usa `this` | `OrdemServicoRequestBuilder.cs` | Mantido como metodo de instancia de proposito, para preservar a consistencia do padrao Builder fluente (`Novo().Build...()`) usado pelos demais metodos `Build` da classe. |

Como essas resolucoes foram aplicadas via `Won't Fix`/`Safe` diretamente no SonarQube (com comentario de justificativa em cada issue, mecanismo padrao da ferramenta para risco aceito), o dashboard final mostra **0 issues abertas em todas as categorias** e nota **A** em Reliability, Security e Maintainability.

## Evidencia visual

> Print opcional: a instancia local (`http://localhost:9000/dashboard?id=oficina_mecanica_api`, login `admin`) ficou disponivel para captura manual de tela caso queira complementar a tabela acima com um print do dashboard. Os dados acima ja foram extraidos e conferidos diretamente pela API do SonarQube (`/api/qualitygates/project_status` e `/api/measures/component`), entao sao numeros reais e verificados, independente do print.
