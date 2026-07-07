# Arquitetura

Esta pasta concentra a documentacao de arquitetura da Oficina Mecanica API usando Structurizr DSL e C4 Model.

## Objetivo

Manter a arquitetura como codigo, versionada no repositorio e validada por IA + Structurizr MCP antes de cada PR.

## Arquivos principais

| Caminho | Finalidade |
| --- | --- |
| `workspace.dsl` | Workspace Structurizr com C4, fluxos dinamicos e deployment. |
| `mcp/structurizr-mcp.json` | Configuracao de referencia para conectar um agente ao Structurizr MCP local. |
| `mcp/README.md` | Como rodar o MCP local e usar as ferramentas de validacao/export. |
| `scripts/start-structurizr-mcp.ps1` | Sobe o Structurizr MCP com DSL, Mermaid e PlantUML habilitados. |
| `scripts/validate-architecture.ps1` | Valida o DSL usando a imagem oficial consolidada do Structurizr. |
| `scripts/export-architecture.ps1` | Exporta as views para Mermaid e PlantUML. |

## Views C4

| View key | Nivel | Conteudo |
| --- | --- | --- |
| `SystemLandscape` | Landscape | Pessoas, sistemas externos, GitHub, Docker Hub, AWS e plataforma de entrega. |
| `SystemContext` | C4 L1 | Contexto da Oficina Mecanica API. |
| `Containers` | C4 L2 | API REST e SQL Server. |
| `ComponentsApi` | C4 L3 | Controllers, seguranca, Application, Domain, Infrastructure e DbContext. |
| `CodeWorkOrder` | C4 L4 | Detalhe de codigo do fluxo critico de Ordem de Servico. |
| `DynamicOpenWorkOrder` | Dinamico | Abertura de ordem de servico. |
| `DynamicBudgetDecision` | Dinamico | Decisao externa de orcamento via webhook. |
| `DeploymentLocalCompose` | Deployment | Docker Compose local. |
| `DeploymentKubernetesLocal` | Deployment | Kubernetes local no Docker Desktop. |
| `DeploymentAwsDev` | Deployment | AWS dev atual e proximos passos planejados. |

## Ciclo de trabalho

1. Atualize `workspace.dsl`.
2. Rode `scripts/start-structurizr-mcp.ps1` quando for usar um agente com MCP.
3. Use o agente para chamar as ferramentas do Structurizr MCP: validar DSL, inspecionar DSL e exportar views.
4. Rode `scripts/validate-architecture.ps1`.
5. Rode `scripts/export-architecture.ps1`.
6. Revise as views geradas em `docs/architecture/generated/`.

## Regras de modelagem

- O DSL deve refletir o codigo e manifestos existentes.
- Itens planejados devem ter tag `Planned`.
- Mudancas de infraestrutura devem atualizar as views de deployment.
- Mudancas de endpoints ou casos de uso criticos devem atualizar as views dinamicas.
- O MCP entra como validador/linter: nenhum ajuste grande deve ir para PR sem `validate`, `inspect` e export.

## Referencias

- C4 Model: https://c4model.com/
- Structurizr DSL: https://docs.structurizr.com/dsl
- Structurizr MCP: https://docs.structurizr.com/ai/mcp
- Structurizr commands: https://docs.structurizr.com/commands
