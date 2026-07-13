# ADR-0013 — Controle de apply/destroy do Terraform via arquivo versionado com aprovação por PR

## Status

**Status:** ✅ Aceito
**Data:** 13/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> Rodar `terraform apply` ou `terraform destroy` na AWS tem consequência real de custo e de disponibilidade do ambiente de demonstração. É preciso um mecanismo que decida qual ação a esteira executa a cada `push` em `develop`, sem exigir que alguém dispare manualmente um workflow (o que teria menos rastreabilidade) e sem permitir que uma ação destrutiva aconteça sem revisão.

## 2. Fatores Decisivos (Drivers)

- **Segurança:** um `destroy` acidental apaga toda a infraestrutura de demonstração.
- **Auditabilidade:** a decisão de aplicar ou destruir deve ficar registrada no histórico do Git, não em um clique avulso no GitHub Actions.
- **Simplicidade:** evitar inputs manuais de workflow (`workflow_dispatch`) que não deixam rastro claro de "quem decidiu o quê e quando".
- **Recuperação:** quando o state do Terraform diverge da realidade da AWS (cache expirado, execução manual indevida), precisa existir uma forma segura de reconciliar sem apagar recursos por engano.

## 3. Decisão Proposta

> Controlar a ação via um arquivo versionado, `infra/terraform/environments/dev/terraform-action.env`, contendo `TERRAFORM_ACTION=apply` ou `TERRAFORM_ACTION=destroy`. Mudar esse valor exige uma branch dedicada e um PR revisado para `develop` — o merge do PR é o gatilho. Como camada extra de segurança, `TERRAFORM_ACTION=destroy` só é aceito pela esteira quando o arquivo foi alterado **no próprio merge** (evita que um push futuro não relacionado reexecute um destroy antigo por engano). Como rede de segurança para divergência de state, existe um workflow separado e manual, `AWS Import Existing Resources`, que reconcilia o state com os recursos reais da AWS via `terraform import`, sem nunca aplicar ou destruir nada sozinho (termina em um `plan` para revisão humana).

## 4. Justificativa

- Um arquivo versionado transforma a decisão de aplicar/destruir em um artefato do Git Flow normal: branch → PR → revisão → merge, com histórico completo de quem mudou o quê e quando.
- A trava de "só aceita destroy se alterado no mesmo merge" impede o cenário perigoso de um push não relacionado reexecutar acidentalmente uma ação destrutiva antiga.
- Um workflow de emergência separado (em vez de reconciliação automática) evita que a esteira tente "adivinhar" e corrigir o state sozinha sem revisão — reconciliação de infraestrutura é sensível o suficiente para exigir revisão humana do `plan` antes de continuar.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Toda ação de apply/destroy é rastreável por PR, com revisão obrigatória antes do merge.
- Reduz drasticamente o risco de destruição acidental por push não relacionado.
- Existe um caminho claro e seguro de recuperação quando o state diverge da realidade, sem precisar mexer manualmente na AWS.

### ❌ Negativo (Perdas/Riscos)

- Depende de disciplina da equipe: rodar Terraform manualmente fora da esteira (fora do fluxo documentado) é a causa mais comum de desalinhamento de state, e nada impede tecnicamente que isso aconteça.
- Um passo a mais (branch + PR) comparado a simplesmente disparar um `workflow_dispatch` com um input — troca velocidade por segurança e auditabilidade, intencionalmente.
- Já ocorreu um incidente real (2026-07-11) em que o arquivo ficou esquecido em `destroy` após um merge, bloqueando merges não relacionados subsequentes como proteção — o guardrail funcionou como projetado (bloqueou em vez de aplicar destroy silenciosamente), mas exigiu atenção manual para corrigir.

## 6. Referências

- Detalhamento operacional: [`docs/deploy/deploy-aws.md`](../../deploy/deploy-aws.md) e [`docs/deploy/aws-academy-guardrails.md`](../../deploy/aws-academy-guardrails.md).
- Workflow de emergência: [`.github/workflows/aws-import-existing-resources.yml`](../../../.github/workflows/aws-import-existing-resources.yml).
