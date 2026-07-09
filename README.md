# 🔧 Oficina Mecânica API

API REST para atendimento, execução e acompanhamento de ordens de serviço em uma oficina mecânica.

Projeto desenvolvido para o **Tech Challenge - Fase 2 da Pós Tech FIAP em Arquitetura de Software**, com foco em Clean Architecture, modelagem de domínio, execução containerizada, Kubernetes, AWS e rastreabilidade de qualidade via CI/CD.

---

## 📌 Índice

- [✨ Visão geral](#visao-geral)
- [✅ Funcionalidades](#funcionalidades)
- [🏗️ Arquitetura](#arquitetura)
- [🧭 Documentação e diagramas](#documentacao-e-diagramas)
- [🧰 Tecnologias](#tecnologias)
- [📁 Estrutura do repositório](#estrutura-do-repositorio)
- [🚀 Execução local com Docker Compose](#execucao-local-docker-compose)
- [☸️ Execução local com Kubernetes](#execucao-local-kubernetes)
- [☁️ Deploy AWS](#deploy-aws)
- [🔁 CI/CD](#cicd)
- [🧪 Testes e qualidade](#testes-e-qualidade)
- [🔐 Autenticação](#autenticacao)
- [📚 Swagger, OpenAPI e collection](#swagger-openapi-collection)
- [🗄️ Banco de dados e seed](#banco-de-dados-e-seed)
- [🎬 Entrega final](#entrega-final)
- [📝 Observações](#observacoes)

---

<a id="visao-geral"></a>

## ✨ Visão geral

A solução simula um sistema integrado para uma oficina mecânica, cobrindo o fluxo desde o atendimento inicial até a entrega do veículo ao cliente.

| Perfil | O que faz |
| --- | --- |
| 👩‍💼 **Atendente** | Cadastra clientes e veículos, abre ordens de serviço, acompanha orçamento e registra entrega. |
| 👨‍🔧 **Mecânico** | Inicia diagnóstico, define serviços, reserva peças, executa e finaliza serviços. |
| 👤 **Cliente** | Consulta publicamente o status da ordem de serviço. |
| 🛠️ **Administrador** | Opera cadastros administrativos e apoia a gestão da oficina. |

---

<a id="funcionalidades"></a>

## ✅ Funcionalidades

| Área | Funcionalidades principais |
| --- | --- |
| 🔐 **Identidade** | Login, geração de JWT e autorização por perfil. |
| 🤝 **Atendimento** | Cadastro e consulta de clientes e veículos. |
| 🧾 **Administrativo** | Cadastro de mecânicos, serviços de catálogo e peças/insumos. |
| 📦 **Estoque** | Entrada, consulta, reserva, baixa e estorno de itens. |
| 🧰 **Ordem de Serviço** | Abertura, diagnóstico, orçamento, aprovação, execução, finalização, entrega, cancelamento e status. |
| 📊 **Indicadores** | Consulta de tempo médio de execução de serviços. |
| ❤️ **Healthcheck** | Endpoint `/api/health` usado por Docker, Kubernetes e infraestrutura. |

### Fluxo principal

1. Cliente solicita atendimento.
2. Atendente identifica ou cadastra cliente e veículo.
3. Atendente abre a ordem de serviço.
4. Mecânico inicia diagnóstico.
5. Mecânico define serviços e reserva peças/insumos.
6. Sistema calcula orçamento.
7. Atendente envia orçamento por canal externo.
8. Cliente aprova ou recusa.
9. Mecânico executa e finaliza serviços.
10. Sistema baixa estoque reservado.
11. Atendente entrega o veículo.

Fluxos alternativos como estoque insuficiente, reprovação de orçamento, cancelamento com estorno e conflito de atualização também foram tratados.

---

<a id="arquitetura"></a>

## 🏗️ Arquitetura

O projeto adota **Clean Architecture** em um **monólito modular**, preservando o domínio de detalhes externos como HTTP, Swagger, banco de dados e infraestrutura.

| Camada | Projeto | Responsabilidade |
| --- | --- | --- |
| 🌐 **API** | `OficinaMecanica.API` | Controllers, Swagger, autenticação, autorização, middlewares e healthcheck. |
| 🧠 **Application** | `OficinaMecanica.Application` | Use cases, DTOs, validações, mapeamentos e orquestração dos fluxos. |
| 💎 **Domain** | `OficinaMecanica.Domain` | Agregados, entidades, value objects, enums, regras de negócio e contratos. |
| 🧱 **Infrastructure** | `OficinaMecanica.Infrastructure` | EF Core, SQL Server, repositories, migrations, seed e serviços JWT. |

### Decisões aplicadas

| Decisão | Aplicação prática |
| --- | --- |
| **Monólito modular** | Deploy único com organização por contextos: Administrativo, Atendimento, Estoque e Ordem de Serviço. |
| **Clean Architecture** | Dependências apontam para dentro; domínio não depende de API ou Infrastructure. |
| **DDD tático** | Uso de agregados, entidades, value objects, enums e regras no domínio. |
| **Use Cases** | Regras de aplicação centralizadas na camada Application. |
| **Repository Pattern** | Contratos no domínio e implementação na infraestrutura. |
| **JWT + perfis** | Segurança baseada em autenticação Bearer e autorização por papel. |
| **Resiliência de dados** | RowVersion/concurrency tokens para evitar sobrescrita silenciosa. |

---

<a id="documentacao-e-diagramas"></a>

## 🧭 Documentação e diagramas

| Item | Caminho |
| --- | --- |
| 🧩 C4 Model oficial validado | [`docs/architecture/diagrams/c4-model`](docs/architecture/diagrams/c4-model) |
| ☁️ Diagramas AWS | [`docs/architecture/diagrams/aws`](docs/architecture/diagrams/aws) |
| ☸️ Diagramas Kubernetes | [`docs/architecture/diagrams/deployment/kubernetes`](docs/architecture/diagrams/deployment/kubernetes) |
| 🐳 Diagramas Docker | [`docs/architecture/diagrams/deployment/docker`](docs/architecture/diagrams/deployment/docker) |
| 🔁 Diagramas CI/CD | [`docs/architecture/diagrams/ci-cd`](docs/architecture/diagrams/ci-cd) |
| 📄 Evidências de qualidade | [`docs/evidencias`](docs/evidencias) |
| 🚀 Guias de deploy | [`docs/deploy`](docs/deploy) |
| 📌 Gestão do projeto | [`docs/projeto`](docs/projeto) |

> Os diagramas C4 já estão finalizados e validados. Os diretórios AWS, Kubernetes, Docker e CI/CD foram reservados para receber os arquivos finais da entrega.

---

<a id="tecnologias"></a>

## 🧰 Tecnologias

| Categoria | Tecnologias |
| --- | --- |
| Linguagem e plataforma | C#, .NET 10, ASP.NET Core Web API |
| Banco de dados | SQL Server 2022 |
| ORM | Entity Framework Core |
| API docs | Swagger/OpenAPI com Swashbuckle |
| Segurança | JWT Bearer, autorização por perfis e headers HTTP de segurança |
| Validação e mapeamento | FluentValidation e AutoMapper |
| Testes | xUnit, FluentAssertions, Moq, Testcontainers, Respawn e Coverlet |
| DevOps | Docker, Docker Compose, Kubernetes, GitHub Actions e GHCR |
| AWS | Terraform, ECR, EKS, RDS, VPC e Load Balancer |
| Qualidade | `dotnet format`, cobertura mínima, SonarQube e OWASP ZAP |

---

<a id="estrutura-do-repositorio"></a>

## 📁 Estrutura do repositório

```text
.
├── .github/workflows/              # Esteira CI/CD
├── docs/
│   ├── architecture/diagrams/       # Diagramas versionados
│   ├── deploy/                      # Guias operacionais de deploy
│   ├── evidencias/                  # Evidências de build, testes, segurança e infra
│   ├── openapi/                     # OpenAPI JSON exportado
│   └── projeto/                     # Backlog, decisões e pendências
├── infra/
│   ├── terraform/environments/dev/  # Terraform do ambiente AWS
│   ├── aws/k8s/                     # Manifests Kubernetes para EKS
│   └── terraform/modules/           # Módulos Terraform
├── k8s/                             # Manifests Kubernetes locais
├── src/                             # Código de produção
├── tests/                           # Testes unitários e integração
├── docker-compose.yml
├── Dockerfile
└── OficinaMecanica.sln
```

---

<a id="execucao-local-docker-compose"></a>

## 🚀 Execução local com Docker Compose

### Pré-requisitos

- Docker Desktop em execução.
- .NET SDK 10 instalado para comandos locais fora do container.

### Subir ambiente local

```powershell
Copy-Item .env.example .env
docker compose up -d --build
```

O Docker Compose sobe:

- SQL Server em `localhost,14333`;
- API em `http://localhost:5093`;
- migrations e seed demo automaticamente.

### Validar API

```powershell
curl.exe http://localhost:5093/api/health
```

Resposta esperada:

```text
Healthy
```

### Acessos locais

| Recurso | URL |
| --- | --- |
| Swagger | `http://localhost:5093/swagger` |
| Healthcheck | `http://localhost:5093/api/health` |
| OpenAPI JSON | `http://localhost:5093/swagger/v1/swagger.json` |

### Parar ambiente

```powershell
docker compose down
```

Para recriar banco/volume do zero:

```powershell
docker compose down -v
docker compose up -d --build
```

---

<a id="execucao-local-kubernetes"></a>

## ☸️ Execução local com Kubernetes

### Pré-requisitos

- Docker Desktop com Kubernetes habilitado.
- `kubectl` apontando para o contexto local.

### Aplicar manifests

```powershell
kubectl apply -R -f k8s/
kubectl rollout status deployment/sqlserver -n oficina --timeout=180s
kubectl rollout status deployment/oficina-api -n oficina --timeout=180s
```

### Acessar API por port-forward

```powershell
kubectl port-forward service/oficina-api 5093:8080 -n oficina
```

Depois acesse:

```text
http://localhost:5093/swagger
http://localhost:5093/api/health
```

### Validar recursos

```powershell
kubectl get all,hpa,ingress -n oficina
kubectl describe hpa oficina-api-hpa -n oficina
```

---

<a id="deploy-aws"></a>

## ☁️ Deploy AWS

A infraestrutura é provisionada por Terraform e a aplicação é entregue automaticamente pela esteira Git Flow quando o ambiente de deploy está habilitado.

> Regra operacional: qualquer recurso criado em ambiente temporário precisa ter cleanup Kubernetes e `terraform destroy` planejados após a demonstração.

### Fluxo operacional

1. Provisionar a infraestrutura com Terraform em `infra/terraform/environments/dev`.
2. Configurar secrets e variables nos GitHub Environments.
3. Habilitar deploy com `AWS_DEPLOY_ENABLED=true`.
4. Integrar feature em `develop`.
5. A esteira faz deploy em `development` e abre PR automático para `release`.
6. O merge em `release` faz deploy em `homologation` e abre PR automático para `main`.
7. O merge em `main` exige revisão/proteção e dispara o deploy em `production`.
8. Ao final da demonstração, executar cleanup Kubernetes e `terraform destroy`.

Guias:

- [`docs/deploy/aws-academy-guardrails.md`](docs/deploy/aws-academy-guardrails.md)
- [`docs/deploy/deploy-aws.md`](docs/deploy/deploy-aws.md)
- [`docs/deploy/github-actions.md`](docs/deploy/github-actions.md)

---

<a id="cicd"></a>

## 🔁 CI/CD

O workflow principal fica em [`/.github/workflows/ci-cd.yml`](.github/workflows/ci-cd.yml).

### Quando roda automaticamente

| Evento | O que acontece |
| --- | --- |
| `push` em `feature/**`, `bugfix/**`, `hotfix/**`, `fix/**`, `refactor/**`, `chore/**`, `docs/**`, `test/**` ou `ci/**` | Build, format, testes, cobertura, build da imagem Docker, dry-run Kubernetes e abertura automática de PR para `develop`. |
| `pull_request` para `develop`, `release` ou `main` | Validação completa antes do merge manual. |
| `push` em `develop` | Validação completa, deploy automático em `development` quando habilitado e abertura automática de PR para `release`. |
| `push` em `release` ou `release/**` | Validação completa, deploy automático em `homologation` quando habilitado e abertura automática de PR para `main`. |
| `push` em `main` | Validação completa e deploy em `production`, protegido por environment/branch protection. |
| `workflow_dispatch` | Execução manual complementar de CI ou cleanup Kubernetes. |

### Bloqueios de qualidade

A esteira falha se:

- algum teste falhar;
- algum teste for ignorado durante a coleta de cobertura;
- a cobertura global de linhas ficar abaixo de **90%**;
- `dotnet format --verify-no-changes` detectar formatação pendente;
- manifests Kubernetes não passarem no dry-run.

### Fluxo Git Flow automatizado

```text
feature/* -> PR develop -> deploy development -> PR release -> deploy homologation -> PR main -> deploy production
```

O merge continua manual via PR e revisão. A automação só promove o próximo PR depois que a validação e o deploy do estágio anterior passam.

Para evitar deploy acidental, os jobs AWS só executam quando `AWS_DEPLOY_ENABLED=true` estiver configurado no repositório. O cleanup Kubernetes fica disponível por `workflow_dispatch` com `operation=cleanup-kubernetes`.

---

<a id="testes-e-qualidade"></a>

## 🧪 Testes e qualidade

### Comandos principais

```powershell
dotnet build OficinaMecanica.sln --no-restore
dotnet format OficinaMecanica.sln --verify-no-changes --no-restore
dotnet test OficinaMecanica.sln --no-build
```

### Cobertura

```powershell
dotnet test OficinaMecanica.sln `
  --configuration Release `
  --no-build `
  --collect:"XPlat Code Coverage" `
  --settings tests/sonarqube.runsettings `
  --results-directory TestResults `
  --logger "trx"
```

Evidência atual:

- **424 testes aprovados**
- **0 testes ignorados**
- **91.2% de cobertura global de linhas**
- CI configurado para bloquear cobertura abaixo de **90%**

Arquivos de evidência:

| Evidência | Caminho |
| --- | --- |
| Build, testes e cobertura | [`docs/evidencias/build-test.md`](docs/evidencias/build-test.md) |
| SonarQube | [`docs/evidencias/sonarqube.md`](docs/evidencias/sonarqube.md) |
| OWASP ZAP | [`docs/evidencias/owasp-zap.md`](docs/evidencias/owasp-zap.md) |
| Kubernetes HPA | [`docs/evidencias/kubernetes-hpa.md`](docs/evidencias/kubernetes-hpa.md) |
| Terraform apply/destroy | [`docs/evidencias/terraform-apply.md`](docs/evidencias/terraform-apply.md) |

---

<a id="autenticacao"></a>

## 🔐 Autenticação

Endpoint:

```http
POST /api/v1/identidade/autenticacao/login
```

Usuários demo:

| Perfil | Login | Senha |
| --- | --- | --- |
| Administrador | `admin` | `admin123` |
| Atendente | `atendente` | `atendente123` |
| Mecânico | `mecanico` | `mecanico123` |

No Swagger, clique em **Authorize** e informe:

```text
Bearer <token>
```

Consulta pública de status não exige autenticação.

---

<a id="swagger-openapi-collection"></a>

## 📚 Swagger, OpenAPI e collection

| Item | Caminho |
| --- | --- |
| Swagger local | `http://localhost:5093/swagger` |
| OpenAPI local | `http://localhost:5093/swagger/v1/swagger.json` |
| OpenAPI versionado | [`docs/openapi/oficina-mecanica-openapi.json`](docs/openapi/oficina-mecanica-openapi.json) |

O arquivo OpenAPI versionado pode ser importado como collection no Postman, Insomnia ou Bruno.

---

<a id="banco-de-dados-e-seed"></a>

## 🗄️ Banco de dados e seed

O projeto usa SQL Server com Entity Framework Core.

Configurações relevantes:

| Configuração | Função |
| --- | --- |
| `Database:ApplyMigrationsOnStartup` | Aplica migrations ao iniciar a API. |
| `Database:SeedDemoData` | Carrega dados demo para avaliação local. |

No Docker Compose local, o SQL Server fica disponível em:

```text
localhost,14333
```

Credenciais locais ficam no arquivo `.env`, criado a partir de `.env.example`.

---

<a id="entrega-final"></a>

## 🎬 Entrega final

Itens técnicos já estruturados:

- ✅ API com fluxo principal da oficina.
- ✅ Docker Compose local.
- ✅ Kubernetes local.
- ✅ Terraform para AWS.
- ✅ Healthcheck `/api/health`.
- ✅ CI/CD com cobertura mínima de 90%.
- ✅ Evidências versionadas.
- ✅ OpenAPI versionado.
- ✅ C4 Model oficial versionado.

Itens manuais restantes:

- ⏳ Colar prints reais de SonarQube, OWASP ZAP, HPA e Terraform.
- ⏳ Finalizar diagramas AWS, Kubernetes, Docker e CI/CD.
- ⏳ Executar demonstração AWS com `destroy` ao final.
- ⏳ Gravar vídeo.
- ⏳ Montar PDF final.

---

<a id="observacoes"></a>

## 📝 Observações

- Não versionar credenciais, tokens, kubeconfig, secrets ou outputs sensíveis.
- Ambientes AWS temporários devem ser destruídos após a demonstração.
- O deploy para `main` deve usar branch protection e environment `production` com aprovação obrigatória.
- O projeto prioriza rastreabilidade e simplicidade operacional para a banca avaliar sem depender de contexto externo.
