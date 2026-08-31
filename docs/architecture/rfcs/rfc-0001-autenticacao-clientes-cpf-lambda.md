# RFC-0001 — Autenticação de Clientes por CPF com Função Serverless

## Status

Aceita para implementação na Fase 3.

## Contexto

A aplicação já possui autenticação interna por usuário e senha para os papéis `Administrador`, `Atendente` e `Mecanico`. Esse fluxo permanece na API por atender à equipe interna e por ser uma funcionalidade herdada da Fase 2.

A Fase 3 exige um fluxo adicional para clientes: validar o CPF, consultar a existência e o status do cliente no banco de dados e emitir um JWT para o consumo de APIs protegidas. O projeto também deve demonstrar o uso desse token em uma rota real da API.

## Decisão

Serão adotados dois fluxos de autenticação, com um único contrato de JWT:

1. A API continuará oferecendo o login interno por usuário e senha somente para os papéis `Administrador`, `Atendente` e `Mecanico`.
2. A Função Serverless `oficina-mecanica-auth-lambda` será o único fluxo de autenticação de `Cliente`, exposto pela rota pública `POST /auth/cpf` no API Gateway.
3. A Lambda normalizará e validará o CPF, consultará o cliente no RDS e emitirá um token somente quando o cliente existir e possuir `StatusCliente = Ativo`.
4. A API validará todos os tokens — internos e de clientes — com o mesmo emissor, audiência, algoritmo e segredo.

### Contrato JWT único

| Campo | Valor ou regra |
| --- | --- |
| `iss` | `oficina-mecanica-auth` |
| `aud` | `oficina-mecanica-api` |
| Assinatura | Simétrica, HMAC-SHA-256 |
| Segredo | Um único segredo no AWS Secrets Manager, acessível somente pela API e pela Lambda |
| Expiração | 60 minutos |
| `sub` | Identificador da identidade autenticada |
| `role` | Um dos papéis `Administrador`, `Atendente`, `Mecanico` ou `Cliente` |
| `cliente_id` | Obrigatório para tokens com o papel `Cliente`; corresponde ao identificador interno do cliente |
| `jti` | Identificador único do token |

O CPF em formato puro não será incluído no token nem registrado em logs. O `cliente_id` será suficiente para identificar o cliente na API e aplicar a autorização.

### Rota protegida de demonstração

Será criada a rota `GET /api/v1/clientes/me/ordens-servico`, protegida para o papel `Cliente`.

`/me` não é uma sigla: é uma convenção REST que representa o usuário autenticado na requisição. A API obterá o `cliente_id` a partir do JWT; portanto, nenhum identificador de cliente será recebido na URL.

As ordens de serviço deverão ser filtradas pela relação já existente:

```text
Ordem de Serviço → Veículo → Cliente
```

Dessa forma, um cliente poderá consultar somente as ordens associadas aos seus veículos.

### Respostas de autenticação

| Situação | Resposta |
| --- | --- |
| CPF com formato inválido | `400 Bad Request`, com mensagem genérica de validação |
| CPF inexistente | `401 Unauthorized`, com `CPF não autorizado.` |
| Cliente inativo | `401 Unauthorized`, com `CPF não autorizado.` |
| Cliente ativo | `200 OK`, com JWT válido |

As respostas para CPF inexistente e cliente inativo serão intencionalmente iguais, para não revelar se um CPF está cadastrado.

## Consequências

### Positivas

- Cumpre o fluxo de autenticação por CPF exigido na Fase 3 sem remover o acesso da equipe interna.
- Um único contrato JWT simplifica a validação na API e evita múltiplos emissores ou audiências.
- A rota `/me` reduz o risco de manipulação de identificadores na URL para consultar dados de outro cliente.
- O papel `Cliente` permite uma autorização explícita e independente dos papéis internos.

### Trade-offs e cuidados

- Tokens anteriores, emitidos com `issuer = OficinaMecanica` e `audience = OficinaMecanica.API`, deixarão de ser aceitos. Isso é aceitável por serem temporários e pelo ambiente ainda estar em evolução.
- API e Lambda passarão a depender do mesmo segredo; ele não poderá ser versionado no código, exposto em logs ou devolvido por endpoints.
- A API deverá manter a validação JWT mesmo atrás do API Gateway; o Gateway não substitui a proteção da aplicação.
- A alteração exigirá a inclusão de `StatusCliente` no modelo, a criação de migration e a atualização dos dados de seed para contemplar clientes ativos e inativos.

## Fora de escopo

- Remover ou migrar o login interno por usuário e senha.
- Implementar Lambda Authorizer no API Gateway. A validação do token continuará na API neste MVP.
- Implementar revogação imediata de JWT antes da expiração.
- Implementar infraestrutura, permissões AWS ou deploy.

## Critérios de aceite

- `POST /auth/cpf` emite JWT somente para cliente ativo.
- CPF inexistente e cliente inativo recebem a mesma resposta `401 Unauthorized`.
- Um token de `Cliente` autentica `GET /api/v1/clientes/me/ordens-servico`.
- A rota retorna somente ordens ligadas ao cliente autenticado.
- Um token de cliente não permite executar operações restritas a funcionários.
- Tokens internos continuam funcionando para seus respectivos papéis, usando o novo contrato JWT.