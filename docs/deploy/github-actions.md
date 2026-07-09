# GitHub Actions CI/CD

A esteira foi separada em workflows menores para deixar o Git Flow simples de visualizar e operar.

## Workflows

| Workflow | Arquivo | Quando roda | Objetivo |
| --- | --- | --- | --- |
| `Auto PR to Develop` | `.github/workflows/auto-pr-develop.yml` | `push` em branch de trabalho | Abrir ou manter PR automático para `develop`. |
| `CI` | `.github/workflows/ci.yml` | `pull_request` | Validar qualidade antes do merge. |
| `CD Development` | `.github/workflows/cd-development.yml` | `push` na `develop` | Fazer deploy em `development` e abrir PR para `release`. |
| `CD Release` | `.github/workflows/cd-release.yml` | `push` na `release` ou `release/**` | Registrar deploy lógico em `homologation` e abrir PR para `main`. |
| `CD Production` | `.github/workflows/cd-production.yml` | `push` na `main` | Registrar deploy lógico em `production`. |
| `AWS Cleanup` | `.github/workflows/aws-cleanup.yml` | Manual | Remover recursos Kubernetes do environment escolhido. |

Workflows reutilizáveis:

- `.github/workflows/reusable-quality-gate.yml`
- `.github/workflows/reusable-aws-deploy.yml`

## Fluxo esperado

```text
feature/*, bugfix/*, hotfix/* ...
  -> PR automático para develop
  -> CI no pull request
  -> merge manual/revisado
  -> CD Development
  -> deploy development na AWS
  -> PR automático para release
  -> merge manual/revisado
  -> CD Release
  -> deploy lógico em homologation
  -> PR automático para main
  -> aprovação obrigatória
  -> CD Production
  -> deploy lógico em production
```

No estágio `development`, o deploy AWS é o último passo antes da abertura do PR para `release`. Como `homologation` e `production` não existem como ambientes físicos neste projeto, esses estágios registram deploys lógicos para manter o Git Flow completo e auditável.

## CI

O fluxo de integração tem dois workflows separados:

- `.github/workflows/auto-pr-develop.yml`: em `push` nas branches `feature/**`, `bugfix/**`, `hotfix/**`, `fix/**`, `refactor/**`, `chore/**`, `docs/**`, `test/**` e `ci/**`, abre ou mantém PR para `develop`.
- `.github/workflows/ci.yml`: em `pull_request` para `develop`, `release`, `release/**` ou `main`, executa os checks completos.

Valida:

1. restore;
2. build;
3. format;
4. testes com cobertura;
5. zero testes ignorados;
6. cobertura global mínima de `90%`;
7. build da imagem Docker;
8. dry-run dos manifests `k8s/` e `infra/aws/k8s/`.

Em `push` de branch de trabalho, a esteira não roda checks pesados. Ela apenas abre ou mantém o PR para `develop`; os checks completos rodam uma vez no próprio PR.

O workflow usa `concurrency` por branch/PR para cancelar execuções antigas quando um novo commit chega na mesma branch. Isso evita fila duplicada e reduz custo de tempo no GitHub Actions.

Para acelerar execuções repetidas, a esteira usa cache de pacotes NuGet e cache de camadas Docker via GitHub Actions cache.

## CD Development

Roda após merge/push na `develop`.

Fluxo:

1. Deploy real em `development`, se `AWS_DEPLOY_ENABLED=true`.
2. Build e push da imagem para ECR durante o deploy AWS.
3. PR automático de `develop` para `release`, somente se o deploy passou.

## CD Release

Roda após merge/push na `release` ou `release/**`.

Fluxo:

1. Deploy lógico em `homologation`.
2. PR automático de `release` para `main`, se o deploy lógico passou.

## CD Production

Roda após merge/push na `main`.

Fluxo:

1. Deploy lógico em `production`.

O PR para `main` deve exigir aprovação/reviewer antes do merge.

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
| `AWS_DEPLOY_ENABLED` | Repository variable | Habilita deploy automático real em `development` quando `true`. |
| `AUTO_PR_ENABLED` | Repository variable | Habilita abertura automática de PR quando `true`. |
| `RELEASE_BRANCH` | Repository variable opcional | Nome da branch de release. Default: `release`. |

Para usar `AUTO_PR_ENABLED=true`, também é necessário habilitar no GitHub:

```text
Settings > Actions > General > Workflow permissions >
Allow GitHub Actions to create and approve pull requests
```

Sem essa permissão, o GitHub bloqueia a criação automática de PR por segurança.

## Environments

Obrigatório para o deploy real:

- `development`

O environment `development` precisa conter:

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
- exigir reviewer obrigatório antes do merge para `main`.

Com isso, o fluxo fica coerente: ninguém commita direto nas branches protegidas, e o deploy entre estágios acontece por PR.

> Observação: em repositório privado, branch protection pode depender do plano do GitHub. Se a proteção não estiver disponível, manter a regra operacional de não commitar direto em `develop` e `main`.
