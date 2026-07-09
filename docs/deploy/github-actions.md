# GitHub Actions CI/CD

A esteira foi separada em workflows menores para deixar o Git Flow simples de visualizar e operar.

## Workflows

| Workflow | Arquivo | Quando roda | Objetivo |
| --- | --- | --- | --- |
| `CI` | `.github/workflows/ci.yml` | Branches de trabalho e PRs | Validar qualidade e abrir PR automático para `develop` quando aplicável. |
| `CD Development` | `.github/workflows/cd-development.yml` | `push` na `develop` | Validar, fazer deploy em `development` e abrir PR para `release`. |
| `CD Release` | `.github/workflows/cd-release.yml` | `push` na `release` ou `release/**` | Validar, fazer deploy em `homologation` e abrir PR para `main`. |
| `CD Production` | `.github/workflows/cd-production.yml` | `push` na `main` | Validar e fazer deploy em `production`. |
| `AWS Cleanup` | `.github/workflows/aws-cleanup.yml` | Manual | Remover recursos Kubernetes do environment escolhido. |

Workflows reutilizáveis:

- `.github/workflows/reusable-quality-gate.yml`
- `.github/workflows/reusable-aws-deploy.yml`

## Fluxo esperado

```text
feature/*, bugfix/*, hotfix/* ...
  -> CI
  -> PR automático para develop
  -> merge manual/revisado
  -> CD Development
  -> deploy development
  -> PR automático para release
  -> merge manual/revisado
  -> CD Release
  -> deploy homologation
  -> PR automático para main
  -> aprovação obrigatória
  -> CD Production
  -> deploy production
```

O deploy é o último passo operacional de cada CD. A abertura automática do próximo PR acontece apenas depois do deploy bem-sucedido.

## CI

Executa para:

- `feature/**`
- `bugfix/**`
- `hotfix/**`
- `fix/**`
- `refactor/**`
- `chore/**`
- `docs/**`
- `test/**`
- `ci/**`
- `pull_request` para `develop`, `release`, `release/**` ou `main`

Valida:

1. restore;
2. build;
3. format;
4. testes com cobertura;
5. zero testes ignorados;
6. cobertura global mínima de `90%`;
7. build da imagem Docker;
8. dry-run dos manifests `k8s/` e `infra/aws/k8s/`.

Em `push` de branch de trabalho, se tudo passar, abre PR automático para `develop`.

## CD Development

Roda após merge/push na `develop`.

Fluxo:

1. Quality gate.
2. Publicação da imagem no GHCR.
3. Deploy em `development`, se `AWS_DEPLOY_ENABLED=true`.
4. PR automático de `develop` para `release`, se o deploy passou.

## CD Release

Roda após merge/push na `release` ou `release/**`.

Fluxo:

1. Quality gate.
2. Publicação da imagem no GHCR.
3. Deploy em `homologation`, se `AWS_DEPLOY_ENABLED=true`.
4. PR automático de `release` para `main`, se o deploy passou.

## CD Production

Roda após merge/push na `main`.

Fluxo:

1. Quality gate.
2. Publicação da imagem no GHCR.
3. Deploy em `production`, se `AWS_DEPLOY_ENABLED=true`.

O environment `production` deve exigir aprovação/reviewer no GitHub.

## AWS Cleanup

Workflow manual para remover recursos Kubernetes:

1. Abrir `Actions > AWS Cleanup`.
2. Clicar em `Run workflow`.
3. Escolher `target_environment`.
4. Rodar o cleanup.
5. Executar `terraform destroy` depois, quando a infraestrutura foi criada para demonstração.

## Repository variables

| Nome | Tipo | Uso |
| --- | --- | --- |
| `AWS_DEPLOY_ENABLED` | Repository variable | Habilita deploy automático quando `true`. |
| `AUTO_PR_ENABLED` | Repository variable | Habilita abertura automática de PR quando `true`. |
| `RELEASE_BRANCH` | Repository variable opcional | Nome da branch de release. Default: `release`. |

Para usar `AUTO_PR_ENABLED=true`, também é necessário habilitar no GitHub:

```text
Settings > Actions > General > Workflow permissions >
Allow GitHub Actions to create and approve pull requests
```

Sem essa permissão, o GitHub bloqueia a criação automática de PR por segurança.

## Environments

Criar:

- `development`
- `homologation`
- `production`

Cada environment precisa conter:

| Nome | Tipo | Uso |
| --- | --- | --- |
| `AWS_ACCESS_KEY_ID` | Secret | Access key do ambiente. |
| `AWS_SECRET_ACCESS_KEY` | Secret | Secret key do ambiente. |
| `AWS_SESSION_TOKEN` | Secret | Session token quando aplicável. |
| `JWT_SECRET` | Secret | Chave JWT da API, com pelo menos 32 caracteres. |
| `WEBHOOK_TOKEN` | Secret | Token do webhook de orçamento, com pelo menos 32 caracteres. |
| `RDS_CONNECTION_STRING` | Secret | Connection string completa do banco. |
| `ECR_REPOSITORY_URL` | Variable | Output `terraform output ecr_repository_url`. |
| `EKS_CLUSTER_NAME` | Variable | Output `terraform output eks_cluster_name`. |

## Proteções obrigatórias recomendadas

Configurar branch protection em `develop` e `main`:

- bloquear push direto;
- exigir PR antes de merge;
- exigir status checks do `CI`;
- exigir pelo menos um reviewer para `main`;
- configurar reviewer obrigatório no environment `production`.

Com isso, o fluxo fica coerente: ninguém commita direto nas branches protegidas, e a promoção acontece por PR.
