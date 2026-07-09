# C4 Model

Modelo C4 oficial do projeto, mantido em Structurizr DSL.

## Arquivo fonte

- `workspace.dsl`

## Views exportadas

- `C4ModelL1SystemContext.svg`: visão de contexto do sistema.
- `C4ModelL1SystemContext-key.svg`: legenda da visão L1.
- `C4ModelL2Containers.svg`: visão de contêineres.
- `C4ModelL2Containers-key.svg`: legenda da visão L2.
- `C4ModelL3ComponentsApi.svg`: visão de componentes da API.
- `C4ModelL3ComponentsApi-key.svg`: legenda da visão L3.

## Validação

O workspace DSL foi validado com o MCP oficial do Structurizr.

- `validate`: OK
- `inspect`: sem erros. Há apenas regras ignoradas intencionalmente para documentação e decisões, mantendo o DSL autocontido para uso direto no Structurizr Playground.

## Escopo

- Inclui L1, L2 e L3 do C4 Model.
- Não inclui L4.
- Não inclui Mermaid ou PlantUML.
- Não mistura AWS, Kubernetes ou deployment nos diagramas C4.
