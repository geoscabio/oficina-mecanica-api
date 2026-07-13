# ADR-0008 — Terraform state em backend local com cache do GitHub Actions

## Status

**Status:** ✅ Aceito
**Data:** 13/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O Terraform precisa lembrar, entre uma execução e outra da esteira, quais recursos AWS já foram criados (o "state"). O padrão mais comum em produção é guardar esse state remotamente em um bucket S3 dedicado. No AWS Academy Learner Lab, porém, a política de conta (`voc-cancel-cred`) às vezes nega a permissão `s3:CreateBucket`, o que travaria a esteira sem nenhuma alternativa se o backend S3 fosse obrigatório.

## 2. Fatores Decisivos (Drivers)

- **Restrição do laboratório:** IAM restrito a `LabRole`, sem garantia de permissão para criar um bucket S3 novo.
- **Enunciado da Fase 2** não exige backend remoto dedicado, apenas infraestrutura como código funcional e reproduzível.
- **Simplicidade operacional:** evitar depender de um recurso AWS adicional só para guardar o state.

## 3. Decisão Proposta

> Usar o backend `local` do Terraform (arquivo `terraform.tfstate` dentro de `infra/terraform/environments/dev/`, não versionado no repositório), persistindo esse arquivo entre execuções da esteira através do **cache do GitHub Actions** (`actions/cache`, chave `tfstate-development-*`). Cada execução do `CD Development` restaura o cache no início e salva a versão atualizada no final, mesmo se algum passo posterior falhar.

## 4. Justificativa

- Elimina a dependência de criar um bucket S3, contornando a restrição de permissão do Learner Lab.
- O cache do GitHub Actions já está disponível gratuitamente na esteira, sem infraestrutura adicional.
- Mantém o state persistente e localizável entre execuções, preservando a principal função de um backend remoto (não perder a referência dos recursos já criados).

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Nenhuma dependência de criar/gerenciar um bucket S3 adicional.
- Funciona dentro das restrições reais do AWS Academy Learner Lab.
- Simples de entender e depurar (arquivo local, sem locking distribuído a configurar).

### ❌ Negativo (Perdas/Riscos)

- O cache expira após 7 dias sem uso, ou pode ser limpo manualmente no GitHub — nesses casos a esteira perde a referência dos recursos já existentes.
- Sem locking distribuído real (diferente de um backend S3 + DynamoDB): se dois `apply` rodassem em paralelo, poderiam colidir. Mitigado porque a esteira só permite uma execução por vez via `concurrency` no workflow.
- Exige um mecanismo de contingência para quando o state diverge da realidade (ver ADR-0013, workflow `AWS Import Existing Resources`).

## 6. Referências

- **HASHICORP.** *Terraform Backend Configuration: local*. 2026.
- **GITHUB.** *Caching dependencies to speed up workflows (actions/cache)*. 2026.
- Detalhamento operacional: [`docs/deploy/deploy-aws.md`](../../deploy/deploy-aws.md).
