# Backlog Tecnico Consolidado

Ultima atualizacao: 2026-05-20
Fonte de consolidacao: `code_review_v4_final.md` + `FIAP/Snapshots projeto/Code Reviews/backlog_tecnico.md` + validacao local em `develop`.

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

## Status Atual Dos Itens Pendentes

| ID | Prioridade | Tipo | Item | Status | Evidencia atual | Proxima acao |
| --- | --- | --- | --- | --- | --- | --- |
| DT-011 | P2 | Convencao C# | Enums em `SCREAMING_CASE` | Pendente | `StatusOrdemServico` e `StatusServico` ainda usam valores como `RECEBIDA` | Planejar ajuste com impacto em JSON/testes (breaking change controlado) |
| DT-013 | P2 | DevEx | Padronizar formatacao global | Pendente | Estilos de codigo ainda heterogeneos entre modulos | Rodar formatacao automatica e revisar diff por camada |
| DT-014 | P2 | Qualidade | Relatorio Sonar/cobertura | Pendente | Sem evidencia versionada de analise estatica/cobertura | Gerar relatorio e anexar evidencias |
| DT-016 | P2 | Seguranca | Analise OWASP ZAP | Pendente | Sem relatorio de runtime | Rodar ZAP na API local e anexar saida |
| DT-017 | P2 | Observabilidade | Logging estruturado | Pendente | Logs ainda focados em startup/seed/migrations | Definir layout de logs e correlacao de requisicoes |
| DT-018 | P3 | Identidade | Login demo com senha em texto puro | Pendente | Modelo atual e academico | Evoluir para hash + provider de identidade em fase pos-MVP |
| DT-019 | P3 | API | Response de autenticacao pode ser simplificado | Pendente | Retorna token + login + perfil | Reavaliar apos MVP e compatibilidade de clientes |
| DT-020 | P3 | DDD/Escala | `Estoque` como aggregate unico global | Pendente | Modelo atual atende MVP | Reavaliar modelagem quando volume/carga aumentar |

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
- DT-012 Erros de validacao retornam lista completa no campo `erros` (corrigido)
- DT-015 Auditoria de vulnerabilidade NuGet sem pacotes vulneraveis encontrados (corrigido)
- DT-024 Fluxo Git formalizado com branch por tarefa, commits convencionais e PR (corrigido)

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
