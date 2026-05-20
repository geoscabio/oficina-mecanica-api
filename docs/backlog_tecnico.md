# Backlog Tecnico Consolidado

Ultima atualizacao: 2026-05-20
Fonte de consolidacao: `code_review_v4_final.md` + `FIAP/Snapshots projeto/Code Reviews/backlog_tecnico.md` + `consolidado_final_codex_entrega_oficina_mecanica.md` + validacao local na branch `feature/backlog-tecnico-ajustes`.

## Diretriz Final De Entrega

- Nao reescrever a solucao.
- Nao alterar arquitetura em larga escala.
- Nao mover responsabilidades entre camadas sem necessidade.
- Nao expor use cases internos de estoque como endpoints publicos sem necessidade.
- Priorizar correcoes pequenas, seguras, testaveis e faceis de revisar.
- Tratar limitacoes de MVP como documentacao/backlog, nao como refatoracao grande de ultima hora.
- Toda correcao deve sair de branch de trabalho e commit convencional.

## Atualizacoes Deste Ciclo

| ID | Item | Status anterior | Status atual | Evidencia |
| --- | --- | --- | --- | --- |
| DC-003 | Criar `.env.example` | Postergado | Corrigido | Arquivo `.env.example` adicionado na raiz |
| DT-021 | Auto-skip de testes de integracao quando Docker nao estiver acessivel | Nao existia | Corrigido | `RequiresDockerFactAttribute` aplicado nos testes que dependem de container |
| DT-023 | Remover metodos orfaos de `IEstoqueRepository` | Nao existia | Corrigido | `ObterItemPorPecaInsumoCatalogoIdAsync` e `AtualizarItemAsync` removidos de interface/repositorio/testes |
| DT-022 | Dotnet test da solution falha sem Docker | Nao existia | Corrigido | Testes de integracao dependentes de Docker agora sao ignorados quando Docker nao estiver acessivel |
| DT-015 | Auditoria de vulnerabilidade NuGet | Pendente | Corrigido | Evidencia registrada em `docs/evidencias/auditoria_nuget_vulnerabilidades.md` |
| DT-024 | Evitar alteracoes diretas na `develop` | Pendente | Corrigido | Fluxo Git formalizado neste documento |
| DT-012 | Retornar todos os erros de validacao | Pendente | Corrigido | `ErrorResponse` agora inclui `erros` e use cases retornam todas as mensagens do FluentValidation |
| DT-011 | Enums em `SCREAMING_CASE` | Pendente | Corrigido | `StatusOrdemServico` e `StatusServico` renomeados para PascalCase |
| DT-013 | Padronizar formatacao global | Pendente | Corrigido | Quebras compactadas em `src` e `tests`; `.editorconfig` adicionado; `dotnet format` passou |
| DT-040 | Consolidar backlog final de entrega | Nao existia | Corrigido | Itens P0/P1/P2 do consolidado final adicionados com IDs rastreaveis |
| DT-014 | Evidencia SonarQube | Pendente | Em andamento | Analise publicada em SonarQube local; metricas registradas em `docs/evidencias/sonarqube_analise_2026-05-20.md`; falta exportar PDF do dashboard |
| DT-025 | README com `.env` multiplataforma | Pendente | Corrigido | README mostra `Copy-Item .env.example .env` e `cp .env.example .env` |
| DT-026 | README diferenciando Docker de .NET SDK local | Pendente | Corrigido | Pre-requisitos separados por fluxo Docker Compose e build/testes locais |
| DT-029 | Arquivos locais/snapshot fora do versionamento | Pendente | Corrigido | `git ls-files .vs "*.csproj.user" "*snapshot*" "Contexto para IA.zip"` sem retorno |
| DT-041 | Sonar `Program.cs` | Nao existia | Corrigido | `RunAsync` aguardado e `Program` mantido publico com justificativa para testes integrados |
| DT-042 | Sonar security hotspot no `Dockerfile` | Nao existia | Corrigido | Runtime passa a usar `USER $APP_UID` |
| DT-043 | Sonar parametros excessivos no seed | Nao existia | Corrigido | `OrdemSeedInfo` agrupa dados da OS no seed |
| DT-044 | Sonar loop simplificavel | Nao existia | Corrigido | Verificacao de estoque usa `All` |
| DT-045 | Sonar `Enum.IsDefined` generico | Nao existia | Corrigido | Validacoes usam overload generica |
| DT-046 | Sonar arrays constantes em CPF/CNPJ | Nao existia | Corrigido | Pesos do CNPJ movidos para campos `static readonly` |
| DT-047 | Sonar tipo concreto em fixture | Nao existia | Corrigido | Fixture retorna `Dictionary<string, string?>` |
| DT-028 | Evidencia final de execucao | Pendente | Corrigido | Build, testes, Dockerfile, Docker Compose, Swagger e logs registrados em `docs/evidencias/validacao_final_2026-05-20.md` |
| DT-027 | Revisar TODOs de entrega e documentacao | Pendente | Corrigido | Busca `TODO/FIXME/HACK/posteriormente/SQL Server 2025`; pendencia Postman removida do README |
| DT-034 | Alinhar SQL Server 2022 na documentacao | Pendente | Corrigido | README explicita SQL Server 2022 e imagem `mcr.microsoft.com/mssql/server:2022-latest` |
| DT-032 | `Endereco` com factory de Value Object | Pendente | Corrigido | Construtor tornado privado e consumidores migrados para `Endereco.Criar(...)` |
| DT-035..DT-039 | Limitacoes MVP de ordem de servico | Pendente | Documentado | README registra aprovacao implicita, estoque insuficiente, consulta por cliente, numeracao e persistencia como limitacoes/evolucoes |
| DT-017..DT-020 | Limitacoes tecnicas pos-MVP | Pendente | Documentado | README registra logging, login demo, response de autenticacao e modelagem de estoque como evolucoes |

