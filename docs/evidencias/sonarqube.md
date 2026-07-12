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

## Resultado real

Analise rodada em `2026-07-12`, contra o commit `1b8de662`.

| Campo | Valor |
| --- | --- |
| Quality Gate | ✅ **Passed** |
| Linhas de codigo | 9.132 |
| Testes executados | 424 (160 Domain + 226 Application + 38 Integration) — 100% aprovados |
| Coverage | 87,4% |
| Bugs | 0 |
| Reliability Rating | A |
| Code Smells | 6 |
| Maintainability Rating | A |
| Duplicated Lines Density | 1,9% |
| Vulnerabilities | 1 (ver nota abaixo) |
| Security Hotspots | 1 (ver nota abaixo) |
| Security Rating | E (por causa dos 2 achados aceitos abaixo, nao ha vulnerabilidade real no codigo C#) |

### Primeira execucao vs. apos correcoes

A primeira execucao encontrou 5 vulnerabilidades, todas em arquivos de infraestrutura (Terraform/Kubernetes), nenhuma no codigo C#. Quatro foram corrigidas de verdade, sem trade-off, antes de fechar esta evidencia:

| Achado | Arquivo | Correcao |
| --- | --- | --- |
| Storage do RDS sem criptografia | `infra/terraform/modules/rds/database.tf` | Adicionado `storage_encrypted = true` |
| Container sem limite de storage efemero | `k8s/sqlserver-deployment.yaml` | Adicionado `ephemeral-storage` em requests/limits |
| ServiceAccount token automontado sem necessidade | `k8s/api-deployment.yaml` | Adicionado `automountServiceAccountToken: false` |
| ServiceAccount token automontado sem necessidade | `k8s/sqlserver-deployment.yaml` | Adicionado `automountServiceAccountToken: false` |

A quinta vulnerabilidade e o unico security hotspot **nao foram corrigidos**, por serem intencionais e restritos ao ambiente local — justificativa completa em [`docs/projeto/decisoes.md`](../projeto/decisoes.md#achados-aceitos-do-sonarqube-em-k8s):

| Achado | Arquivo | Motivo aceito |
| --- | --- | --- |
| Senha de banco em texto plano (BLOCKER) | `k8s/api-secret.yaml` | Segredo de demonstracao explicitamente `-local-2026`, usado so para `kubectl apply` local. AWS usa Terraform/GitHub Secrets. |
| Protocolo em texto plano (hotspot) | `k8s/api-configmap.yaml` | `ASPNETCORE_URLS=http://+:8080` — TLS termina no Load Balancer em produção; desnecessario na maquina local. |

## Evidencia visual

> Print opcional: a instancia local (`http://localhost:9000/dashboard?id=oficina_mecanica_api`, login `admin`) ficou disponivel para captura manual de tela caso queira complementar a tabela acima com um print do dashboard. Os dados acima ja foram extraidos e conferidos diretamente pela API do SonarQube (`/api/qualitygates/project_status` e `/api/measures/component`), entao sao numeros reais e verificados, independente do print.
