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
| `F3-001` | `P0` | Fase 3 | Autenticação | Criar uma Lambda de autenticação por documento, em repositório próprio, emitindo JWT somente para Cliente ativo. | `POST /auth/documento` valida e identifica CPF ou CNPJ, consulta por `Documento + TipoDocumento` e emite JWT com os claims aprovados; o cenário CPF é obrigatório para aceite e demonstração da Fase 3. | Bloqueado |
| `F3-002` | `P0` | Fase 3 | API | Alinhar a API ao contrato único de JWT e manter a autorização dentro da aplicação. | Contrato-base JWT alinhado e compatibilidade com futuro token de Cliente preparada; a exigência definitiva de `cliente_id` e a remoção do perfil `Cliente` do login interno permanecem condicionadas ao fluxo por documento para CPF e CNPJ em `F3-001`. | Em andamento |
| `F3-003` | `P0` | Fase 3 | Banco | Usar banco gerenciado no RDS, com credenciais e rede compatíveis com os consumidores da Fase 3. | RDS provisionado por Terraform, credenciais em Secrets Manager, referência não secreta publicada somente após decisão e acesso da API/Lambda comprovado em ambiente de demonstração. | Em andamento |
| `F3-004` | `P0` | Fase 3 | Kubernetes | Executar a API em Kubernetes com escalabilidade. | API publicada no EKS, healthcheck funcional e HPA evidenciado. | Em andamento |
| `F3-005` | `P0` | Fase 3 | API Gateway | Expor a entrada pública via API Gateway, depois de fechar o contrato da Lambda e da integração. | Gateway usa o tipo de payload acordado, roteia `POST /auth/documento` para Lambda e `/api/*` para API no Kubernetes sem transformar silenciosamente request, response ou erros. | Bloqueado |
| `F3-006` | `P0` | Fase 3 | Terraform | Separar infraestrutura em repositórios/esteiras por recurso. | Repositórios criados com README, Terraform, CI e instruções de apply/destroy; a regra de entrada 1433 do RDS é de responsabilidade de `oficina-mecanica-infra-rds`. | Em andamento |
| `F3-007` | `P0` | Fase 3 | CI/CD | Manter branch protegida, PR obrigatório e quality gate. | Branches principais protegidas, PR, aprovação e CI exigidos antes do merge; a nova Lambda nasce usando o padrão maduro de CI/CD, sem herdar divergências históricas. | Em andamento |
| `F3-008` | `P0` | Fase 3 | Observabilidade | Enviar logs, métricas e traces para Datadog. | Datadog mostra API, Lambda, Gateway e Kubernetes com tags padronizadas. | A fazer |
| `F3-009` | `P0` | Fase 3 | Observabilidade | Criar dashboards e alertas pedidos no enunciado. | Evidências de latência, CPU/memória, healthcheck, uptime e falhas de ordem de serviço. | A fazer |
| `F3-010` | `P0` | Fase 3 | Documentação | Consolidar diagramas, ADRs/RFCs, vídeo e PDF final. | Documentação explica requisitos, decisões, execução e evidências da entrega. | A fazer |

## Setup inicial de novos repositórios

