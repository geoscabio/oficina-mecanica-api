# ADR-0010 — Pipeline CI/CD em estágios com Git Flow automatizado e deploys lógicos

## Status

**Status:** ✅ Aceito
**Data:** 09/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O enunciado da Fase 2 exige uma "Pipeline de CI/CD configurada (GitHub Actions, GitLab CI, etc.)" que execute: build da aplicação, testes automatizados, build da imagem Docker, deploy no cluster Kubernetes, deploy do banco de dados e aplicação dos manifestos YAML no cluster. Isso é o requisito mínimo — uma pipeline, com essas etapas. O enunciado **não** especifica Git Flow, múltiplos ambientes (`homologation`/`production`) nem deploys lógicos: esse desenho é decisão nossa, para demonstrar um fluxo mais completo e profissional do que "uma pipeline única fazendo tudo em `main`".

**O que é exigência literal:** ter uma pipeline de CI/CD (GitHub Actions é um exemplo aceito pelo próprio enunciado) executando build, testes, build de imagem, deploy no cluster e no banco, e aplicação dos manifestos. **O que é decisão da equipe:** desenhar isso como 4 workflows em estágios (CI, CD Development, CD Release, CD Production), seguindo Git Flow com `develop`/`release`/`main`, com PR automático entre estágios e "deploys lógicos" simulando `homologation`/`production` sem triplicar o ambiente AWS físico.

## 2. Fatores Decisivos (Drivers)

- **Requisito mínimo do enunciado:** uma pipeline de CI/CD com as etapas citadas acima.
- **Orçamento e tempo do Learner Lab** não comportam múltiplos ambientes AWS físicos simultâneos, caso decidíssemos ir além do mínimo com múltiplos estágios.
- **Rastreabilidade:** se formos demonstrar progressão entre estágios, cada um precisa deixar evidência real (execução de workflow, PR, log), não só documentação descritiva.
- **Economia de execução:** mudanças só de documentação não deveriam disparar rebuild/redeploy desnecessário.

## 3. Decisão Proposta

> Dividir a esteira em 4 workflows por responsabilidade: **CI** (validação em `pull_request`), **CD Development** (`push` em `develop`, deploy AWS real — aqui é onde as etapas exigidas pelo enunciado realmente acontecem), **CD Release** (`push` em `release`, deploy lógico de `homologation`) e **CD Production** (`push` em `main`, deploy lógico de `production`). Um mecanismo de **PR automático** (`AUTO_PR_ENABLED`) abre `develop → release` após deploy bem-sucedido, e `release → main` após o deploy lógico de homologation. Merges que alteram apenas Markdown/`docs/` pulam o deploy AWS real.

## 4. Justificativa

- Só o estágio `development` provisiona AWS de verdade — de fato cumprindo as etapas exigidas pelo enunciado (build, teste, imagem, deploy k8s, deploy banco, manifests). `homologation` e `production` são um acréscimo nosso: registram um "deploy lógico" (execução real do workflow, com log e PR como evidência), demonstrando um fluxo Git Flow completo sem triplicar custo de infraestrutura.
- Workflows separados por estágio deixam o pipeline mais fácil de visualizar e depurar do que um único workflow monolítico com múltiplos `if` condicionais.
- O PR automático entre estágios reduz esforço manual, mas mantém revisão humana obrigatória: o merge — não a abertura do PR — é o gatilho real de cada estágio.
- Pular o deploy AWS em mudanças só de documentação evita consumir tempo de execução e ciclos de `apply`/`destroy` sem necessidade real.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Cumpre o requisito mínimo do enunciado e vai além, demonstrando um fluxo Git Flow completo e auditável.
- Custo de infraestrutura limitado a um único ambiente AWS real (`development`).
- Pipeline modular, cada workflow com responsabilidade única, mais simples de entender e manter.

### ❌ Negativo (Perdas/Riscos)

- Deploys lógicos em `homologation`/`production` não validam o comportamento real da aplicação nesses estágios — só simulam a progressão do Git Flow. Um bug que só aparecesse em produção real não seria detectado por esse mecanismo.
- Depende de disciplina: só a esteira deve executar `apply`/`destroy` (ver ADR-0011) para o state não desalinhar.
- Requer a permissão "Allow GitHub Actions to create and approve pull requests" habilitada nas configurações do repositório para o PR automático funcionar.
- Complexidade adicional que não seria necessária apenas para satisfazer o enunciado — escolhida conscientemente para ir além do mínimo.

## 6. Referências

- **FIAP, Pós-Tech Software Architecture.** [Enunciado do Tech Challenge — Fase 2](../../projeto/enunciado-fase-2-tech-challenge.pdf), seção "Integração Contínua/Entrega Contínua (CI/CD)".
- **ATLASSIAN.** *Gitflow Workflow*. 2026.
- Detalhamento operacional: [`docs/deploy/github-actions.md`](../../deploy/github-actions.md).
