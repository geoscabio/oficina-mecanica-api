# Networking

## Objetivo

Este módulo é responsável por provisionar a infraestrutura de rede da aplicação na AWS.

A implementação segue boas práticas recomendadas pela AWS e pela HashiCorp, fornecendo a base necessária para os demais serviços da infraestrutura, como Amazon EKS, Amazon RDS e Amazon ECR.

## Recursos criados

Atualmente este módulo provisiona os seguintes recursos:

- Virtual Private Cloud (VPC)
- Internet Gateway
- Subnets públicas
- Subnets privadas
- Route Table pública
- Route Table privada
- Associações entre Subnets e Route Tables

## Arquitetura

```text
                    Internet
                        │
                Internet Gateway
                        │
               Public Route Table
                  │            │
          Public Subnet A  Public Subnet B

──────────────────────────────────────────────

              Private Route Table
                  │            │
        Private Subnet A  Private Subnet B
```

## Estrutura do módulo

```text
networking/
├── internet-gateway.tf
├── locals.tf
├── outputs.tf
├── README.md
├── route-tables.tf
├── subnets.tf
├── variables.tf
└── vpc.tf
```

## Próximos passos

Este módulo será expandido nas próximas etapas do projeto para incluir:

- Elastic IP
- NAT Gateway
- Rotas privadas para acesso à Internet
- Tags específicas para integração com Amazon EKS

## Boas práticas adotadas

- Organização por responsabilidade de cada recurso.
- Utilização de módulos reutilizáveis do Terraform.
- Centralização das tags utilizando `locals` e `merge()`.
- Separação entre ambientes (`environments`) e módulos (`modules`).
- Padronização da nomenclatura dos recursos.

## Observações

Este módulo representa apenas a camada de rede da infraestrutura.

Os recursos de banco de dados, registro de imagens, Kubernetes e identidade serão implementados em módulos independentes.