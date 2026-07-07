# 📄[ADR 004] Definição da Autenticação e Autorização com JWT

## Status

**Status:** ✅ Aceito **Data:** 01/05/2026 **Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O sistema da oficina mecânica precisa de uma porta de entrada segura. Precisamos saber quem está acessando a API (se é o Atendente criando uma OS ou o Mecânico reservando uma peça) e garantir que as pessoas certas acessem apenas as áreas permitidas. Tudo isso precisa ser feito de forma rápida e sem criar gargalos de memória no servidor do nosso monólito.
>

## 2. Fatores Decisivos (Drivers)

- **Performance e Leveza (Stateless):** O servidor não deve precisar "lembrar" ou guardar a sessão do usuário na memória a cada clique que ele der no sistema.
- **Controle de Perfil de Acesso:** Precisamos de uma forma simples de dizer "este usuário é mecânico" e carregar essa informação junto com as requisições.
- **Facilidade de Implementação:** A solução escolhida deve ser de fácil integração com o framework principal do projeto (C# / ASP.NET Core 10).

## 3. Decisão Proposta

> Utilizaremos **JSON Web Tokens (JWT)** como o padrão para autenticação e autorização das requisições na API.
>

## 4. Justificativa

>
>
> - **Praticidade do Padrão:** O JWT é a solução mais adotada pelo mercado para APIs. Ele resolve o problema de autenticação entregando um "crachá digital" para o front-end. Toda vez que o front-end chama a API, ele mostra esse crachá.
> - **Redução de idas ao Banco de Dados:** Como o token JWT possui uma assinatura digital, nós não precisamos ir ao SQL Server em toda requisição para conferir se o usuário existe e se está logado. Isso deixa o sistema muito mais rápido.
> - **Proteção das Camadas:** Na nossa *Clean Architecture*, o token será validado logo na porta de entrada. Se o token for inválido ou não tiver a permissão correta, a requisição é barrada antes mesmo de chegar nas regras de negócio (Domínio).

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- **Integração Nativa:** O .NET possui bibliotecas nativas excelentes para gerar e validar JWT com poucas linhas de código.
- **Desacoplamento Front/Back:** O front-end consegue ler o payload do token facilmente para saber o nome do usuário e o seu perfil, ajudando a montar o menu da tela.
- **Escalabilidade:** Se no futuro a oficina crescer e o monólito for quebrado em microserviços, o JWT continua funcionando perfeitamente entre eles.

### ❌ Negativo (Perdas/Riscos)

- **Revogação de Acesso (Invalidação):** Como o servidor não guarda o estado, se um funcionário for demitido, o token dele continuará válido até o tempo de expiração acabar.
- **Dados Expostos (Payload em Base64):** Os dados dentro do token não são criptografados, apenas codificados. Isso significa que não podemos colocar dados sensíveis lá dentro, apenas identificadores básicos (IDs e Perfis).
- **Gestão da Chave Secreta:** Se a chave secreta que o C# usa para assinar os tokens vazar, qualquer pessoa poderá gerar tokens válidos e comprometer toda a segurança da oficina.

## 6. Referências

- **IETF.** *RFC 7519: JSON Web Token (JWT)*. Internet Engineering Task Force, 2015.
- **MICROSOFT.** *Overview of ASP.NET Core authentication*. Documentação Oficial .NET, 2025.
- **FIAP, Pós-Tech Software Architecture.** Fase 1 - Discussões sobre trade-offs, segurança e camadas de entrada (Interface).
