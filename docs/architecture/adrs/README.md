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

## Origem

ADR-0001 a ADR-0004 foram criadas originalmente na Fase 1 do Tech Challenge e trazidas para este repositório na Fase 2. ADR-0005 a ADR-0007 registram decisões arquiteturais tomadas durante a própria Fase 2 (infraestrutura, integração externa e escalabilidade), mantendo o mesmo padrão de estrutura (Contexto, Fatores Decisivos, Decisão, Justificativa, Consequências, Referências).
