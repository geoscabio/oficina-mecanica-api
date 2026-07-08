# Diagramas de arquitetura

Esta pasta concentra os artefatos de diagramas da arquitetura do projeto.

## Estado atual

- `c4-model/`: modelo C4 oficial em Structurizr DSL, com exports SVG dos níveis L1, L2 e L3.
- `aws/`: especificação técnica e prompt para diagrama AWS baseado no Terraform existente.
- `kubernetes/`: especificação técnica e prompt para diagrama Kubernetes baseado nos manifests existentes.
- `terraform/`: especificação técnica e prompt para diagrama da organização de infraestrutura como código.
- `runtime/`: especificação técnica e prompt para diagrama de execução local com Docker.
- `ci-cd/`: avaliação do estado atual de CI/CD. Não há workflow versionado no momento.

## Convenções

- C4 Model deve permanecer em Structurizr DSL.
- Diagramas AWS, Kubernetes, Terraform, Runtime e CI/CD devem ser produzidos no Eraser.
- Diagramas não devem incluir recursos planejados como se já existissem.
- Exports finais devem ser salvos na pasta da categoria correspondente.
