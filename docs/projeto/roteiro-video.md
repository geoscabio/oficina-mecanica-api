# Roteiro do vídeo de demonstração

Rascunho de roteiro para a gravação final (limite de 15 minutos). Ajuste o texto livremente na hora de gravar — o objetivo aqui é garantir que nenhum item obrigatório fique de fora e que a ordem faça sentido.

Pré-requisito antes de gravar: `terraform apply` executado (ambiente AWS `development` no ar). Ver [`docs/deploy/deploy-aws.md`](../deploy/deploy-aws.md).

## 0:00 – 1:00 · Abertura

- Apresentação rápida: nome do projeto (Oficina Mecânica API), contexto (Tech Challenge Fase 2, FIAP Pós-Tech Software Architecture).
- Uma frase sobre a arquitetura: monólito em Clean Architecture (.NET 10), modularizado por bounded contexts (Identidade, Atendimento, Administrativo, Gestão de Estoque, Gestão de Ordem de Serviço), publicado em Kubernetes (Amazon EKS) via Terraform e GitHub Actions.

## 1:00 – 3:30 · CI/CD

- Mostrar um Pull Request real sendo aberto contra `develop` (pode ser um PR já existente no histórico, ex. [#177](https://github.com/geoscabio/oficina_mecanica_api/pull/177) ou similar).
- Mostrar a esteira `CI` rodando: build, testes (424 testes, cobertura), quality gate — aba **Actions** do GitHub.
- Mostrar o merge disparando a esteira `CD Development`: `terraform init/plan/apply`, build e push da imagem Docker no ECR, rollout no EKS.
- Falar rapidamente sobre o guardrail: `terraform destroy` só roda quando alguém altera `terraform-action.env` no próprio PR — evita destruição acidental.

## 3:30 – 5:30 · Deploy real na AWS

- AWS Console: mostrar os recursos vivos — EKS Cluster (`oficina-mecanica-eks-dev`), RDS, Load Balancer, ECR com a imagem publicada.
- Terminal: `kubectl get pods,svc,hpa -n oficina-mecanica` confirmando o pod `Running` e o Service com o hostname do Load Balancer.
- `curl <endpoint>/api/health` retornando `Healthy`.

## 5:30 – 9:30 · Consumo da API

- Abrir o Swagger publicado (`<endpoint>/swagger`) para mostrar o contrato completo.
- Trocar para o **Postman** (collection em [`docs/postman/`](../postman/), guia em [`docs/evidencias/postman.md`](../evidencias/postman.md)):
  - Selecionar o environment **AWS Dev**.
  - Rodar o login (token capturado automaticamente).
  - Rodar o fluxo principal de uma Ordem de Serviço: cadastrar cliente → cadastrar veículo → cadastrar mecânico → abrir OS → iniciar diagnóstico → definir serviços → aguardar aprovação → notificar decisão do orçamento (webhook) → iniciar execução → finalizar → entregar.
  - Destacar a consulta pública de status (`consultar-status`, sem autenticação) — é o endpoint que o cliente final usaria.

## 9:30 – 13:30 · Escalabilidade automática (HPA)

- Abrir a AWS Console em **EKS → `oficina-mecanica-eks-dev` → Resources → HorizontalPodAutoscalers → `oficina-mecanica-api-hpa`** e um terminal com `kubectl get hpa oficina-mecanica-api-hpa -n oficina-mecanica --watch` lado a lado.
- No Postman, rodar o **Performance Test** (Collection Runner) contra o environment AWS Dev — mesma configuração documentada em [`docs/evidencias/kubernetes-hpa.md`](../evidencias/kubernetes-hpa.md) (20 Virtual Users, 5 minutos, Login + Listar clientes).
- Narrar o que está acontecendo enquanto a réplica sobe: "o HPA está reagindo à carga real gerada pelo Postman, sem nenhuma intervenção manual — CPU passa de X% e ele decide escalar".
- Se der tempo, deixar a carga cessar em câmera e mostrar o downscale automático de volta a 1 réplica (senão, citar que a evidência completa do ciclo de subida e descida está documentada no repositório).

## 13:30 – 14:30 · Encerramento

- Reforçar: todo o ciclo (deploy, CI/CD, consumo, escalabilidade) foi demonstrado contra o ambiente real na AWS, não simulado localmente.
- Falar que ao final da gravação/avaliação o ambiente é destruído pela esteira, para não gerar custo indevido no AWS Academy Learner Lab.
- Encerrar com o link do repositório: <https://github.com/geoscabio/oficina_mecanica_api>.

## Checklist antes de gravar

- [ ] Ambiente AWS aplicado e saudável (`/api/health` = `Healthy`).
- [ ] Postman com environment **AWS Dev** configurado e testado (login funcionando).
- [ ] Terminal com `kubectl` configurado e testado (`kubectl get hpa` retornando dados).
- [ ] Janelas organizadas na tela antes de começar a gravar (Console AWS, terminal, Postman, Swagger).
- [ ] Cronômetro/rascunho de tempo por seção para não estourar os 15 minutos.

## Depois de gravar

- Subir o vídeo (YouTube não listado, Google Drive ou similar) e anotar o link.
- Preencher o link em [`docs/projeto/pdf-entrega.md`](pdf-entrega.md).
- Rodar o `terraform destroy` final (ver [`docs/deploy/aws-academy-guardrails.md`](../deploy/aws-academy-guardrails.md)).
