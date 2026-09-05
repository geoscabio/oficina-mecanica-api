# Backlog tecnico

Este backlog guarda melhorias tecnicas, itens de codigo e evolucoes operacionais que nao precisam entrar imediatamente na entrega principal. A ideia e separar bem o que e escopo do Tech Challenge do que e maturidade de producao para depois.

## Como usar

| Campo | Regra |
| --- | --- |
| Prioridade | `P0` obrigatorio para a entrega, `P1` importante se couber, `P2` pos-entrega, `P3` oportunidade futura. |
| Horizonte | `Fase 3`, `Antes da demo`, `Pos-entrega` ou `Pesquisa`. |
| Status | `A fazer`, `Em andamento`, `Bloqueado`, `Concluido` ou `Nao priorizado agora`. |
| Criterio de aceite | Evidencia objetiva de que o item saiu do backlog. |

## Visao de prioridade

| Prioridade | Significado | Decisao pratica |
| --- | --- | --- |
| `P0` | Necessario para cumprir o Tech Challenge com seguranca. | Fazer antes da entrega. |
| `P1` | Melhora clareza, demonstracao ou confiabilidade. | Fazer se nao ameaçar o prazo. |
| `P2` | Padrao mais proximo de empresa em producao. | Guardar para depois da entrega. |
| `P3` | Ideia tecnica, pesquisa ou refinamento. | Reavaliar quando o produto estiver estavel. |

## Escopo da Fase 3

| ID | Prioridade | Horizonte | Area | Item | Criterio de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `F3-001` | `P0` | Fase 3 | Autenticacao | Criar Lambda de autenticacao por CPF emitindo JWT. | `POST /auth/cpf` retorna token para cliente ativo e erro controlado para CPF invalido, inexistente ou inativo. | A fazer |
| `F3-002` | `P0` | Fase 3 | API | Manter a API protegida validando JWT internamente. | Rotas protegidas retornam `401` sem token e funcionam com token valido. | A fazer |
| `F3-003` | `P0` | Fase 3 | Banco | Usar banco gerenciado no RDS. | RDS provisionado por Terraform e usado pela API/Lambda em ambiente de demonstracao. | A fazer |
| `F3-004` | `P0` | Fase 3 | Kubernetes | Executar a API em Kubernetes com escalabilidade. | API publicada no EKS, healthcheck funcional e HPA evidenciado. | A fazer |
| `F3-005` | `P0` | Fase 3 | API Gateway | Expor a entrada publica via API Gateway. | Gateway roteia `/auth/*` para Lambda e `/api/*` para API no Kubernetes. | A fazer |
| `F3-006` | `P0` | Fase 3 | Terraform | Separar infraestrutura em repositorios/esteiras por recurso. | Repositorios criados com README, Terraform, CI e instrucoes de apply/destroy. | A fazer |
| `F3-007` | `P0` | Fase 3 | CI/CD | Manter branch protegida, PR obrigatorio e quality gate. | Branches principais protegidas, PR, aprovacao e CI exigidos antes do merge. | Em andamento |
| `F3-008` | `P0` | Fase 3 | Observabilidade | Enviar logs, metricas e traces para Datadog. | Datadog mostra API, Lambda, Gateway e Kubernetes com tags padronizadas. | A fazer |
| `F3-009` | `P0` | Fase 3 | Observabilidade | Criar dashboards e alertas pedidos no enunciado. | Evidencias de latencia, CPU/memoria, healthcheck, uptime e falhas de ordem de servico. | A fazer |
| `F3-010` | `P0` | Fase 3 | Documentacao | Consolidar diagramas, ADRs/RFCs, video e PDF final. | Documentacao explica requisitos, decisoes, execucao e evidencias da entrega. | A fazer |

## Setup inicial de novos repositorios

| ID | Prioridade | Horizonte | Area | Item | Criterio de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `SETUP-001` | `P0` | Fase 3 | Repositorio | Criar repositorio com nome padronizado `oficina-mecanica-*`. | Nome reflete o recurso e aparece no plano da Fase 3. | Em andamento |
| `SETUP-002` | `P0` | Fase 3 | Branches | Criar `main`, `develop` e `release` quando aplicavel. | Branches existem antes do primeiro fluxo de PR. | Em andamento |
| `SETUP-003` | `P0` | Fase 3 | Branch protection | Aplicar protecao nas branches principais. | Push direto, force push e delecao ficam bloqueados; PR, aprovacao e CI sao obrigatorios. | Em andamento |
| `SETUP-004` | `P0` | Fase 3 | CI/CD | Criar workflow de CI minimo para cada tipo de repo. | PR executa validacao compativel com o repositorio: `.NET`, Terraform, Lambda ou manifests. | A fazer |
| `SETUP-005` | `P0` | Fase 3 | Documentacao | Criar README inicial com objetivo, stack, execucao e deploy. | README permite entender o papel do repo sem depender de conversa externa. | A fazer |
| `SETUP-006` | `P1` | Antes da demo | Operacao | Padronizar repository variables, secrets e environments. | Variaveis obrigatorias documentadas e cadastradas antes da esteira real. | A fazer |
| `SETUP-007` | `P1` | Antes da demo | Governanca | Confirmar acesso do usuario `soat-architecture`. | Usuario aparece com acesso exigido em todos os repositorios da entrega. | A fazer |

