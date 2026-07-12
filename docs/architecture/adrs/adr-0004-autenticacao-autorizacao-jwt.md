# ADR-0004 — Autenticação e autorização (JWT)

## Status

**Status:** ✅ Aceito
**Data:** 01/05/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O sistema da oficina mecânica precisa de uma porta de entrada segura. Precisamos saber quem está acessando a API (se é o Atendente criando uma OS ou o Mecânico reservando uma peça) e garantir que as pessoas certas acessem apenas as áreas permitidas. Tudo isso precisa ser feito de forma rápida e sem criar gargalos de memória no servidor do nosso monólito.

## 2. Fatores Decisivos (Drivers)

- **Performance e Leveza (Stateless):** servidor não deve guardar sessão em memória.
- **Controle de Perfil de Acesso:** perfis/roles junto das requisições.
- **Facilidade de Implementação:** integração simples com C# / ASP.NET Core 10.

## 3. Decisão Proposta

> Utilizaremos **JSON Web Tokens (JWT)** como o padrão para autenticação e autorização das requisições na API.

## 4. Justificativa

- **Praticidade:** padrão amplamente adotado para APIs.
- **Redução de idas ao Banco:** assinatura do token evita consultas constantes.
- **Proteção das Camadas:** validação na entrada (Interface) antes das regras de negócio.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Bibliotecas nativas no .NET.
- Desacoplamento Front/Back (leitura do payload para perfil/claims).
- Escalabilidade para cenários futuros.

### ❌ Negativo (Perdas/Riscos)

- Revogação difícil (token válido até expirar).
- Payload em Base64 (não armazenar dados sensíveis).
- Gestão da chave secreta.

## 6. Referências

- **IETF.** *RFC 7519: JSON Web Token (JWT)*. 2015.
- **MICROSOFT.** *Overview of ASP.NET Core authentication*. 2025.
- **FIAP, Pós-Tech Software Architecture.** Discussões Fase 1.