## P0 - Obrigatorio Antes Da Entrega

| ID | Origem | Item | Status | Evidencia atual | Proxima acao |
| --- | --- | --- | --- | --- | --- |
| DT-014 | Tech Challenge / Backlog | Gerar evidencia SonarQube com Quality Gate, bugs, vulnerabilities, security hotspots, code smells, coverage e duplications | Em andamento | Dashboard local gerado; Quality Gate `OK`, bugs `0`, vulnerabilities `0`, hotspots `1`, code smells `17`, coverage `89.9%`, duplications `0.2%`; evidencia em `docs/evidencias/sonarqube_analise_2026-05-20.md` | Exportar o dashboard do SonarQube em PDF pelo navegador |
| DT-025 | Review final | README com criacao de `.env` multiplataforma | Corrigido | README documenta PowerShell e Bash/Git Bash/macOS/Linux | Nenhuma |
| DT-026 | Review final | README diferenciando Docker de .NET SDK local | Corrigido | README separa pre-requisitos de Docker Compose e build/testes locais | Nenhuma |
| DT-027 | Review final | Revisar TODOs de entrega e documentacao | Corrigido | Busca por `TODO`, `FIXME`, `HACK`, `posteriormente` e `SQL Server 2025` executada; pendencia de Postman removida do README | Nenhuma |
| DT-028 | Review final | Evidencia final de execucao | Corrigido | `dotnet build`, `dotnet test`, `docker build`, `docker compose up --build -d`, Swagger `200` e logs registrados em `docs/evidencias/validacao_final_2026-05-20.md` | Nenhuma |
| DT-029 | Auditoria final | Garantir que arquivos locais/snapshot nao estejam versionados | Corrigido | `git ls-files .vs "*.csproj.user" "*snapshot*" "Contexto para IA.zip"` sem retorno; `.sonarqube/` tambem foi ignorado | Nenhuma |

## P1 - Correcoes Pequenas De Codigo Recomendadas

