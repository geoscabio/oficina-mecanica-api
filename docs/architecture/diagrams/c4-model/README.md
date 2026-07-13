# C4 Model

Modelo C4 oficial do projeto, mantido em Structurizr DSL.

## Arquivo fonte

- `workspace.dsl`

## Views exportadas

- `c4-model-l1-system-context.svg`: visão de contexto do sistema.
- `c4-model-l1-system-context-key.svg`: legenda da visão L1.
- `c4-model-l2-containers.svg`: visão de contêineres.
- `c4-model-l2-containers-key.svg`: legenda da visão L2.
- `c4-model-l3-components-api.svg`: visão de componentes da API.
- `c4-model-l3-components-api-key.svg`: legenda da visão L3.

## Validação

O workspace DSL foi validado com o MCP oficial do Structurizr.

- `validate`: OK
- `inspect`: sem erros. Há apenas regras ignoradas intencionalmente para documentação e decisões, mantendo o DSL autocontido para uso direto no Structurizr Playground.

## Escopo

- Inclui L1, L2 e L3 do C4 Model.
- Não inclui L4.
- Não inclui Mermaid ou PlantUML.
- Não mistura AWS, Kubernetes ou deployment nos diagramas C4.
