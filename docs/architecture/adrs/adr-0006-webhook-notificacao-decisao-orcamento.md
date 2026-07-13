# ADR-0006 — Notificação externa de decisão de orçamento via webhook com token compartilhado

## Status

**Status:** ✅ Aceito
**Data:** 13/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O fluxo de negócio inclui um canal externo de orçamento: depois que a oficina calcula o orçamento de uma ordem de serviço, esse canal comunica o valor ao cliente e devolve a decisão (aprovado ou recusado) para a API. Esse canal externo não é um usuário autenticado do sistema (não é Atendente, Mecânico ou Cliente logado via JWT) — é outro sistema avisando um evento que já aconteceu fora da nossa API.

## 2. Fatores Decisivos (Drivers)

- **O chamador não é um usuário do domínio:** não faz sentido emitir login/JWT para um sistema externo.
- **O endpoint precisa ser público** (sem exigir sessão prévia), mas não pode ficar aberto para qualquer chamador.
- **Simplicidade proporcional ao cenário acadêmico:** não há necessidade de infraestrutura de mensageria (fila, broker de eventos) para um único callback.

## 3. Decisão Proposta

> Expor um endpoint HTTP dedicado (`POST /api/v1/gestao-ordem-servico/ordens-servico/{id}/orcamento/notificacoes`), público (`[AllowAnonymous]`), protegido por um **token compartilhado fixo** enviado no header `X-Webhook-Token`, comparado em tempo constante (`CryptographicOperations.FixedTimeEquals`) para mitigar timing attack. O token vem de configuração/secret (nunca versionado) e sua ausência falha a inicialização da aplicação (fail-fast).

## 4. Justificativa

- É o padrão de mercado para esse tipo de integração — o mesmo modelo usado por gateways de pagamento (ex.: Stripe, Mercado Pago) para avisar sistemas parceiros sobre eventos ocorridos externamente.
- Evita a complexidade de emitir e gerenciar credenciais de "usuário" para um sistema que não é uma pessoa.
- Evita reinventar infraestrutura de mensageria (fila/evento assíncrono) para um cenário de callback único e simples.
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

- **STRIPE.** *Webhooks Documentation*. 2026.
- **OWASP.** *Timing Attack Cheat Sheet*. 2026.