| ID | Origem | Item | Status | Evidencia atual | Proxima acao |
| --- | --- | --- | --- | --- | --- |
| DT-030 | Claude | `Servico`, `PecaInsumo` e `ItemEstoque` sem construtor privado sem parametros | Corrigido | Construtores privados sem parametros adicionados nas entidades filhas | Nenhuma |
| DT-031 | Claude | `OrdemServico.DataInicio` nullable sem necessidade | Corrigido | `DataInicio` alterado para `DateTime`, response atualizado e coluna marcada como obrigatoria no EF Core | Nenhuma |
| DT-032 | Claude | `Endereco` Value Object com construtor publico | Corrigido | `Endereco.Criar(...)` criado, construtor tornado privado e consumidores/testes atualizados | Nenhuma |
| DT-033 | Review final | Warnings EF Core por multiplos `Include` de colecoes | Corrigido | `.AsSplitQuery()` aplicado nas queries de OS com `Servicos` e `PecasInsumos` | Nenhuma |
| DT-034 | Auditoria final | ADR/documentacao menciona SQL Server 2025, mas Docker/Testcontainers usam SQL Server 2022 | Corrigido | README alinhado para SQL Server 2022; Docker Compose e Testcontainers usam `mcr.microsoft.com/mssql/server:2022-latest` | Nenhuma |

## P2 - Documentar Como Limitacao MVP Ou Pos-MVP

| ID | Origem | Item | Status | Evidencia atual | Proxima acao |
| --- | --- | --- | --- | --- | --- |
| DT-035 | Auditoria final | Aprovacao de orcamento implicita em `IniciarExecucao` | Documentado | README registra que iniciar execucao representa orcamento aprovado fora do sistema no MVP | Evoluir para endpoint/evento explicito pos-MVP |
| DT-036 | Auditoria final | Estoque insuficiente bloqueia reserva, mas Domain Storytelling fala em cancelar OS | Documentado | README registra que a reserva retorna erro e bloqueia a operacao no MVP | Avaliar cancelamento automatico pos-MVP |
| DT-037 | Auditoria final | Cliente com role `Cliente` pode consultar status de qualquer OS conhecendo o ID | Documentado | README registra limitacao do usuario demo `Cliente` por ID da OS | Vincular usuario autenticado a `ClienteId` real pos-MVP |
| DT-038 | Auditoria final | Repositories chamam `SaveChangesAsync` mesmo existindo `IUnitOfWork` | Documentado | README registra persistencia mista como decisao de MVP | Padronizar toda persistencia em torno do Unit of Work pos-MVP |
| DT-039 | Auditoria final | Numero da OS via `MAX + 1` tem risco teorico de concorrencia | Documentado | README registra numeracao por maior numero existente como decisao de MVP | Usar sequence/identity transacional pos-MVP |
| DT-016 | Backlog | Analise OWASP ZAP | Pendente | Sem relatorio de runtime | Rodar ZAP na API local e anexar saida se houver tempo |
| DT-017 | Backlog | Logging estruturado | Documentado | README registra logs atuais e evolucao para correlacao por request | Implementar logging estruturado pos-MVP |
| DT-018 | Backlog | Login demo com senha em texto puro | Documentado | README registra usuarios/senhas demo para avaliacao local | Evoluir para hash, ASP.NET Identity ou provedor externo |
| DT-019 | Backlog | Response de autenticacao pode ser simplificado | Documentado | Mantido por compatibilidade e clareza do demo | Reavaliar contrato pos-MVP |
| DT-020 | Backlog | `Estoque` como aggregate unico global | Documentado | Modelo atual atende MVP e fica registrado como evolucao de modelagem | Reavaliar aggregate quando volume/carga aumentar |

## Achados SonarQube Mapeados

