# Deploy na AWS

Este guia descreve como preparar a infraestrutura AWS real do environment `development` e como a esteira Git Flow executa deploy lógico para `homologation` e `production`.

## Regra de ouro

Ambientes temporários devem ser destruídos após a demonstração. Antes de qualquer criação de recurso, tenha um plano claro de `terraform destroy`.

## Provisionamento da infraestrutura

O provisionamento do ambiente `development` acontece no workflow `CD Development`, após merge/push na branch `develop`.

O CD executa:

1. cria ou reutiliza o bucket S3 do Terraform state;
2. executa `terraform init`, `validate`, `plan` e `apply`;
3. cria VPC, ECR, RDS, EKS e recursos Kubernetes da API;
4. publica a imagem Docker no ECR;
5. reinicia o Deployment no EKS e valida o rollout.

Execução local equivalente para diagnóstico:

```powershell
terraform -chdir=infra/terraform/environments/dev init `
  -backend-config="bucket=<tf-state-bucket>" `
  -backend-config="key=oficina-mecanica/development/terraform.tfstate" `
  -backend-config="region=us-east-1" `
  -backend-config="encrypt=true" `
  -backend-config="use_lockfile=true" `
  -reconfigure

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
| `TF_STATE_BUCKET` | Nome de bucket S3 existente, por exemplo `oficina-mecanica-tfstate-<account-id>`. | Obrigatório para CD com Terraform. A esteira não cria o bucket automaticamente. |
| `TF_STATE_KEY` | `oficina-mecanica/development/terraform.tfstate` | Opcional. Caminho do arquivo de state dentro do bucket. |
| `EKS_CLUSTER_ROLE_NAME` | `LabRole` | Opcional se o AWS Academy usar `LabRole`. Ajustar se o lab informar outro nome. |
| `EKS_NODE_ROLE_NAME` | `LabRole` | Opcional se o AWS Academy usar `LabRole`. Ajustar se o lab informar outro nome. |

Exemplos de valores criados pelo grupo:

```text
DB_PASSWORD=<senha forte, não colar no repositório>
JWT_SECRET=<string aleatória com pelo menos 32 caracteres>
WEBHOOK_TOKEN=<string aleatória com pelo menos 32 caracteres>
TF_STATE_BUCKET=oficina-mecanica-tfstate-<account-id>
```

Os valores reais devem ser cadastrados somente no GitHub ou no ambiente local seguro. Não adicionar esses valores em `.env`, YAML, Terraform ou Markdown.

## Criação do bucket de state

O bucket de state é um detalhe operacional do Terraform para permitir que o GitHub Actions lembre quais recursos foram criados entre uma execução e outra. Ele não faz parte da arquitetura da aplicação.

Criar uma vez no AWS Academy antes do primeiro deploy:

```powershell
aws s3api create-bucket `
  --bucket oficina-mecanica-tfstate-<account-id> `
  --region us-east-1
```

Depois, usar exatamente esse nome em `TF_STATE_BUCKET`.

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
| `develop` | `development` | Executa Terraform apply, faz deploy AWS real e abre PR para `release`. |
| `release` ou `release/**` | `homologation` | Registra deploy lógico e abre PR para `main`. |
| `main` | `production` | Registra deploy lógico final após PR aprovado. |

O deploy AWS real gerencia os recursos Kubernetes da API pelo Terraform, aguarda rollout e imprime o endpoint do Load Balancer. Os estágios `homologation` e `production` não provisionam AWS enquanto não existirem ambientes físicos separados.

## Validação

- [ ] Validar rollout da API.
- [ ] Validar `kubectl get pods -n oficina`.
- [ ] Validar `kubectl get svc oficina-api -n oficina`.
- [ ] Validar `/api/health`.
- [ ] Validar Swagger.

## Encerramento obrigatório

Ao final da demonstração, executar o destroy explícito do Terraform usando o mesmo backend/state da esteira:

Antes do comando, manter disponíveis as mesmas variáveis usadas no apply, principalmente `TF_VAR_db_password`, `TF_VAR_eks_cluster_role_name` e `TF_VAR_eks_node_role_name`.

```powershell
terraform -chdir=infra/terraform/environments/dev init `
  -backend-config="bucket=<tf-state-bucket>" `
  -backend-config="key=oficina-mecanica/development/terraform.tfstate" `
  -backend-config="region=us-east-1" `
  -backend-config="encrypt=true" `
  -backend-config="use_lockfile=true" `
  -reconfigure

terraform -chdir=infra/terraform/environments/dev destroy
```

Como os recursos Kubernetes da API também estão no state, o `terraform destroy` remove Service/Load Balancer, Deployment, Secret, ConfigMap, Namespace, EKS, RDS, ECR, NAT Gateway e VPC.

Por fim, conferir que não restaram EKS, EC2, RDS, NAT Gateway, ECR ou Load Balancer ativos. Se o bucket de state foi criado apenas para a demonstração, remover também após confirmar que o destroy terminou com sucesso.
