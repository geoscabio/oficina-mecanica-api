# RFC-0001 — Autenticação de Clientes por Documento com Função Serverless

## Status

Aceita para implementação na Fase 3.

## Contexto

A aplicação já possui autenticação interna por usuário e senha para os papéis `Administrador`, `Atendente` e `Mecanico`. Esse fluxo permanece na API por atender à equipe interna e por ser uma funcionalidade herdada da Fase 2.

A Fase 3 exige um fluxo adicional para clientes: validar o CPF, consultar a existência e o status do cliente no banco de dados e emitir um JWT para o consumo de APIs protegidas. O domínio existente, porém, já identifica `Cliente` por CPF ou CNPJ. O projeto deve demonstrar o uso do token em uma rota real da API pelo fluxo de CPF obrigatório.

## Decisão

Serão adotados dois fluxos de autenticação, com um único contrato de JWT:

1. A API continuará oferecendo o login interno por usuário e senha somente para os papéis `Administrador`, `Atendente` e `Mecanico`.
2. A Função Serverless `oficina-mecanica-auth-lambda` será o único fluxo de autenticação de `Cliente`, exposto pela rota pública `POST /auth/documento` no API Gateway.
3. A Lambda receberá `{ "documento": "..." }`, normalizará, validará e identificará CPF ou CNPJ sem exigir `tipoDocumento`; consultará o cliente no RDS por `Documento + TipoDocumento` e emitirá token somente para `StatusCliente = Ativo`.
4. A API validará todos os tokens — internos e de clientes — com o mesmo emissor, audiência, algoritmo e segredo.
5. A retirada do perfil `Cliente` do login interno por usuário/senha ocorrerá somente junto da implementação do fluxo por documento, pois ele cobre CPF e CNPJ. Os perfis `Administrador`, `Atendente` e `Mecanico` permanecem no login interno.

> O requisito obrigatório de autenticação via CPF é atendido pelo fluxo `POST /auth/documento`. A solução também aceita CNPJ porque o domínio existente permite Clientes identificados por CPF ou CNPJ e a Auth Lambda passa a ser o mecanismo de autenticação de todo o perfil Cliente.

### Contrato JWT único

| Campo | Valor ou regra |
| --- | --- |
| `iss` | `oficina-mecanica-auth` |
| `aud` | `oficina-mecanica-api` |
| Assinatura | Simétrica, HMAC-SHA-256 |
| Segredo | Um único segredo no AWS Secrets Manager, acessível somente pela API e pela Lambda |
| Expiração | 60 minutos |
| `sub` | `cliente_id` da identidade autenticada |
| `role` | Um dos papéis `Administrador`, `Atendente`, `Mecanico` ou `Cliente` |
| `cliente_id` | Obrigatório para tokens com o papel `Cliente`; corresponde ao identificador interno do cliente |
| `jti` | Identificador único do token |

CPF e CNPJ em formato puro não serão incluídos no token nem registrados em logs. O `cliente_id` será suficiente para identificar o cliente na API e aplicar a autorização. Não haverá `cpf_hash`, `cnpj_hash` ou hash genérico no JWT sem necessidade funcional.

### Contrato HTTP e integração com API Gateway

O endpoint público é `POST /auth/documento`. O API Gateway será uma **HTTP API** com **payload format 2.0** e Lambda Proxy Integration direta. A futura Function consumirá o evento `APIGatewayHttpApiV2ProxyRequest`; não haverá mapeamento intermediário de request ou response.

Esse formato é suficiente para uma única Lambda, reduz o contrato de integração e evita a complexidade da REST API payload v1 sem benefício para o MVP. O transporte externo será protegido por HTTPS/TLS no API Gateway.

#### Request

```json
{
  "documento": "12345678909"
}
```

O campo `documento` é obrigatório e aceita CPF ou CNPJ. A Lambda é responsável por normalizar, validar e identificar o tipo; `tipoDocumento` não é aceito nem exigido no request.

#### Responses

As respostas de erro preservam o envelope já utilizado pela API: `mensagem`, `tipo` e, quando necessário, `erros`. Mensagens e a lista `erros` não devem revelar o motivo específico de invalidez do documento nem informações de infraestrutura.

