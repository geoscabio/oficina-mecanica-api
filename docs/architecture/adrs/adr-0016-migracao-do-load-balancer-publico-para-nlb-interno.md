# ADR-0016 — Migração do Load Balancer Público para NLB Interno

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio  
**Substitui:** ADR-0009 — Load Balancer provisionado via Kubernetes Service

---

## 1. Contexto e Problema

Na Fase 2, a API no Kubernetes foi exposta por um Classic Load Balancer público criado por um Service `LoadBalancer`. Na Fase 3, o API Gateway HTTP API é a porta pública única; manter essa exposição permitiria contornar o Gateway.

É necessário um backend privado, com listener determinístico para o VPC Link, sem introduzir AWS Load Balancer Controller, Helm, Pod Identity, IRSA, TargetGroupBinding ou o Service Controller legado.

## 2. Decisão

O repositório `oficina-mecanica-infra-kubernetes` cria explicitamente, por Terraform, o NLB interno `oficina-mecanica-api-nlb-dev`, seu Security Group, listener TCP/80, Target Group `oficina-mecanica-api-tg-dev` e o vínculo deste Target Group ao ASG do Managed Node Group.

O fluxo final é:

```text
API Gateway HTTP API
  → VPC Link
  → NLB interno
  → Target Group (target_type = instance)
  → EKS Managed Node Group / ASG
  → Kubernetes Service NodePort
  → Pods oficina-mecanica-api:8080
```

O contrato de NodePort é `30080`. O ASG é descoberto dinamicamente a partir do EKS Managed Node Group, sem fixar o seu nome físico. O NLB usa as sub-redes privadas e habilita explicitamente cross-zone load balancing: o laboratório possui um único worker node, que pode estar em apenas uma das duas AZs do NLB.

O repositório da API continua dono do Deployment, HPA, ConfigMap, Secret e Service NodePort. O Service não cria este NLB.

## 3. Alternativas avaliadas

1. **AWS Load Balancer Controller com target IP:** abordagem moderna avaliada, mas requer identidade AWS apropriada.
2. **Pod Identity:** bloqueada pelas restrições de IAM do AWS Academy.
3. **IRSA:** bloqueado pelas restrições de IAM do AWS Academy.
4. **LBC com a LabRole dos nodes:** rejeitado por não fornecer isolamento nem menor privilégio adequados.
5. **Service Controller legado:** rejeitado para recurso novo.

Foi escolhida a combinação NLB explícito, target `instance` e NodePort por manter recursos nomeados e state Terraform explícito, listener ARN determinístico, compatibilidade com o laboratório, ausência de mecanismo legado e menor complexidade operacional sem usar uma identidade IAM inadequada.

## 4. Consequências

### Positivas

- O API Gateway é a única entrada pública final.
- A infraestrutura de rede do private ingress possui ownership explícito e contratos SSM claros.
- O NLB encaminha apenas para o NodePort, com regra SG por referência ao SG do NLB.
- A associação ao ASG acompanha a substituição de instâncias do Managed Node Group.

### Cuidados

- O SG do NLB inicia sem ingress; a futura esteira do API Gateway criará a regra VPC Link → NLB em TCP/80.
- Até o PR posterior criar o Service NodePort, o Target Group pode não possuir alvo saudável. Isso não representa prontidão E2E.
- O Classic Load Balancer público existente só será removido após a validação E2E documentada no backlog F3-012.

## 5. Referências

- RFC-0002 — API Gateway com VPC Link e NLB Interno.
- RFC-0003 — Separação de Esteiras por Recurso e Responsabilidade.
- RFC-0004 — Compartilhamento de Outputs entre Esteiras via AWS Systems Manager Parameter Store.
- Tech Challenge FIAP — Fase 3.
