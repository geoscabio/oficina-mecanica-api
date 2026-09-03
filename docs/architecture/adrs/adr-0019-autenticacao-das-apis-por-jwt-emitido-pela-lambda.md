# ADR-0019 — Autenticação das APIs por JWT Emitido pela Lambda

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

A Fase 3 exige que as rotas sensíveis da aplicação sejam protegidas por autenticação via CPF e que uma Function Serverless seja responsável por validar o CPF, consultar a existência e o status do cliente e gerar um token JWT válido para consumo das APIs protegidas.

Na arquitetura definida, a `oficina-mecanica-auth-lambda` será responsável pelo processo inicial de autenticação. Após a autenticação, o cliente precisará utilizar o token recebido para acessar as rotas protegidas da `oficina-mecanica-api`.

Era necessário definir o mecanismo de autenticação das chamadas posteriores à API e quais informações mínimas seriam transportadas no token.

## 2. Fatores Decisivos

- Utilizar um mecanismo de autenticação compatível com APIs REST.
- Permitir que a Lambda emita um token após validar o CPF e o status do cliente.
- Permitir que a API valide o token de forma independente da Lambda após sua emissão.
- Identificar o cliente autenticado sem transportar o CPF em todas as chamadas.
- Representar o papel do usuário autenticado.
- Permitir controle de validade e identificação individual dos tokens.
- Manter a autenticação compatível com o middleware de autorização já utilizado pela aplicação.
- Evitar a criação de uma dependência da API em chamadas à Lambda para cada requisição autenticada.

## 3. Decisão

A autenticação das APIs protegidas será baseada em **JSON Web Token (JWT)** emitido pela `oficina-mecanica-auth-lambda` após a validação do CPF e do status do cliente.

O fluxo será:

```text
Cliente
  → POST /auth/cpf
  → API Gateway
  → Auth Lambda
  → RDS
  → JWT
  → Cliente

Cliente
  → /api/*
  → API Gateway
  → API no EKS
  → Validação do JWT
  → Recurso protegido
```

A Lambda será responsável por:

1. Receber o CPF.
2. Validar o formato do CPF.
3. Consultar o cliente no RDS.
4. Verificar se o cliente existe e está ativo.
5. Gerar o JWT.
6. Retornar o token ao cliente.

A `oficina-mecanica-api` continuará responsável pela validação do JWT nas rotas protegidas, utilizando o mecanismo de autenticação e autorização do ASP.NET Core.

A API **não dependerá de uma nova chamada à Lambda para validar cada requisição autenticada**. A validade do token será verificada localmente pela própria aplicação.

O JWT deverá conter, no mínimo, as seguintes claims:

| Claim | Finalidade |
|---|---|
| `sub` | Identificador principal do cliente |
| `cpf_hash` | Representação não reversível do CPF para correlação controlada |
| `cliente_id` | Identificador interno do cliente |
| `role` | Papel de autorização, inicialmente `Cliente` |
| `jti` | Identificador único do token |
| `iss` | Emissor do token |
| `aud` | Audiência prevista para o token |
| `exp` | Data/hora de expiração |

O CPF **não será utilizado como claim em texto puro nem registrado nos logs**. Quando houver necessidade de identificação, será utilizado o `cliente_id`, uma representação mascarada ou um hash.

## 4. Justificativa

O JWT permite separar claramente as responsabilidades entre autenticação e consumo das APIs.

A Lambda concentra o processo de autenticação por CPF e a consulta do cliente no banco. Depois que o cliente é autenticado, o JWT permite que as requisições seguintes sejam validadas pela própria API sem necessidade de consultar novamente a Lambda ou o banco de dados.

Essa abordagem reduz a dependência entre os componentes durante o processamento das APIs protegidas e mantém o fluxo compatível com a arquitetura atual da aplicação ASP.NET Core.

A utilização das claims `cliente_id` e `role` também permite que a aplicação identifique o cliente autenticado e aplique regras de autorização sem expor o CPF nas chamadas subsequentes.

A claim `jti` fornece um identificador único para cada token, permitindo rastreabilidade sem registrar o conteúdo do JWT. As claims `iss`, `aud` e `exp` permitem validar, respectivamente, o emissor esperado, o destinatário do token e seu período de validade.

A decisão atende diretamente ao requisito da Fase 3 de utilizar uma Function Serverless para validar o CPF, consultar o cliente e gerar um JWT para consumo das APIs protegidas.

## 5. Consequências

### Positivas

- A autenticação por CPF fica centralizada na Lambda.
- A API consegue validar o token sem realizar nova chamada à Lambda.
- O cliente autenticado pode ser identificado por `cliente_id`.
- O papel `Cliente` pode ser utilizado pelo mecanismo de autorização da API.
- O CPF não precisa ser enviado em todas as chamadas autenticadas.
- O JWT possui validade controlada por expiração.
- Cada token pode ser identificado individualmente por meio da claim `jti`.
- A solução é compatível com a utilização de `[Authorize]` na API.
- A arquitetura atende diretamente ao requisito de autenticação da Fase 3.

### Negativas e riscos

- A segurança depende da proteção adequada da chave utilizada para assinatura dos tokens.
- Tokens emitidos antes de uma alteração de status do cliente continuam válidos até sua expiração, caso não exista mecanismo adicional de revogação.
- Alterações futuras nas regras de autorização poderão exigir novas claims ou mudanças na validação do token.
- Uma chave de assinatura comprometida pode permitir a criação de tokens inválidos.
- A Lambda e a API precisam utilizar configurações compatíveis de emissor, audiência, assinatura e validade.

## 6. Referências

- RFC-0001 — Autenticação de Clientes por CPF com Função Serverless.
- ADR-0018 — Execução da Lambda de Autenticação dentro da VPC.
- ADR-0007 — Topologia de Rede AWS: VPC, 2 AZs e NAT Gateway Único.
- Tech Challenge FIAP — Fase 3.
- Plano Final de Execução — Fase 3.
