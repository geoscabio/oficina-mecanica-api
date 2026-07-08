# AWS - Diagrama de arquitetura cloud

# Objetivo

Gerar no Eraser um diagrama AWS profissional, usando ícones oficiais da AWS, representando somente os recursos provisionados pelo Terraform atual.

# Escopo

Ambiente `dev` em `us-east-1`, com VPC, subnets, tabelas de rota, internet gateway e Amazon ECR.

# Recursos identificados no projeto

- Provider AWS com região configurável por `var.aws_region`.
- VPC `oficina-vpc-dev` com CIDR `10.0.0.0/16`.
- Duas Availability Zones: `us-east-1a` e `us-east-1b`.
- Duas subnets públicas:
  - `10.0.1.0/24`
  - `10.0.2.0/24`
- Duas subnets privadas:
  - `10.0.11.0/24`
  - `10.0.12.0/24`
- Internet Gateway associado à VPC.
- Public Route Table com rota `0.0.0.0/0` para o Internet Gateway.
- Private Route Table sem NAT Gateway definido.
- Amazon ECR repository `oficina-api`.
- ECR com `image_tag_mutability = IMMUTABLE`.
- ECR com scan on push habilitado.

# Recursos planejados

Não representar recursos planejados como existentes. EKS, RDS, ALB, NAT Gateway e Secrets Manager podem ser citados apenas como itens fora do escopo atual, se necessário em nota lateral.

# Recursos que não devem aparecer

- Amazon EKS.
- Amazon RDS.
- Application Load Balancer.
- NAT Gateway.
- EC2.
- S3.
- CloudFront.
- Route 53.
- Secrets Manager.
- Recursos Kubernetes internos.

# Layout recomendado

Use uma moldura externa "AWS Cloud - us-east-1". Dentro dela, coloque a VPC `oficina-vpc-dev`.

Organize a VPC em duas colunas por Availability Zone:

- Coluna `us-east-1a`: public subnet `10.0.1.0/24` acima e private subnet `10.0.11.0/24` abaixo.
- Coluna `us-east-1b`: public subnet `10.0.2.0/24` acima e private subnet `10.0.12.0/24` abaixo.

Posicione o Internet Gateway fora da VPC, no topo esquerdo, ligado à VPC e à Public Route Table. Posicione o ECR fora da VPC, no topo direito, como serviço regional.

# Hierarquia visual

- Nível 1: AWS Cloud.
- Nível 2: Region `us-east-1`.
- Nível 3: VPC `oficina-vpc-dev`.
- Nível 4: Availability Zones.
- Nível 5: Subnets e route tables.
- Serviço regional separado: Amazon ECR `oficina-api`.

# Fluxos

- Public Route Table envia tráfego `0.0.0.0/0` para Internet Gateway.
- Public subnets são associadas à Public Route Table.
- Private subnets são associadas à Private Route Table.
- Pipeline ou operador publica imagem Docker no ECR `oficina-api`.

# Prompt final para o Eraser

Crie um Cloud Architecture Diagram no Eraser usando ícones oficiais AWS. O diagrama deve representar a infraestrutura Terraform atual do projeto Oficina Mecânica API no ambiente dev, região us-east-1. Mostre uma moldura "AWS Cloud - us-east-1" e dentro dela uma VPC chamada "oficina-vpc-dev" com CIDR 10.0.0.0/16. Divida a VPC em duas Availability Zones: us-east-1a e us-east-1b. Em us-east-1a, mostre uma public subnet 10.0.1.0/24 e uma private subnet 10.0.11.0/24. Em us-east-1b, mostre uma public subnet 10.0.2.0/24 e uma private subnet 10.0.12.0/24. Mostre um Internet Gateway conectado à VPC e uma Public Route Table com rota 0.0.0.0/0 para o Internet Gateway, associada às subnets públicas. Mostre uma Private Route Table associada às subnets privadas, sem NAT Gateway. Mostre Amazon ECR como serviço regional fora da VPC, com repository "oficina-api", image tags immutable e scan on push. Não desenhe EKS, RDS, ALB, NAT Gateway, EC2, S3, CloudFront, Route 53 ou Secrets Manager, pois não existem no Terraform atual. Use fundo claro, texto legível, legenda simples e ícones oficiais AWS.
