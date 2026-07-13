# ADR-0008 — Orquestração com Kubernetes/EKS e Terraform como IaC

## Status

**Status:** ✅ Aceito
**Data:** 08/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O enunciado da Fase 2 exige, textualmente, "Orquestração com Kubernetes (K8s)" com manifests contemplando Deployments, Services, ConfigMaps/Secrets e HPA — e, em Infraestrutura como Código, "criar scripts em **Terraform** para provisionamento do cluster Kubernetes (**local ou cloud**)". Ou seja: **Kubernetes é obrigatório, Terraform é obrigatório por nome, mas rodar na nuvem é explicitamente opcional** — o enunciado aceita cluster local. Precisamos decidir, dentro dessa liberdade, se cumprimos o mínimo (cluster local) ou se vamos além.

**O que é exigência literal:** usar Kubernetes; usar Terraform para provisionar o cluster; o cluster pode ser local ou cloud, à nossa escolha. **O que é decisão da equipe, além do mínimo exigido:** rodar especificamente na AWS via **Amazon EKS** (em vez de manter só um cluster local Kind/Minikube, que também teria sido aceito com a mesma nota), e usar o provider `kubernetes` do próprio Terraform para os recursos da API na nuvem, em vez de aplicar manifests YAML separados.

## 2. Fatores Decisivos (Drivers)

- **Requisito do enunciado:** orquestração de containers com Kubernetes e infraestrutura como código via Terraform.
- **Escalabilidade real e observável:** já que optamos por ir além do mínimo, queríamos evidenciar o HPA escalando de verdade em nuvem, não só em ambiente local.
- **Ambiente de laboratório restrito:** AWS Academy com IAM limitado a `LabRole` e sem permissão para criar roles/políticas customizadas.
- **Reprodutibilidade e descarte controlado:** o ambiente precisa ser criado e destruído sem operação manual no console AWS, para não deixar recurso cobrável esquecido.

## 3. Decisão Proposta

> Utilizaremos **Amazon EKS** (Kubernetes gerenciado) como orquestrador de containers, com toda a infraestrutura — incluindo VPC, RDS, ECR, EKS e os próprios recursos Kubernetes da API (Deployment, Service, HPA, Secret, ConfigMap) — provisionada e destruída via **Terraform**, usando o provider `kubernetes` do próprio Terraform em vez de manifests YAML separados para o ambiente AWS. A pasta `k8s/` continua existindo à parte, dedicada à execução local (também aceita pelo enunciado, e mantida como alternativa mais simples de rodar o projeto).

## 4. Justificativa

- **EKS é o padrão gerenciado da AWS para Kubernetes**, com integração nativa a VPC, IAM e Load Balancer — evita operar um control plane próprio, e demonstra um cenário mais próximo de produção do que um cluster local.
- **Terraform versiona e audita todo o ciclo de vida**: o mesmo `terraform apply`/`destroy` que cria a rede e o banco também aplica os manifests Kubernetes da API, então um único `plan` mostra o ambiente inteiro antes de qualquer mudança.
- **Um único pipeline de CI/CD (`CD Development`) controla tudo**, alternando `TERRAFORM_ACTION=apply`/`destroy` por um arquivo versionado (`terraform-action.env`), reduzindo o risco de esquecer recursos ativos entre sessões do Learner Lab.
- Evita duplicar lógica entre `k8s/` (execução local) e um conjunto de YAMLs para AWS: a pasta `k8s/` fica dedicada à execução local (ver `decisoes.md`), e o ambiente AWS usa os mesmos conceitos via Terraform.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Ambiente 100% reproduzível e versionado; nenhuma configuração manual "no console".
- Um único `terraform destroy` remove todos os recursos (rede, banco, cluster e workload Kubernetes), reduzindo risco de custo esquecido.
- HPA real, mensurável e evidenciável em ambiente de nuvem de verdade, não em simulação local — um diferencial acima do mínimo exigido.

### ❌ Negativo (Perdas/Riscos)

- Maior complexidade operacional do que rodar só localmente (que já satisfaria o enunciado) ou direto numa instância EC2.
- Dependência das cotas e restrições do AWS Academy Learner Lab (`LabRole` fixo, credenciais temporárias que expiram por sessão).
- Ciclo de `apply`/`destroy` do ambiente completo leva entre 15 e 20 minutos, o que adiciona fricção para demonstrações rápidas ou iterações de curto prazo.

## 6. Referências

- **FIAP, Pós-Tech Software Architecture.** [Enunciado do Tech Challenge — Fase 2](../../projeto/enunciado-fase-2-tech-challenge.pdf), seções "Orquestração com Kubernetes (K8s)" e "Infraestrutura como Código (IaC)".
- **AWS.** *Amazon EKS User Guide*. 2026.
- **HASHICORP.** *Terraform Kubernetes Provider Documentation*. 2026.
