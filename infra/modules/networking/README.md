# Módulo Networking

## Objetivo

Este módulo é responsável pelo provisionamento da infraestrutura de rede da aplicação na AWS.

## Recursos

Atualmente o módulo contempla:

- Virtual Private Cloud (VPC)

Recursos planejados:

- Internet Gateway
- Subnets públicas
- Subnets privadas
- Route Tables
- Route Table Associations
- NAT Gateway

## Variáveis

| Nome | Descrição |
|------|-----------|
| name | Nome da VPC |
| cidr_block | Bloco CIDR da VPC |
| availability_zones | Zonas de disponibilidade |
| public_subnet_cidrs | CIDRs das subnets públicas |
| private_subnet_cidrs | CIDRs das subnets privadas |

## Outputs

| Nome | Descrição |
|------|-----------|
| vpc_id | Identificador da VPC |