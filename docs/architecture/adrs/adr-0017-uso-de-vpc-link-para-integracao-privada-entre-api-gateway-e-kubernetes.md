# ADR-0017 — Uso de VPC Link para Integração Privada entre API Gateway e Kubernetes

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

O API Gateway será a porta pública única da solução, enquanto a API principal continuará executando em pods no Kubernetes dentro de uma VPC privada.

O API Gateway precisa encaminhar requisições destinadas a `/api/*` para a aplicação sem expor o Load Balancer ou os pods diretamente à internet.

Era necessário definir o mecanismo de comunicação entre o API Gateway e o NLB interno que expõe a API no Kubernetes.

## 2. Fatores Decisivos

- Manter a API principal privada dentro da VPC.
- Impedir que o NLB interno seja acessado diretamente por clientes externos.
- Permitir que o API Gateway encaminhe tráfego para o Kubernetes.
- Utilizar uma integração compatível com o modelo HTTP API escolhido.
- Evitar a criação de endpoints públicos adicionais.
- Manter uma arquitetura simples para o MVP.

## 3. Decisão

Será utilizado o VPC Link na versão compatível com HTTP API para integrar o AWS API Gateway ao NLB interno da API.

As requisições recebidas na rota:

```text
ANY /api/{proxy+}
```

serão encaminhadas pelo API Gateway para o listener do NLB interno por meio do VPC Link.

O fluxo será:

```text
Cliente
  → API Gateway público
  → VPC Link
  → NLB interno
  → Service Kubernetes
  → Pods da oficina-mecanica-api
```

O VPC Link, o API Gateway e o NLB deverão pertencer à mesma conta AWS e ao mesmo ambiente da solução.

## 4. Justificativa

O VPC Link permite que o API Gateway se comunique com recursos privados dentro da VPC sem exigir que esses recursos tenham exposição pública.

A decisão mantém a API no Kubernetes isolada da internet e reforça o API Gateway como única camada pública de entrada.

O uso de VPC Link evita a necessidade de criar uma segunda URL pública para o NLB ou de permitir acesso externo direto aos pods da aplicação.

A integração por VPC Link também preserva a responsabilidade de cada componente:

- API Gateway: roteamento público e logs de acesso.
- VPC Link: conectividade privada entre Gateway e VPC.
- NLB interno: encaminhamento de tráfego para a API.
- API: autenticação JWT, autorização e regras de negócio.

## 5. Consequências

### Positivas

- A API permanece privada dentro da VPC.
- O NLB não precisa ser público.
- O API Gateway se torna a única entrada pública da aplicação.
- O tráfego entre o Gateway e a API ocorre por integração privada.
- A arquitetura fica mais alinhada aos requisitos de segurança e roteamento da Fase 3.
- A separação entre camada pública e aplicação de negócio fica mais clara.

### Negativas e riscos

- O VPC Link adiciona um recurso adicional para provisionar, monitorar e destruir.
- O API Gateway dependerá da disponibilidade do VPC Link e do NLB interno.
- A configuração incorreta das sub-redes, grupos de segurança ou listener pode impedir a comunicação.
- O VPC Link deve ser removido antes da destruição dos recursos de rede dependentes.
- O caminho encaminhado à API deverá ser validado para garantir compatibilidade com as rotas existentes.

## 6. Referências

- ADR-0015 — Uso do AWS API Gateway como Porta Pública Única.
- ADR-0016 — Migração do Load Balancer Público para NLB Interno.
- RFC-0002 — API Gateway com VPC Link e NLB Interno.
- Tech Challenge FIAP — Fase 3.