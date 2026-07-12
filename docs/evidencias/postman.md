# Evidencia: Collection Postman

Este arquivo documenta a collection Postman completa da API, usada tanto para exploracao manual das rotas quanto para o teste de carga que validou o HPA (ver [`kubernetes-hpa.md`](kubernetes-hpa.md)).

## Arquivos

| Arquivo | Descricao |
| --- | --- |
| [`../postman/oficina-mecanica-api.postman_collection.json`](../postman/oficina-mecanica-api.postman_collection.json) | Collection completa (Postman Collection v2.1), importada diretamente do `swagger.json` publicado pela API. |
| [`../postman/oficina-mecanica-local.postman_environment.json`](../postman/oficina-mecanica-local.postman_environment.json) | Environment para execucao local (`baseUrl = http://localhost:5093`). |
| [`../postman/oficina-mecanica-aws-dev.postman_environment.json`](../postman/oficina-mecanica-aws-dev.postman_environment.json) | Environment para o ambiente AWS de demonstracao. |

## Metodologia de construcao

A collection **nao foi escrita manualmente**: foi importada diretamente da URL `swagger.json` publicada pela propria API (`{{baseUrl}}/swagger/v1/swagger.json`), garantindo fidelidade total ao contrato real exposto pelo Swashbuckle — inclusive a autenticacao. Como a especificacao OpenAPI gerada pela API ja declara quais operacoes exigem Bearer token (a partir dos atributos `[Authorize]`/`[AllowAnonymous]` dos controllers), o importador do Postman aplicou automaticamente um bloco de autenticacao Bearer em cada requisicao protegida, e nenhum nas rotas publicas (`login`, `consultar-status` de ordem de servico, `health`, webhook de notificacao de orcamento).

## Autenticacao automatica

A requisicao de login (`POST /api/v1/identidade/autenticacao/login`) tem um script de pos-resposta que captura o token e salva na variavel de environment `token`:

```javascript
if (pm.response.code === 200) {
    const jsonData = pm.response.json();
    pm.environment.set('token', jsonData.token);
}
```

Todas as demais requisicoes protegidas referenciam `{{token}}` no cabecalho de autorizacao, entao basta rodar o login uma vez por sessao de testes.

> Correcao aplicada antes de commitar: o importador do Postman gerou os blocos de autenticacao referenciando `{{bearerToken}}` (nome generico padrao do importador OpenAPI), enquanto o script de login salva em `{{token}}`. Substituido em todas as 47 ocorrencias para manter consistencia — sem essa correcao, a autenticacao automatica falharia silenciosamente em toda rota protegida.

## Environments

| Environment | `baseUrl` | Uso |
| --- | --- | --- |
| Local | `http://localhost:5093` | Execucao via Docker Compose ou Kubernetes local (port-forward). |
| AWS Dev | Hostname do Load Balancer da AWS (ex.: `http://a69481265379a4560b49618419fb7363-769502012.us-east-1.elb.amazonaws.com`) | Ambiente publicado na AWS. |

O `baseUrl` do environment AWS Dev reflete o Load Balancer ativo no momento da captura desta evidencia. Como o ambiente e recriado a cada `terraform apply` (e destruido ao final da demonstracao), esse hostname muda a cada nova execucao — o valor atual sempre pode ser obtido via `terraform output api_service_hostname`.

Em ambos os environments, a variavel `token` foi exportada **vazia** de proposito (nunca commitar um JWT capturado, mesmo de ambiente de demonstracao).

## Uso no teste de carga (HPA)

Essa mesma collection foi usada como gerador de carga real no **Postman Performance Test** (Collection Runner), contra o ambiente AWS, para validar o escalonamento automatico do HPA. Detalhes completos, configuracao do teste e evidencia visual em [`kubernetes-hpa.md`](kubernetes-hpa.md).

## Como importar

1. Postman -> **Import** -> **File** -> selecionar os 3 arquivos acima (collection + 2 environments).
2. Selecionar o environment desejado (`Local` ou `AWS Dev`) no seletor superior direito.
3. Rodar a requisicao de login uma vez (captura o token automaticamente).
4. Rodar qualquer outra requisicao — a autenticacao Bearer ja e aplicada automaticamente onde a rota exige.
