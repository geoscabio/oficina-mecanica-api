# Structurizr MCP

O Structurizr MCP e usado como camada de validacao e revisao assistida por IA para o workspace DSL.

## Servidor local

Suba o servidor:

```powershell
.\docs\architecture\scripts\start-structurizr-mcp.ps1
```

O script executa a imagem oficial:

```powershell
docker run -it --rm -p 3000:3000 -e PORT=3000 structurizr/mcp -dsl -mermaid -plantuml
```

Ferramentas habilitadas:

- `-dsl`: validar, parsear e inspecionar Structurizr DSL.
- `-mermaid`: exportar uma view para Mermaid.
- `-plantuml`: exportar uma view para PlantUML e C4-PlantUML.

## Conexao do agente

Use `mcp/structurizr-mcp.json` como referencia. O servidor local expõe:

```text
http://localhost:3000/mcp
```

## Checklist de IA como lint

Ao revisar `workspace.dsl`, o agente deve:

1. Validar se o DSL e parseavel.
2. Inspecionar violacoes de arquitetura apontadas pelo Structurizr.
3. Conferir se cada view tem uma narrativa clara e escopo consistente.
4. Exportar cada view para Mermaid e C4-PlantUML.
5. Comparar a modelagem com codigo, Docker, Kubernetes e Terraform do repositorio.

## Servidor remoto

A documentacao oficial tambem informa uma instancia publica do MCP em:

```text
https://mcp.structurizr.com/mcp
```

Para este repositorio, o padrao preferido e local via Docker, porque evita enviar contexto privado do projeto para fora da maquina.
