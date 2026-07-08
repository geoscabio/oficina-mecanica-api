# Terraform - Diagrama de infraestrutura como código

# Objetivo

Gerar no Eraser um diagrama de organização Terraform mostrando como o ambiente `dev` compõe os módulos existentes.

# Escopo

Código Terraform em `infra/environments/dev` e módulos em `infra/modules/networking` e `infra/modules/registry`.

# Recursos identificados no projeto

- Ambiente `infra/environments/dev`.
- Provider AWS `hashicorp/aws ~> 6.0`.
- Terraform required version `~> 1.15.7`.
- Variável `aws_region`.
- `locals.common_tags` com `Project`, `Environment` e `ManagedBy`.
- Módulo `networking`.
- Módulo `registry`.
- Outputs de VPC, subnets e ECR.

# Recursos planejados

Não há módulo EKS, RDS, compute, load balancer ou observabilidade no Terraform atual.

# Recursos que não devem aparecer

- Módulos inexistentes.
- Recursos Kubernetes.
- Docker Compose.
- GitHub Actions.

# Layout recomendado

Use um diagrama de dependências ou arquitetura IaC:

`infra/environments/dev` no topo, ligado ao `provider aws`, `locals.common_tags`, `module.networking` e `module.registry`.

Dentro do módulo `networking`, mostrar VPC, subnets públicas, subnets privadas, internet gateway e route tables.

Dentro do módulo `registry`, mostrar Amazon ECR repository.

# Hierarquia visual

- Nível 1: Ambiente dev.
- Nível 2: Provider, variáveis e tags.
- Nível 3: Módulos.
- Nível 4: Recursos provisionados por módulo.
- Nível 5: Outputs.

# Fluxos

- O ambiente dev configura o provider AWS.
- O ambiente dev chama `module.networking`.
- O ambiente dev chama `module.registry`.
- O módulo networking expõe outputs de VPC e subnets.
- O módulo registry expõe outputs de repository name, URL e ARN.

# Prompt final para o Eraser

Crie um diagrama de infraestrutura como código no Eraser representando a organização Terraform do projeto Oficina Mecânica API. Mostre no topo o ambiente "infra/environments/dev". Dentro dele, represente provider AWS hashicorp/aws ~> 6.0, required_version ~> 1.15.7, variável aws_region e locals.common_tags com Project=OficinaMecanica, Environment=Development e ManagedBy=Terraform. A partir do ambiente dev, desenhe duas dependências: module.networking e module.registry. No module.networking, mostre os recursos AWS VPC "oficina-vpc-dev" CIDR 10.0.0.0/16, public subnets 10.0.1.0/24 e 10.0.2.0/24, private subnets 10.0.11.0/24 e 10.0.12.0/24, Internet Gateway, Public Route Table e Private Route Table. No module.registry, mostre Amazon ECR repository "oficina-api", immutable tags e scan on push. Mostre outputs: vpc_id, public_subnet_ids, private_subnet_ids, repository_name, repository_url e repository_arn. Não desenhe módulos EKS, RDS, ALB, compute, Kubernetes ou GitHub Actions porque não existem no Terraform atual.
