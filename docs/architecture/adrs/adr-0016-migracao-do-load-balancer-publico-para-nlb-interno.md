# ADR-0016 — Migração do Load Balancer Público para NLB Interno

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio  
**Substitui:** ADR-0009 — Load Balancer provisionado via Kubernetes Service

---

## 1. Contexto e Problema

Na Fase 2, a API executada no Kubernetes foi exposta por um Load Balancer público criado a partir de um Service do tipo `LoadBalancer`. Essa decisão permitiu demonstrar a API em ambiente AWS, mas deixa a aplicação acessível diretamente pela internet.

Na Fase 3, o API Gateway será adotado como porta pública única. Portanto, manter um Load Balancer público criaria uma rota alternativa de acesso à API, permitindo que consumidores externos tentassem contornar o Gateway.

A integração privada do API Gateway com recursos dentro da VPC exige um alvo acessível internamente. Era necessário definir o tipo de Load Balancer adequado para receber o tráfego encaminhado pelo VPC Link e manter a API privada.

## 2. Fatores Decisivos

- Garantir que o API Gateway seja a única entrada pública da aplicação.
- Impedir acesso direto à API executada no Kubernetes.
- Permitir integração privada entre API Gateway e Kubernetes por VPC Link.
- Manter a solução simples para o MVP.
- Preservar a capacidade de expor um Service Kubernetes para os pods da API.
- Substituir o Load Balancer público utilizado na Fase 2.

## 3. Decisão

O Load Balancer público da API será substituído por um Network Load Balancer interno.

O Service Kubernetes da API continuará sendo responsável por expor a aplicação dentro do cluster, mas será configurado para criar um NLB com visibilidade interna, sem endereço público.

O fluxo de acesso será:

```text
Cliente
  → API Gateway público
  → VPC Link
  → NLB interno
  → Service Kubernetes
  → Pods da oficina-mecanica-api
```

O DNS e o ARN do listener do NLB serão publicados como outputs para consumo da esteira do API Gateway.

## 4. Justificativa

O NLB interno permite que o API Gateway encaminhe requisições para a API dentro da VPC, sem tornar o Load Balancer acessível pela internet.

Essa decisão elimina a rota pública direta utilizada na Fase 2 e reforça a separação de responsabilidades:

- API Gateway: entrada pública, roteamento e logs de acesso.
- NLB interno: encaminhamento de rede para a API privada.
- API ASP.NET Core: validação de JWT, autorização e regras de negócio.

A criação do NLB a partir do Service Kubernetes mantém a integração natural entre Kubernetes e AWS, evitando a necessidade de gerenciar manualmente os targets dos pods.

## 5. Consequências

### Positivas

- A API deixa de possuir um endpoint público direto.
- O API Gateway se torna a única porta pública da solução.
- O tráfego entre Gateway e Kubernetes permanece dentro da VPC.
- O NLB atende à integração privada por VPC Link.
- O ciclo de vida do Load Balancer continua vinculado ao Service Kubernetes.
- A arquitetura fica mais segura e mais simples de explicar.

### Negativas e riscos

- O NLB interno não poderá ser usado diretamente por clientes externos.
- A API dependerá da configuração correta do API Gateway e do VPC Link.
- A criação do NLB pode levar alguns minutos após o deploy do Service Kubernetes.
- O API Gateway dependerá do ARN do listener do NLB antes de criar sua integração privada.
- Configurações incorretas de sub-rede, grupo de segurança ou anotação do Service podem impedir a comunicação interna.

## 6. Referências

- ADR-0009 — Load Balancer provisionado via Kubernetes Service.
- ADR-0015 — Uso do AWS API Gateway como Porta Pública Única.
- RFC-0002 — API Gateway com VPC Link e NLB Interno.
- Tech Challenge FIAP — Fase 3.