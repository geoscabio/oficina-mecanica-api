# 📄[ADR 003] Definição do Banco de Dados Relacional (SQL Server)

## Status

**Status:** ✅ Aceito **Data:** 01/05/2026 **Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O sistema da oficina mecânica está dividido em 4 Bounded Contexts principais: Ordem de Serviço, Atendimento, Estoque e Administrativo. Como a arquitetura de entrega é um Monólito, precisamos de um mecanismo de persistência que suporte a separação lógica desses contextos, mas que garanta a integridade transacional quando o fluxo exige consistência imediata.
>

## 2. Fatores Decisivos (Drivers)

- Domínio técnico da equipe (Experiência profissional do time em SQL Server).
- **Segurança e Confiança nos Dados:** O banco deve impedir erros humanos de digitação ou "dados órfãos" (ex: uma OS sem dono).
- **Fácil Integração com C#:** A ferramenta deve conversar "nativamente" com a linguagem que escolhemos (.NET Core 10).
- **Facilidade para Relatórios:** Precisamos conseguir extrair informações rápidas, como "qual foi o tempo médio de execução dos serviços de uma OS?".

## 3. Decisão Proposta

> Adotaremos o **Microsoft SQL Server 2022** como o Sistema Gerenciador de Banco de Dados (SGBD) para persistência de todos os contextos do monólito.
>

## 4. Justificativa

>
>
> - **Compatibilidade com .NET:** O SQL Server é desenvolvido pela Microsoft, assim como o C#. Isso significa que a integração é muito simples, as bibliotecas são maduras e raramente teremos problemas de compatibilidade.
> - **Organização por Relacionamentos:** Como nossa oficina tem regras bem definidas, o modelo relacional do SQL se encaixa como uma luva, pois ele "obriga" o sistema a respeitar essas conexões através de chaves.
> - **Poder de Consulta:** O SQL nos permite fazer buscas complexas cruzando dados de diferentes áreas de forma muito performática e organizada.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- **Integridade Total:** O banco não aceita dados incompletos ou desconectados, o que protege o negócio da oficina.
- **Ferramentas de Gestão:** O uso do SQL Server Management Studio (SSMS) facilita muito a vida para visualizar as tabelas e fazer correções rápidas.
- **Consistência Imediata:** Assim que o mecânico dá baixa em uma peça na oficina, o estoque já é atualizado instantaneamente para todo o sistema.

### ❌ Negativo (Perdas/Riscos)

- **Rigidez no Esquema:** Se precisarmos mudar algo rápido, precisaremos fazer um processo de *Migration* no código, o que exige cuidado para não quebrar o que já existe.
- **Configuração Inicial:** O SQL Server é um banco "pesado". Ele exige mais memória do servidor para rodar do que opções mais leves (como SQLite ou NoSQL).
- **Trabalho de Mapeamento:** Teremos que gastar um tempo configurando o *Entity Framework* para garantir que as classes do C# sejam mapeadas para as tabelas do banco.
- **Dependência de Licenciamento:** Em um cenário real de empresa, o SQL Server pode ter custos altos de licença, diferente de bancos gratuitos como o PostgreSQL.

## 6. Referências

- **DATE, C. J.** *Introdução a Sistemas de Bancos de Dados*. 2004.
- **MICROSOFT.** *Documentação do Entity Framework Core 10*. 2025.
- **FIAP, Pós-Tech Software Architecture.** Fase 1 - Aula 05 (Implementando arquitetura e lógica).
