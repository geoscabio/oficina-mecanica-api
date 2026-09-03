# ADR-0021 — Datadog como Solução de Observabilidade

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

A Fase 3 do Tech Challenge tem como objetivo elevar a aplicação da oficina mecânica para um cenário de operação corporativa, incluindo práticas de cloud, infraestrutura como código, segurança, escalabilidade e observabilidade.

O enunciado exige a implementação de uma solução de monitoramento e observabilidade capaz de acompanhar a latência das APIs, o consumo de recursos do Kubernetes, healthchecks e uptime, além de gerar alertas para falhas no processamento das ordens de serviço. Também é necessário implementar logs estruturados em JSON, incluindo correlação entre requisições.

Além do monitoramento técnico, a solução deve disponibilizar dashboards contendo informações como volume diário de ordens de serviço, tempo médio de execução por status e erros ou falhas nas integrações.

Diante desses requisitos, era necessário definir uma solução de observabilidade que pudesse acompanhar os diferentes componentes da arquitetura da Fase 3, incluindo a aplicação principal executada no Kubernetes, a Function Serverless de autenticação e o API Gateway.

## 2. Fatores Decisivos

- Atender aos requisitos de monitoramento e observabilidade definidos no Tech Challenge.
- Monitorar a latência das APIs.
- Monitorar CPU e memória do Kubernetes.
- Monitorar healthchecks e uptime da aplicação.
- Monitorar falhas no processamento de ordens de serviço.
- Centralizar logs, métricas e traces em uma única plataforma.
- Permitir correlação entre logs e traces.
- Disponibilizar dashboards técnicos e de negócio.
- Permitir criação de alertas para falhas e degradação dos serviços.
- Monitorar a aplicação principal executada no Kubernetes.
- Monitorar a Lambda responsável pela autenticação.
- Monitorar os access logs do API Gateway.
- Evitar a introdução de múltiplas ferramentas de observabilidade sem necessidade para o escopo da Fase 3.
- Manter a solução simples e alinhada ao princípio de evitar overengineering definido no plano de execução.

## 3. Decisão

Será utilizado o **Datadog como solução única de observabilidade da arquitetura da Fase 3**.

O Datadog será responsável pela centralização de logs, métricas, traces, dashboards e alertas dos componentes monitorados.

A arquitetura de observabilidade será composta pelos seguintes elementos:

```text
                         ┌──────────────────────┐
                         │       Datadog        │
                         │ Logs / APM / Metrics │
                         │ Dashboards / Alerts  │
                         └──────────┬───────────┘
                                    │
              ┌─────────────────────┼─────────────────────┐
              │                     │                     │
              ▼                     ▼                     ▼
       ┌─────────────┐       ┌─────────────┐       ┌─────────────┐
       │ API Gateway │       │ Auth Lambda │       │     EKS     │
       │ Access Logs │       │ Logs/Traces │       │ Agent/APM   │
       └─────────────┘       └─────────────┘       └──────┬──────┘
                                                          │
                                                          ▼
                                                   ┌─────────────┐
                                                   │     API     │
                                                   │ Logs/Traces │
                                                   │   Metrics   │
                                                   └─────────────┘
```

No Kubernetes, será utilizado o **Datadog Agent/Cluster Agent** para coleta de métricas de infraestrutura, pods, nodes e logs da aplicação.

Na `oficina-mecanica-auth-lambda`, será utilizada a **Datadog Lambda Extension** para coleta de métricas, logs e traces.

O API Gateway terá seus **access logs** integrados ao Datadog por meio dos mecanismos de logging da AWS.

A `oficina-mecanica-api` utilizará **Datadog APM para .NET** para instrumentação dos traces da aplicação e **Serilog com logs estruturados em JSON**.

Os componentes monitorados utilizarão informações padronizadas para identificação dos serviços e ambientes:

- `DD_ENV`
- `DD_SERVICE`
- `DD_VERSION`

A correlação funcional das requisições será realizada utilizando o `X-Correlation-Id`.

A correlação técnica entre logs e traces utilizará:

- `dd.trace_id`
- `dd.span_id`

Os logs deverão permitir acompanhar uma mesma jornada funcional entre Gateway, Lambda e API, enquanto os traces permitirão identificar tecnicamente o caminho percorrido por uma requisição.

Os dashboards deverão contemplar os indicadores definidos para a Fase 3, incluindo:

