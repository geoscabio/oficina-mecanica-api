# ADR-0013 — Autoscaling horizontal via HPA e ajuste de runtime .NET

## Status

**Status:** ✅ Aceito
**Data:** 12/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> O enunciado da Fase 2 exige, textualmente, "Horizontal Pod Autoscaler (HPA), escalando conforme consumo de CPU/memória" nos manifests Kubernetes, e pede no vídeo demonstrativo "escalabilidade automática (pode simular aumento de carga ou múltiplas ordens de serviço)". Ou seja, o mecanismo (HPA) e o critério (CPU/memória) são exigência literal. A API roda em containers com recursos limitados (ambiente de laboratório com 1 node `t3.small`), e durante os testes reais em AWS o autoscaler estava reagindo ao custo natural de inicialização do runtime .NET, não a carga de uso — um problema real que exigiu investigação e ajuste, além do que o enunciado pedia.

**O que é exigência literal:** usar HPA, escalando por CPU/memória, e demonstrar escalabilidade automática no vídeo. **O que é decisão/ajuste da equipe:** os valores específicos (min=1, max=5 réplicas, targets de 70% CPU e 80% memória) e, principalmente, a correção de `requests.memory` e do modo de Garbage Collector do .NET — nada disso está no enunciado, foi necessário porque observamos o HPA escalando sem carga real.

## 2. Fatores Decisivos (Drivers)

- **Requisito literal do enunciado:** HPA escalando conforme CPU/memória, com escalabilidade automática demonstrável em vídeo.
- **Ambiente com poucos recursos:** um único node, então a escala precisa ser horizontal (mais réplicas do pod), não vertical.
- **Runtime .NET tem custo de inicialização** (JIT, Entity Framework, geração do Swagger, pipeline de middlewares) que estava sendo confundido com carga real pelo autoscaler — problema descoberto em observação real no ambiente AWS, não previsto de antemão.

## 3. Decisão Proposta

> Utilizar o **HorizontalPodAutoscaler (HPA)** nativo do Kubernetes, escalando entre 1 e 5 réplicas, com metas de 70% de utilização de CPU e 80% de memória. Complementarmente, ajustar `requests.memory` de 128Mi para 256Mi (baseado em medição real do baseline de inicialização em ambiente AWS) e configurar o Garbage Collector do .NET como **Workstation GC** em vez do padrão Server GC (`<ServerGarbageCollection>false</ServerGarbageCollection>`).

## 4. Justificativa

- HPA é o mecanismo exigido pelo enunciado para esse requisito, sem necessidade de ferramenta externa.
- Em observação real no ambiente AWS, um pod recém-criado sem nenhuma requisição já consumia ~77Mi, subindo para ~100–128Mi em poucos minutos — o `requests.memory=128Mi` original era pequeno demais, e como o HPA calcula percentual sobre o `requests` (não o `limits`), o autoscaler reagia ao "ruído" de inicialização em vez de carga real. Sem esse ajuste, a demonstração de escalabilidade exigida pelo enunciado ficaria mascarada por escalonamento falso.
- O Server GC (padrão do .NET) cria um heap separado por núcleo de CPU visível ao processo, otimizado para alto paralelismo — mas o container roda com `limits.cpu=500m` (meio núcleo), um cenário onde essa estratégia reserva memória para paralelismo que o container não tem disponível. O Workstation GC é mais alinhado a esse perfil de baixo paralelismo.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Escalonamento reflete carga real, evidenciado com teste real de carga (Postman Performance Test), com ciclo orgânico de 1 → 3 → 1 réplicas capturado e documentado — cumprindo com evidência real o requisito do enunciado.
- Menor desperdício de memória reservada pelo Garbage Collector.
- `requests.cpu` não precisou de ajuste: uso observado (1–6m de 100m) já estava bem abaixo do target de 70%.

### ❌ Negativo (Perdas/Riscos)

- O node group do EKS permanece fixo em 1 node (decisão de custo do AWS Academy, ver ADR-0008); em um cenário de carga muito acima do teto configurado (5 réplicas), o HPA não teria para onde crescer sem também escalar o node group — fora do escopo atual.
- Os ajustes de memória/GC foram calibrados para o perfil observado neste ambiente específico (t3.small, 1 node); uma mudança de tipo de instância ou de carga de trabalho exigiria remedir o baseline.

## 6. Referências

- **FIAP, Pós-Tech Software Architecture.** [Enunciado do Tech Challenge — Fase 2](../../projeto/enunciado-fase-2-tech-challenge.pdf), seções "Orquestração com Kubernetes (K8s)" e "vídeo demonstrativo".
- **KUBERNETES.** *Horizontal Pod Autoscaling*. 2026.
- **MICROSOFT.** *.NET Garbage Collection: Workstation vs Server GC*. 2026.
- Evidência real: [`docs/evidencias/kubernetes-hpa.md`](../../evidencias/kubernetes-hpa.md).