| ID | Regra | Severidade | Local | Status | Acao |
| --- | --- | --- | --- | --- | --- |
| DT-041 | `csharpsquid:S6966`, `csharpsquid:S1118`, `external_roslyn:ASP0027` | Major / Info | `Program.cs` | Corrigido | `RunAsync` aguardado, construtor protegido adicionado e `Program` publico mantido para `WebApplicationFactory<Program>` |
| DT-042 | Security Hotspot | Medium | `Dockerfile` | Corrigido | Runtime configurado para usuario nao-root com `USER $APP_UID` |
| DT-043 | `csharpsquid:S107` | Major | `OrdemServicoSeedData.cs` | Corrigido | Parametros comuns agrupados em `OrdemSeedInfo` |
| DT-044 | `csharpsquid:S3267` | Minor | `ReservarPecaInsumoUseCase.cs` | Corrigido | Loop substituido por `All` |
| DT-045 | `external_roslyn:CA2263` | Info | Validadores e entidades com `Enum.IsDefined` | Corrigido | Chamadas alteradas para overload generica |
| DT-046 | `external_roslyn:CA1861` | Info | `CpfCnpj.cs` | Corrigido | Arrays de pesos de CNPJ movidos para campos `static readonly` |
| DT-047 | `external_roslyn:CA1859` | Info | `OficinaMecanicaApiFixture.cs` | Corrigido | Metodo de configuracao retorna tipo concreto |

## Checklist Final De Entrega

| Area | Item | Status |
| --- | --- | --- |
| Codigo | Construtores EF em entidades filhas | Corrigido |
| Codigo | `DataInicio` nao-nullable | Corrigido |
| Codigo | `Endereco` com factory de Value Object | Corrigido |
| Codigo | `.AsSplitQuery()` em multiplos Includes de OS | Corrigido |
| Codigo | Nenhum `.vs/` ou `.csproj.user` versionado | Corrigido |
| Codigo | ADR SQL Server alinhada com Docker/Testcontainers | Corrigido |
| Documentacao | README com `.env` para Windows e Mac/Linux | Corrigido |
| Documentacao | README explicando Docker vs .NET SDK local | Corrigido |
| Documentacao | README/evidencia com validacao final | Corrigido |
| Documentacao | Limitacoes MVP documentadas | Corrigido |
| Documentacao | Backlog tecnico atualizado | Corrigido |
| Documentacao | TODOs de entrega revisados | Corrigido |
| Validacao | `dotnet build --nologo` passou apos ajustes finais | Corrigido |
| Validacao | `dotnet test --nologo` passou apos ajustes finais | Corrigido |
| Validacao | `docker compose up --build` validado | Corrigido |
| Evidencias | SonarQube PDF para professor | Em andamento |
| Evidencias | Auditoria NuGet | Corrigido |
| Evidencias | Evidencia de testes/build/Docker | Corrigido |

## Itens Ja Corrigidos (Resumo)

