# Architecture Decision Records (ADRs)

Registro das decisões arquiteturais do projeto, no padrão definido pela equipe desde a Fase 1.

## Índice

| ADR | Título | Data | Status |
| --- | --- | --- | --- |
| [ADR-0001](adr-0001-clean-architecture-monolito.md) | Clean Architecture em monólito | 01/05/2026 | ✅ Aceito |
| [ADR-0002](adr-0002-linguagem-framework-dotnet.md) | Linguagem e framework (.NET) | 01/05/2026 | ✅ Aceito |
| [ADR-0003](adr-0003-banco-dados-sql-server.md) | Banco de dados relacional (SQL Server) | 01/05/2026 | ✅ Aceito |
| [ADR-0004](adr-0004-autenticacao-autorizacao-jwt.md) | Autenticação e autorização (JWT) | 01/05/2026 | ✅ Aceito |
| [ADR-0005](adr-0005-webhook-notificacao-decisao-orcamento.md) | Webhook com token compartilhado para decisão de orçamento | 07/07/2026 | ✅ Aceito |
| [ADR-0006](adr-0006-rds-single-az-sem-backup.md) | RDS single-AZ sem backup automatizado | 07/07/2026 | ✅ Aceito |
| [ADR-0007](adr-0007-topologia-rede-aws-vpc-nat-unico.md) | Topologia de rede AWS: VPC, 2 AZs e NAT Gateway único | 07/07/2026 | ✅ Aceito |
| [ADR-0008](adr-0008-orquestracao-kubernetes-eks-terraform.md) | Orquestração com Kubernetes/EKS e Terraform como IaC | 08/07/2026 | ✅ Aceito |
| [ADR-0009](adr-0009-load-balancer-via-kubernetes-service.md) | Load Balancer provisionado via Kubernetes Service | 08/07/2026 | ✅ Aceito |
| [ADR-0010](adr-0010-pipeline-cicd-estagios-deploys-logicos.md) | Pipeline CI/CD em estágios com deploys lógicos | 09/07/2026 | ✅ Aceito |
| [ADR-0011](adr-0011-controle-apply-destroy-arquivo-versionado.md) | Controle de apply/destroy via arquivo versionado | 11/07/2026 | ✅ Aceito |
| [ADR-0012](adr-0012-terraform-state-local-cache-github-actions.md) | Terraform state em backend local com cache do GitHub Actions | 11/07/2026 | ✅ Aceito |
| [ADR-0013](adr-0013-autoscaling-hpa-runtime-dotnet.md) | Autoscaling horizontal via HPA e ajuste de runtime .NET | 12/07/2026 | ✅ Aceito |

## Origem

ADR-0001 a ADR-0004 foram criadas originalmente na Fase 1 do Tech Challenge e trazidas para este repositório na Fase 2. ADR-0005 a ADR-0013 registram decisões arquiteturais tomadas durante a própria Fase 2, **numeradas na ordem cronológica real em que foram tomadas** (conferida no histórico de commits do repositório, não na ordem em que foram documentadas): primeiro a API de webhook e as peças de infraestrutura AWS (RDS, rede, orquestração EKS, Load Balancer), depois o desenho do pipeline de CI/CD, e por último os ajustes operacionais de Terraform e o tuning de autoscaling, descobertos já com o ambiente rodando de verdade.

Cada ADR desta fase deixa explícito, na seção "Contexto e Problema", o que é **exigência literal do enunciado do Tech Challenge** e o que foi **decisão adicional da equipe** — várias peças de infraestrutura AWS (RDS, VPC, NAT Gateway, Load Balancer, EKS gerenciado) foram escolhas voluntárias para ir além do mínimo exigido (o enunciado aceita cluster Kubernetes local com a mesma nota), enquanto o webhook de orçamento e o HPA por CPU/memória são requisitos obrigatórios citados textualmente.

Fonte: [`docs/projeto/enunciado-fase-2-tech-challenge.pdf`](../../projeto/enunciado-fase-2-tech-challenge.pdf) (enunciado oficial da FIAP).
