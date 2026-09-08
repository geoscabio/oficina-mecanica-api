# ⚙️ GitHub Actions CI/CD

A esteira foi separada em workflows menores para deixar o Git Flow simples de visualizar e operar.

## 🔀 Workflows

| Workflow | Arquivo | Quando roda | Objetivo |
| --- | --- | --- | --- |
| `🧪 CI` | `.github/workflows/ci.yml` | `pull_request` e `push` em `develop`, `release`, `release/**` e `main` | Validar o código e liberar o CD do mesmo commit. |
| `🚀 CD Development` | `.github/workflows/cd-development.yml` | `push` na `develop` | Detectar escopo, executar deploy AWS quando necessário e abrir PR para `release`. |
| `☁️ AWS Deploy` | `.github/workflows/aws-deploy.yml` | `workflow_call` | Executar `apply` ou `destroy` da API na AWS conforme controle versionado. |
| `🔀 CD Release` | `.github/workflows/cd-release.yml` | `push` na `release` ou `release/**` | Registrar deploy lógico em `homologation` e abrir PR para `main`. |
| `🏁 CD Production` | `.github/workflows/cd-production.yml` | `push` na `main` | Registrar deploy lógico em `production`. |

Workflow reutilizável principal:

- `☁️ AWS Deploy`, em `.github/workflows/aws-deploy.yml`.

## 🔁 Fluxo esperado

```text
feature/*, bugfix/*, docs/*, test/*, ci/*, chore/* ...
  -> PR manual para develop
  -> CI única no pull request
  -> merge manual/revisado
  -> 🚀 CD Development
  -> terraform apply + deploy development na AWS quando houver mudança deployable
  -> PR automático para release
  -> merge manual/revisado
  -> 🔀 CD Release
  -> deploy lógico em homologation
  -> PR automático para main
  -> aprovação obrigatória
  -> 🏁 CD Production
  -> deploy lógico em production
```

No estágio `development`, o deploy AWS é o último passo antes da abertura do PR para `release` quando o merge altera código, infraestrutura, Docker ou manifests Kubernetes. Merges sem alteração de runtime pulam o deploy AWS e a promoção automática para release para evitar rebuild desnecessário, `terraform apply` sem mudança funcional e rollout vazio. Como `homologation` e `production` não existem como ambientes físicos neste projeto, esses estágios registram deploys lógicos para manter o Git Flow completo e auditável.

## ✅ CI única para o código

O fluxo de integração economiza GitHub Actions no plano gratuito:

- O PR de branch de trabalho para `develop` é aberto manualmente.
- `🧪 CI` roda nos PRs e pushes das quatro branches protegidas, com uma única definição.
- PR automático fica reservado para os CDs: `develop -> release` e `release -> main`.
- PR somente de documentação/Markdown passa pelo `Quality gate`, mas pula os jobs pesados de build, testes, Docker e Kubernetes.

Valida:

1. restore;
2. build;
3. format;
4. testes com cobertura;
5. zero testes ignorados;
6. cobertura global mínima de `90%`;
7. build da imagem Docker e exportação do artefato em push para develop, sem acesso AWS;
8. dry-run client-side dos manifests `k8s/` em cluster KinD efêmero no CI.

Em `push` de branch de trabalho, a esteira não roda checks pesados nem abre PR automático. O PR valida a integração proposta; após o merge, a CI valida o SHA definitivo que o CD entregará.

O workflow usa `concurrency` por branch/PR para cancelar execuções antigas quando um novo commit chega na mesma branch. Isso evita fila duplicada e reduz custo de tempo no GitHub Actions.

Para acelerar execuções repetidas, a esteira usa cache de pacotes NuGet e cache de camadas Docker via GitHub Actions cache. A validação Kubernetes fica leve no PR; a validação real contra cluster acontece no deploy AWS em EKS.

### Separação por responsabilidade

O workflow único de CI usa jobs separados para deixar claro o princípio de separação de responsabilidades:

| Job | Responsabilidade |
| --- | --- |
| `validate_git_flow` | Bloquear PR fora do fluxo `branch de trabalho -> develop -> release -> main`. |
| `build_application` | Restaurar dependências e compilar a solution. |
| `verify_code_style` | Validar formatação com `dotnet format`. |
| `test_application` | Executar testes automatizados, cobertura e artefatos. |
| `build_container_image` | Construir a imagem e exportar o artefato de develop para o CD. |
| `validate_kubernetes_manifests` | Validar manifests locais em cluster KinD efêmero. |
| `quality_gate` | Consolidar o resultado dos jobs anteriores para branch protection. |

Os nomes técnicos dos jobs usam `snake_case` porque são identificadores estáveis no YAML. Os nomes exibidos no GitHub Actions usam texto legível, como `Build application`, `Test application` e `Quality gate`.

## 🚀 CD Development

Roda após merge/push na `develop`.

Fluxo:

1. Detecta se o merge tem mudança deployable ou apenas mudança sem impacto de runtime.
2. Aguarda a CI do mesmo SHA. Sem mudança deployable, pula o deploy AWS e não abre promoção automática.
3. Lê `infra/terraform/environments/dev/terraform-action.env`.
4. Restaura o cache do Terraform state (GitHub Actions cache, chave `tfstate-development-*`).
5. Executa `terraform init` e `validate`.
6. Se `TERRAFORM_ACTION=apply`, garante o ECR, publica a imagem Docker no ECR, executa `plan`/`apply`, provisiona VPC, RDS, EKS e o workload Kubernetes da API, aguarda rollout e imprime o endpoint do Load Balancer.
7. Salva o Terraform state atualizado de volta no cache do GitHub Actions (sempre, mesmo se um passo posterior falhar).
7. Se `TERRAFORM_ACTION=destroy`, executa `plan -destroy`/`apply` e encerra os recursos AWS gerenciados pelo Terraform.
8. Abre PR automático de `develop` para `release` somente após a operação física concluir com sucesso.