## Backlog de codigo e qualidade

| ID | Prioridade | Horizonte | Area | Item | Criterio de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `CODE-001` | `P0` | Fase 3 | Dominio | Adicionar status do cliente para suportar autenticacao por CPF. | Modelo, migration, seed/demo e testes cobrem cliente ativo e inativo. | A fazer |
| `CODE-002` | `P0` | Fase 3 | Seguranca | Evitar CPF puro em logs e respostas tecnicas. | Logs usam `cliente_id`, CPF mascarado ou hash; nenhum log registra CPF completo. | A fazer |
| `CODE-003` | `P0` | Fase 3 | Testes | Cobrir fluxo de autenticacao e autorizacao. | Testes validam sucesso, CPF invalido, cliente inexistente, cliente inativo e rota protegida. | A fazer |
| `CODE-004` | `P1` | Antes da demo | Contratos | Atualizar OpenAPI e Postman para os fluxos da Fase 3. | Colecoes e ambientes permitem demonstrar autenticacao por CPF e consumo com JWT. | A fazer |
| `CODE-005` | `P1` | Antes da demo | Observabilidade | Padronizar `X-Correlation-Id`, `dd.trace_id` e `dd.span_id`. | Logs da API e Lambda permitem seguir a mesma requisicao ponta a ponta. | A fazer |
| `CODE-006` | `P2` | Pos-entrega | Banco | Mover migrations e seed inicial do startup da API para Kubernetes Job versionado. | Deploy da API nao executa migration automaticamente no startup. | Nao priorizado agora |
| `CODE-007` | `P2` | Pos-entrega | Banco | Avaliar lock distribuido para migrations concorrentes. | Estrategia definida, por exemplo com `sp_getapplock`, antes de escalar replicas com migration automatica. | Nao priorizado agora |
| `CODE-008` | `P2` | Pos-entrega | Qualidade | Adicionar analise de dependencias e vulnerabilidades. | Pipeline publica resultado de auditoria de pacotes sem bloquear indevidamente a entrega academica. | Nao priorizado agora |

## Backlog operacional pos-entrega

| ID | Prioridade | Horizonte | Area | Item | Criterio de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `OPS-001` | `P2` | Pos-entrega | Hotfix | Definir fluxo `hotfix/*` a partir de `main`. | Hotfix entra por PR para `main`, passa por CI/aprovacao e depois e sincronizado para `develop` e `release`. | Nao priorizado agora |
| `OPS-002` | `P2` | Pos-entrega | Rollback | Criar rollback manual por versao/tag de imagem. | Esteira permite redeploy de uma versao anterior conhecida como boa. | Nao priorizado agora |
| `OPS-003` | `P2` | Pos-entrega | Rollback | Avaliar rollback automatico apos falha de healthcheck. | Rollback automatico e limitado a deploy de aplicacao e nunca reverte banco de forma destrutiva sem aprovacao humana. | Nao priorizado agora |
| `OPS-004` | `P2` | Pos-entrega | Banco | Definir politica de rollback de migrations. | Migrations possuem estrategia segura para forward fix, compatibilidade ou rollback manual controlado. | Nao priorizado agora |
| `OPS-005` | `P2` | Pos-entrega | Infra | Evoluir RDS para Multi-AZ, backups e snapshot final. | Ambiente deixa de depender das simplificacoes da AWS Academy. | Nao priorizado agora |
| `OPS-006` | `P2` | Pos-entrega | Segredos | Formalizar rotacao de secrets e chaves JWT. | Segredos possuem dono, periodicidade e procedimento de rotacao. | Nao priorizado agora |
| `OPS-007` | `P2` | Pos-entrega | DNS | Trocar hostname bruto do Load Balancer por DNS amigavel. | API usa dominio proprio com Route 53 ou provedor equivalente. | Nao priorizado agora |
| `OPS-008` | `P3` | Pesquisa | Plataforma | Avaliar trunk-based development como alternativa ao Git Flow. | Decisao documentada somente se houver ganho real para o contexto do time. | Nao priorizado agora |

## Fora do escopo imediato

Estes itens ficam registrados para nao serem esquecidos, mas nao devem competir com a entrega da Fase 3:

- rollback automatico completo;
- fluxo formal de hotfix;
- banco Multi-AZ com politica completa de backup;
- DNS com dominio proprio;
- rotacao corporativa de segredos;
- esteira corporativa com change management;
- plataforma compartilhada de templates para todos os repositorios.

## Proxima revisao

Antes de iniciar uma nova frente, revisar primeiro os itens `P0`. Depois da entrega, reabrir os itens `P2` e decidir o que vira melhoria real do projeto e o que continua apenas como referencia de arquitetura.