- Volume diário de ordens de serviço.
- Tempo médio por status: Diagnóstico, Execução e Finalização.
- Latência das APIs.
- Healthchecks e uptime.
- CPU e memória do Kubernetes.
- Invocações, erros e duração da Lambda.
- Erros do API Gateway.
- Falhas no processamento de ordens de serviço.

Também serão criados alertas para situações relevantes, incluindo:

- API fora do ar.
- Aumento de erros HTTP 5xx.
- Latência acima do limite definido.
- Falhas no processamento de ordens de serviço.
- Pods reiniciando em loop.
- Erros ou timeout da Lambda.
- CPU ou conexões do RDS em nível crítico.

Não será utilizado **OpenTelemetry** no escopo principal da Fase 3. A instrumentação utilizará os recursos nativos do Datadog para manter a solução simples e adequada ao escopo do projeto.

## 4. Justificativa

O Tech Challenge permite a utilização de ferramentas como **Datadog ou New Relic** para implementação da observabilidade. A escolha da ferramenta é livre, desde que os requisitos de monitoramento e observabilidade sejam atendidos.

A escolha do Datadog permite centralizar diferentes dimensões da observabilidade em uma única plataforma, evitando a necessidade de manter ferramentas distintas para logs, métricas, traces, dashboards e alertas.

A solução também é adequada à arquitetura definida para a Fase 3, pois permite monitorar componentes executados em diferentes modelos:

- API Gateway como porta pública de entrada.
- Lambda como função serverless de autenticação.
- EKS como plataforma de execução da aplicação.
- API ASP.NET Core como aplicação principal.
- RDS como banco de dados gerenciado.

A utilização do Datadog Agent/Cluster Agent permite acompanhar a infraestrutura e os workloads do Kubernetes, enquanto o APM possibilita acompanhar o comportamento da aplicação e suas requisições.

A Datadog Lambda Extension permite adicionar observabilidade à função serverless sem criar uma solução paralela específica para a Lambda.

A integração dos access logs do API Gateway permite observar também a camada responsável pela entrada e roteamento das requisições.

A padronização das informações `DD_ENV`, `DD_SERVICE` e `DD_VERSION` facilita a identificação do serviço e do ambiente em que determinado evento ocorreu.

O uso conjunto de `X-Correlation-Id`, `dd.trace_id` e `dd.span_id` permite combinar uma visão funcional e uma visão técnica da mesma requisição, facilitando a investigação de problemas ponta a ponta.

A decisão também está alinhada ao plano de execução da Fase 3, que estabelece o Datadog como ferramenta única de observabilidade e define sua utilização no EKS, Lambda, API Gateway e aplicação principal.

## 5. Consequências

### Positivas

- A observabilidade fica centralizada em uma única plataforma.
- Logs, métricas e traces podem ser analisados em conjunto.
- A aplicação principal pode ser monitorada por meio do APM.
- O Kubernetes pode ser monitorado em nível de infraestrutura e workloads.
- A Lambda pode ter suas invocações, erros e duração acompanhados.
- Os access logs do API Gateway podem ser analisados.
- Dashboards técnicos e de negócio podem ser construídos na mesma plataforma.
- Alertas podem ser configurados para falhas e degradação dos serviços.
- A correlação por `X-Correlation-Id`, `dd.trace_id` e `dd.span_id` facilita a investigação ponta a ponta.
- A solução atende aos requisitos de observabilidade definidos no Tech Challenge.
- A utilização dos recursos nativos do Datadog reduz a quantidade de componentes adicionais necessários.

### Negativas e riscos

- A solução passa a depender do Datadog para centralização da observabilidade.
- A configuração incorreta dos agentes ou integrações pode resultar em perda de logs, métricas ou traces.
- O volume de dados coletados pode gerar custos adicionais dependendo da configuração utilizada.
- A instrumentação precisa ser mantida durante a evolução da aplicação.
- Falhas na configuração de correlação podem dificultar o rastreamento de uma requisição entre os componentes.
- A utilização de recursos nativos do Datadog reduz a portabilidade da instrumentação para outras plataformas.
- Dashboards e alertas precisam ser configurados e validados com dados reais para garantir que os indicadores representem corretamente o comportamento da solução.

## 6. Referências

- Tech Challenge FIAP — Fase 3.
- Plano Final de Execução — Fase 3.
- RFC-0006 — Observabilidade com Datadog.
- ADR-0018 — Execução da Lambda de Autenticação dentro da VPC.
- ADR-0019 — Autenticação das APIs por JWT Emitido pela Lambda.
- ADR-0020 — Inclusão do Status do Cliente no Modelo Relacional.
