# Deploy

Esta pasta concentra os guias operacionais de CI/CD, Kubernetes, Terraform e AWS.

## Arquivos

| Arquivo | Objetivo |
| --- | --- |
| `github-actions.md` | Explica a esteira Git Flow automatizada, artifacts, GHCR, deploys e PRs automáticos. |
| `deploy-aws.md` | Checklist de provisionamento, configuração dos environments e encerramento seguro. |
| `aws-academy-guardrails.md` | Regras obrigatórias para evitar gasto indevido no Learner Lab. |

## Regras de segurança

- Não executar `terraform apply` sem aprovação explícita.
- Planejar e executar `terraform destroy` ao final de qualquer teste AWS temporário.
- Não versionar kubeconfig, tokens, senhas ou secrets reais.
- Deploy em PR não executa: PR valida qualidade, build, imagem e manifests.
- Deploy automático só roda em `develop`, `release`/`release/**` e `main` quando `AWS_DEPLOY_ENABLED=true`.
- A esteira bloqueia testes ignorados e cobertura global abaixo de 90%.
