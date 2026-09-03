# ADR-0020 — Inclusão do Status do Cliente no Modelo Relacional

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

A Fase 3 introduz a autenticação de clientes por CPF. Durante o processo de autenticação, não basta verificar se o CPF informado corresponde a um cliente existente: também é necessário verificar se esse cliente está apto a utilizar as APIs da aplicação.

O requisito da Fase 3 determina que a Function Serverless seja responsável por validar o CPF e consultar a existência e o status do cliente na base de dados antes de gerar o JWT.

Na versão atual do modelo de dados, era necessário estabelecer uma forma explícita de representar a situação do cliente para que a Lambda pudesse diferenciar um cliente válido e ativo de um cliente existente, porém inativo.

## 2. Fatores Decisivos

- Permitir verificar o status do cliente durante a autenticação.
- Diferenciar clientes ativos de clientes que não devem receber um token.
- Manter a regra de autenticação baseada em uma informação persistida no banco.
- Integrar a alteração ao modelo relacional existente.
- Permitir atualização do banco por meio de migration do Entity Framework Core.
- Permitir que os dados de demonstração representem clientes ativos e inativos.
- Evitar depender de regras implícitas ou informações externas para determinar se um cliente pode se autenticar.

## 3. Decisão

Será adicionado um campo **`StatusCliente`** ao modelo de dados de cliente.

Esse campo representará a situação atual do cliente e será utilizado pela `oficina-mecanica-auth-lambda` durante o processo de autenticação por CPF.

O fluxo de validação será:

```text
CPF informado
    ↓
Cliente encontrado?
    ↓
StatusCliente permite autenticação?
    ↓
Sim → Gerar JWT
Não → Recusar autenticação
```

A existência do cliente e seu status serão considerados condições independentes para a emissão do JWT:

- **Cliente inexistente:** autenticação recusada.
- **Cliente existente e ativo:** autenticação permitida e JWT emitido.
- **Cliente existente e inativo:** autenticação recusada e nenhum JWT emitido.

A alteração será incorporada ao modelo relacional por meio de uma **migration do Entity Framework Core**.

Os dados de demonstração/seed também serão atualizados para contemplar clientes em diferentes situações, permitindo validar tanto o fluxo de autenticação bem-sucedido quanto o fluxo de cliente inativo.

A definição da representação física do campo no banco — incluindo seu tipo e valores exatos — será tratada na implementação da alteração do modelo, mantendo esta ADR focada na decisão arquitetural de persistir o status do cliente.

## 4. Justificativa

A autenticação da Fase 3 exige que a aplicação consulte não apenas a existência do cliente, mas também seu status antes de emitir o token.

Persistir essa informação no próprio modelo de cliente fornece uma fonte única e consistente para essa decisão.

A separação entre **existência** e **status** também permite representar situações nas quais o cadastro do cliente continua armazenado, mas seu acesso às APIs precisa ser bloqueado.

Essa abordagem evita que a Lambda precise manter uma lista própria de clientes autorizados ou utilizar regras externas para determinar quem pode receber um JWT.

A migration mantém a evolução do modelo de dados rastreável e compatível com o mecanismo de persistência já utilizado pela aplicação. Além disso, a atualização do seed permite demonstrar o comportamento esperado durante a validação da Fase 3.

A decisão está alinhada ao plano da solução, que determina a inclusão do status do cliente, a criação de migration e a atualização dos dados de demonstração.

## 5. Consequências

### Positivas

- A aplicação passa a possuir uma informação explícita sobre a situação do cliente.
- A Lambda consegue verificar o status antes de emitir o JWT.
- Clientes inativos podem permanecer cadastrados sem possuir acesso às APIs protegidas.
- A regra de autenticação utiliza uma informação persistida e consistente.
- A alteração do banco fica rastreável por migration.
- O comportamento pode ser validado com clientes ativos e inativos no ambiente de demonstração.
- A solução atende diretamente ao requisito de consultar o status do cliente durante a autenticação.

### Negativas e riscos

- O modelo de dados passa a possuir uma nova informação que precisa ser mantida corretamente.
- Alterações incorretas no status podem impedir a autenticação de clientes válidos.
- O processo de atualização do banco passa a depender da execução correta da migration.
- O comportamento da autenticação fica diretamente relacionado à consistência do status persistido.
- Caso novas situações de cliente sejam necessárias no futuro, o modelo e as regras de autenticação poderão precisar ser evoluídos.

## 6. Referências

- RFC-0001 — Autenticação de Clientes por CPF com Função Serverless.
- ADR-0019 — Autenticação das APIs por JWT Emitido pela Lambda.
- ADR-0018 — Execução da Lambda de Autenticação dentro da VPC.
- Tech Challenge FIAP — Fase 3.
- Plano Final de Execução — Fase 3.
