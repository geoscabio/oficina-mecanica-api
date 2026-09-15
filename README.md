# Oficina Mecânica API

API principal da solução da Oficina Mecânica para a Fase 3 do Tech Challenge FIAP. Implementa os contextos de atendimento, estoque, catálogo e ordens de serviço em .NET, com Clean Architecture e DDD.

## Repositórios da solução

| Repositório | Responsabilidade |
| --- | --- |
| [API principal](https://github.com/geoscabio/oficina-mecanica-api) | Aplicação .NET, regras de negócio, endpoints, testes e workload Kubernetes. |
| [Auth Lambda](https://github.com/geoscabio/oficina-mecanica-auth-lambda) | Autenticação por documento e emissão do JWT de Cliente. |
| [Infra VPC](https://github.com/geoscabio/oficina-mecanica-infra-vpc) | Rede base, subnets, NAT e contratos de VPC no SSM. |
| [Infra Kubernetes](https://github.com/geoscabio/oficina-mecanica-infra-kubernetes) | EKS, ECR, NLB interno e observabilidade Datadog do cluster. |
| [Infra RDS](https://github.com/geoscabio/oficina-mecanica-infra-rds) | SQL Server privado, secret gerenciado e contratos de banco no SSM. |
| [Infra API Gateway](https://github.com/geoscabio/oficina-mecanica-infra-api-gateway) | HTTP API pública, VPC Link, integração Lambda/NLB e access logs. |

## Responsabilidade e arquitetura

Este repositório é dono do código da API e do seu deployment/Service Kubernetes. Ele não cria VPC, EKS, ECR, NLB, RDS ou API Gateway.

```text
Internet
  -> API Gateway HTTP API
      -> POST /auth/documento -> Auth Lambda -> RDS
      -> ANY /api/{proxy+} -> VPC Link -> NLB interno -> target group
         -> EKS NodePort -> API Pods :8080 -> RDS
```

O Service da API é `NodePort`; não existe LoadBalancer público da API. Endpoints relevantes:

- `GET /api/health`;
- `POST /auth/documento` (publicado pelo API Gateway e atendido pela Auth Lambda);
- `GET /api/v1/clientes/me/ordens-servico` (JWT Cliente com claim `cliente_id`);
- rotas administrativas sob `/api/v1/...`, protegidas por perfil.

## Tecnologias

.NET 10, ASP.NET Core, Entity Framework Core, SQL Server, FluentValidation, AutoMapper, xUnit, FluentAssertions, Moq, Testcontainers, Docker, Kubernetes, Terraform e GitHub Actions.

## Pré-requisitos e execução local

Para desenvolvimento local: .NET SDK 10 e Docker Desktop. Copie `.env.example` para `.env` e execute:

```powershell
docker compose --env-file .env -f docker-compose.yml up -d --build
curl.exe http://localhost:5093/api/health
```

Swagger local: `http://localhost:5093/swagger`. Para parar: `docker compose --env-file .env -f docker-compose.yml down`.

O deployment AWS exige que VPC, Kubernetes/ECR e RDS já tenham publicado seus contratos SSM. A API consome:

| Parâmetro SSM | Finalidade |
| --- | --- |
| `/oficina-mecanica/development/status/kubernetes` | Confirma o cluster pronto. |
| `/oficina-mecanica/development/kubernetes/cluster_name` | Configura o provider Kubernetes. |
| `/oficina-mecanica/development/kubernetes/ecr_repository_name` e `ecr_repository_url` | Publica a imagem `sha-$GITHUB_SHA`. |
| `/oficina-mecanica/development/kubernetes/api_internal_node_port` | Mantém o contrato NodePort. |
| `/oficina-mecanica/development/status/rds` e `/rds/endpoint` | Confirma e localiza o banco. |
| `/oficina-mecanica/development/rds/master_secret_arn` | ARN do secret gerenciado; o valor nunca entra no Terraform state. |

## Secrets, variables e parâmetros

Cadastre itens GitHub em **Settings > Environments > development**. Valores reais nunca devem ser versionados.

| Nome | Tipo | Escopo | Obrigatório | Finalidade | Como configurar |
| --- | --- | --- | --- | --- | --- |
| `AWS_ACCESS_KEY_ID` | GitHub Environment Secret | `development` | Sim para deploy AWS | Credencial AWS Academy | Copiar de AWS Details/credenciais temporárias. |
| `AWS_SECRET_ACCESS_KEY` | GitHub Environment Secret | `development` | Sim para deploy AWS | Credencial AWS Academy | Copiar de AWS Details. |
| `AWS_SESSION_TOKEN` | GitHub Environment Secret | `development` | Sim no modelo Academy | Sessão temporária AWS | Renovar quando a sessão expirar. |
| `JWT_SECRET` | GitHub Environment Secret | `development` | Sim | Assinatura do JWT da API | Gerar valor aleatório forte; não reutilizar exemplo. |
| `WEBHOOK_TOKEN` | GitHub Environment Secret | `development` | Sim | Autentica o webhook de orçamento | Gerar valor aleatório forte. |
| `AWS_REGION` | GitHub Environment Variable | `development` | Sim | Região AWS | Exemplo seguro: `us-east-1`. |
| `AUTO_PR_ENABLED` | GitHub Variable | Repositório | Opcional | Habilita PRs automáticos de promoção | Usar `true` somente quando desejado. |
| `RELEASE_BRANCH` | GitHub Variable | Repositório | Opcional | Branch de promoção | Fallback: `release`. |

O RDS mantém usuário/senha no AWS Secrets Manager. A esteira lê o ARN pelo SSM e consulta o secret somente durante o deploy; não existe secret de connection string cadastrado no GitHub.

## CI/CD e deploy

Pull requests executam CI de build, formatação, testes, cobertura, imagem Docker, manifests e Terraform. O merge em `develop` aciona o CD quando houver mudança deployável. A ação é controlada por `infra/terraform/environments/dev/terraform-action.env`: `TERRAFORM_ACTION=apply` atualiza recursos da API; `destroy` exige PR dedicado que altere o mesmo arquivo.

O deploy publica imagem imutável `sha-$GITHUB_SHA`, aplica somente recursos da API no EKS e valida rollout/Service NodePort. A promoção segue `branch de trabalho -> develop -> release -> main`.

## Observabilidade

O caminho de borda possui access logs CloudWatch no repositório de API Gateway. A instrumentação Datadog da API é tratada em esteira própria e só deve ser considerada ativa após merge, deploy e evidência de runtime; nenhum segredo Datadog pertence a este repositório nesta configuração base.

## Testes e validações

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
terraform fmt -check -recursive infra/terraform
terraform -chdir=infra/terraform/environments/dev init -backend=false
terraform -chdir=infra/terraform/environments/dev validate
```

## Documentação relacionada e links úteis

- [Índice de documentação](docs/README.md)
- [ADRs](docs/architecture/adrs)
- [RFCs](docs/architecture/rfcs)
- [OpenAPI versionado](docs/openapi/oficina-mecanica-openapi.json)
- [Postman](docs/postman)
- [GitHub Actions](.github/workflows)

O OpenAPI e a collection Postman devem ser revisados após a consolidação final dos endpoints; a documentação de entrega permanece no backlog técnico.
