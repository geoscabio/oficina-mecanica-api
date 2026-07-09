# Deploy

Esta pasta concentra os guias operacionais de CI/CD, Kubernetes, Terraform e AWS Academy.

## Arquivos

| Arquivo | Objetivo |
| --- | --- |
| `github-actions.md` | Explica o workflow de CI/CD, artifacts, GHCR e deploy manual protegido. |
| `deploy-aws.md` | Checklist de provisionamento e implantacao no AWS Academy. |
| `aws-academy-guardrails.md` | Regras obrigatorias para evitar gasto indevido no Learner Lab. |

## Regras de seguranca

- Nao executar `terraform apply` sem aprovacao explicita.
- Planejar e executar `terraform destroy` ao final de qualquer teste AWS.
- Nao versionar kubeconfig, tokens, senhas ou secrets reais.
- Em PR, o deploy real fica skipped; deploy AWS so roda via `workflow_dispatch` com approval no environment `aws-academy`.
- A esteira bloqueia testes ignorados e cobertura global abaixo de 90%.
