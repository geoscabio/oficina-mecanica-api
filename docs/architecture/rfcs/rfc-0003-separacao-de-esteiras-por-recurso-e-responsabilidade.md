# RFC-0003 — Separação de Esteiras por Recurso e Responsabilidade

## Status

Aceita para implementação na Fase 3.

## Contexto

O projeto atual concentra aplicação, infraestrutura de rede, banco de dados, Kubernetes e pipeline de deploy em um único repositório.

A Fase 3 exige quatro repositórios com CI/CD:

1. Lambda de autenticação.
2. Infraestrutura Kubernetes.
3. Infraestrutura do banco de dados gerenciado.
4. Aplicação principal executando em Kubernetes.

Além desses entregáveis, a arquitetura definida para a solução inclui uma infraestrutura de rede compartilhada e um API Gateway como porta pública única. Manter todos os recursos no mesmo repositório dificultaria a separação de responsabilidades, o controle do ciclo de vida dos recursos e a rastreabilidade dos deploys.

## Decisão

A solução será organizada em seis repositórios, separados por recurso e responsabilidade:

| Repositório | Responsabilidade principal |
| --- | --- |
| `oficina-mecanica-infra-vpc` | VPC, sub-redes, rotas, Internet Gateway, NAT Gateway e grupos de segurança base |
| `oficina-mecanica-infra-rds` | RDS SQL Server, grupos de sub-redes, grupo de segurança do banco e outputs |
| `oficina-mecanica-infra-kubernetes` | EKS, node groups, ECR e componentes de infraestrutura do cluster |
| `oficina-mecanica-auth-lambda` | Lambda .NET de autenticação por CPF e emissão de JWT |
| `oficina-mecanica-api` | API ASP.NET Core, Dockerfile, migrations, manifests da carga de trabalho, Swagger e coleção Postman |
| `oficina-mecanica-infra-api-gateway` | API Gateway, VPC Link, rotas, integrações e logs de acesso |

## Relação com os requisitos da FIAP

A separação em seis repositórios não substitui os quatro entregáveis obrigatórios. Ela organiza responsabilidades adicionais necessárias para uma arquitetura mais clara.

| Entregável obrigatório | Repositórios relacionados |
| --- | --- |
| Lambda de autenticação | `oficina-mecanica-auth-lambda` |
| Infraestrutura Kubernetes | `oficina-mecanica-infra-vpc` e `oficina-mecanica-infra-kubernetes` |
| Infraestrutura do banco de dados gerenciado | `oficina-mecanica-infra-rds` |
| Aplicação principal em Kubernetes | `oficina-mecanica-api` |

O repositório `oficina-mecanica-infra-api-gateway` é uma esteira adicional. Ele existe para isolar a camada pública de entrada, seu roteamento e suas integrações privadas com a VPC.

## Limites de responsabilidade

Cada repositório deverá possuir um propósito claro e não deverá provisionar recursos pertencentes a outra esteira.

### Infraestrutura de rede

O repositório `oficina-mecanica-infra-vpc` será responsável somente pelos recursos compartilhados de rede. Ele não criará RDS, EKS, Lambda, API Gateway ou cargas de trabalho da aplicação.

### Infraestrutura de banco de dados

O repositório `oficina-mecanica-infra-rds` será responsável pelo RDS e pela conectividade necessária para o banco.

As migrations e os dados de seed continuarão sendo responsabilidade da aplicação, no repositório `oficina-mecanica-api`. A esteira de banco não deve conter regras de negócio nem código da API.

### Infraestrutura Kubernetes

O repositório `oficina-mecanica-infra-kubernetes` será responsável pelo cluster EKS, node groups, repositório ECR e complementos de infraestrutura do cluster.

O repositório da API será responsável pela imagem da aplicação e pelos manifests ou definições de carga de trabalho necessários para executar a API no cluster.

### Lambda de autenticação

O repositório `oficina-mecanica-auth-lambda` será responsável exclusivamente pela autenticação de clientes por CPF, consulta ao RDS e emissão de JWT.

Ele não deverá assumir responsabilidades da API principal, como consulta de ordens de serviço ou regras operacionais da oficina.

### API Gateway

O repositório `oficina-mecanica-infra-api-gateway` será responsável pelas rotas públicas, integrações com a Lambda, VPC Link, integração privada com o NLB interno e logs de acesso.

Ele não deverá conter regras de negócio nem código da API principal.

## Integração entre esteiras

As esteiras serão integradas por contratos explícitos de entrada e saída.

Cada infraestrutura publicará apenas os dados necessários para os repositórios dependentes, como identificadores de VPC, sub-redes privadas, endpoint do RDS, nome do cluster, URL do ECR, ARN da Lambda e ARN do listener do NLB.

A convenção de compartilhamento desses dados será detalhada na RFC de uso do AWS Systems Manager Parameter Store.

Nenhuma esteira deverá depender de valores copiados manualmente entre repositórios.

## CI/CD

Cada repositório terá sua própria pipeline de CI/CD, com responsabilidades compatíveis com o recurso gerenciado.

Exemplos:

- Repositórios Terraform: validação de formatação, validação de configuração, plano, apply ou destroy controlado e validação do recurso criado.
- API: build, testes, análise de código, build da imagem Docker, publicação no ECR e deploy no Kubernetes.
- Lambda: build, testes, empacotamento, publicação e validação da função.
- API Gateway: validação Terraform, plano, provisionamento das rotas e validação das integrações.

As pipelines deverão respeitar branch protegida, Pull Request obrigatório e o padrão de branches definido pelo projeto.

## Ordem de dependências

A separação de esteiras exige uma ordem explícita de provisionamento:

```text
1. infra-vpc
2. infra-kubernetes e infra-rds
3. auth-lambda
4. api
5. infra-api-gateway
```

A destruição do ambiente deverá seguir a ordem inversa:

```text
1. infra-api-gateway
2. api
3. auth-lambda
4. infra-kubernetes e infra-rds
5. infra-vpc
```

A definição detalhada dos guardrails de apply e destroy será tratada em RFC específica.

## Consequências

### Positivas

- Responsabilidades de infraestrutura e aplicação ficam mais claras.
- Cada recurso possui pipeline, histórico e ciclo de deploy independentes.
- Falhas e mudanças ficam mais fáceis de localizar.
- O projeto atende aos quatro entregáveis exigidos e documenta as esteiras adicionais.
- A destruição controlada do ambiente fica mais previsível.
- A documentação e a apresentação da arquitetura ficam mais fáceis de explicar.

### Trade-offs e cuidados

- A solução passa a exigir coordenação entre repositórios.
- Uma alteração pode depender da publicação prévia de outputs de outra esteira.
- É necessário manter convenções consistentes de nomes, ambientes e parâmetros.
- A equipe deve evitar duplicar recursos ou responsabilidades entre repositórios.
- O processo de deploy possui mais etapas do que um único repositório centralizado.

## Fora de escopo

- Implementar a extração física dos repositórios.
- Definir detalhes do AWS Systems Manager Parameter Store.
- Definir os guardrails de apply e destroy.
- Implementar pipelines, infraestrutura ou deploys.

## Critérios de aceite

- Os seis repositórios possuem responsabilidade clara e documentada.
- Os quatro entregáveis obrigatórios da FIAP podem ser identificados sem ambiguidade.
- Nenhum recurso é provisionado por mais de uma esteira.
- As dependências entre repositórios são explícitas.
- Cada repositório possui pipeline de CI/CD própria.
- O compartilhamento de dados entre esteiras não depende de cópia manual.