# ADR-0023 — Separação de ownership da infraestrutura compartilhada e deploy da API

**Data:** 13/09/2026  
**Status:** ✅ Aceito

## Contexto

A Fase 3 possui seis esteiras por responsabilidade. VPC, Kubernetes/EKS/ECR e RDS possuem ciclos de vida próprios e publicam contratos não secretos via SSM; a credencial do RDS permanece no AWS Secrets Manager.

## Decisão

Este repositório deixa de criar VPC, EKS, RDS e ECR compartilhado. Ele consome os contratos SSM e resolve temporariamente o segredo do RDS no CD para formar uma variável sensível de conexão. A API mantém o build/publicação da imagem, o deploy e os recursos Kubernetes específicos da aplicação.

O ECR permanece temporariamente em `oficina-mecanica-infra-kubernetes` para reduzir risco próximo à entrega. A possível migração futura para o repositório da API está registrada no backlog.

## Consequências

Há menor duplicidade, states separados, pipelines independentes e ownership explícito. A ordem operacional passa a ser VPC → Kubernetes/EKS/ECR → RDS → API → Auth Lambda → API Gateway. Helm não é introduzido nesta mudança; a possível centralização futura de manifests fica no backlog.