| Situação | HTTP | Response |
| --- | --- | --- |
| Cliente ativo | `200 OK` | `{ "accessToken": "<jwt>", "tokenType": "Bearer", "expiresIn": 3600 }` |
| JSON inválido, campo ausente, documento vazio, CPF inválido ou CNPJ inválido | `400 Bad Request` | `{ "mensagem": "Requisição inválida.", "tipo": "Validacao", "erros": ["Requisição inválida."] }` |
| Documento inexistente ou cliente inativo | `401 Unauthorized` | `{ "mensagem": "Documento não autorizado.", "tipo": "NaoAutorizado" }` |
| Timeout/indisponibilidade do RDS ou falha ao recuperar segredo no Secrets Manager | `503 Service Unavailable` | `{ "mensagem": "Serviço temporariamente indisponível.", "tipo": "ErroInterno" }` |

O response de sucesso não retorna CPF, CNPJ, documento mascarado, `cliente_id` separado ou dados cadastrais: essas informações não são necessárias fora do JWT. O contrato `401` é exatamente o mesmo para cliente inexistente e inativo, impedindo enumeração. Em `503`, exceção, SQL, endpoint, ARN, segredo e documento ficam restritos a logs e observabilidade seguros.

### Rota protegida de demonstração

Será criada a rota `GET /api/v1/clientes/me/ordens-servico`, protegida para o papel `Cliente`.

`/me` não é uma sigla: é uma convenção REST que representa o usuário autenticado na requisição. A API obterá o `cliente_id` a partir do JWT; portanto, nenhum identificador de cliente será recebido na URL.

As ordens de serviço deverão ser filtradas pela relação já existente:

```text
Ordem de Serviço → Veículo → Cliente
```

Dessa forma, um cliente poderá consultar somente as ordens associadas aos seus veículos.

## Consequências

### Positivas

- Cumpre o fluxo de autenticação por CPF exigido na Fase 3 sem deixar clientes identificados por CNPJ sem mecanismo de autenticação.
- Um único contrato JWT simplifica a validação na API e evita múltiplos emissores ou audiências.
- A rota `/me` reduz o risco de manipulação de identificadores na URL para consultar dados de outro cliente.
- O papel `Cliente` permite uma autorização explícita e independente dos papéis internos.

### Trade-offs e cuidados

- Tokens anteriores, emitidos com `issuer = OficinaMecanica` e `audience = OficinaMecanica.API`, deixarão de ser aceitos. Isso é aceitável por serem temporários e pelo ambiente ainda estar em evolução.
- API e Lambda passarão a depender do mesmo segredo; ele não poderá ser versionado no código, exposto em logs ou devolvido por endpoints.
- A API deverá manter a validação JWT mesmo atrás do API Gateway; o Gateway não substitui a proteção da aplicação.
- A alteração exigirá a inclusão de `StatusCliente` no modelo, a criação de migration e a atualização dos dados de seed para contemplar clientes ativos e inativos.

## Fora de escopo

- Implementar Lambda Authorizer no API Gateway. A validação do token continuará na API neste MVP.
- Implementar revogação imediata de JWT antes da expiração.
- Implementar infraestrutura, permissões AWS ou deploy.

## Critérios de aceite

- `POST /auth/documento` recebe evento HTTP API payload 2.0 e emite JWT somente para cliente ativo, para CPF ou CNPJ válido.
- JSON inválido, campo ausente, documento vazio, CPF inválido e CNPJ inválido retornam o contrato genérico `400 Bad Request`.
- Documento inexistente e cliente inativo retornam exatamente o mesmo contrato `401 Unauthorized`; o cenário CPF é obrigatório na validação e no vídeo da Fase 3.
- Falhas de RDS ou Secrets Manager retornam o contrato genérico `503 Service Unavailable`, sem expor detalhes internos.
- Um token de `Cliente` autentica `GET /api/v1/clientes/me/ordens-servico`.
- A rota retorna somente ordens ligadas ao cliente autenticado.
- Um token de cliente não permite executar operações restritas a funcionários.
- Tokens internos continuam funcionando para seus respectivos papéis, usando o novo contrato JWT.
