# GitHub Actions CI/CD

A esteira foi separada em workflows menores para deixar o Git Flow simples de visualizar e operar.

## Workflows

| Workflow | Arquivo | Quando roda | Objetivo |
| --- | --- | --- | --- |
| `CI` | `.github/workflows/ci.yml` | `pull_request` | Validar qualidade antes do merge. |
| `CD Development` | `.github/workflows/cd-development.yml` | `push` na `develop` | Detectar escopo, executar deploy AWS quando necessário e abrir PR para `release`. |
| `CD Release` | `.github/workflows/cd-release.yml` | `push` na `release` ou `release/**` | Registrar deploy lógico em `homologation` e abrir PR para `main`. |
| `CD Production` | `.github/workflows/cd-production.yml` | `push` na `main` | Registrar deploy lógico em `production`. |

Workflows reutilizáveis:

- `.github/workflows/aws-deploy.yml`

## Fluxo esperado

```text
feature/*, bugfix/*, hotfix/* ...
  -> PR manual para develop
  -> CI no pull request
  -> merge manual/revisado
  -> CD Development
  -> terraform apply + deploy development na AWS quando houver mudança deployable
  -> PR automático para release
  -> merge manual/revisado
  -> CD Release
  -> deploy lógico em homologation
  -> PR automático para main
  -> aprovação obrigatória
  -> CD Production
  -> deploy lógico em production
```

No estágio `development`, o deploy AWS é o último passo antes da abertura do PR para `release` quando o merge altera código, infraestrutura, Docker, workflows ou manifests. Merges somente de Markdown/`docs/` pulam o deploy AWS para evitar rebuild desnecessário e rollout sem mudança funcional. Como `homologation` e `production` não existem como ambientes físicos neste projeto, esses estágios registram deploys lógicos para manter o Git Flow completo e auditável.

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
7. build local da imagem Docker, sem push para ECR;
8. dry-run client-side dos manifests `k8s/` em cluster KinD efemero no CI.

Em `push` de branch de trabalho, a esteira não roda checks pesados nem abre PR automático. Os checks completos rodam uma vez no próprio PR.

O workflow usa `concurrency` por branch/PR para cancelar execuções antigas quando um novo commit chega na mesma branch. Isso evita fila duplicada e reduz custo de tempo no GitHub Actions.

Para acelerar execuções repetidas, a esteira usa cache de pacotes NuGet e cache de camadas Docker via GitHub Actions cache. A validação Kubernetes fica leve no PR; a validação real contra cluster acontece no deploy AWS em EKS.

### Separacao por responsabilidade

O workflow `CI` usa jobs separados para deixar claro o principio de separacao de responsabilidades:

| Job | Responsabilidade |
| --- | --- |
| `build_application` | Restaurar dependencias e compilar a solution. |
| `verify_code_style` | Validar formatacao com `dotnet format`. |
| `test_application` | Executar testes automatizados, cobertura e artefatos. |
| `build_container_image` | Validar o build da imagem Docker sem publicar. |
| `validate_kubernetes_manifests` | Validar manifests locais em cluster KinD efemero. |
| `quality_gate` | Consolidar o resultado dos jobs anteriores para branch protection. |

Os nomes tecnicos dos jobs usam `snake_case` porque sao identificadores estaveis no YAML. Os nomes exibidos no GitHub Actions usam texto legivel, como `Build application`, `Test application` e `Quality gate`.

## CD Development

Roda após merge/push na `develop`.

Fluxo:

1. Detecta se o merge tem mudança deployable ou apenas documentação Markdown.
2. Se for somente documentação, pula o deploy AWS e pode abrir PR para `release` quando `AUTO_PR_ENABLED=true`.
3. Lê `infra/terraform/environments/dev/terraform-action.env`.
4. Restaura o cache do Terraform state (GitHub Actions cache, chave `tfstate-development-*`).
5. Executa `terraform init` e `validate`.
6. Se `TERRAFORM_ACTION=apply`, garante o ECR, publica a imagem Docker no ECR, executa `plan`/`apply`, provisiona VPC, RDS, EKS e o workload Kubernetes da API, aguarda rollout e imprime o endpoint do Load Balancer.
7. Salva o Terraform state atualizado de volta no cache do GitHub Actions (sempre, mesmo se um passo posterior falhar).
7. Se `TERRAFORM_ACTION=destroy`, executa `plan -destroy`/`apply` e encerra os recursos AWS gerenciados pelo Terraform.
8. Abre PR automático de `develop` para `release` quando o deploy `apply` passou ou quando a alteração era somente documentação.

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

