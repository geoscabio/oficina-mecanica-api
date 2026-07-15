# ADR-0006 — RDS single-AZ sem backup automatizado para ambiente de laboratório

## Status

**Status:** ✅ Aceito
**Data:** 07/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> A ADR-0003 (Fase 1) já definiu SQL Server como o banco relacional do projeto. O enunciado da Fase 2 pede, sob Infraestrutura como Código, que o Terraform provisione "Banco de Dados" — sem especificar qual serviço. Rodar esse banco na AWS via **Amazon RDS**, especificamente, não é uma exigência textual: segundo esclarecimento dos professores, RDS é recomendado mas não obrigatório, e um banco em container recebe a mesma nota. Optamos por usar RDS mesmo assim, para demonstrar um cenário mais completo — e esta ADR trata de como esse RDS é configurado, dentro do orçamento limitado do AWS Academy Learner Lab.

**O que é exigência literal:** o enunciado pede que o Terraform provisione "Banco de Dados" (sem especificar tecnologia/serviço). **O que é decisão da equipe:** usar RDS (em vez de banco em container) e toda a configuração específica desta ADR (single-AZ, sem backup, `skip_final_snapshot`).

## 2. Fatores Decisivos (Drivers)

- **Ciclo de vida efêmero:** o ambiente inteiro é destruído ao final de cada sessão de demonstração; não existe continuidade de dados entre uma execução e outra.
- **Custo:** Multi-AZ do RDS praticamente dobra o custo da instância (mantém uma réplica em standby o tempo todo).
- **Velocidade de destroy:** snapshots finais e proteção contra exclusão atrasam ou bloqueiam um `terraform destroy` limpo.

## 3. Decisão Proposta

> RDS SQL Server (`db.t3.micro`, 20GB) executando em **single-AZ** (`multi_az` não habilitado), com `backup_retention_period = 0`, `deletion_protection = false` e `skip_final_snapshot = true`, na subnet privada (sem exposição direta à internet, acesso restrito aos CIDRs das subnets privadas).

## 4. Justificativa

- Sem Multi-AZ: não há necessidade de alta disponibilidade de banco para uma demonstração de curta duração sem usuários reais concorrentes.
- Sem retenção de backup automatizado: os dados são recriados via migration + seed a cada `terraform apply`, então não há dado de produção real para proteger.
- `skip_final_snapshot = true` e `deletion_protection = false`: garantem que o `terraform destroy` complete sem exigir confirmação manual extra ou deixar um snapshot órfão gerando custo de armazenamento após o encerramento.
- `db.t3.micro` é dimensionado para o volume de uma demonstração acadêmica, não para carga de produção.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Custo significativamente menor que uma configuração Multi-AZ com backups.
- `terraform destroy` sempre limpo e completo, sem recursos órfãos (snapshots) sobrando e gerando custo após a demonstração.
- Ambiente rápido de recriar do zero a cada ciclo de apply.

### ❌ Negativo (Perdas/Riscos)

- Nenhuma tolerância a falha de instância: se o RDS cair, não há failover automático.
- Nenhum backup para recuperação de dados — aceitável apenas porque o ambiente não guarda dado real de produção.
- Um design de produção real exigiria, no mínimo, Multi-AZ, backup retention adequado e `deletion_protection = true` (ver "Evolução pós-entrega" em `docs/projeto/pendencias.md`).

## 6. Referências

- **FIAP, Pós-Tech Software Architecture.** [Enunciado do Tech Challenge — Fase 2](../../projeto/enunciado-fase-2-tech-challenge.pdf), seção "Infraestrutura como Código (IaC)".
- **AWS.** *Amazon RDS Multi-AZ Deployments*. 2026.
- Configuração real: [`infra/terraform/environments/dev/rds.tf`](../../../infra/terraform/environments/dev/rds.tf).
