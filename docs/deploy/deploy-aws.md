# Deploy na AWS

Este guia descreve como preparar a infraestrutura AWS real do environment `development` e como a esteira Git Flow executa deploy lógico para `homologation` e `production`.

## Regra de ouro

Ambientes temporários devem ser destruídos após a demonstração. Antes de qualquer criação de recurso, tenha um plano claro de `terraform destroy`.

## Provisionamento da infraestrutura

O provisionamento do ambiente `development` acontece no workflow `CD Development`, após merge/push na branch `develop`.

O CD executa:

1. restaura o Terraform state do cache do GitHub Actions (chave `tfstate-development-*`);
2. executa `terraform init` e `validate`;
3. garante o repositório ECR antes do build;
4. publica a imagem Docker no ECR antes do workload apontar para a nova tag;
5. executa `terraform plan` e `apply`;
6. cria ou atualiza VPC, RDS, EKS e recursos Kubernetes da API;
7. valida o rollout do Deployment no EKS e imprime o endpoint do Load Balancer;
8. salva o Terraform state atualizado de volta no cache do GitHub Actions, mesmo se algum passo posterior falhar.

O ambiente publicado na AWS roda com `ASPNETCORE_ENVIRONMENT=Staging`. Swagger e usuários demo são habilitados explicitamente via `appsettings.Staging.json` para permitir a avaliação do Tech Challenge; nunca replicar este padrão, com credenciais fixas e Swagger público, em um ambiente de produção real.

Execução local equivalente para diagnóstico (usa o state local em `infra/terraform/environments/dev/terraform.tfstate` — não é o mesmo state da esteira, que fica no cache do GitHub Actions):

```powershell
terraform -chdir=infra/terraform/environments/dev init
terraform -chdir=infra/terraform/environments/dev validate
terraform -chdir=infra/terraform/environments/dev plan
terraform -chdir=infra/terraform/environments/dev apply
```

## GitHub Environments

Criar para o deploy real:

- `development`

O environment `development` precisa conter:

Environment secrets:

| Nome | Origem do valor | Observação |
| --- | --- | --- |
| `AWS_ACCESS_KEY_ID` | AWS Academy > AWS Details > credenciais CLI. | Geralmente começa com `ASIA` em credenciais temporárias. |
| `AWS_SECRET_ACCESS_KEY` | AWS Academy > AWS Details > credenciais CLI. | Copiar exatamente como fornecido. |
| `AWS_SESSION_TOKEN` | AWS Academy > AWS Details > credenciais CLI. | Obrigatório no Learner Lab e expira a cada sessão. |
| `DB_PASSWORD` | Valor criado pelo grupo. | Senha forte do administrador do RDS SQL Server. Não versionar. |
| `JWT_SECRET` | Valor criado pelo grupo. | String aleatória com pelo menos 32 caracteres para assinar tokens JWT. |
| `WEBHOOK_TOKEN` | Valor criado pelo grupo. | String aleatória com pelo menos 32 caracteres para proteger o webhook de orçamento. |

Repository variables:

| Nome | Valor esperado | Observação |
| --- | --- | --- |
| `AUTO_PR_ENABLED` | `true` ou `false` | `true` abre PR automático após deploy: `develop -> release` e `release -> main`. |
| `RELEASE_BRANCH` | `release` | Opcional. Default: `release`. |
| `AWS_REGION` | `us-east-1` | Opcional. Default: `us-east-1`. |
| `EKS_CLUSTER_ROLE_NAME` | `LabRole` | Opcional se o AWS Academy usar `LabRole`. Ajustar se o lab informar outro nome. |
| `EKS_NODE_ROLE_NAME` | `LabRole` | Opcional se o AWS Academy usar `LabRole`. Ajustar se o lab informar outro nome. |

Exemplos de valores criados pelo grupo:

```text
DB_PASSWORD=<senha forte, não colar no repositório>
JWT_SECRET=<string aleatória com pelo menos 32 caracteres>
WEBHOOK_TOKEN=<string aleatória com pelo menos 32 caracteres>
```

Os valores reais devem ser cadastrados somente no GitHub ou no ambiente local seguro. Não adicionar esses valores em `.env`, YAML, Terraform ou Markdown.

## Terraform state entre execuções

O Terraform usa backend `local` (arquivo `terraform.tfstate` dentro de `infra/terraform/environments/dev/`). Esse arquivo não é versionado no repositório; para lembrar o que foi criado entre uma execução e outra da esteira, o `CD Development` guarda e recupera o state usando o cache do GitHub Actions (`actions/cache`, chave `tfstate-development-*`).

Motivo da escolha: um bucket S3 dedicado ao state não é exigido pelo enunciado da Fase 2, e o AWS Academy Learner Lab às vezes nega `s3:CreateBucket` por política de conta (`voc-cancel-cred`), o que travava a esteira sem necessidade real de S3.

Se o cache expirar (7 dias sem uso) ou for limpo manualmente no GitHub, a esteira perde a referência dos recursos já criados — nesse caso, use o workflow `AWS Import Existing Resources` (ver seção abaixo) para reconciliar o state com o que já existe na AWS antes de rodar `apply`/`destroy` de novo.

## Workflow de emergência: AWS Import Existing Resources

