# ADR-0003 — Banco de dados relacional (SQL Server)

## Status

**Status:** ✅ Aceito
**Data:** 01/05/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O sistema da oficina mecânica está dividido em 4 Bounded Contexts principais: Ordem de Serviço, Atendimento, Estoque e Administrativo. Como a arquitetura de entrega é um Monólito, precisamos de um mecanismo de persistência que suporte a separação lógica desses contextos, mas que garanta a integridade transacional quando o fluxo exige consistência imediata.

## 2. Fatores Decisivos (Drivers)

- Domínio técnico da equipe (Experiência profissional do time em SQL Server).
- **Segurança e Confiança nos Dados:** O banco deve impedir erros humanos de digitação ou "dados órfãos" (ex: uma OS sem dono).
- **Fácil Integração com C#:** A ferramenta deve conversar "nativamente" com a linguagem que escolhemos (.NET Core 10).
- **Facilidade para Relatórios:** Precisamos conseguir extrair informações rápidas, como "qual foi o tempo médio de execução dos serviços de uma OS?".

## 3. Decisão Proposta

> Adotaremos o **Microsoft SQL Server 2022** como o Sistema Gerenciador de Banco de Dados (SGBD) para persistência de todos os contextos do monólito.

## 4. Justificativa

- **Compatibilidade com .NET:** Integração simples e bibliotecas maduras.
- **Organização por Relacionamentos:** Modelo relacional reforça as conexões via chaves.
- **Poder de Consulta:** Consultas complexas com boa performance.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- **Integridade Total:** Protege o negócio contra dados incompletos/desconectados.
- **Ferramentas de Gestão:** SSMS facilita visualizar tabelas e correções.
- **Consistência Imediata:** Atualizações transacionais no fluxo.

### ❌ Negativo (Perdas/Riscos)

- **Rigidez no Esquema:** Mudanças exigem migrations cuidadosas.
- **Configuração Inicial:** Banco mais "pesado" (memória/infra).
- **Trabalho de Mapeamento:** Ajustes no Entity Framework.
- **Dependência de Licenciamento:** Pode gerar custos em cenário real.

## 6. Referências

- **DATE, C. J.** *Introdução a Sistemas de Bancos de Dados*. 2004.
- **MICROSOFT.** *Documentação do Entity Framework Core 10*. 2025.
- **FIAP, Pós-Tech Software Architecture.** Fase 1 - Aula 05.
