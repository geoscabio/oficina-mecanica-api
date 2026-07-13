# Architecture Decision Records (ADRs)

Registro das decisões arquiteturais do projeto, no padrão definido pela equipe desde a Fase 1.

## Índice

| ADR | Título | Status |
| --- | --- | --- |
| [ADR-0001](adr-0001-clean-architecture-monolito.md) | Clean Architecture em monólito | ✅ Aceito |
| [ADR-0002](adr-0002-linguagem-framework-dotnet.md) | Linguagem e framework (.NET) | ✅ Aceito |
| [ADR-0003](adr-0003-banco-dados-sql-server.md) | Banco de dados relacional (SQL Server) | ✅ Aceito |
| [ADR-0004](adr-0004-autenticacao-autorizacao-jwt.md) | Autenticação e autorização (JWT) | ✅ Aceito |
| [ADR-0005](adr-0005-orquestracao-kubernetes-eks-terraform.md) | Orquestração com Kubernetes/EKS e Terraform como IaC | ✅ Aceito |
| [ADR-0006](adr-0006-webhook-notificacao-decisao-orcamento.md) | Webhook com token compartilhado para decisão de orçamento | ✅ Aceito |
| [ADR-0007](adr-0007-autoscaling-hpa-runtime-dotnet.md) | Autoscaling horizontal via HPA e ajuste de runtime .NET | ✅ Aceito |
| [ADR-0008](adr-0008-terraform-state-local-cache-github-actions.md) | Terraform state em backend local com cache do GitHub Actions | ✅ Aceito |
| [ADR-0009](adr-0009-topologia-rede-aws-vpc-nat-unico.md) | Topologia de rede AWS: VPC, 2 AZs e NAT Gateway único | ✅ Aceito |
| [ADR-0010](adr-0010-rds-single-az-sem-backup.md) | RDS single-AZ sem backup automatizado | ✅ Aceito |
| [ADR-0011](adr-0011-load-balancer-via-kubernetes-service.md) | Load Balancer provisionado via Kubernetes Service | ✅ Aceito |
| [ADR-0012](adr-0012-pipeline-cicd-estagios-deploys-logicos.md) | Pipeline CI/CD em estágios com deploys lógicos | ✅ Aceito |
| [ADR-0013](adr-0013-controle-apply-destroy-arquivo-versionado.md) | Controle de apply/destroy via arquivo versionado | ✅ Aceito |

## Origem

ADR-0001 a ADR-0004 foram criadas originalmente na Fase 1 do Tech Challenge e trazidas para este repositório na Fase 2. ADR-0005 a ADR-0013 registram decisões arquiteturais tomadas durante a própria Fase 2 (orquestração, integração externa, escalabilidade, rede, dados, deploy e pipeline de CI/CD), mantendo o mesmo padrão de estrutura (Contexto, Fatores Decisivos, Decisão, Justificativa, Consequências, Referências).
