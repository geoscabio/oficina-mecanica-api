# ADR-0001 — Clean Architecture em monólito

## Status

**Status:** ✅ Aceito
**Data:** 01/05/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O desafio proposto pela FIAP exige a entrega de um monolito. No entanto, o domínio da oficina mecânica tende a crescer e se tornar complexo. Precisamos de uma organização de código que impeça que regras de negócio se misturem com detalhes de banco de dados ou frameworks web, garantindo testabilidade e manutenibilidade.

## 2. Fatores Decisivos (Drivers)

- Necessidade de aplicar o princípio de *Separation of Concerns* (Separação de Preocupações).
- Foco total no isolamento e na testabilidade das regras de negócio (Domínio).
- Necessidade de preparar a base de código para suportar o crescimento da aplicação sem perda de qualidade.

## 3. Decisão Proposta

> Adotaremos a **Clean Architecture**, organizando o monólito internamente em **4 camadas lógicas** (Interface, Aplicação, Domínio e Infraestrutura).

- **Domínio:** Entidades e Objetos de Valor (Núcleo).
- **Aplicação:** Casos de Uso (Orquestração).
- **Interface:** Adaptadores (Controllers).
- **Infraestrutura:** Detalhes externos (DB, Gateway, APIs).

## 4. Justificativa

Conforme abordado em Arquiteturas da Atualidade (Aula 1), a Clean Architecture é ideal para sistemas focados no domínio. Ao utilizarmos 4 camadas bem definidas dentro do .NET (C#), conseguimos aplicar a **Inversão de Dependência**. Isso significa que o Domínio define as "regras e contratos" (Interfaces de Repositórios), mas quem implementa os detalhes de banco de dados é a camada de Infraestrutura, no anel mais externo. O monólito representará apenas a forma de *deploy* (um único executável), mas internamente o sistema será modularizado e desacoplado.

**Algumas referências utilizadas:**

- **Independência de Framework:** Segundo Uncle Bob, a arquitetura não deve depender da existência de uma biblioteca de software. Usando Clean Architecture, o ASP.NET Core torna-se apenas um "detalhe" na camada de Interface.
- **A Regra da Dependência:** Ao isolar o **Domínio** (conforme Eric Evans define no DDD), garantimos que mudanças no banco de dados (Infraestrutura) não forcem alterações nas regras de cálculo de orçamento da oficina.
- **Inversão de Dependência (DIP):** Aplicaremos o princípio do SOLID onde a camada de Aplicação define uma **Interface** (Abstração) para o repositório, e a Infraestrutura a implementa. Isso permite que o monólito seja "limpo" e facilite uma futura decomposição em microserviços, se necessário (Fowler, 2014).

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- **Testabilidade:** É possível testar as regras de negócio da oficina sem uma UI, sem banco de dados e sem servidor web.
- **Longevidade:** Protege o investimento no código de negócio contra a obsolescência de frameworks.
- **Manutenibilidade:** Alto nível de manutenibilidade, pois o domínio não possui dependências externas.
- **Flexibilidade:** Flexibilidade para trocar tecnologias de Infraestrutura (ex: trocar o banco de dados) sem alterar os Casos de Uso ou o Domínio.

### ❌ Negativo (Perdas/Riscos)

- **Overhead de Mapeamento:** Necessidade de transformar objetos entre as camadas (Entity para DTO).
- **Maior complexidade inicial:** a equipe precisará criar mais abstrações (Interfaces) e mapear objetos frequentemente (Entity para DTO).
- **Aumento da verbosidade:** aumento do número de arquivos/projetos estruturais se comparado a um monólito tradicional (arquitetura em camadas simples).

## 6. Referências

- **MARTIN, Robert C. (Uncle Bob).** *Clean Architecture: A Craftsman's Guide to Software Structure and Design*. Prentice Hall, 2017. (Capítulo 22: The Clean Architecture).
- **EVANS, Eric.** *Domain-Driven Design: Tackling Complexity in the Heart of Software*. Addison-Wesley, 2003. (Capítulo 4: Isolating the Domain).
- **FOWLER, Martin.** *Patterns of Enterprise Application Architecture*. Addison-Wesley, 2002.
- **FIAP, Pós-Tech Software Architecture.** Fase 1, Aula 01 - *Arquiteturas da Atualidade e seus Trade-offs*.
- **FIAP, Pós-Tech Software Architecture.** Fase 1, Aula 04 - *Propostas Arquiteturais com RFCs e ADRs.*
