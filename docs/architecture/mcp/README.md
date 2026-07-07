# Structurizr MCP

O MCP é opcional. Ele serve para um agente de IA revisar o `workspace.dsl` como um lint de arquitetura, sem entrar no build, nos testes ou no deploy da aplicação.

## Subir localmente

```powershell
powershell -ExecutionPolicy Bypass -File docs\architecture\scripts\start-structurizr-mcp.ps1
```

O script executa a imagem oficial com suporte apenas ao DSL:

```powershell
docker run -it --rm -p 3000:3000 -e PORT=3000 structurizr/mcp -dsl
```

Endpoint local:

```text
http://localhost:3000/mcp
```

## Conectar um agente

Use `mcp/structurizr-mcp.json` como exemplo de configuração. Ele aponta para o servidor local usando `npx mcp-remote`.

## Checklist de revisão com IA

Ao revisar `workspace.dsl`, o agente deve:

1. Validar se o DSL é parseável.
2. Inspecionar inconsistências apontadas pelo Structurizr.
3. Conferir se as views C4 continuam refletindo código, Docker, Kubernetes e Terraform.
4. Sugerir ajustes no DSL sem gerar arquivos derivados.

## Observação

Se Docker, Node, `npx` ou MCP não estiverem disponíveis, nada da aplicação deve falhar. Nesse caso, use apenas o Structurizr Playground para validar visualmente o `workspace.dsl`.
