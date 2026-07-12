# ADR-0002 — Linguagem e framework (.NET)

## Status

**Status:** ✅ Aceito
**Data:** 01/05/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O sistema da oficina mecânica exige uma modelagem complexa de domínio (DDD). Precisamos de uma linguagem que suporte fortemente os padrões táticos e garanta produtividade para a entrega dos módulos de Ordem de Serviço, Atendimento, Estoque e Administrativo.

## 2. Fatores Decisivos (Drivers)

- Domínio técnico da equipe (Senioridade em C#).
- Suporte robusto a padrões de Orientação a Objetos e DDD.
- Facilidade de integração com ORMs (Entity Framework) para persistência complexa.

## 3. Decisão Proposta

> Utilizaremos **C# com .NET 10** como linguagem e framework principal para o backend.

## 4. Justificativa

O C# permite uma tradução quase literal dos diagramas táticos de DDD para o código através de classes tipadas, interfaces e records. A presença de uma desenvolvedora Sênior no time reduz drasticamente o risco técnico e acelera a implementação da Clean Architecture.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Alta manutenibilidade, tipagem forte que evita erros em regras de negócio e ecossistema maduro.

### ❌ Negativo (Perdas/Riscos)

- Maior consumo de recursos computacionais se comparado a linguagens de baixo nível ou Go, e necessidade de configuração mais rigorosa de injeção de dependência.

## 6. Referências

- [Design a DDD-oriented microservice (Microsoft)](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)
- **FIAP - Aula 05 (Implementando arquitetura e lógica)**
