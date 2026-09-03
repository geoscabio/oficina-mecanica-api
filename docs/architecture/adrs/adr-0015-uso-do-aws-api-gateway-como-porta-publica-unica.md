# ADR-0015 — Uso do AWS API Gateway como Porta Pública Única

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

A API da Oficina Mecânica é executada no Kubernetes. No modelo atual, a aplicação pode ser exposta por um Load Balancer público, permitindo acesso direto à API sem uma camada central de entrada, roteamento e observabilidade.

A Fase 3 exige a implementação de um API Gateway para controle e roteamento. A arquitetura também precisa encaminhar a autenticação por CPF para uma Lambda e manter a API principal protegida dentro da VPC.

Era necessário definir qual componente seria responsável pela entrada pública das requisições e como separar o acesso à Lambda de autenticação e à API executada no Kubernetes.

## 2. Fatores Decisivos

- Cumprir o requisito de API Gateway da Fase 3.
- Centralizar a entrada pública da solução.
- Expor a Lambda de autenticação por CPF sem expor a API principal diretamente.
- Manter a API no Kubernetes privada dentro da VPC.
- Reduzir a complexidade operacional do MVP.
- Permitir logs de acesso e monitoramento em uma camada única.
- Preservar o estilo RESTful da API existente.

## 3. Decisão

Será utilizado o AWS API Gateway no modelo HTTP API como porta pública única da solução.

A aplicação continuará sendo uma API RESTful. A escolha por HTTP API representa apenas o tipo de serviço gerenciado adotado dentro do AWS API Gateway e não altera o estilo arquitetural da API.

O API Gateway possuirá as seguintes rotas principais:

| Rota | Destino | Integração |
| --- | --- | --- |
| `POST /auth/cpf` | `oficina-mecanica-auth-lambda` | Lambda Proxy Integration |
| `ANY /api/{proxy+}` | API no Kubernetes | HTTP Proxy Integration via VPC Link |

Será utilizado o estágio padrão `$default`, para que a URL pública não possua prefixos como `/development`.

## 4. Justificativa

O modelo HTTP API atende ao escopo da solução ao suportar integração com Lambda e integração privada com recursos na VPC por meio de VPC Link.

A escolha reduz a complexidade operacional ao evitar recursos não necessários para o MVP, como API Keys, Usage Plans, cache e mapeamentos avançados da REST API.

O API Gateway centraliza o roteamento entre os dois fluxos principais:

- Autenticação de clientes por CPF na Lambda.
- Consumo das rotas da API principal no Kubernetes.

O uso do estágio `$default` mantém a URL pública mais simples e permite que as rotas da API sejam expostas sem um prefixo de ambiente.

## 5. Consequências

### Positivas

- O API Gateway passa a