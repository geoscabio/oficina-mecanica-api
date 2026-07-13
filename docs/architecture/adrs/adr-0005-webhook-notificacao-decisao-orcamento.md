# ADR-0005 — Notificação externa de decisão de orçamento via webhook com token compartilhado

## Status

**Status:** ✅ Aceito
**Data:** 07/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O enunciado da Fase 2 exige, textualmente, entre as APIs obrigatórias: **"Aprovação de orçamento: endpoint para receber notificações externas de aprovação ou recusa do orçamento do cliente"** e **"Atualização de status da OS via alguma ferramenta como e-mail"**. O fluxo de negócio funciona em duas etapas: primeiro o Atendente calcula o orçamento e aciona `POST .../aguardar-aprovacao` (endpoint comum, autenticado por JWT, sem nenhuma decisão especial), comunicando o orçamento ao cliente por fora da API. Depois, o cliente decide (aprova ou recusa) nesse canal externo, e é esse canal — não o Atendente, não o Cliente diretamente — quem precisa devolver a decisão para a API. Esse canal externo não é um usuário autenticado do sistema (não é Atendente, Mecânico ou Cliente logado via JWT).

**O que é exigência literal:** a existência de um endpoint que receba essa notificação externa de decisão do orçamento, atualizando o status da OS. **O que é decisão da equipe:** o enunciado cita "e-mail" apenas como exemplo de ferramenta ("via alguma ferramenta **como** e-mail") — escolhemos implementar como um webhook HTTP dedicado em vez de, por exemplo, uma integração de caixa de entrada de e-mail, e o mecanismo específico de autenticação (token compartilhado) foi desenho nosso.

## 2. Fatores Decisivos (Drivers)

- **Exigência do enunciado:** precisa existir um endpoint para receber a decisão externa do orçamento.
- **O chamador não é um usuário do domínio:** não faz sentido emitir login/JWT para um sistema externo.
- **O endpoint precisa ser público** (sem exigir sessão prévia), mas não pode ficar aberto para qualquer chamador.
- **Simplicidade proporcional ao cenário acadêmico:** não há necessidade de infraestrutura de mensageria (fila, broker de eventos) para um único callback.

## 3. Decisão Proposta

> Expor um endpoint HTTP dedicado (`POST /api/v1/gestao-ordem-servico/ordens-servico/{id}/orcamento/notificacoes`), público (`[AllowAnonymous]`), protegido por um **token compartilhado fixo** enviado no header `X-Webhook-Token`, comparado em tempo constante (`CryptographicOperations.FixedTimeEquals`) para mitigar timing attack. O token vem de configuração/secret (nunca versionado) e sua ausência falha a inicialização da aplicação (fail-fast).

## 4. Justificativa

- Satisfaz a exigência literal do enunciado (endpoint para notificação externa de aprovação/recusa) com o padrão de mercado para esse tipo de integração — o mesmo modelo usado por gateways de pagamento (ex.: Stripe, Mercado Pago) para avisar sistemas parceiros sobre eventos ocorridos externamente.
- Evita a complexidade de emitir e gerenciar credenciais de "usuário" para um sistema que não é uma pessoa.
- Evita reinventar infraestrutura de mensageria (fila/evento assíncrono) para um cenário de callback único e simples — nem exigido pelo enunciado, nem necessário no escopo acadêmico.
- Mantém a mesma estrutura arquitetural dos demais endpoints do projeto (controller → validator → caso de uso), sem introduzir um padrão especial só para essa feature.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Implementação simples e testável nos três níveis (domínio, caso de uso, integração — incluindo o cenário sem token).
- Sem dependência de infraestrutura externa (fila/broker).
- Falha rápida e visível se o token não estiver configurado (não sobe a aplicação silenciosamente insegura).

### ❌ Negativo (Perdas/Riscos)

- Token fixo compartilhado é mais fraco que alternativas como HMAC de payload ou OAuth client credentials — aceitável no escopo acadêmico, mas não seria a escolha ideal para um cenário de produção real com múltiplos parceiros externos.
- Sem rotação automática de token.
- Sem idempotência explícita contra notificações duplicadas do canal externo — mitigado indiretamente pela regra de domínio que valida o status atual da ordem de serviço antes de aplicar a transição (uma segunda notificação repetida não teria efeito, pois a ordem já não estaria mais aguardando aprovação).

## 6. Referências

- **FIAP, Pós-Tech Software Architecture.** [Enunciado do Tech Challenge — Fase 2](../../projeto/enunciado-fase-2-tech-challenge.pdf), seção "Alterar/criar as seguintes APIs".
- **STRIPE.** *Webhooks Documentation*. 2026.
- **OWASP.** *Timing Attack Cheat Sheet*. 2026.
