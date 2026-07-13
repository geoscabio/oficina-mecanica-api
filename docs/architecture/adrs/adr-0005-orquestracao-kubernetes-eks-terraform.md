# ADR-0005 — Orquestração com Kubernetes/EKS e Terraform como IaC

## Status

**Status:** ✅ Aceito
**Data:** 13/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> A Fase 2 exige demonstrar a aplicação rodando em Kubernetes (local e na nuvem) e infraestrutura provisionada como código, incluindo escalabilidade automática observável de verdade. Precisamos de um ambiente reproduzível, versionado e descartável — importante porque rodamos no AWS Academy Learner Lab, com créditos e sessão limitados.

## 2. Fatores Decisivos (Drivers)

- **Requisito da Fase 2:** orquestração de containers com Kubernetes e infraestrutura como código.
- **Escalabilidade real e observável:** precisamos evidenciar o HPA escalando de verdade, não simulado.
- **Ambiente de laboratório restrito:** AWS Academy com IAM limitado a `LabRole` e sem permissão para criar roles/políticas customizadas.
- **Reprodutibilidade e descarte controlado:** o ambiente precisa ser criado e destruído sem operação manual no console AWS, para não deixar recurso cobrável esquecido.

## 3. Decisão Proposta

> Utilizaremos **Amazon EKS** (Kubernetes gerenciado) como orquestrador de containers, com toda a infraestrutura — incluindo VPC, RDS, ECR, EKS e os próprios recursos Kubernetes da API (Deployment, Service, HPA, Secret, ConfigMap) — provisionada e destruída via **Terraform**, usando o provider `kubernetes` do próprio Terraform em vez de manifests YAML separados para o ambiente AWS.

## 4. Justificativa

- **EKS é o padrão gerenciado da AWS para Kubernetes**, com integração nativa a VPC, IAM e Load Balancer — evita operar um control plane próprio.
- **Terraform versiona e audita todo o ciclo de vida**: o mesmo `terraform apply`/`destroy` que cria a rede e o banco também aplica os manifests Kubernetes da API, então um único `plan` mostra o ambiente inteiro antes de qualquer mudança.
- **Um único pipeline de CI/CD (`CD Development`) controla tudo**, alternando `TERRAFORM_ACTION=apply`/`destroy` por um arquivo versionado (`terraform-action.env`), reduzindo o risco de esquecer recursos ativos entre sessões do Learner Lab.
- Evita duplicar lógica entre `k8s/` (execução local) e um conjunto de YAMLs para AWS: a pasta `k8s/` fica dedicada à execução local (ver `decisoes.md`), e o ambiente AWS usa os mesmos conceitos via Terraform.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Ambiente 100% reproduzível e versionado; nenhuma configuração manual "no console".
- Um único `terraform destroy` remove todos os recursos (rede, banco, cluster e workload Kubernetes), reduzindo risco de custo esquecido.
- HPA real, mensurável e evidenciável em ambiente de nuvem de verdade, não em simulação local.

### ❌ Negativo (Perdas/Riscos)

- Maior complexidade operacional do que rodar a aplicação direto numa instância EC2.
- Dependência das cotas e restrições do AWS Academy Learner Lab (`LabRole` fixo, credenciais temporárias que expiram por sessão).
- Ciclo de `apply`/`destroy` do ambiente completo leva entre 15 e 20 minutos, o que adiciona fricção para demonstrações rápidas ou iterações de curto prazo.

## 6. Referências

- **AWS.** *Amazon EKS User Guide*. 2026.
- **HASHICORP.** *Terraform Kubernetes Provider Documentation*. 2026.
- **FIAP, Pós-Tech Software Architecture.** Enunciado do Tech Challenge — Fase 2.
