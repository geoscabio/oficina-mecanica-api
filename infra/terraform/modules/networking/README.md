# Networking

## Objetivo

Este módulo é responsável por provisionar a infraestrutura de rede da aplicação na AWS.

A implementação segue boas práticas recomendadas pela AWS e pela HashiCorp, fornecendo a base necessária para os demais serviços da infraestrutura, como Amazon EKS, Amazon RDS e Amazon ECR.

## Recursos criados

Atualmente este módulo provisiona os seguintes recursos:

- Virtual Private Cloud (VPC)
- Internet Gateway
- Elastic IP
- NAT Gateway
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
                  │                 │
          Public Subnet A    Public Subnet B
                  │
            NAT Gateway
                  │
──────────────────────────────────────────────────────
                  │
            Private Route Table
                  │
       ┌──────────┴──────────┐
       │                     │
Private Subnet A     Private Subnet B
```

## Estrutura do módulo

```text
networking/
├── internet-gateway.tf
├── locals.tf
├── nat-gateway.tf
├── outputs.tf
├── README.md
├── route-tables.tf
├── subnets.tf
├── variables.tf
└── vpc.tf
```

## Decisões de arquitetura

A VPC foi projetada seguindo a arquitetura recomendada pela AWS para aplicações executadas em ambientes privados.

Os recursos de banco de dados e Kubernetes são implantados em sub-redes privadas, enquanto o NAT Gateway permanece em uma subnet pública, permitindo que esses recursos realizem conexões de saída para serviços da AWS sem exposição direta à Internet.

Essa arquitetura atende aos requisitos de isolamento de rede definidos para a segunda fase do Tech Challenge e aproxima a solução de um ambiente de produção.

## Boas práticas adotadas

- Organização por responsabilidade de cada recurso.
- Utilização de módulos reutilizáveis do Terraform.
- Centralização das tags utilizando `locals` e `merge()`.
- Separação entre ambientes (`environments`) e módulos (`modules`).
- Padronização da nomenclatura dos recursos.

## Observações

Este módulo representa apenas a camada de rede da infraestrutura.

Os recursos de banco de dados, registro de imagens, Kubernetes e identidade serão implementados em módulos independentes.