| ID | Prioridade | Horizonte | Área | Item | Critério de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `SETUP-001` | `P0` | Fase 3 | Repositório | Criar repositório com nome padronizado `oficina-mecanica-*`. | Nome reflete o recurso e aparece no plano da Fase 3. | Em andamento |
| `SETUP-002` | `P0` | Fase 3 | Branches | Criar `main`, `develop` e `release` quando aplicável. | Branches existem antes do primeiro fluxo de PR. | Em andamento |
| `SETUP-003` | `P0` | Fase 3 | Branch protection | Aplicar proteção nas branches principais. | Dois rulesets ativos: Git Flow sem bypass exige PR, validação de origem, Quality gate e bloqueio de push direto, force push e deleção; aprovação humana separada permite bypass via PR para os integrantes autorizados e maintain/admin. Repetir o padrão em cada novo repositório e revisar alterações nos workflows de validação. | Em andamento |
| `SETUP-004` | `P0` | Fase 3 | CI/CD | Criar workflow de CI mínimo para cada tipo de repo. | PR executa validação compatível com o repositório: `.NET`, Terraform, Lambda ou manifests; a Lambda usa `ci.yml`, quality gate e jobs equivalentes às esteiras maduras. | Em andamento |
| `SETUP-005` | `P0` | Fase 3 | Documentação | Criar README inicial com objetivo, stack, execução e deploy. | README permite entender o papel do repo sem depender de conversa externa. | Em andamento |
| `SETUP-006` | `P1` | Antes da demo | Operação | Padronizar repository variables, secrets e environments. | Variáveis obrigatórias documentadas e cadastradas antes da esteira real. | A fazer |
| `SETUP-007` | `P1` | Antes da demo | Governança | Confirmar acesso do usuário `soat-architecture`. | Usuário aparece com acesso exigido em todos os repositórios da entrega. | A fazer |
| `SETUP-008` | `P0` | Fase 3 | CI/CD | Padronizar os novos fluxos de CI/CD pelo padrão maduro atual. | VPC, Kubernetes, API e a nova Lambda usam `ci.yml` com os estágios compatíveis (`🧪 CI Development`, `🔎 CI Release`, `🛡️ CI Production`, `🚀 CD Development`, `☁️ AWS Deploy`, `🔀 CD Release` e `🏁 CD Production`); o check obrigatório no ruleset corresponde ao Quality gate. A divergência legada do RDS é tratada em `PIPE-001` a `PIPE-004`, sem bloquear a Lambda. | Em andamento |
| `SETUP-009` | `P1` | Antes da demo | Documentação | Revisar acentuação e português dos textos das esteiras. | Workflows, READMEs e docs de todas as esteiras ficam legíveis em português, sem palavras sem acento por padronização manual ou mojibake. | A fazer |

## Backlog de código e qualidade

| ID | Prioridade | Horizonte | Área | Item | Critério de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `CODE-001` | `P0` | Fase 3 | Domínio | Adicionar `StatusCliente` para suportar autenticação de Cliente por documento. | Aggregate/modelo, mapeamento EF, migration e snapshot são atualizados; seed/demo possui cliente ativo e inativo; testes cobrem os documentos CPF e CNPJ nos dois estados. A representação física do status é documentada antes da migration. | Concluído |
| `CODE-002` | `P0` | Fase 3 | Segurança | Evitar CPF/CNPJ puro em logs, respostas técnicas e JWT. | Logs usam `cliente_id` ou documento mascarado; nenhum log registra CPF/CNPJ completo e o JWT não transporta documento ou hashes sem necessidade funcional. | A fazer |
| `CODE-003` | `P0` | Fase 3 | Testes | Cobrir fluxo de autenticação e autorização por documento. | Testes validam CPF e CNPJ válidos, documento inválido, cliente inexistente, cliente inativo, `401` sem token, claims sem documento e autorização por papel; o cenário CPF é obrigatório para a demonstração. | Bloqueado |
| `CODE-004` | `P1` | Antes da demo | Contratos | Atualizar OpenAPI e Postman para os fluxos da Fase 3. | Coleções e ambientes permitem demonstrar `POST /auth/documento` com CPF (obrigatório) e CNPJ, além do consumo com JWT. | A fazer |
| `CODE-005` | `P1` | Antes da demo | Observabilidade | Padronizar `X-Correlation-Id`, `dd.trace_id` e `dd.span_id`. | Logs da API e Lambda permitem seguir a mesma requisição ponta a ponta. | A fazer |
| `CODE-006` | `P2` | Pós-entrega | Banco | Mover migrations e seed inicial do startup da API para Kubernetes Job versionado. | Deploy da API não executa migration automaticamente no startup. | Não priorizado agora |
| `CODE-007` | `P2` | Pós-entrega | Banco | Avaliar lock distribuído para migrations concorrentes. | Estratégia definida, por exemplo com `sp_getapplock`, antes de escalar réplicas com migration automática. | Não priorizado agora |
| `CODE-008` | `P2` | Pós-entrega | Qualidade | Adicionar análise de dependências e vulnerabilidades. | Pipeline publica resultado de auditoria de pacotes sem bloquear indevidamente a entrega acadêmica. | Não priorizado agora |
| `CODE-009` | `P0` | Fase 3 | API | Criar a rota do próprio cliente para consultar suas ordens de serviço. | `GET /api/v1/clientes/me/ordens-servico` usa somente `cliente_id` validado do JWT, não aceita substituição por parâmetro de rota/query e possui testes de autorização para o papel `Cliente`. | Bloqueado |

## Decisões e contratos pendentes da autenticação

