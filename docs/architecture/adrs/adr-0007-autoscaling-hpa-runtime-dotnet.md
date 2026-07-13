# ADR-0007 — Autoscaling horizontal via HPA e ajuste de runtime .NET

## Status

**Status:** ✅ Aceito
**Data:** 13/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> A Fase 2 exige demonstrar escalabilidade automática de verdade. A API roda em containers com recursos limitados de CPU e memória (ambiente de laboratório com 1 node `t3.small`), e o mecanismo de escala precisa reagir a carga real de uso — não ao custo natural de inicialização do runtime .NET.

## 2. Fatores Decisivos (Drivers)

- **Requisito de demonstrar autoscaling automático**, evidenciado com carga real, não simulada.
- **Ambiente com poucos recursos:** um único node, então a escala precisa ser horizontal (mais réplicas do pod), não vertical.
- **Runtime .NET tem custo de inicialização** (JIT, Entity Framework, geração do Swagger, pipeline de middlewares) que pode ser confundido com carga real pelo autoscaler.

## 3. Decisão Proposta

> Utilizar o **HorizontalPodAutoscaler (HPA)** nativo do Kubernetes, escalando entre 1 e 5 réplicas, com metas de 70% de utilização de CPU e 80% de memória. Complementarmente, ajustar `requests.memory` de 128Mi para 256Mi (baseado em medição real do baseline de inicialização em ambiente AWS) e configurar o Garbage Collector do .NET como **Workstation GC** em vez do padrão Server GC (`<ServerGarbageCollection>false</ServerGarbageCollection>`).

## 4. Justificativa

- HPA é o mecanismo nativo do Kubernetes para esse requisito, sem necessidade de ferramenta externa.
- Em observação real no ambiente AWS, um pod recém-criado sem nenhuma requisição já consumia ~77Mi, subindo para ~100–128Mi em poucos minutos — o `requests.memory=128Mi` original era pequeno demais, e como o HPA calcula percentual sobre o `requests` (não o `limits`), o autoscaler reagia ao "ruído" de inicialização em vez de carga real.
- O Server GC (padrão do .NET) cria um heap separado por núcleo de CPU visível ao processo, otimizado para alto paralelismo — mas o container roda com `limits.cpu=500m` (meio núcleo), um cenário onde essa estratégia reserva memória para paralelismo que o container não tem disponível. O Workstation GC é mais alinhado a esse perfil de baixo paralelismo.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Escalonamento reflete carga real, evidenciado com teste real de carga (Postman Performance Test), com ciclo orgânico de 1 → 3 → 1 réplicas capturado e documentado.
- Menor desperdício de memória reservada pelo Garbage Collector.
- `requests.cpu` não precisou de ajuste: uso observado (1–6m de 100m) já estava bem abaixo do target de 70%.

### ❌ Negativo (Perdas/Riscos)

- O node group do EKS permanece fixo em 1 node (decisão de custo do AWS Academy, ver `decisoes.md`); em um cenário de carga muito acima do teto configurado (5 réplicas), o HPA não teria para onde crescer sem também escalar o node group — fora do escopo atual.
- Os ajustes de memória/GC foram calibrados para o perfil observado neste ambiente específico (t3.small, 1 node); uma mudança de tipo de instância ou de carga de trabalho exigiria remedir o baseline.

## 6. Referências

- **KUBERNETES.** *Horizontal Pod Autoscaling*. 2026.
- **MICROSOFT.** *.NET Garbage Collection: Workstation vs Server GC*. 2026.
- Evidência real: [`docs/evidencias/kubernetes-hpa.md`](../../evidencias/kubernetes-hpa.md).
