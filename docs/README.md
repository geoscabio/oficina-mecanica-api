# Documentação

Este é o índice mestre da documentação do projeto. Use este arquivo para navegar sem depender do contexto da conversa.

## Leitura recomendada

1. [`../README.md`](../README.md) para visão geral, execução local, CI/CD e entrega.
2. [`projeto/decisoes.md`](projeto/decisoes.md) para decisões arquiteturais e trade-offs AWS Academy.
3. [`deploy/github-actions.md`](deploy/github-actions.md) para entender a esteira e o Git Flow.
4. [`deploy/deploy-aws.md`](deploy/deploy-aws.md) para provisionar, validar e destruir o ambiente AWS.
5. [`projeto/pendencias.md`](projeto/pendencias.md) para itens manuais antes do vídeo/PDF final.

## Índice por tema

| Tema | Caminho | Quando usar |
| --- | --- | --- |
| Visão geral do projeto | [`../README.md`](../README.md) | Apresentação principal para avaliadores e execução rápida. |
| Deploy e esteira | [`deploy/README.md`](deploy/README.md) | Entrada para guias operacionais de CI/CD, AWS e destroy. |
| GitHub Actions | [`deploy/github-actions.md`](deploy/github-actions.md) | Explicar CI, CD Development, release lógico e production lógico. |
| AWS real | [`deploy/deploy-aws.md`](deploy/deploy-aws.md) | Configurar GitHub Environment, rodar deploy e encerrar recursos. |
| Guardrails AWS Academy | [`deploy/aws-academy-guardrails.md`](deploy/aws-academy-guardrails.md) | Evitar gasto indevido e operar com segurança no Learner Lab. |
| Projeto | [`projeto/README.md`](projeto/README.md) | Entrada para backlog, decisões e pendências. |
| Backlog | [`projeto/backlog.md`](projeto/backlog.md) | Conferir status técnico e próximos passos da entrega. |
| Decisões | [`projeto/decisoes.md`](projeto/decisoes.md) | Justificar arquitetura, deploy e trade-offs. |
| Pendências | [`projeto/pendencias.md`](projeto/pendencias.md) | Controlar evidências, diagramas, vídeo e PDF. |
| Evidências | [`evidencias/`](evidencias/) | Registrar prints e saídas reais de qualidade, segurança e infra. |
| OpenAPI | [`openapi/`](openapi/) | Consultar contrato versionado da API. |
| C4 Model | [`architecture/diagrams/c4-model/README.md`](architecture/diagrams/c4-model/README.md) | Ver modelo C4 validado e arquivos Structurizr/SVG. |
| Diagramas finais | [`architecture/diagrams/`](architecture/diagrams/) | Guardar AWS, Kubernetes, Docker e CI/CD finais. |

## Índice de infraestrutura

| Área | Caminho | Papel |
| --- | --- | --- |
| Runtime local | [`../local/README.md`](../local/README.md) | Explica Docker Compose e Kubernetes locais. |
| Docker local | [`../local/docker/README.md`](../local/docker/README.md) | Comandos do Docker Compose local. |
| Kubernetes local | [`../local/kubernetes/README.md`](../local/kubernetes/README.md) | Manifests locais para Docker Desktop Kubernetes. |
| Terraform EKS | [`../infra/terraform/modules/eks/README.md`](../infra/terraform/modules/eks/README.md) | Módulo do cluster EKS e node group. |
| Terraform RDS | [`../infra/terraform/modules/rds/README.md`](../infra/terraform/modules/rds/README.md) | M?dulo RDS SQL Server e security group. |
| Terraform VPC | [`../infra/terraform/modules/networking/vpc/README.md`](../infra/terraform/modules/networking/vpc/README.md) | M?dulo VPC, subnets, rotas e NAT Gateway. |
| Terraform ECR | [`../infra/terraform/modules/ecr/README.md`](../infra/terraform/modules/ecr/README.md) | M?dulo ECR usado pela imagem Docker da API. |

## Regras

- Não versionar credenciais, tokens, kubeconfig, secrets ou outputs sensíveis.
- Diagramas ainda não finalizados devem permanecer apenas como diretórios reservados com `.gitkeep`.
- Evidências com prints reais devem ser coladas antes da montagem do PDF final.
- O ambiente AWS Academy é temporário: todo `apply` deve ter plano de `destroy`.
