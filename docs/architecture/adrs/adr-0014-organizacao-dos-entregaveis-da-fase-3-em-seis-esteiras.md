# ADR-0014 — Organização dos Entregáveis da Fase 3 em Seis Esteiras

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

O Tech Challenge da Fase 3 exige quatro repositórios separados, cada um com CI/CD implementado:

1. Lambda de autenticação.
2. Infraestrutura Kubernetes.
3. Infraestrutura do banco de dados gerenciado.
4. Aplicação principal executando em Kubernetes.

A arquitetura definida para a Oficina Mecânica também inclui uma VPC compartilhada e um API Gateway como porta pública única. Esses recursos possuem responsabilidades, dependências e ciclos de vida próprios.

Manter VPC, EKS, RDS, Lambda, API e API Gateway em um único repositório aumentaria o acoplamento entre as alterações e dificultaria o controle de deploy, destroy e responsabilidades. Por outro lado, criar repositórios adicionais não pode tornar ambígua a entrega dos quatro itens obrigatórios da FIAP.

## 2. Fatores Decisivos

- Cumprir os quatro entregáveis obrigatórios da Fase 3.
- Separar recursos com responsabilidades e ciclos de vida distintos.
- Manter a VPC como infraestrutura compartilhada, sem vinculá-la exclusivamente ao banco ou ao Kubernetes.
- Isolar o API Gateway como camada pública de entrada da solução.
- Permitir pipelines de CI/CD independentes por tipo de recurso.
- Tornar a ordem de provisionamento e destruição mais clara.
- Evitar que alterações na aplicação provoquem alterações desnecessárias em toda a infraestrutura.

## 3. Decisão

A solução será organizada em seis esteiras e seis repositórios:

| Repositório | Responsabilidade |
| --- | --- |
| `oficina-mecanica-infra-vpc` | VPC, sub-redes, rotas, Internet Gateway, NAT Gateway e grupos de segurança base |
| `oficina-mecanica-infra-rds` | RDS SQL Server, grupo de sub-redes, grupo de segurança do banco e outputs |
| `oficina-mecanica-infra-kubernetes` | EKS, node groups, ECR e componentes de infraestrutura do cluster |
| `oficina-mecanica-auth-lambda` | Lambda .NET de autenticação por CPF e emissão de JWT |
| `oficina-mecanica-api` | API ASP.NET Core, Dockerfile, migrations, carga de trabalho no Kubernetes, Swagger e Postman |
| `oficina-mecanica-infra-api-gateway` | API Gateway, VPC Link, rotas, integrações e logs de acesso |

Os quatro entregáveis obrigatórios serão apresentados da seguinte forma:

| Entregável obrigatório da FIAP | Esteira ou repositório correspondente |
| --- | --- |
| Lambda de autenticação | `oficina-mecanica-auth-lambda` |
| Infraestrutura Kubernetes | `oficina-mecanica-infra-vpc` e `oficina-mecanica-infra-kubernetes` |
| Infraestrutura do banco de dados gerenciado | `oficina-mecanica-infra-rds` |
| Aplicação principal em Kubernetes | `oficina-mecanica-api` |

O repositório `oficina-mecanica-infra-api-gateway` será uma esteira adicional, justificada pela responsabilidade exclusiva de controlar a entrada pública e integrar os consumidores externos à API privada.

## 4. Justificativa

A VPC será separada porque é uma fundação compartilhada pelo RDS, EKS, Lambda, VPC Link e API Gateway. Sua criação e destruição devem ser controladas independentemente dos demais recursos.

O Kubernetes e o RDS serão separados porque representam infraestruturas gerenciadas diferentes, com configurações, validações e ciclos de alteração próprios.

A Lambda será isolada da API principal porque possui uma responsabilidade específica: autenticar clientes por CPF e emitir JWT. Essa separação atende diretamente ao requisito de Function Serverless.

A API continuará em seu próprio repositório porque concentra regras de negócio, migrations, testes, imagem Docker, documentação de endpoints e carga de trabalho executada no Kubernetes.

O API Gateway será isolado para manter a configuração de rotas públicas, VPC Link e integrações privadas independente da aplicação e da infraestrutura do cluster.

## 5. Consequências

### Positivas

- Responsabilidades e limites de cada repositório ficam explícitos.
- Cada esteira possui CI/CD, histórico e ciclo de deploy independentes.
- A VPC, o RDS, o EKS, a Lambda, a API e o API Gateway podem ser validados de forma isolada.
- A ordem de apply e destroy torna-se mais previsível.
- O projeto mantém os quatro entregáveis obrigatórios identificáveis para avaliação.
- A documentação arquitetural e a apresentação da solução tornam-se mais fáceis de explicar.

### Negativas e riscos

- A solução exige coordenação entre múltiplos repositórios.
- Algumas esteiras dependem de outputs produzidos por outras.
- A equipe deverá manter convenções consistentes de nomes, ambientes e parâmetros.
- O processo de deploy possui mais etapas do que um repositório centralizado.
- Falhas em uma esteira podem bloquear esteiras dependentes.

## 6. Referências

- Tech Challenge FIAP — Fase 3.
- RFC-0003 — Separação de Esteiras por Recurso e Responsabilidade.
- RFC-0004 — Compartilhamento de Outputs entre Esteiras via AWS Systems Manager Parameter Store.
- RFC-0005 — Ordem e Guardrails de Apply e Destroy entre Esteiras.