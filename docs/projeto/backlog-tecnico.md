# Backlog técnico

Este backlog guarda melhorias técnicas, itens de código e evoluções operacionais que não precisam entrar imediatamente na entrega principal. A ideia é separar bem o que é escopo do Tech Challenge do que é maturidade de produção para depois.

## Como usar

| Campo | Regra |
| --- | --- |
| Prioridade | `P0` obrigatório para a entrega, `P1` importante se couber, `P2` pós-entrega, `P3` oportunidade futura. |
| Horizonte | `Fase 3`, `Antes da demo`, `Pós-entrega` ou `Pesquisa`. |
| Status | `A fazer`, `Em andamento`, `Bloqueado`, `Concluído` ou `Não priorizado agora`. |
| Critério de aceite | Evidência objetiva de que o item saiu do backlog. |

## Visão de prioridade

| Prioridade | Significado | Decisão prática |
| --- | --- | --- |
| `P0` | Necessário para cumprir o Tech Challenge com segurança. | Fazer antes da entrega. |
| `P1` | Melhora clareza, demonstração ou confiabilidade. | Fazer se não ameaçar o prazo. |
| `P2` | Padrão mais próximo de empresa em produção. | Guardar para depois da entrega. |
| `P3` | Ideia técnica, pesquisa ou refinamento. | Reavaliar quando o produto estiver estável. |

## Escopo da Fase 3

| ID | Prioridade | Horizonte | Área | Item | Critério de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `F3-001` | `P0` | Fase 3 | Autenticação | Criar Lambda de autenticação por CPF emitindo JWT. | `POST /auth/cpf` retorna token para cliente ativo e erro controlado para CPF inválido, inexistente ou inativo. | A fazer |
| `F3-002` | `P0` | Fase 3 | API | Manter a API protegida validando JWT internamente. | Rotas protegidas retornam `401` sem token e funcionam com token válido. | A fazer |
| `F3-003` | `P0` | Fase 3 | Banco | Usar banco gerenciado no RDS. | RDS provisionado por Terraform e usado pela API/Lambda em ambiente de demonstração. | A fazer |
| `F3-004` | `P0` | Fase 3 | Kubernetes | Executar a API em Kubernetes com escalabilidade. | API publicada no EKS, healthcheck funcional e HPA evidenciado. | Em andamento |
| `F3-005` | `P0` | Fase 3 | API Gateway | Expor a entrada pública via API Gateway. | Gateway roteia `/auth/*` para Lambda e `/api/*` para API no Kubernetes. | A fazer |
| `F3-006` | `P0` | Fase 3 | Terraform | Separar infraestrutura em repositórios/esteiras por recurso. | Repositórios criados com README, Terraform, CI e instruções de apply/destroy. | Em andamento |
| `F3-007` | `P0` | Fase 3 | CI/CD | Manter branch protegida, PR obrigatório e quality gate. | Branches principais protegidas, PR, aprovação e CI exigidos antes do merge. | Em andamento |
| `F3-008` | `P0` | Fase 3 | Observabilidade | Enviar logs, métricas e traces para Datadog. | Datadog mostra API, Lambda, Gateway e Kubernetes com tags padronizadas. | A fazer |
| `F3-009` | `P0` | Fase 3 | Observabilidade | Criar dashboards e alertas pedidos no enunciado. | Evidências de latência, CPU/memória, healthcheck, uptime e falhas de ordem de serviço. | A fazer |
| `F3-010` | `P0` | Fase 3 | Documentação | Consolidar diagramas, ADRs/RFCs, vídeo e PDF final. | Documentação explica requisitos, decisões, execução e evidências da entrega. | A fazer |

## Setup inicial de novos repositórios

| ID | Prioridade | Horizonte | Área | Item | Critério de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `SETUP-001` | `P0` | Fase 3 | Repositório | Criar repositório com nome padronizado `oficina-mecanica-*`. | Nome reflete o recurso e aparece no plano da Fase 3. | Em andamento |
| `SETUP-002` | `P0` | Fase 3 | Branches | Criar `main`, `develop` e `release` quando aplicável. | Branches existem antes do primeiro fluxo de PR. | Em andamento |
| `SETUP-003` | `P0` | Fase 3 | Branch protection | Aplicar proteção nas branches principais. | Push direto, force push e deleção ficam bloqueados; PR, aprovação, CI e bypass via PR para maintain/admin ficam configurados. | Em andamento |
| `SETUP-004` | `P0` | Fase 3 | CI/CD | Criar workflow de CI mínimo para cada tipo de repo. | PR executa validação compatível com o repositório: `.NET`, Terraform, Lambda ou manifests. | Em andamento |
| `SETUP-005` | `P0` | Fase 3 | Documentação | Criar README inicial com objetivo, stack, execução e deploy. | README permite entender o papel do repo sem depender de conversa externa. | Em andamento |
| `SETUP-006` | `P1` | Antes da demo | Operação | Padronizar repository variables, secrets e environments. | Variáveis obrigatórias documentadas e cadastradas antes da esteira real. | A fazer |
| `SETUP-007` | `P1` | Antes da demo | Governança | Confirmar acesso do usuário `soat-architecture`. | Usuário aparece com acesso exigido em todos os repositórios da entrega. | A fazer |
| `SETUP-008` | `P0` | Fase 3 | CI/CD | Padronizar todos os fluxos de CI/CD com o desenho da API. | VPC, Kubernetes e próximas esteiras usam o mesmo encadeamento: `🧪 CI Development`, `🔎 CI Release`, `🛡️ CI Production`, `🚀 CD Development`, `☁️ AWS Deploy`, `🔀 CD Release` e `🏁 CD Production`, mudando apenas a responsabilidade técnica de cada job. | Em andamento |
| `SETUP-009` | `P1` | Antes da demo | Documentação | Revisar acentuação e português dos textos das esteiras. | Workflows, READMEs e docs de todas as esteiras ficam legíveis em português, sem palavras sem acento por padronização manual ou mojibake. | A fazer |

