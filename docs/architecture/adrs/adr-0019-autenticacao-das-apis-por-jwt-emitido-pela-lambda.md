# ADR-0019 — Autenticação das APIs por JWT Emitido pela Lambda

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

A Fase 3 exige que as rotas sensíveis da aplicação sejam protegidas por autenticação via CPF e que uma Function Serverless seja responsável por validar o CPF, consultar a existência e o status do cliente e gerar um token JWT válido para consumo das APIs protegidas.

O domínio existente identifica `Cliente` por documento, que pode ser CPF ou CNPJ. Antes da implementação, a decisão foi refinada para que a Lambda seja o mecanismo de autenticação de todo o perfil `Cliente`, sem deixar clientes identificados por CNPJ sem fluxo de autenticação. O CPF continua obrigatório para a Fase 3 e será o fluxo demonstrado.

Na arquitetura definida, a `oficina-mecanica-auth-lambda` será responsável pelo processo inicial de autenticação. Após a autenticação, o cliente precisará utilizar o token recebido para acessar as rotas protegidas da `oficina-mecanica-api`.

Era necessário definir o mecanismo de autenticação das chamadas posteriores à API e quais informações mínimas seriam transportadas no token.

## 2. Fatores Decisivos

- Utilizar um mecanismo de autenticação compatível com APIs REST.
- Permitir que a Lambda emita um token após validar e normalizar CPF ou CNPJ e verificar o status do cliente.
- Permitir que a API valide o token de forma independente da Lambda após sua emissão.
- Identificar o cliente autenticado sem transportar o CPF em todas as chamadas.
- Representar o papel do usuário autenticado.
- Permitir controle de validade e identificação individual dos tokens.
- Manter a autenticação compatível com o middleware de autorização já utilizado pela aplicação.
- Evitar a criação de uma dependência da API em chamadas à Lambda para cada requisição autenticada.

## 3. Decisão

A autenticação das APIs protegidas será baseada em **JSON Web Token (JWT)** emitido pela `oficina-mecanica-auth-lambda` após a validação de um documento e do status do cliente.

O fluxo será:

```text
Cliente
  → POST /auth/documento
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

1. Receber `documento`, sem exigir `tipoDocumento` no request.
2. Normalizar, validar e identificar se o documento é CPF ou CNPJ.
3. Consultar o cliente no RDS por `Documento + TipoDocumento`.
4. Verificar se o cliente existe e está ativo.
5. Gerar o JWT.
6. Retornar o token ao cliente.

A `oficina-mecanica-api` continuará responsável pela validação do JWT nas rotas protegidas, utilizando o mecanismo de autenticação e autorização do ASP.NET Core.

A API **não dependerá de uma nova chamada à Lambda para validar cada requisição autenticada**. A validade do token será verificada localmente pela própria aplicação.

O JWT deverá conter, no mínimo, as seguintes claims:

| Claim | Finalidade |
|---|---|
| `sub` | Identificador principal do cliente; corresponde ao `cliente_id` |
| `cliente_id` | Identificador interno do cliente |
| `role` | Papel de autorização, inicialmente `Cliente` |
| `jti` | Identificador único do token |
| `iss` | Emissor do token |
| `aud` | Audiência prevista para o token |
| `exp` | Data/hora de expiração |

CPF e CNPJ **não serão utilizados como claims em texto puro nem registrados em logs**. Depois da autenticação, `cliente_id` é a identidade suficiente para autorização e correlação. Não será criado `cpf_hash`, `cnpj_hash` ou hash genérico sem necessidade funcional.

## 4. Justificativa

O JWT permite separar claramente as responsabilidades entre autenticação e consumo das APIs.

A Lambda concentra o processo de autenticação por documento e a consulta do cliente no banco. Depois que o cliente é autenticado, o JWT permite que as requisições seguintes sejam validadas pela própria API sem necessidade de consultar novamente a Lambda ou o banco de dados.

Essa abordagem reduz a dependência entre os componentes durante o processamento das APIs protegidas e mantém o fluxo compatível com a arquitetura atual da aplicação ASP.NET Core.

A utilização das claims `cliente_id` e `role` também permite que a aplicação identifique o cliente autenticado e aplique regras de autorização sem expor o CPF nas chamadas subsequentes.

A claim `jti` fornece um identificador único para cada token, permitindo rastreabilidade sem registrar o conteúdo do JWT. As claims `iss`, `aud` e `exp` permitem validar, respectivamente, o emissor esperado, o destinatário do token e seu período de validade.

A decisão atende diretamente ao requisito da Fase 3: o fluxo `POST /auth/documento` valida CPF, consulta o cliente e gera o JWT. A aceitação de CNPJ é extensão necessária para a coerência com o domínio existente, não substituição do requisito de CPF.

## 5. Consequências

### Positivas

- A autenticação de clientes por documento fica centralizada na Lambda.
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

- RFC-0001 — Autenticação de Clientes por Documento com Função Serverless.
- ADR-0018 — Execução da Lambda de Autenticação dentro da VPC.
- ADR-0007 — Topologia de Rede AWS: VPC, 2 AZs e NAT Gateway Único.
- Tech Challenge FIAP — Fase 3.
- Plano Final de Execução — Fase 3.
