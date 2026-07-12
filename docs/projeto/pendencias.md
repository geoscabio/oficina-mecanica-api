# Pendências

## ADRs

- ADRs oficiais da Fase 1 trazidas para `docs/architecture/adrs/` (ADR-0001 a ADR-0004: Clean Architecture, .NET, SQL Server, JWT).
- Revisar se falta registrar alguma decisão arquitetural tomada só na Fase 2 (ex.: EKS/Terraform, HPA, escolha de GC) como ADR nova.

## Diagramas ainda aguardando arquivos finais

- `docs/architecture/diagrams/aws/`
- `docs/architecture/diagrams/deployment/kubernetes/`
- `docs/architecture/diagrams/deployment/docker/`
- `docs/architecture/diagrams/ci-cd/`

## AWS

- Ambiente `development` destruído após a coleta das evidências de HPA e Postman (validado via log real da esteira: `Apply complete! Resources: 0 added, 0 changed, 26 destroyed.`).
- Reaplicar (`TERRAFORM_ACTION=apply`) quando for gravar o vídeo de demonstração, e destruir de novo ao final (destroy derradeiro da entrega).

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
