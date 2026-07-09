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

## Entrega

- Montar roteiro do vídeo.
- Gravar demonstração.
- Gerar PDF final.