`.github/workflows/aws-import-existing-resources.yml` **não faz parte do fluxo normal de CI/CD**. É uma rede de segurança, não um passo do dia a dia.

**Quando usar:** só nestes dois sintomas:

- `terraform apply` falha com erro de recurso "já existe" (`RepositoryAlreadyExistsException`, cluster/instância já existe, etc.);
- `terraform destroy` termina com sucesso mas não destruiu nada de verdade (sinal de state vazio).

Os dois sintomas indicam a mesma causa: o state (cache do GitHub Actions) não sabe que um recurso real existe na AWS. Isso só acontece se:

1. Alguém rodou Terraform manualmente, fora da esteira (não faça isso — é a causa mais comum e mais fácil de evitar); ou
2. O cache do state expirou (7+ dias sem nenhuma execução da esteira).

**Como usar:** GitHub → Actions → `AWS Import Existing Resources` → *Run workflow*. Ele descobre os IDs reais via AWS CLI, roda `terraform import` para VPC, subnets, EKS, RDS e ECR, e termina com um `terraform plan` (revisão apenas, não aplica nada). Revisar esse plano antes de rodar `apply`/`destroy` normalmente de novo.

**Por que não roda automaticamente:** se a esteira (`CD Development`) for sempre o único lugar que executa `terraform apply`/`destroy`, o state nunca desalinha, e este workflow nunca precisa ser usado. Ele existe só para o caso de a disciplina falhar ou o cache expirar — não é uma etapa a mais do pipeline normal.

## Evolução futura: Secrets Manager e Parameter Store

Para este Tech Challenge, os segredos são injetados no Kubernetes Secret pelo Terraform a partir do GitHub Environment. Isso é suficiente para demonstrar CI/CD, deploy em EKS e uso seguro de secrets sem versionar valores sensíveis.

Em uma evolução mais próxima de produção, é válido mover:

| Tipo de configuração | Serviço AWS recomendado | Exemplos |
| --- | --- | --- |
| Segredos sensíveis | AWS Secrets Manager | Senha do banco, `JWT_SECRET`, `WEBHOOK_TOKEN`. |
| Configurações não sensíveis | AWS Systems Manager Parameter Store | Região, nome de recursos, flags de ambiente. |

Nesse modelo futuro, a aplicação ou o cluster buscariam os valores em runtime usando integração como External Secrets Operator, AWS Secrets Store CSI Driver ou permissões IAM específicas. Isso reduz a dependência de secrets longos no GitHub, mas adiciona complexidade de IAM e operação.

## Deploy pela esteira

| Branch | Ambiente | Próximo passo automático |
| --- | --- | --- |
| `develop` | `development` | Executa Terraform apply/deploy AWS real quando houver mudança deployable e abre PR para `release`. |
| `release` ou `release/**` | `homologation` | Registra deploy lógico e abre PR para `main`. |
| `main` | `production` | Registra deploy lógico final após PR aprovado. |

O deploy AWS real gerencia os recursos Kubernetes da API pelo Terraform, aguarda rollout e imprime o endpoint do Load Balancer. Os estágios `homologation` e `production` não provisionam AWS enquanto não existirem ambientes físicos separados.

## Onde o Load Balancer é criado

Não existe um recurso `aws_lb` explícito no Terraform porque o Load Balancer da API é criado pelo cloud provider da AWS quando o Kubernetes Service da API é criado com `type = "LoadBalancer"`.

Fonte de verdade:

```text
infra/terraform/environments/dev/api-service.tf
```

Recurso responsável:

```text
kubernetes_service_v1.oficina_api
```

O Terraform mantém o Service no state; por consequência, o `terraform destroy` remove o Service Kubernetes e a AWS remove o Load Balancer associado.

## Validação

- [ ] Validar rollout da API.
- [ ] Validar `kubectl get pods -n oficina`.
- [ ] Validar `kubectl get svc oficina-api -n oficina`.
- [ ] Validar `/api/health`.
- [ ] Validar Swagger.

## Encerramento obrigatório pela esteira

Ao final da demonstração, executar o destroy explícito pela própria esteira de CD, usando o mesmo backend/state do deploy.

Arquivo de controle:

```text
infra/terraform/environments/dev/terraform-action.env
```

Para destruir os recursos AWS:

```env
TERRAFORM_ACTION=destroy
```

Passo a passo:

1. Criar uma branch a partir da `develop`.
2. Alterar `infra/terraform/environments/dev/terraform-action.env` para `TERRAFORM_ACTION=destroy`.
3. Abrir PR para `develop`.
4. Fazer merge do PR.
5. Acompanhar o workflow `CD Development` no GitHub Actions.
6. Confirmar que a etapa `Terraform destroy` terminou com sucesso.
7. Abrir novo PR voltando o arquivo para `TERRAFORM_ACTION=apply` antes do próximo deploy.

Como os recursos Kubernetes da API também estão no state, o `terraform destroy` remove Service/Load Balancer, Deployment, Secret, ConfigMap, Namespace, EKS, RDS, ECR, NAT Gateway e VPC.

Por fim, conferir que não restaram EKS, EC2, RDS, NAT Gateway, ECR ou Load Balancer ativos.
