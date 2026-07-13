# ADR-0012 — Pipeline CI/CD em estágios com Git Flow automatizado e deploys lógicos

## Status

**Status:** ✅ Aceito
**Data:** 13/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> A Fase 2 exige uma esteira de CI/CD completa seguindo Git Flow, com estágios de `development`, `homologation` e `production`. Manter três ambientes AWS físicos e independentes (um por estágio) triplicaria o custo e a complexidade operacional dentro do orçamento limitado do AWS Academy Learner Lab — inviável para o escopo do projeto.

## 2. Fatores Decisivos (Drivers)

- **Requisito de Git Flow completo e auditável**, com evidência de progressão entre estágios.
- **Orçamento e tempo do Learner Lab** não comportam múltiplos ambientes AWS físicos simultâneos.
- **Rastreabilidade:** cada estágio precisa deixar evidência real (execução de workflow, PR, log), não só documentação descritiva.
- **Economia de execução:** mudanças só de documentação não deveriam disparar rebuild/redeploy desnecessário.

## 3. Decisão Proposta

> Dividir a esteira em 4 workflows por responsabilidade: **CI** (validação em `pull_request`), **CD Development** (`push` em `develop`, deploy AWS real), **CD Release** (`push` em `release`, deploy lógico de `homologation`) e **CD Production** (`push` em `main`, deploy lógico de `production`). Um mecanismo de **PR automático** (`AUTO_PR_ENABLED`) abre `develop → release` após deploy bem-sucedido, e `release → main` após o deploy lógico de homologation, mantendo o Git Flow completo mesmo sem três ambientes físicos. Merges que alteram apenas Markdown/`docs/` pulam o deploy AWS real.

## 4. Justificativa

- Só o estágio `development` provisiona AWS de verdade — `homologation` e `production` registram um "deploy lógico" (execução real do workflow, com log e PR como evidência), preservando o fluxo Git Flow completo e auditável sem triplicar custo de infraestrutura.
- Workflows separados por estágio deixam o pipeline mais fácil de visualizar e depurar do que um único workflow monolítico com múltiplos `if` condicionais.
- O PR automático entre estágios reduz esforço manual, mas mantém revisão humana obrigatória: o merge — não a abertura do PR — é o gatilho real de cada estágio.
- Pular o deploy AWS em mudanças só de documentação evita consumir tempo de execução e ciclos de `apply`/`destroy` sem necessidade real.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Custo de infraestrutura limitado a um único ambiente AWS real (`development`).
- Git Flow completo e demonstrável (`develop → release → main`) com evidência real de cada estágio.
- Pipeline modular, cada workflow com responsabilidade única, mais simples de entender e manter.

### ❌ Negativo (Perdas/Riscos)

- Deploys lógicos em `homologation`/`production` não validam o comportamento real da aplicação nesses estágios — só simulam a progressão do Git Flow. Um bug que só aparecesse em produção real não seria detectado por esse mecanismo.
- Depende de disciplina: só a esteira deve executar `apply`/`destroy` (ver ADR-0013) para o state não desalinhar.
- Requer a permissão "Allow GitHub Actions to create and approve pull requests" habilitada nas configurações do repositório para o PR automático funcionar.

## 6. Referências

- **ATLASSIAN.** *Gitflow Workflow*. 2026.
- Detalhamento operacional: [`docs/deploy/github-actions.md`](../../deploy/github-actions.md).
