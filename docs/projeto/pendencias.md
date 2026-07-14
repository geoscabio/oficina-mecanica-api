# Pendências

## ADRs

- ADRs oficiais da Fase 1 trazidas para `docs/architecture/adrs/` (ADR-0001 a ADR-0004: Clean Architecture, .NET, SQL Server, JWT).
- Concluído: decisões arquiteturais da Fase 2 registradas em `docs/architecture/adrs/` (ADR-0005 a ADR-0013), numeradas na ordem cronológica real (conferida no histórico de commits, não na ordem de documentação): webhook de orçamento, RDS, topologia de rede AWS, orquestração Kubernetes/EKS + Terraform, Load Balancer via Kubernetes Service, pipeline CI/CD em estágios, controle de apply/destroy via arquivo versionado, state do Terraform e autoscaling HPA + runtime .NET.
- Cada ADR da Fase 2 deixa explícito o que é exigência literal do enunciado ([`docs/projeto/enunciado-fase-2-tech-challenge.pdf`](enunciado-fase-2-tech-challenge.pdf), agora versionado no repositório) e o que foi decisão adicional da equipe além do mínimo pedido.

## Diagramas

- `docs/architecture/diagrams/aws/` — concluído: diagrama combinado de infraestrutura e solução (VPC, EKS, RDS, NAT/Internet Gateway, Load Balancer, ECR, IAM Role), validado campo a campo contra o Terraform real.
- `docs/architecture/diagrams/ci-cd/ci-cd-pipeline.drawio` — concluído: fluxo CI → CD Development → CD Release → CD Production, desenhado manualmente, refletindo os jobs reais das esteiras (`ci.yml`, `cd-development.yml`, `cd-release.yml`, `cd-production.yml`).

O professor havia confirmado que diagramas separados de Kubernetes e Docker não são obrigatórios (o nível de componentes já está coberto pelo C4 Model, e a infraestrutura combinada AWS já cobre Kubernetes/Docker num único diagrama) — por isso ficam como opcionais, não bloqueiam a entrega.

**Backlog (opcional, pós-entrega):** diagramas de deployment local Kubernetes (`kubernetes-local`) e Docker (`docker-local`) — ainda não desenhados; não são exigência da Fase 2, ficam como material extra para quando houver tempo disponível.

## AWS

- Ambiente `development` destruído após a coleta das evidências de HPA e Postman (validado via log real da esteira: `Apply complete! Resources: 0 added, 0 changed, 26 destroyed.`).
- Reaplicar (`TERRAFORM_ACTION=apply`) quando for gravar o vídeo de demonstração, e destruir de novo ao final (destroy derradeiro da entrega).
- Evidenciar e executar de fato o deploy lógico nos ambientes `homologation` (branch `release`) e `production` (branch `main`) — ainda não validamos esses dois estágios do Git Flow com uma execução real registrada, só o deploy real em `development`. Confirmado em auditoria (2026-07-13): `release` está 155 commits atrás de `develop` e `main` igual a `release` — a PR automática `develop → release` abre e fecha sozinha a cada novo commit sem nunca ter sido mergeada. Próximo passo: mergear a próxima PR automática que abrir (ou disparar uma nova) para gerar a primeira execução real do `CD Release`, depois mergear `release → main` para `CD Production`.

## Evolução pós-entrega

- Mover migrations e seed inicial do startup da API para um Kubernetes Job versionado.
- Como alternativa intermediária, proteger migrations concorrentes com lock distribuído no SQL Server, por exemplo `sp_getapplock`.
- Evoluir o ambiente AWS para Multi-AZ, backups, snapshot final e rotação formal de segredos quando sair do contexto AWS Academy.
- Trocar o hostname bruto do Load Balancer (ex: `a8bceeb...elb.amazonaws.com`) por um DNS amigável via domínio próprio + Route 53 (ex: `api.oficinamecanica.com`). Não fizemos agora porque exige domínio comprado e, como o ambiente é destruído/recriado a cada demo, o registro DNS precisaria ser atualizado automaticamente a cada apply (mais uma peça de infra pra manter).

## Revisão final (depois de tudo o resto pronto)

- Auditoria geral final concluída nesta sessão: sem links quebrados, sem âncoras órfãs, sem referência a nomenclatura antiga (`oficina-api`), sem linguagem de avaliação vazada nos arquivos versionados.

## Entrega

- Roteiro do vídeo rascunhado: [`docs/projeto/roteiro-video.md`](roteiro-video.md).
- Estrutura do PDF final pronta (só faltam vídeo/diagramas/integrantes): [`docs/projeto/pdf-entrega.md`](pdf-entrega.md).
- Só restam: diagramas finais, gravar demonstração e gerar o PDF.
