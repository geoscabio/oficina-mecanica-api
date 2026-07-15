# Pendências

## AWS

- Ambiente `development` destruído com sucesso após a gravação do vídeo de demonstração (`terraform destroy` real, via esteira `CD Development`).
- Deploy lógico nos ambientes `homologation` (branch `release`) e `production` (branch `main`) — a `main` é a branch usada para a avaliação da Fase 2. Próximo passo: mergear o PR automático `develop → release` (dispara `CD Release`), depois mergear o PR automático `release → main` que se abre em seguida (dispara `CD Production`).

## Evolução pós-entrega

- Mover migrations e seed inicial do startup da API para um Kubernetes Job versionado.
- Como alternativa intermediária, proteger migrations concorrentes com lock distribuído no SQL Server, por exemplo `sp_getapplock`.
- Evoluir o ambiente AWS para Multi-AZ, backups, snapshot final e rotação formal de segredos quando sair do contexto AWS Academy.
- Trocar o hostname bruto do Load Balancer (ex: `a8bceeb...elb.amazonaws.com`) por um DNS amigável via domínio próprio + Route 53 (ex: `api.oficinamecanica.com`). Não fizemos agora porque exige domínio comprado e, como o ambiente é destruído/recriado a cada demo, o registro DNS precisaria ser atualizado automaticamente a cada apply (mais uma peça de infra pra manter).
