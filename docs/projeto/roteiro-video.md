# Roteiro do vídeo de demonstração

Roteiro para gravação em **6 mini-vídeos** (um por seção), depois unidos no Clipchamp até totalizar no máximo 15 minutos. Cada seção separa **Falar** (o que verbalizar) de **Mostrar** (o que demonstrar na tela).

Pré-requisito antes de gravar os vídeos 2 a 5: `terraform apply` executado (ambiente AWS `development` no ar). Ver [`docs/deploy/deploy-aws.md`](../deploy/deploy-aws.md). Os vídeos 1 e 6 não dependem do ambiente estar de pé — podem ser gravados a qualquer momento.

## Vídeo 1 · Abertura (~1:00)

**Falar:**
- Nome do projeto (Oficina Mecânica API) e contexto (Tech Challenge Fase 2, FIAP Pós-Tech Software Architecture).
- Uma frase de arquitetura: monólito em Clean Architecture (.NET 10), modularizado por bounded contexts (Identidade, Atendimento, Administrativo, Gestão de Estoque, Gestão de Ordem de Serviço), publicado em Kubernetes (Amazon EKS) via Terraform e GitHub Actions.

**Mostrar:** nada além de você falando (ou o README do projeto de fundo).

## Vídeo 2 · CI/CD (~2:30)

**Falar:**
- Que todo merge em `develop` dispara a esteira automaticamente.
- Que o guardrail de `terraform destroy` só roda quando alguém altera `terraform-action.env` no próprio PR — evita destruição acidental.

**Mostrar:**
- Um PR real já mergeado (ex. [#177](https://github.com/geoscabio/oficina_mecanica_api/pull/177)) e a aba **Actions** do GitHub.
- Esteira `CI` rodando: build, testes (424 testes, cobertura), quality gate.
- Esteira `CD Development` disparada pelo merge: `terraform init/plan/apply`, build e push da imagem no ECR, rollout no EKS.

## Vídeo 3 · Deploy real na AWS (~2:00)

**Falar:** que os recursos abaixo estão vivos na AWS agora, não simulados.

**Mostrar:**
- AWS Console: EKS Cluster (`oficina-mecanica-eks-dev`), RDS, Load Balancer, ECR com a imagem publicada.
- Terminal: `kubectl get pods,svc,hpa -n oficina-mecanica` com pod `Running` e Service com o hostname do Load Balancer.
- `curl <endpoint>/api/health` retornando `Healthy`.

## Vídeo 4 · Consumo da API (~4:00)

**Falar:** que o fluxo inteiro de uma Ordem de Serviço está sendo executado contra o endpoint real da AWS, incluindo a consulta pública de status (sem autenticação) — o endpoint que o cliente final usaria.

**Mostrar:**
- Swagger publicado (`<endpoint>/swagger`) com o contrato completo.
- Postman (collection em [`docs/postman/`](../postman/), guia em [`docs/evidencias/postman.md`](../evidencias/postman.md)), environment **AWS Dev**:
  - Login (token capturado automaticamente).
  - Fluxo principal: cadastrar cliente → cadastrar veículo → cadastrar mecânico → abrir OS → iniciar diagnóstico → definir serviços → aguardar aprovação → notificar decisão do orçamento (webhook) → iniciar execução → finalizar → entregar.
  - `consultar-status` (endpoint público).

## Vídeo 5 · Escalabilidade automática — HPA (~4:00)

**Falar:** que o HPA está reagindo à carga real gerada pelo Postman, sem intervenção manual — a CPU passa de X% e ele decide escalar sozinho.

**Mostrar:**
- Lado a lado: AWS Console (**EKS → `oficina-mecanica-eks-dev` → Resources → HorizontalPodAutoscalers**) e terminal com `kubectl get hpa oficina-mecanica-api-hpa -n oficina-mecanica --watch`.
- Postman **Performance Test** (Collection Runner) contra o environment AWS Dev — mesma configuração de [`docs/evidencias/kubernetes-hpa.md`](../evidencias/kubernetes-hpa.md) (20 Virtual Users, 5 minutos, Login + Listar clientes).
- Se der tempo, o downscale automático de volta a 1 réplica após a carga cessar (senão, citar que o ciclo completo de subida e descida já está documentado no repositório).

## Vídeo 6 · Encerramento (~1:00)

**Falar:**
- Que todo o ciclo (deploy, CI/CD, consumo, escalabilidade) foi demonstrado contra o ambiente real na AWS.
- Que ao final da avaliação o ambiente é destruído pela esteira, para não gerar custo indevido no AWS Academy Learner Lab.
- Link do repositório: <https://github.com/geoscabio/oficina_mecanica_api>.

**Mostrar:** nada além de você falando (ou a tela final do repositório no GitHub).

## Checklist antes de gravar

- [ ] Vídeos 1 e 6: podem ser gravados a qualquer momento, ambiente AWS não precisa estar no ar.
- [ ] Antes dos vídeos 2 a 5: `terraform apply` rodado, ambiente saudável (`/api/health` = `Healthy`).
- [ ] Postman com environment **AWS Dev** configurado e testado (login funcionando).
- [ ] Terminal com `kubectl` configurado e testado (`kubectl get hpa` retornando dados).
- [ ] Janelas organizadas na tela antes de gravar cada vídeo (Console AWS, terminal, Postman, Swagger).
- [ ] Gravar os vídeos 2 a 5 em sequência, sem reaplicar/destruir o Terraform entre eles.
- [ ] Mirar o tempo de cada mini-vídeo perto do alvo da seção — o Clipchamp só concatena, não corta por tempo.

## Depois de gravar

- Juntar os 6 mini-vídeos no Clipchamp (modo IA) e conferir se o total ficou dentro de 15 minutos.
- Subir o vídeo final (YouTube não listado, Google Drive ou similar) e anotar o link.
- Preencher o link em [`docs/projeto/pdf-entrega.md`](pdf-entrega.md).
- Rodar o `terraform destroy` final (ver [`docs/deploy/aws-academy-guardrails.md`](../deploy/aws-academy-guardrails.md)).
