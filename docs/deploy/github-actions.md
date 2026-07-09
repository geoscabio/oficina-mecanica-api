# GitHub Actions CI/CD

A esteira foi separada em workflows menores para deixar o Git Flow simples de visualizar e operar.

## Workflows

| Workflow | Arquivo | Quando roda | Objetivo |
| --- | --- | --- | --- |
| `CI` | `.github/workflows/ci.yml` | `pull_request` | Validar qualidade antes do merge. |
| `CD Development` | `.github/workflows/cd-development.yml` | `push` na `develop` | Executar Terraform apply, deploy em `development` e abrir PR para `release`. |
| `CD Release` | `.github/workflows/cd-release.yml` | `push` na `release` ou `release/**` | Registrar deploy lógico em `homologation` e abrir PR para `main`. |
| `CD Production` | `.github/workflows/cd-production.yml` | `push` na `main` | Registrar deploy lógico em `production`. |

Workflows reutilizáveis:

- `.github/workflows/reusable-quality-gate.yml`
- `.github/workflows/reusable-aws-deploy.yml`

## Fluxo esperado

```text
feature/*, bugfix/*, hotfix/* ...
  -> PR manual para develop
  -> CI no pull request
  -> merge manual/revisado
  -> CD Development
  -> terraform apply + deploy development na AWS
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

O fluxo de integração economiza GitHub Actions no plano gratuito:

- O PR de branch de trabalho para `develop` é aberto manualmente.
- `.github/workflows/ci.yml` roda em `pull_request` para `develop`, `release`, `release/**` ou `main`.
- PR automático fica reservado para os CDs: `develop -> release` e `release -> main`.

Valida:

1. restore;
2. build;
3. format;
4. testes com cobertura;
5. zero testes ignorados;
6. cobertura global mínima de `90%`;
7. build local da imagem Docker, sem push para registry;
8. dry-run client-side dos manifests `k8s/` e `infra/aws/k8s/`, sem subir cluster KinD no CI.

Em `push` de branch de trabalho, a esteira não roda checks pesados nem abre PR automático. Os checks completos rodam uma vez no próprio PR.

O workflow usa `concurrency` por branch/PR para cancelar execuções antigas quando um novo commit chega na mesma branch. Isso evita fila duplicada e reduz custo de tempo no GitHub Actions.

Para acelerar execuções repetidas, a esteira usa cache de pacotes NuGet e cache de camadas Docker via GitHub Actions cache. A validação Kubernetes fica leve no PR; a validação real contra cluster acontece no deploy AWS em EKS.

## CD Development

Roda após merge/push na `develop`.

Fluxo:

1. Prepara backend S3 do Terraform state.
2. Executa `terraform init`, `validate`, `plan` e `apply`.
3. Provisiona VPC, ECR, RDS, EKS e recursos Kubernetes da API via Terraform.
4. Faz build e push da imagem Docker para ECR.
5. Reinicia o Deployment no EKS, aguarda rollout e imprime o endpoint do Load Balancer.
6. Abre PR automático de `develop` para `release`, somente se o deploy passou.

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

## Encerramento AWS

Destroy não fica acoplado na esteira de CD. A esteira faz delivery/deploy; o encerramento do ambiente é uma operação explícita de Terraform feita pelos desenvolvedores após a demonstração.

1. Rodar `terraform init` apontando para o mesmo bucket/key de state usado pelo CD.
2. Executar `terraform destroy` em `infra/terraform/environments/dev`.
3. Conferir no console AWS se não restaram EKS, RDS, ECR, Load Balancer, NAT Gateway ou EC2 ativos.

## Repository variables

| Nome | Tipo | Uso |
| --- | --- | --- |
| `AUTO_PR_ENABLED` | Repository variable | Habilita PR automático após deploy: `develop -> release` e `release -> main`. |
| `RELEASE_BRANCH` | Repository variable opcional | Nome da branch de release. Default: `release`. |
| `AWS_REGION` | Repository ou environment variable opcional | Região AWS. Default: `us-east-1`. |
| `TF_STATE_BUCKET` | Environment variable obrigatória | Bucket S3 preexistente do Terraform state. A esteira não cria bucket automaticamente. |
| `TF_STATE_KEY` | Environment variable opcional | Caminho do state. Default: `oficina-mecanica/development/terraform.tfstate`. |
| `EKS_CLUSTER_ROLE_NAME` | Environment variable opcional | Role IAM existente para o cluster EKS. Default: `LabRole`. |
| `EKS_NODE_ROLE_NAME` | Environment variable opcional | Role IAM existente para o node group. Default: `LabRole`. |

Para usar `AUTO_PR_ENABLED=true` nos workflows de CD, também é necessário habilitar no GitHub:

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
| `DB_PASSWORD` | Secret | Senha do usuário administrador do RDS usada pelo Terraform. |
| `JWT_SECRET` | Secret | Chave JWT da API, com pelo menos 32 caracteres. |
| `WEBHOOK_TOKEN` | Secret | Token do webhook de orçamento, com pelo menos 32 caracteres. |

## Proteções obrigatórias recomendadas

Configurar branch protection em `develop` e `main`:

- bloquear push direto;
- exigir PR antes de merge;
- exigir status checks do `CI`;
- exigir pelo menos um reviewer para `main`;
- exigir reviewer obrigatório antes do merge para `main`.

Com isso, o fluxo fica coerente: ninguém commita direto nas branches protegidas, e o deploy entre estágios acontece por PR.

> Observação: em repositório privado, branch protection pode depender do plano do GitHub. Se a proteção não estiver disponível, manter a regra operacional de não commitar direto em `develop` e `main`.
