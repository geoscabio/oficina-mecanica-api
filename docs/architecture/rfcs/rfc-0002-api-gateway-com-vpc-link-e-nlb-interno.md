# RFC-0002 — API Gateway com VPC Link e NLB Interno

## Status

Aceita para implementação na Fase 3.

## Contexto

Atualmente, a API executada no Kubernetes é exposta por um Load Balancer público. Esse modelo permite que requisições externas cheguem diretamente à aplicação, sem passar por uma camada central de controle, roteamento e observabilidade.

Na Fase 3, será utilizado um API Gateway como porta pública única da solução. A autenticação de clientes por CPF será realizada por uma Lambda, enquanto as rotas da aplicação continuarão sendo atendidas pela API executada no Kubernetes.

A arquitetura deve garantir que a API permaneça privada dentro da VPC e que todo o tráfego público passe somente pelo API Gateway.

## Decisão

Será adotado o AWS API Gateway no modelo HTTP API como única porta pública da solução.

Será utilizado o estágio padrão `$default`, permitindo URLs públicas sem um prefixo de ambiente, como `/development`.

O Gateway possuirá duas integrações:

| Rota pública | Destino | Tipo de integração |
| --- | --- | --- |
| `POST /auth/cpf` | `oficina-mecanica-auth-lambda` | Lambda Proxy Integration |
| `ANY /api/{proxy+}` | API no Kubernetes | HTTP Proxy Integration via VPC Link |

A integração com a API no Kubernetes seguirá este fluxo:

```text
Cliente
  → API Gateway
  → VPC Link
  → NLB interno
  → Service Kubernetes
  → Pods da oficina-mecanica-api
```

O NLB será interno e não terá exposição pública. O API Gateway será o único componente acessível diretamente pela internet.

## Roteamento

### Autenticação de cliente

A rota pública `POST /auth/cpf` será direcionada à Lambda de autenticação.

A Lambda será responsável por:

1. Validar o formato do CPF.
2. Consultar o cliente no RDS.
3. Verificar se o cliente está ativo.
4. Emitir um JWT válido quando a autenticação for autorizada.
5. Retornar resposta genérica quando o CPF não existir ou o cliente estiver inativo.

### Aplicação principal

As rotas iniciadas por `/api/` serão encaminhadas pelo VPC Link para o NLB interno, preservando o caminho necessário para a API ASP.NET Core.

Exemplo:

```text
GET /api/v1/clientes/me/ordens-servico
```

A requisição será encaminhada para a API no Kubernetes, que continuará validando o JWT e aplicando as regras de autorização por papel.

## Segurança

O API Gateway não substituirá a autenticação da API.

A aplicação continuará validando:

- Assinatura do JWT.
- Emissor.
- Audiência.
- Expiração.
- Papel do usuário.
- Claim `cliente_id` nas rotas destinadas ao cliente.

Não será utilizado Lambda Authorizer neste MVP. A Lambda será responsável apenas pela autenticação inicial do cliente por CPF e pela emissão do JWT.

Essa decisão reduz a complexidade inicial e mantém a segurança da API independente do Gateway.

## Observabilidade

O API Gateway deverá gerar logs estruturados de acesso em formato JSON, incluindo informações que permitam a correlação com a jornada da requisição.

A definição detalhada de logs, traces, métricas, dashboards e alertas será tratada na RFC de observabilidade com Datadog.

## Consequências

### Positivas

- O API Gateway se torna a única porta pública da solução.
- A API no Kubernetes deixa de ficar acessível diretamente pela internet.
- A autenticação de cliente por CPF fica isolada em uma Lambda.
- O roteamento entre Lambda e API principal fica centralizado.
- A arquitetura fica alinhada aos requisitos de API Gateway, Function Serverless e Kubernetes.
- A solução permite registrar logs de acesso em uma única camada de entrada.

### Trade-offs e cuidados

- A solução passa a depender de API Gateway, VPC Link e NLB, aumentando a quantidade de recursos a serem provisionados.
- O NLB, o VPC Link e o API Gateway devem pertencer à mesma conta AWS.
- O NLB interno deve estar disponível antes da criação da integração privada do API Gateway.
- Mudanças no caminho das rotas devem preservar a compatibilidade com os endpoints existentes da API.
- O API Gateway adiciona uma camada de latência, aceitável para o escopo do projeto.

## Fora de escopo

- Implementar Lambda Authorizer.
- Expor diretamente o NLB ou a API no Kubernetes para a internet.
- Criar múltiplos API Gateways.
- Definir dashboards, alertas e instrumentação completa do Datadog.
- Implementar infraestrutura, permissões AWS ou deploy.

## Critérios de aceite

- `POST /auth/cpf` é atendido pela Lambda de autenticação.
- As rotas `/api/*` são encaminhadas para a API no Kubernetes por meio do VPC Link.
- O NLB da API é interno e não pode ser acessado diretamente pela internet.
- O API Gateway é a única entrada pública da API.
- Uma chamada sem JWT para rota protegida retorna `401 Unauthorized`.
- Uma chamada com JWT válido chega à API e respeita as regras de autorização existentes.
- O Gateway registra logs de acesso estruturados para posterior integração com Datadog.

## Referências

- [AWS API Gateway HTTP APIs](https://docs.aws.amazon.com/apigateway/latest/developerguide/http-api.html)
- [AWS API Gateway: integrações privadas com VPC Link](https://docs.aws.amazon.com/apigateway/latest/developerguide/http-api-develop-integrations-private.html)
- Tech Challenge FIAP — Fase 3.