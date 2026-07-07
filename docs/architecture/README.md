# Arquitetura

Esta pasta mantém a documentação de arquitetura da Oficina Mecânica API em Structurizr DSL.

As decisões arquiteturais ficam em `docs/ADRs`, mantendo o padrão usado na Fase 1.

## Artefato oficial

O arquivo oficial do projeto é `workspace.dsl`. Ele contém as views C4, fluxos dinâmicos e views de deployment.

Não versionamos exports em Mermaid, PlantUML ou imagens nesta pasta. Para revisar visualmente, abra o DSL no Structurizr Playground:

```text
https://playground.structurizr.com
```

## Arquivos

| Caminho | Finalidade |
| --- | --- |
| `workspace.dsl` | Workspace Structurizr oficial da arquitetura. |
| `scripts/validate-architecture.ps1` | Validação local opcional do DSL via Docker. |
| `scripts/start-structurizr-mcp.ps1` | Sobe o Structurizr MCP local somente com ferramentas DSL. |
| `mcp/structurizr-mcp.json` | Exemplo de configuração para conectar um agente ao MCP local. |
| `mcp/README.md` | Uso simples do MCP no contexto deste repositório. |

## Uso simples

1. Edite `docs/architecture/workspace.dsl`.
2. Cole o conteudo no Structurizr Playground e revise os diagramas.
3. Opcionalmente, rode a validação local:

```powershell
powershell -ExecutionPolicy Bypass -File docs\architecture\scripts\validate-architecture.ps1
```

4. Quando quiser revisar com IA via MCP, suba o servidor local:

```powershell
powershell -ExecutionPolicy Bypass -File docs\architecture\scripts\start-structurizr-mcp.ps1
```

Os scripts são auxiliares manuais. Eles não participam do build, dos testes, do deploy ou da inicialização da aplicação. Se Docker ou MCP não estiverem disponíveis, a aplicação continua independente dessa documentação.

## Views C4

| View key | Nivel | Conteudo |
| --- | --- | --- |
| `SystemLandscape` | Landscape | Pessoas, sistemas externos, GitHub, Docker Hub, AWS e plataforma de entrega. |
| `SystemContext` | C4 L1 | Contexto da Oficina Mecânica API. |
| `Containers` | C4 L2 | API REST e SQL Server. |
| `ComponentsApi` | C4 L3 | Controllers, seguranca, Application, Domain, Infrastructure e DbContext. |
| `CodeWorkOrder` | C4 L4 | Detalhe de codigo do fluxo critico de Ordem de Servico. |
| `DynamicOpenWorkOrder` | Dinâmico | Abertura de ordem de serviço. |
| `DynamicBudgetDecision` | Dinâmico | Decisão externa de orçamento via webhook. |
| `DeploymentLocalCompose` | Deployment | Docker Compose local. |
| `DeploymentKubernetesLocal` | Deployment | Kubernetes local no Docker Desktop. |
| `DeploymentAwsDev` | Deployment | AWS dev atual e proximos passos planejados. |

## Regras de modelagem

- O DSL deve refletir o código e manifestos existentes.
- Itens planejados devem ter tag `Planned`.
- Mudanças de infraestrutura devem atualizar as views de deployment.
- Mudanças de endpoints ou casos de uso críticos devem atualizar as views dinâmicas.
- MCP e validação local servem como lint manual, não como dependência da aplicação.

## Referencias

- C4 Model: https://c4model.com/
- Structurizr DSL: https://docs.structurizr.com/dsl
- Structurizr MCP: https://docs.structurizr.com/ai/mcp