- DT-001 Atomicidade multi-aggregate com `IUnitOfWork` e `CreateExecutionStrategy` (corrigido)
- DT-002 Tipo de erro interno no middleware (corrigido)
- DT-003 Protecao `Response.HasStarted` no middleware (corrigido)
- DT-004 `OrdemServicoResponse` com servicos e pecas/insumos (corrigido)
- DT-005 Atualizacao de estoque via aggregate root (corrigido)
- DT-006 N+1 em definicao de servicos (corrigido)
- DT-007 `UsuarioId` estavel no JWT demo (corrigido)
- DT-008 Construtor privado EF em `ServicoCatalogo` (corrigido)
- DT-009 Desacoplamento de `EnderecoRequest` (corrigido)
- DT-010 `PerfisAcesso` movido para Application (corrigido)
- DT-011 Enums de status renomeados para PascalCase (corrigido)
- DT-012 Erros de validacao retornam lista completa no campo `erros` (corrigido)
- DT-013 Formatacao global compactada em `src` e `tests` com `.editorconfig` para preservar blocos de linha unica (corrigido)
- DT-015 Auditoria de vulnerabilidade NuGet sem pacotes vulneraveis encontrados (corrigido)
- DT-021 Auto-skip dos testes de integracao dependentes de Docker (corrigido)
- DT-022 `dotnet test` da solution nao falha sem Docker acessivel (corrigido)
- DT-023 Metodos orfaos de `IEstoqueRepository` removidos (corrigido)
- DT-024 Fluxo Git formalizado com branch por tarefa, commits convencionais e PR (corrigido)
- DT-025 README com criacao de `.env` multiplataforma (corrigido)
- DT-026 README diferenciando Docker Compose de .NET SDK local (corrigido)
- DT-027 TODOs e pendencias de documentacao revisados (corrigido)
- DT-028 Evidencia final de build, testes, Docker Compose, Swagger e logs (corrigido)
- DT-029 Arquivos locais e snapshots fora do versionamento (corrigido)
- DT-030 Construtores privados EF em entidades filhas (corrigido)
- DT-031 `OrdemServico.DataInicio` nao-nullable (corrigido)
- DT-032 `Endereco` com factory e construtor privado (corrigido)
- DT-033 `.AsSplitQuery()` em queries de OS com multiplas colecoes (corrigido)
- DT-034 SQL Server 2022 alinhado na documentacao (corrigido)
- DT-035..DT-039 Limitacoes MVP de ordem de servico documentadas (documentado)
- DT-017..DT-020 Limitacoes tecnicas pos-MVP documentadas (documentado)
- DT-041 Achados Sonar em `Program.cs` tratados (corrigido)
- DT-042 Security hotspot do Dockerfile tratado com usuario nao-root (corrigido)
- DT-043 Seed de OS com parametros agrupados (corrigido)
- DT-044 Loop de estoque simplificado (corrigido)
- DT-045 `Enum.IsDefined` generico (corrigido)
- DT-046 Pesos CNPJ como `static readonly` (corrigido)
- DT-047 Fixture de integracao com retorno concreto (corrigido)
- DC-003 `.env.example` criado e fluxo Docker documentado (corrigido)
- DT-040 Backlog final de entrega consolidado com P0/P1/P2 rastreaveis (corrigido)

## Riscos Residuais Aceitos Para MVP

- Autorizacao de cliente ainda pode ser simplificada se nao houver vinculo real `UsuarioId -> ClienteId`.
- Estoque como aggregate unico global.
- Numero da OS via `MAX + 1` com risco teorico de concorrencia.
- Repositories salvando diretamente com `SaveChangesAsync`, apesar do `IUnitOfWork`.
- Login demo com senha em texto puro.
- Ausencia de analise OWASP ZAP, se nao houver tempo para executar.
- Logging ainda basico.
- Envio real de orcamento ao cliente nao implementado, se o fluxo continuar externo/manual.
- Aprovacao de orcamento implicita em `IniciarExecucao`.

## Fluxo Git Do Projeto

- Toda correcao deve sair de uma branch de trabalho criada a partir da base combinada.
- Nao fazer commits diretamente na `develop`.
- Usar commits convencionais no formato `<type>(scope): <description>`.
- Separar commits por intencao tecnica quando houver mudancas de naturezas diferentes.
- Antes de push ou PR, rodar ao menos `dotnet build --nologo` e os testes aplicaveis.
- Abrir PR para revisao e merge, mantendo a `develop` como linha de integracao.

## Observacao Operacional

- Testes de integracao dependem de Docker ativo.
- Testes marcados com `RequiresDockerFactAttribute` sao ignorados quando Docker nao estiver acessivel.
- Para forcar skip local dos cenarios que dependem de Docker, definir `OFICINA_SKIP_DOCKER_TESTS=true`.
- Migrations geradas pelo Entity Framework nao devem ser reformatadas manualmente para evitar ruido em codigo gerado.
- Evidencias SonarQube devem vir do dashboard/analise do SonarQube, nao apenas de `TestResults` do Coverlet.
