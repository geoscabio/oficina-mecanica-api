# ADR-0011 — Load Balancer provisionado via Kubernetes Service em vez de recurso Terraform explícito

## Status

**Status:** ✅ Aceito
**Data:** 13/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> A API precisa de um ponto de entrada público na internet, direcionando o tráfego para o Pod correto dentro do EKS. Existem duas formas comuns de expor isso na AWS: criar o Load Balancer explicitamente como um recurso Terraform (`aws_lb`) apontando para o cluster, ou deixar o próprio Kubernetes provisionar o Load Balancer automaticamente a partir de um `Service` do tipo `LoadBalancer`.

## 2. Fatores Decisivos (Drivers)

- **Menos duplicação de configuração:** apontar manualmente um `aws_lb` para os targets do EKS exigiria sincronizar IPs/portas de pods manualmente ou usar um Target Group Binding adicional.
- **Simplicidade:** o cenário não exige regras de roteamento HTTP avançadas (path-based routing, múltiplos serviços) que justificariam um Application Load Balancer via AWS Load Balancer Controller e Ingress.
- **Padrão nativo do Kubernetes:** o próprio Kubernetes já resolve esse problema através do cloud provider integrado do EKS.

## 3. Decisão Proposta

> Não criar nenhum recurso `aws_lb` explícito no Terraform. Em vez disso, o recurso `kubernetes_service_v1.oficina_mecanica_api` (em `infra/terraform/environments/dev/api-service.tf`) é declarado com `type = "LoadBalancer"`. O cloud provider nativo do EKS cria automaticamente um **Classic Load Balancer**, sem instalar o AWS Load Balancer Controller e sem anotação para NLB/ALB.

## 4. Justificativa

- Sem o AWS Load Balancer Controller instalado e sem anotação `service.beta.kubernetes.io/aws-load-balancer-type`, o comportamento padrão/legado do EKS ao ver `type: LoadBalancer` é provisionar um Classic Load Balancer — suficiente para expor uma única API HTTP.
- O Terraform mantém o `Service` no seu state; como consequência direta, um único `terraform destroy` remove o `Service` e a AWS remove automaticamente o Load Balancer associado — não é necessário gerenciar o ciclo de vida do Load Balancer separadamente.
- As subnets públicas já são tagueadas (`kubernetes.io/role/elb = "1"`) especificamente para que o cloud provider do EKS saiba onde posicionar esse Load Balancer.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Menos código Terraform: nenhum recurso `aws_lb`, `aws_lb_target_group` ou `aws_lb_listener` para manter.
- Ciclo de vida do Load Balancer totalmente amarrado ao `Service` Kubernetes — criado e destruído junto, sem risco de ficar órfão.
- Simples de auditar: `kubectl get svc` já mostra o endpoint público real.

### ❌ Negativo (Perdas/Riscos)

- Classic Load Balancer é um recurso mais antigo da AWS, sem os recursos de um Application Load Balancer (roteamento por path/host, WAF integrado) ou a eficiência de um Network Load Balancer.
- Sem controle granular via Terraform sobre o Load Balancer (ex.: não é possível anexar regras específicas de segurança nele diretamente como recurso Terraform nomeado).
- Para múltiplos serviços HTTP no futuro, seria necessário migrar para Ingress + AWS Load Balancer Controller (ALB), evitando um Load Balancer por serviço.

## 6. Referências

- **AWS.** *Network Load Balancing on Amazon EKS*. 2026.
- **KUBERNETES.** *Service — type LoadBalancer*. 2026.
- Detalhamento: [`docs/deploy/deploy-aws.md`](../../deploy/deploy-aws.md), seção "Onde o Load Balancer é criado".