### O que exige deploy AWS

O `🚀 CD Development` decide pelo conteúdo alterado no merge para `develop`, não pelo prefixo da branch.

Arquivos considerados deployable:

- `src/*`
- `k8s/*`
- `infra/terraform/*`
- `Dockerfile`
- `.dockerignore`
- `OficinaMecanica.sln`
- `Directory.Build.props`, `Directory.Build.targets`, `Directory.Packages.props`, `global.json` ou `NuGet.config`

Arquivos que não disparam deploy AWS sozinhos:

- `docs/*`
- `README.md`
- qualquer `*.md`
- `.github/workflows/*`

Sem deploy AWS, o workflow registra a ausência de mudança de runtime e não abre promoção automática para `release`.

## 🏷️ CD Release

Roda após merge/push na `release` ou `release/**`.

Fluxo:

1. Deploy lógico em `homologation`.
2. PR automático de `release` para `main`, se o deploy lógico passou.

## 🏭 CD Production

Roda após merge/push na `main`.

Fluxo:

1. Deploy lógico em `production`.

O PR para `main` deve exigir aprovação/reviewer antes do merge.

## 🧹 Encerramento AWS

O destroy é acionado por PR para `develop`, alterando o arquivo versionado:

```text
infra/terraform/environments/dev/terraform-action.env
```

Valores aceitos:

```env
TERRAFORM_ACTION=apply
TERRAFORM_ACTION=destroy
```

Procedimento completo (branch, PR, acompanhamento da esteira, verificação): ver a seção "Encerramento obrigatório pela esteira" em [`deploy-aws.md`](deploy-aws.md).

`TERRAFORM_ACTION=destroy` só é aceito quando o arquivo `terraform-action.env` foi alterado no próprio merge. Isso evita que pushes futuros destruam recursos sem intenção.

Se o arquivo ficar em `TERRAFORM_ACTION=destroy` depois de um encerramento, mudanças deployable futuras serão bloqueadas de propósito. Para reabilitar deploy real, abrir um PR dedicado voltando `terraform-action.env` para `TERRAFORM_ACTION=apply`. Mudanças não deployable, como documentação ou ajustes de workflow, podem seguir até `release`/`main` sem aplicar AWS.

## 🧩 Repository variables

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

## 🔑 Environments

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

## 🔒 Proteções obrigatórias recomendadas

Configurar dois rulesets ativos em `develop`, `release`, `release/*` e `main`, tanto neste repositório quanto em cada novo repositório da solução.

### 🔒 Proteção Git Flow: sem bypass

- Exigir PR antes do merge e resolução das conversas de revisão.
- Exigir os checks `🔀 01 · Validar fluxo de branches` e `🚦 07 · Quality gate` da integração GitHub Actions. Na VPC e no Kubernetes, o Quality gate é `🚦 03 · Quality gate`.
- Bloquear push direto, force push e deleção da branch.
- Manter a lista de bypass vazia, inclusive para o dono do repositório, admins e maintainers.
- Usar `required_approving_review_count=0` e `require_last_push_approval=false` neste ruleset: a exigência de aprovação pertence exclusivamente ao segundo ruleset.
- Usar `strict_required_status_checks_policy=false`: os checks continuam obrigatórios, mas não se exige incorporar `main` em `release` ou `release` em `develop` para promover as branches. Conflitos reais ainda precisam ser resolvidos e os checks precisam passar.

O GitHub permite abrir um PR com origem incorreta; a validação bloqueia o merge. Para `release` ou `release/*`, a origem aceita é `develop`; para `main`, é `release` ou `release/*`. Branches de trabalho entram por `develop`. Falha ou ausência de qualquer check obrigatório não pode ser dispensada pelo bypass de aprovação.

### 👥 Aprovação de PR: bypass somente da revisão humana

- Exigir uma aprovação (`required_approving_review_count=1`), descartar aprovações antigas após novos commits e exigir aprovação de alguém diferente do último autor do push.
- Incluir somente a regra de aprovação por PR, sem checks ou regras de proteção do histórico neste ruleset.
- Permitir bypass em modo `pull_request` para `geoscabio`, `sousagabriel14`, `RepositoryRole maintain` (ID `2`) e `RepositoryRole admin` (ID `5`).

Quando o outro integrante estiver indisponível, usar o bypass para mesclar o próprio PR depois que os checks passarem. Isso dispensa a revisão de outra pessoa; não cria uma autoaprovação nem dispensa as regras do primeiro ruleset.

Os dois rulesets são aplicados em conjunto. O dono do repositório ainda pode editar ou desativar as configurações administrativas; não é possível retirar esse poder do proprietário com um ruleset do próprio repositório. A garantia de bloqueio vale com as regras ativas. A integridade dos workflows que produzem os checks também faz parte da revisão das mudanças.

O `🔀 CD Release` usa o título de execução `🔀 Registrar deploy em release`: registra o deploy lógico da release e abre o PR de promoção. O registro de produção ocorre no `🏁 CD Production`, após merge em `main`.

Fluxo formal de `hotfix/*` e rollback automatizado ficam no backlog técnico pós-entrega, porque não fazem parte do escopo obrigatório do Tech Challenge.

> Observação: em repositório privado, branch protection pode depender do plano do GitHub. Se a proteção não estiver disponível, manter a regra operacional de não commitar direto em `develop` e `main`.

O CD publica a imagem exportada pela CI, sem executar `docker build`. O download usa o ID da execução aprovada e o SHA exato; retenção de sete dias. Veja [auditoria](../auditoria-ci-cd.md).
