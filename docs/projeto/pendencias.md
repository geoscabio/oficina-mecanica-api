# Pendências

## Evidências manuais

- Colar print do SonarQube Quality Gate em `docs/evidencias/sonarqube.md`.
- Colar resultado real do OWASP ZAP em `docs/evidencias/owasp-zap.md`.
- Colar print do HPA durante carga em `docs/evidencias/kubernetes-hpa.md`.
- Colar outputs reais de Terraform em `docs/evidencias/terraform-apply.md`.

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

## Entrega

- Montar roteiro do vídeo.
- Gravar demonstração.
- Gerar PDF final.
