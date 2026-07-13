# Estrutura do PDF final de entrega

Rascunho do conteúdo a ser transposto para o PDF final entregue na plataforma da FIAP. Preencher os itens marcados como `[PENDENTE]` e depois exportar como PDF (Word/Google Docs → Download PDF, ou similar).

---

## 1. Capa

- Título: Tech Challenge — Fase 2 — Arquitetura de Software
- Projeto: Oficina Mecânica API
- Integrantes: `[PENDENTE — nomes e RM]`
- Turma: `[PENDENTE]`
- Data: `[PENDENTE]`

## 2. Link do repositório

<https://github.com/geoscabio/oficina_mecanica_api>

Repositório público, com todo o histórico de commits, Pull Requests e esteiras de CI/CD reais.

## 3. Diagrama de arquitetura

`[PENDENTE — inserir a imagem final aqui]`

Fontes versionadas no repositório:

- C4 Model (Contexto, Containers, Componentes): [`docs/architecture/diagrams/c4-model/`](../architecture/diagrams/c4-model/)
- Diagrama de infraestrutura e solução AWS (VPC, EKS, RDS, Kubernetes, Docker): [`docs/architecture/diagrams/aws/`](../architecture/diagrams/aws/)
- Diagrama do fluxo de deploy CI/CD: `docs/architecture/diagrams/ci-cd/` `[PENDENTE]`

## 4. Link do vídeo de demonstração

`[PENDENTE — link do vídeo, máx. 15 minutos]`

Roteiro usado na gravação: [`docs/projeto/roteiro-video.md`](roteiro-video.md).

O vídeo cobre: deploy real na AWS, execução da esteira CI/CD, consumo da API (Swagger + Postman) e escalabilidade automática via HPA sob carga real gerada pelo Postman Performance Test.

## 5. Resumo técnico

| Item | Detalhe |
| --- | --- |
| Linguagem/Framework | C# / .NET 10 |
| Arquitetura | Clean Architecture, monólito modularizado por bounded contexts (DDD) |
| Bounded Contexts | Identidade, Atendimento, Administrativo, Gestão de Estoque, Gestão de Ordem de Serviço |
| Banco de dados | SQL Server (local: container Docker; AWS: Amazon RDS) |
| Autenticação | JWT com perfis de acesso (roles) |
| Infraestrutura como código | Terraform (VPC, EKS, RDS, ECR, workload Kubernetes) |
| Orquestração | Kubernetes (Amazon EKS), com HorizontalPodAutoscaler (1 a 5 réplicas) |
| CI/CD | GitHub Actions (Git Flow: `develop` → `release` → `main`) |
| Qualidade | SonarQube (Quality Gate, 0 bugs/vulnerabilidades/code smells), OWASP ZAP (0 falhas) |
| Testes | 424 testes automatizados (unitários + integração), 87,4% de cobertura |
| Collection de API | [`docs/postman/`](../postman/) — importada diretamente do `swagger.json` real |

## 6. ADRs (Architecture Decision Records)

Registradas em [`docs/architecture/adrs/`](../architecture/adrs/):

- ADR-0001 — Clean Architecture em monólito
- ADR-0002 — Linguagem e framework (.NET)
- ADR-0003 — Banco de dados relacional (SQL Server)
- ADR-0004 — Autenticação e autorização (JWT)

## 7. Evidências técnicas

Todas em [`docs/evidencias/`](../evidencias/), com resultados reais (não simulados):

- Terraform apply/destroy: [`terraform-apply.md`](../evidencias/terraform-apply.md)
- Kubernetes HPA (autoscaling): [`kubernetes-hpa.md`](../evidencias/kubernetes-hpa.md)
- Postman: [`postman.md`](../evidencias/postman.md)
- SonarQube: [`sonarqube.md`](../evidencias/sonarqube.md)
- OWASP ZAP: [`owasp-zap.md`](../evidencias/owasp-zap.md)
- Build e testes: [`build-test.md`](../evidencias/build-test.md)

---

## Checklist final antes de exportar o PDF

- [ ] Todos os `[PENDENTE]` acima preenchidos.
- [ ] Diagramas finais inseridos (imagens, não só links).
- [ ] Link do vídeo testado (abre sem permissão especial).
- [ ] Link do repositório testado (público, acessível).
- [ ] `terraform destroy` final executado e confirmado antes da entrega.
