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

## P0 - Obrigatorio Antes Da Entrega

| ID | Origem | Item | Status | Evidencia atual | Proxima acao |
| --- | --- | --- | --- | --- | --- |
| DT-014 | Tech Challenge / Backlog | Gerar evidencia SonarQube com Quality Gate, bugs, vulnerabilities, security hotspots, code smells, coverage e duplications | Em andamento | Dashboard local gerado; Quality Gate `OK`, bugs `0`, vulnerabilities `0`, hotspots `1`, code smells `17`, coverage `89.9%`, duplications `0.2%`; evidencia em `docs/evidencias/sonarqube_analise_2026-05-20.md` | Exportar o dashboard do SonarQube em PDF pelo navegador |
| DT-025 | Review final | README com criacao de `.env` multiplataforma | Corrigido | README documenta PowerShell e Bash/Git Bash/macOS/Linux | Nenhuma |
| DT-026 | Review final | README diferenciando Docker de .NET SDK local | Corrigido | README separa pre-requisitos de Docker Compose e build/testes locais | Nenhuma |
| DT-027 | Review final | Revisar TODOs de entrega e documentacao | Pendente | Sem evidencia de revisao final dos TODOs visiveis | Buscar TODO/FIXME/Notion pendente e fechar/remover o que estiver obsoleto |
| DT-028 | Review final | Evidencia final de execucao | Pendente | Ha historico de comandos no fluxo, mas falta evidencia consolidada de entrega | Registrar em `docs/evidencias` ou README: `dotnet test`, `docker compose up --build` e observacao sobre Docker nos integrados |
| DT-029 | Auditoria final | Garantir que arquivos locais/snapshot nao estejam versionados | Corrigido | `git ls-files .vs "*.csproj.user" "*snapshot*" "Contexto para IA.zip"` sem retorno; `.sonarqube/` tambem foi ignorado | Nenhuma |

## P1 - Correcoes Pequenas De Codigo Recomendadas

| ID | Origem | Item | Status | Evidencia atual | Proxima acao |
| --- | --- | --- | --- | --- | --- |
| DT-030 | Claude | `Servico`, `PecaInsumo` e `ItemEstoque` sem construtor privado sem parametros | Pendente | Entidades filhas ainda precisam de hardening EF Core | Adicionar construtores privados sem parametros e validar build/testes |
| DT-031 | Claude | `OrdemServico.DataInicio` nullable sem necessidade | Pendente | `DataInicio` representa dado sempre preenchido no dominio | Alterar para `DateTime`, ajustar response/mapping/testes e manter `DataFim` nullable |
| DT-032 | Claude | `Endereco` Value Object com construtor publico | Pendente | Padrao difere de outros Value Objects com factory | Criar `Endereco.Criar(...)`, construtor privado e atualizar consumidores/testes |
| DT-033 | Review final | Warnings EF Core por multiplos `Include` de colecoes | Pendente | Queries de OS podem carregar `Servicos` e `PecasInsumos` juntas | Adicionar `.AsSplitQuery()` nas queries aplicaveis de `OrdemServicoRepository` |
| DT-034 | Auditoria final | ADR/documentacao menciona SQL Server 2025, mas Docker/Testcontainers usam SQL Server 2022 | Pendente | Ambiente validado usa `mcr.microsoft.com/mssql/server:2022-latest` | Alinhar ADR/docs para SQL Server 2022 como versao validada do MVP |

## P2 - Documentar Como Limitacao MVP Ou Pos-MVP

| ID | Origem | Item | Status | Evidencia atual | Proxima acao |
| --- | --- | --- | --- | --- | --- |
| DT-035 | Auditoria final | Aprovacao de orcamento implicita em `IniciarExecucao` | Pendente | Fluxo nao possui endpoint/evento explicito de aprovacao do cliente | Documentar que, no MVP, iniciar execucao representa orcamento aprovado |
| DT-036 | Auditoria final | Estoque insuficiente bloqueia reserva, mas Domain Storytelling fala em cancelar OS | Pendente | Dominio retorna erro e nao cancela automaticamente OS | Documentar decisao MVP ou implementar cancelamento automatico se for simples/seguro |
| DT-037 | Auditoria final | Cliente com role `Cliente` pode consultar status de qualquer OS conhecendo o ID | Pendente | JWT demo nao vincula usuario real a `ClienteId` | Documentar limitacao do JWT demo ou criar validacao de posse se couber no tempo |
| DT-038 | Auditoria final | Repositories chamam `SaveChangesAsync` mesmo existindo `IUnitOfWork` | Pendente | Padrao misto permanece fora dos fluxos transacionais principais | Manter como debito pos-MVP, sem refatoracao ampla antes da entrega |
| DT-039 | Auditoria final | Numero da OS via `MAX + 1` tem risco teorico de concorrencia | Pendente | Implementacao atende MVP, mas nao e segura para alta concorrencia | Documentar limitacao MVP ou criar debito especifico para sequencia atomica |
| DT-016 | Backlog | Analise OWASP ZAP | Pendente | Sem relatorio de runtime | Rodar ZAP na API local e anexar saida se houver tempo |
| DT-017 | Backlog | Logging estruturado | Pendente | Logs ainda focados em startup/seed/migrations | Manter pos-MVP ou definir minimo de correlacao de requisicoes |
| DT-018 | Backlog | Login demo com senha em texto puro | Pendente | Modelo atual e academico | Documentar como demo academico e pos-MVP para hash/provider real |
| DT-019 | Backlog | Response de autenticacao pode ser simplificado | Pendente | Retorna token + login + perfil | Reavaliar apos MVP e compatibilidade de clientes |
| DT-020 | Backlog | `Estoque` como aggregate unico global | Pendente | Modelo atual atende MVP | Reavaliar modelagem quando volume/carga aumentar |

## Checklist Final De Entrega

| Area | Item | Status |
| --- | --- | --- |
| Codigo | Construtores EF em entidades filhas | Pendente |
| Codigo | `DataInicio` nao-nullable | Pendente |
| Codigo | `Endereco` com factory de Value Object | Pendente |
| Codigo | `.AsSplitQuery()` em multiplos Includes de OS | Pendente |
| Codigo | Nenhum `.vs/` ou `.csproj.user` versionado | Corrigido |
| Codigo | ADR SQL Server alinhada com Docker/Testcontainers | Pendente |
| Documentacao | README com `.env` para Windows e Mac/Linux | Corrigido |
| Documentacao | README explicando Docker vs .NET SDK local | Corrigido |
| Documentacao | README/evidencia com validacao final | Pendente |
| Documentacao | Limitacoes MVP documentadas | Pendente |
| Documentacao | Backlog tecnico atualizado | Corrigido |
| Documentacao | TODOs de entrega revisados | Pendente |
| Validacao | `dotnet build --nologo` passou apos ajustes finais | Pendente |
| Validacao | `dotnet test --nologo` passou apos ajustes finais | Pendente |
| Validacao | `docker compose up --build` validado | Pendente |
| Evidencias | SonarQube PDF para professor | Em andamento |
| Evidencias | Auditoria NuGet | Corrigido |
| Evidencias | Evidencia de testes/build/Docker | Pendente |

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
- DT-029 Arquivos locais e snapshots fora do versionamento (corrigido)
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