O destroy é acionado por PR para `develop`, alterando o arquivo versionado:

```text
infra/terraform/environments/dev/terraform-action.env
```

Valores aceitos:

```env
TERRAFORM_ACTION=apply
TERRAFORM_ACTION=destroy
```

Uso operacional:

1. Manter `TERRAFORM_ACTION=apply` para deploy normal.
2. Alterar para `TERRAFORM_ACTION=destroy` em uma branch dedicada quando quiser encerrar a AWS.
3. Abrir PR para `develop`.
4. Fazer merge e acompanhar `CD Development`.
5. Conferir no console AWS se não restaram EKS, RDS, ECR, Load Balancer, NAT Gateway ou EC2 ativos.
6. Abrir novo PR voltando para `TERRAFORM_ACTION=apply` antes do próximo deploy.

`TERRAFORM_ACTION=destroy` só é aceito quando o arquivo `terraform-action.env` foi alterado no próprio merge. Isso evita que pushes futuros destruam recursos sem intenção.

## Repository variables

| Nome | Tipo | Valor esperado | Uso |
| --- | --- | --- | --- |
| `AUTO_PR_ENABLED` | Repository variable | `true` ou `false` | Habilita PR automático após deploy: `develop -> release` e `release -> main`. |
| `RELEASE_BRANCH` | Repository variable opcional | `release` | Nome da branch de release. Default: `release`. |
| `AWS_REGION` | Environment variable opcional | `us-east-1` | Região AWS. Default: `us-east-1`. |
| `EKS_CLUSTER_ROLE_NAME` | Environment variable opcional | `LabRole` | Role IAM existente para o cluster EKS. |
| `EKS_NODE_ROLE_NAME` | Environment variable opcional | `LabRole` | Role IAM existente para o node group. |

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

| Nome | Tipo | Origem do valor | Uso |
| --- | --- | --- | --- |
| `AWS_ACCESS_KEY_ID` | Environment secret | AWS Academy > AWS Details > credenciais CLI. | Access key temporária do ambiente. |
| `AWS_SECRET_ACCESS_KEY` | Environment secret | AWS Academy > AWS Details > credenciais CLI. | Secret key temporária do ambiente. |
| `AWS_SESSION_TOKEN` | Environment secret | AWS Academy > AWS Details > credenciais CLI. | Session token temporário. Obrigatório no Learner Lab. |
| `DB_PASSWORD` | Environment secret | Valor criado pelo grupo. | Senha do usuário administrador do RDS usada pelo Terraform. |
| `JWT_SECRET` | Environment secret | Valor criado pelo grupo. | Chave JWT com pelo menos 32 caracteres. |
| `WEBHOOK_TOKEN` | Environment secret | Valor criado pelo grupo. | Token do webhook de orçamento com pelo menos 32 caracteres. |

Os secrets `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY` e `AWS_SESSION_TOKEN` expiram quando a sessão do AWS Academy expira. Atualizar esses três valores antes de reexecutar o CD em uma nova sessão.

Nenhum valor sensível deve ser escrito nos arquivos `.yml`, `.tf`, `.md` ou `.env` versionados. Os workflows referenciam apenas os nomes dos secrets e variables.

## Proteções obrigatórias recomendadas

Configurar branch protection em `develop` e `main`. Quando a branch `release` existir, aplicar a mesma protecao nela.

- bloquear push direto;
- exigir PR antes de merge;
- exigir status check `Quality gate`;
- exigir pelo menos um reviewer;
- descartar aprovacoes antigas quando novos commits forem enviados;
- bloquear force push e delecao da branch.

Com isso, o fluxo fica coerente: ninguém commita direto nas branches protegidas, e o deploy entre estágios acontece por PR.

> Observação: em repositório privado, branch protection pode depender do plano do GitHub. Se a proteção não estiver disponível, manter a regra operacional de não commitar direto em `develop` e `main`.
