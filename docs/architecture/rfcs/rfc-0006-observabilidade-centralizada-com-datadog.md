# RFC-0006 — Observabilidade Centralizada com Datadog

## Status

Aceita para implementação na Fase 3.

## Contexto

A Fase 3 exige monitoramento de latência das APIs, consumo de CPU e memória do Kubernetes, health checks, uptime, alertas para falhas no processamento de ordens de serviço, logs estruturados e correlação entre requisições.

A solução será composta por API Gateway, Lambda de autenticação, API ASP.NET Core no Kubernetes e RDS. Sem uma ferramenta central de observabilidade, seria necessário consultar logs e métricas em múltiplos serviços, dificultando a análise de incidentes e a demonstração da solução.

## Decisão

O Datadog será adotado como ferramenta única de observabilidade da Fase 3.

Não será adicionado OpenTelemetry ao escopo principal. A instrumentação utilizará os recursos nativos do Datadog para .NET, Kubernetes, Lambda, logs, traces, métricas, dashboards e alertas.

## Componentes monitorados

| Componente | Integração com Datadog | Informações monitoradas |
| --- | --- | --- |
| API ASP.NET Core | Datadog APM .NET e logs JSON | Latência, erros, traces, logs, health checks e métricas da aplicação |
| Kubernetes EKS | Datadog Agent e Cluster Agent | CPU, memória, pods, nodes, reinicializações e eventos do cluster |
| Lambda de autenticação | Datadog Lambda Extension | Invocações, duração, erros, timeouts, logs e traces |
| API Gateway | Logs de acesso no CloudWatch encaminhados ao Datadog | Requisições, latência, status HTTP e erros de integração |
| RDS SQL Server | Integração de infraestrutura do Datadog | CPU, conexões, disponibilidade e métricas operacionais do banco |

## Tags obrigatórias

Todos os componentes monitorados deverão utilizar tags consistentes para permitir a correlação entre logs, traces e métricas.

| Variável | Finalidade | Exemplo |
| --- | --- | --- |
| `DD_ENV` | Identifica o ambiente | `development` |
| `DD_SERVICE` | Identifica o serviço monitorado | `oficina-mecanica-api` |
| `DD_VERSION` | Identifica a versão implantada | `sha-a1b2c3d` |

Os nomes de serviço esperados são:

```text
oficina-mecanica-api
oficina-mecanica-auth-lambda
oficina-mecanica-api-gateway
```

A versão deverá ser preferencialmente o SHA do commit ou a tag da imagem implantada.

## Logs estruturados

A API e a Lambda deverão produzir logs em formato JSON.

Cada log deverá conter, quando aplicável:

| Campo | Descrição |
| --- | --- |
| `timestamp` | Momento em que o evento ocorreu |
| `level` | Nível do log, como `Information`, `Warning` ou `Error` |
| `message` | Mensagem curta e legível |
| `service` | Serviço responsável pelo evento |
| `env` | Ambiente de execução |
| `version` | Versão implantada |
| `x_correlation_id` | Identificador funcional da jornada |
| `dd.trace_id` | Identificador técnico do trace no Datadog |
| `dd.span_id` | Identificador do trecho específico do trace |
| `operation` | Operação de negócio ou endpoint executado |
| `bounded_context` | Contexto responsável pelo evento |
| `http.method` | Método HTTP |
| `http.route` | Rota acessada, sem dados sensíveis |
| `http.status_code` | Código HTTP retornado |
| `cliente_id` | Identificador interno do cliente, quando aplicável |
| `ordem_servico_id` | Identificador da ordem de serviço, quando aplicável |
| `jti` | Identificador do JWT, quando aplicável |

CPF puro, tokens JWT, senhas, segredos, credenciais AWS e dados sensíveis não poderão ser registrados em logs.

## Correlação de requisições

A solução utilizará dois mecanismos complementares de correlação:

1. `X-Correlation-Id`: identificador funcional da jornada de negócio.
2. `dd.trace_id` e `dd.span_id`: identificadores técnicos do trace distribuído no Datadog.

O API Gateway deverá encaminhar ou preservar o `X-Correlation-Id` recebido. Caso esse cabeçalho não exista, a primeira camada da jornada deverá gerar um identificador.

A API e a Lambda deverão incluir esse identificador em seus logs. Assim, será possível acompanhar uma mesma requisição entre API Gateway, Lambda, API principal e banco de dados.

## Dashboards obrigatórios

Serão criados dashboards para acompanhar:

- Volume diário de ordens de serviço.
- Tempo médio por status: Diagnóstico, Execução e Finalização.
- Latência das APIs.
- Health checks e uptime.
- CPU e memória do Kubernetes.
- Invocações, erros e duração da Lambda.
- Erros do API Gateway.
- Falhas no processamento de ordens de serviço.
- CPU e conexões do RDS.

## Alertas obrigatórios

Serão configurados alertas para:

- API indisponível.
- Aumento de erros HTTP 5xx.
- Latência acima do limite definido.
- Falha no processamento de ordens de serviço.
- Reinicialização contínua de pods.
- Erro ou timeout da Lambda.
- CPU ou conexões do RDS em nível crítico.
- Falha de health check.

Os limites numéricos dos alertas serão definidos com base no comportamento observado no ambiente de desenvolvimento e documentados junto aos dashboards.

## Consequências

### Positivas

- Centraliza logs, métricas e traces de todos os componentes.
- Permite identificar rapidamente falhas entre Gateway, Lambda, API, Kubernetes e banco.
- Atende aos requisitos de logs estruturados e correlação entre requisições.
- Melhora a qualidade da demonstração ao permitir evidenciar métricas e falhas reais.
- Facilita a análise de latência, disponibilidade e consumo de recursos.

### Trade-offs e cuidados

- A instrumentação adiciona configuração e dependências aos serviços.
- Logs excessivos podem gerar custo e dificultar a análise; a regra será registrar pouco, mas registrar bem.
- Tags inconsistentes prejudicam a correlação entre serviços.
- Dados pessoais e segredos exigem atenção para não serem enviados ao Datadog.
- Dashboards e alertas só serão confiáveis após os serviços enviarem dados reais.

## Fora de escopo

- Adotar OpenTelemetry como tecnologia adicional de instrumentação.
- Criar observabilidade para ambientes que não fazem parte da Fase 3.
- Definir retenção de longo prazo para logs.
- Implementar um centro de operações ou processo formal de resposta a incidentes.

## Critérios de aceite

- A API, a Lambda, o API Gateway, o Kubernetes e o RDS enviam dados ao Datadog.
- Logs da API e da Lambda são estruturados em JSON.
- Logs e traces possuem `DD_ENV`, `DD_SERVICE` e `DD_VERSION`.
- A jornada de uma requisição pode ser encontrada por `X-Correlation-Id`.
- Os dashboards obrigatórios exibem dados reais.
- Os alertas obrigatórios são configurados e testados em cenário controlado.
- CPF, JWT, senhas e segredos não aparecem em logs.