## Backlog de código e qualidade

| ID | Prioridade | Horizonte | Área | Item | Critério de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `CODE-001` | `P0` | Fase 3 | Domínio | Adicionar status do cliente para suportar autenticação por CPF. | Modelo, migration, seed/demo e testes cobrem cliente ativo e inativo. | A fazer |
| `CODE-002` | `P0` | Fase 3 | Segurança | Evitar CPF puro em logs e respostas técnicas. | Logs usam `cliente_id`, CPF mascarado ou hash; nenhum log registra CPF completo. | A fazer |
| `CODE-003` | `P0` | Fase 3 | Testes | Cobrir fluxo de autenticação e autorização. | Testes validam sucesso, CPF inválido, cliente inexistente, cliente inativo e rota protegida. | A fazer |
| `CODE-004` | `P1` | Antes da demo | Contratos | Atualizar OpenAPI e Postman para os fluxos da Fase 3. | Coleções e ambientes permitem demonstrar autenticação por CPF e consumo com JWT. | A fazer |
| `CODE-005` | `P1` | Antes da demo | Observabilidade | Padronizar `X-Correlation-Id`, `dd.trace_id` e `dd.span_id`. | Logs da API e Lambda permitem seguir a mesma requisição ponta a ponta. | A fazer |
| `CODE-006` | `P2` | Pós-entrega | Banco | Mover migrations e seed inicial do startup da API para Kubernetes Job versionado. | Deploy da API não executa migration automaticamente no startup. | Não priorizado agora |
| `CODE-007` | `P2` | Pós-entrega | Banco | Avaliar lock distribuído para migrations concorrentes. | Estratégia definida, por exemplo com `sp_getapplock`, antes de escalar réplicas com migration automática. | Não priorizado agora |
| `CODE-008` | `P2` | Pós-entrega | Qualidade | Adicionar análise de dependências e vulnerabilidades. | Pipeline publica resultado de auditoria de pacotes sem bloquear indevidamente a entrega acadêmica. | Não priorizado agora |

## Backlog operacional pós-entrega

| ID | Prioridade | Horizonte | Área | Item | Critério de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `OPS-001` | `P2` | Pós-entrega | Hotfix | Definir fluxo `hotfix/*` a partir de `main`. | Hotfix entra por PR para `main`, passa por CI/aprovação e depois é sincronizado para `develop` e `release`. | Não priorizado agora |
| `OPS-002` | `P2` | Pós-entrega | Rollback | Criar rollback manual por versão/tag de imagem. | Esteira permite redeploy de uma versão anterior conhecida como boa. | Não priorizado agora |
| `OPS-003` | `P2` | Pós-entrega | Rollback | Avaliar rollback automático após falha de healthcheck. | Rollback automático é limitado a deploy de aplicação e nunca reverte banco de forma destrutiva sem aprovação humana. | Não priorizado agora |
| `OPS-004` | `P2` | Pós-entrega | Banco | Definir política de rollback de migrations. | Migrations possuem estratégia segura para forward fix, compatibilidade ou rollback manual controlado. | Não priorizado agora |
| `OPS-005` | `P2` | Pós-entrega | Infra | Evoluir RDS para Multi-AZ, backups e snapshot final. | Ambiente deixa de depender das simplificações da AWS Academy. | Não priorizado agora |
| `OPS-006` | `P2` | Pós-entrega | Segredos | Formalizar rotação de secrets e chaves JWT. | Segredos possuem dono, periodicidade e procedimento de rotação. | Não priorizado agora |
| `OPS-007` | `P2` | Pós-entrega | DNS | Trocar hostname bruto do Load Balancer por DNS amigável. | API usa domínio próprio com Route 53 ou provedor equivalente. | Não priorizado agora |
| `OPS-008` | `P3` | Pesquisa | Plataforma | Avaliar trunk-based development como alternativa ao Git Flow. | Decisão documentada somente se houver ganho real para o contexto do time. | Não priorizado agora |

## Fora do escopo imediato

Estes itens ficam registrados para não serem esquecidos, mas não devem competir com a entrega da Fase 3:

- rollback automático completo;
- fluxo formal de hotfix;
- banco Multi-AZ com política completa de backup;
- DNS com domínio próprio;
- rotação corporativa de segredos;
- esteira corporativa com change management;
- plataforma compartilhada de templates para todos os repositórios.

## Próxima revisão

Antes de iniciar uma nova frente, revisar primeiro os itens `P0`. Depois da entrega, reabrir os itens `P2` e decidir o que vira melhoria real do projeto e o que continua apenas como referência de arquitetura.