| ID | Prioridade | Horizonte | Área | Item | Critério de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `DEC-001` | `P0` | Fase 3 | Contrato JWT | Resolver a divergência entre o `cpf_hash` do ADR-0019 e o contrato do RFC-0001. | ADR-0019 e RFC-0001 definem o mesmo claim set: `sub = cliente_id`, `cliente_id`, `role`, `jti`, `iss`, `aud` e expiração; CPF/CNPJ e hashes de documento ficam fora do JWT. | Concluído |
| `DEC-002` | `P0` | Fase 3 | Contrato HTTP | Formalizar o contrato de `POST /auth/documento` e sua integração com o API Gateway. | HTTP API payload format 2.0 com `APIGatewayHttpApiV2ProxyRequest`; request por `documento`; `200` com token mínimo; `400` genérico para entrada inválida; `401` idêntico para inexistente/inativo; `503` genérico para RDS/Secrets Manager; sem exposição de documento ou infraestrutura. | Concluído |
| `RDS-001` | `P0` | Fase 3 | Segredos | Definir e provisionar as credenciais do RDS em AWS Secrets Manager. | `oficina-mecanica-infra-rds` é o dono do segredo de banco; consumidores recebem somente a referência não secreta (ARN via SSM, se essa publicação for confirmada) e não há credencial em variável, estado ou output público. | A fazer |
| `RDS-002` | `P0` | Fase 3 | Rede | Restringir o acesso SQL Server do RDS conforme ADR-0018. | A entrada TCP 1433 deixa de aceitar CIDRs privados amplos e permite somente os security groups da API no EKS e da Lambda; a responsabilidade Terraform da regra e a sequência sem dependência circular são documentadas. | A fazer |

## Dependências P0 da autenticação

| Item | Repositório responsável | Depende de | Bloqueia |
| --- | --- | --- | --- |
| `DEC-001` | `oficina-mecanica-api` | — | — (concluído) |
| `DEC-002` | `oficina-mecanica-infra-api-gateway` com contrato revisado em `oficina-mecanica-api` | — | — (concluído) |
| `CODE-001` | `oficina-mecanica-api` | definição física do status | `F3-001`, `CODE-003`, `CODE-009` |
| `F3-002` | `oficina-mecanica-api` | contrato JWT consolidado em `DEC-001` | `CODE-003`, `CODE-009`, testes ponta a ponta |
| `RDS-001` | `oficina-mecanica-infra-rds` | decisão de publicar ARN não secreto via SSM | `F3-001`, testes ponta a ponta |
| `RDS-002` | `oficina-mecanica-infra-rds` | output do security group da Lambda e definição da regra Terraform | deploy funcional da Lambda |
| `F3-001` | `oficina-mecanica-auth-lambda` | `CODE-001`, `RDS-001` | `RDS-002`, `F3-005`, testes ponta a ponta |
| `CODE-009` | `oficina-mecanica-api` | `CODE-001`, `F3-002` | demonstração de autorização do cliente |
| `F3-005` | `oficina-mecanica-infra-api-gateway` | `F3-001`, endpoint privado da API | testes ponta a ponta |

## Dívida de CI/CD observada na auditoria

| ID | Prioridade | Horizonte | Área | Item | Critério de aceite | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `PIPE-001` | `P2` | Pós-entrega | CI/CD | Convergir a organização do RDS para o padrão `ci.yml` adotado por API, VPC e Kubernetes. | A decisão de unificar ou manter os três workflows do RDS é registrada e, se unificada, preserva os quality gates sem regressão. | Não priorizado agora |
| `PIPE-002` | `P2` | Pós-entrega | CI/CD | Avaliar reutilização do mesmo `tfplan` no apply do RDS. | A esteira do RDS reutiliza o plano aprovado ou documenta tecnicamente por que precisa regenerá-lo. | Não priorizado agora |
| `PIPE-003` | `P2` | Pós-entrega | Documentação | Corrigir a referência de `infra-action.env` onde o padrão real é `terraform-action.env`. | RFCs, planos e READMEs citam o nome real, ou a alteração de nome é aprovada e aplicada de forma coordenada. | Não priorizado agora |
| `PIPE-004` | `P2` | Pós-entrega | Qualidade | Normalizar finais de linha no RDS, caso a alteração ainda exista após nova verificação. | O diff não contém alteração apenas de line ending e `.gitattributes` ou padrão equivalente evita recorrência. | Não priorizado agora |

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
