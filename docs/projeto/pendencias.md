# Pendências

## Evidências manuais

- Colar print do SonarQube Quality Gate em `docs/evidencias/sonarqube.md`.
- Colar resultado real do OWASP ZAP em `docs/evidencias/owasp-zap.md`.

## ADRs

- ADRs oficiais da Fase 1 trazidas para `docs/architecture/adrs/` (ADR-0001 a ADR-0004: Clean Architecture, .NET, SQL Server, JWT).
- Revisar se falta registrar alguma decisão arquitetural tomada só na Fase 2 (ex.: EKS/Terraform, HPA, escolha de GC) como ADR nova.

## Diagramas ainda aguardando arquivos finais

- `docs/architecture/diagrams/aws/`
- `docs/architecture/diagrams/deployment/kubernetes/`
- `docs/architecture/diagrams/deployment/docker/`
- `docs/architecture/diagrams/ci-cd/`

## AWS

- Validar o `terraform apply` executado pela esteira em `develop`.
- Validar o deploy automático real a partir de `develop` e os deploys lógicos em `release` e `main`.
- Demonstrar `/api/health` e Swagger no endpoint publicado.
- Executar `terraform destroy` com o mesmo backend/state da esteira.
- Conferir que não restaram EKS, EC2, RDS, NAT Gateway ou Load Balancer ativos.

## Evolução pós-entrega

- Mover migrations e seed inicial do startup da API para um Kubernetes Job versionado.
- Como alternativa intermediária, proteger migrations concorrentes com lock distribuído no SQL Server, por exemplo `sp_getapplock`.
- Evoluir o ambiente AWS para Multi-AZ, backups, snapshot final e rotação formal de segredos quando sair do contexto AWS Academy.
- Trocar o hostname bruto do Load Balancer (ex: `a8bceeb...elb.amazonaws.com`) por um DNS amigável via domínio próprio + Route 53 (ex: `api.oficinamecanica.com`). Não fizemos agora porque exige domínio comprado e, como o ambiente é destruído/recriado a cada demo, o registro DNS precisaria ser atualizado automaticamente a cada apply (mais uma peça de infra pra manter).

## Revisão final (depois de tudo o resto pronto)

- Varredura completa no projeto procurando palavras/frases sem acentuação gráfica correta (código, docs, README, evidências).
- Auditoria geral final de todo o projeto antes da entrega.

## Entrega

- Montar roteiro do vídeo.
- Gravar demonstração.
- Gerar PDF final.
