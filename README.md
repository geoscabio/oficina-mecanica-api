# 🔧 Oficina Mecânica API

API REST para atendimento, execução e acompanhamento de ordens de serviço em uma oficina mecânica.

Projeto desenvolvido para o **Tech Challenge da Pós Tech FIAP - Arquitetura de Software**, com foco em modelagem de domínio, Clean Architecture, execução local simples e validação de um fluxo completo via API.

---

## 📚 Sumário

- [Sobre o projeto](#-sobre-o-projeto)
- [Funcionalidades](#-funcionalidades)
- [Arquitetura](#-arquitetura)
- [Tecnologias](#-tecnologias)
- [Estrutura do repositório](#-estrutura-do-repositório)
- [Como executar localmente](#-como-executar-localmente)
- [Como acessar o banco local](#-como-acessar-o-banco-local)
- [Autenticação](#-autenticação)
- [Como testar a API](#-como-testar-a-api)
- [Testes automatizados](#-testes-automatizados)
- [Qualidade e evidências](#qualidade-e-evidências)
- [Banco de dados e seed](#-banco-de-dados-e-seed)
- [Documentação complementar](#-documentação-complementar)
- [Observações acadêmicas](#-observações-acadêmicas)
- [Licença](#-licença)

---

## 🎯 Sobre o projeto

A solução simula um sistema integrado para uma oficina mecânica, cobrindo o processo desde o atendimento inicial até a entrega do veículo ao cliente.

O sistema permite que diferentes perfis interajam com o fluxo da oficina:

| Perfil | O que faz no sistema |
| --- | --- |
| **Atendente** | Cadastra clientes e veículos, abre ordens de serviço, acompanha aprovações e registra entrega do veículo |
| **Mecânico** | Inicia diagnóstico, define serviços, reserva peças, executa e finaliza serviços |
| **Cliente** | Consulta o status da ordem de serviço via API |
| **Administrador** | Acessa os cadastros administrativos e apoia a operação do sistema |

---

## ✅ Funcionalidades

| Área | Funcionalidades principais |
| --- | --- |
| **Identidade** | Login, geração de token JWT e autorização por perfil |
| **Atendimento** | Cadastro e consulta de clientes e veículos |
| **Administrativo** | Cadastro de mecânicos, serviços do catálogo e peças/insumos |
| **Gestão de Estoque** | Entrada, consulta, reserva, baixa e estorno de itens |
| **Gestão de Ordem de Serviço** | Abertura, diagnóstico, orçamento, aprovação, execução, finalização, entrega, cancelamento, acompanhamento de status e consulta de tempo médio de execução dos serviços |

### Fluxo principal atendido

1. Cliente solicita atendimento.
2. Atendente identifica ou cadastra cliente e veículo.
3. Atendente abre a ordem de serviço.
4. Mecânico inicia o diagnóstico.
5. Mecânico define serviços e reserva peças/insumos.
6. Sistema calcula o orçamento.
7. Atendente envia o orçamento ao cliente por canal externo.
8. Atendente registra a aprovação e inicia a execução.
9. Mecânico executa e finaliza os serviços.
10. Sistema baixa o estoque reservado.
11. Atendente registra a entrega do veículo.

Fluxos alternativos, como estoque insuficiente, reprovação de orçamento e cancelamento com estorno de estoque, também foram considerados nas regras de negócio.

---

## 🏗️ Arquitetura

O projeto adota **Clean Architecture** em uma estrutura de **monólito modular**.

A ideia principal é manter o domínio protegido de detalhes externos, como HTTP, Swagger, banco de dados e autenticação.

| Camada | Projeto | Responsabilidade |
| --- | --- | --- |
| **API** | `OficinaMecanica.API` | Controllers, Swagger, autenticação JWT, autorização e middlewares |
| **Application** | `OficinaMecanica.Application` | Use Cases, DTOs, validações, mapeamentos e orquestração dos fluxos |
| **Domain** | `OficinaMecanica.Domain` | Agregados, entidades, value objects, enums, regras de negócio e contratos de repositories |
| **Infrastructure** | `OficinaMecanica.Infrastructure` | EF Core, SQL Server, repositories, migrations, seed e JWT services |

### Decisões aplicadas

| Decisão | Aplicação prática |
| --- | --- |
| **Monólito modular** | Deploy único, organizado internamente pelos contextos delimitados: **Administrativo, Atendimento, Gestão de Estoque e Gestão de Ordem de Serviço** |
| **Clean Architecture** | Separação entre domínio, aplicação, infraestrutura e API |
| **DDD tático** | Uso de contexto delimitado, agregados, entidades, value objects e regras no domínio |
| **Use Cases** | Casos de uso da aplicação centralizados na camada Application |
| **Repository Pattern** | Contratos separados das implementações de persistência |
| **JWT** | Autenticação e autorização por perfis |
| **Testes automatizados** | Cobertura de domínio, aplicação e API integrada |

---

## 🧰 Tecnologias

| Categoria | Tecnologias |
| --- | --- |
| Linguagem e plataforma | C#, .NET 10, ASP.NET Core Web API |
| Banco de dados | SQL Server 2022 |
| ORM | Entity Framework Core |
| Documentacao da API | Swagger/OpenAPI com Swashbuckle |
| Seguranca | JWT Bearer, autorizacao por perfis e headers HTTP de seguranca |
| Validacao e mapeamento | FluentValidation e AutoMapper |
| Testes automatizados | xUnit, FluentAssertions, Moq, Testcontainers, Respawn e Coverlet |
| Qualidade e evidencias | SonarQube, OWASP ZAP e `dotnet format` |
| Execucao local | Docker, Docker Compose, Kubernetes (Docker Desktop), Metrics Server, Horizontal Pod Autoscaler (HPA) e .NET SDK |

---

## 📁 Estrutura do repositório

```text
.
├── Dockerfile
├── docker-compose.yml
├── OficinaMecanica.sln
├── README.md
├── k8s
│   ├── api-configmap.yaml
│   ├── api-deployment.yaml
│   ├── api-hpa.yaml
│   ├── api-secret.yaml
│   ├── namespace.yaml
│   ├── sqlserver-deployment.yaml
│   ├── sqlserver-pvc.yaml
│   ├── sqlserver-secret.yaml
│   ├── sqlserver-service.yaml
└── src
│   ├── OficinaMecanica.API
│   ├── OficinaMecanica.Application
│   ├── OficinaMecanica.Domain
│   └── OficinaMecanica.Infrastructure
└── tests
    ├── OficinaMecanica.Domain.UnitTests
    ├── OficinaMecanica.Application.UnitTests
    └── OficinaMecanica.API.IntegrationTests
```

---

## 🚀 Como executar localmente

O fluxo recomendado para avaliação é executar a API e o banco de dados com **Docker Compose**.

### Pré-requisitos

| Item | Obrigatório para Docker Compose? | Obrigatório para build/testes locais? | Observação |
| --- | --- | --- | --- |
| Docker Desktop | Sim | Sim, para testes integrados | Necessário para subir API, SQL Server e containers de teste |
| .NET SDK 10 | Não | Sim | Necessário para `dotnet build`, `dotnet test`, EF Core e SonarScanner |
| Cliente SQL | Opcional | Opcional | SSMS no Windows, ou Azure Data Studio, DBeaver e `sqlcmd` no Linux/macOS |
| Porta `5093` livre | Sim | Não | Porta da API |
| Porta `14333` livre | Sim | Não | Porta local do SQL Server |

### 1. Subir API e banco

Na raiz do repositório, execute um dos comandos abaixo conforme seu sistema operacional. O comando cria o arquivo `.env` para você a partir do `.env.example`; não é necessário criar o arquivo manualmente.

Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

Bash, macOS ou Linux:

```bash
cp .env.example .env
```

O Docker Compose lê o arquivo `.env` automaticamente. Não é necessário criar o arquivo na mão. Se quiser trocar a senha do SQL Server ou o segredo JWT, edite os valores do `.env` antes de subir os containers.

Depois execute. Este comando é igual no Windows, macOS e Linux:

```bash
docker compose up --build
```

Ao iniciar, a aplicação:

- sobe o SQL Server em container;
- sobe a API em container;
- aguarda o banco ficar saudável;
- aplica as migrations automaticamente;
- carrega dados de demonstração;
- disponibiliza o Swagger.

### 2. Acessar a API

| Recurso | Endereço |
| --- | --- |
| Swagger | `http://localhost:5093/swagger` |
| API | `http://localhost:5093` |

### 3. Parar ou recriar o ambiente

| Objetivo | Comando |
| --- | --- |
| Parar containers sem apagar o banco | `docker compose down` |
| Parar containers e apagar o volume do banco | `docker compose down -v` |
| Subir novamente do zero | repetir o comando de subida |

---

## ☸️ Execução com Kubernetes

Além da execução via Docker Compose, o projeto também pode ser executado utilizando o Kubernetes do Docker Desktop por meio dos manifestos disponíveis na pasta `k8s`.

### Recursos implementados

- Namespace dedicado para a aplicação.
- Deployment da API.
- Deployment do SQL Server.
- Services para comunicação entre os Pods.
- ConfigMap para configurações da aplicação.
- Secrets para informações sensíveis.
- PersistentVolumeClaim para persistência dos dados do SQL Server.
- Horizontal Pod Autoscaler (HPA) baseado em utilização de CPU.

### Pré-requisitos

- Docker Desktop com Kubernetes habilitado.
- `kubectl` configurado para acessar o cluster local.
- Metrics Server instalado.

### Aplicação dos manifestos

```bash
kubectl apply -f k8s/
```

### Validação

```bash
kubectl get pods -n oficina
kubectl get svc -n oficina
kubectl get pvc -n oficina
kubectl get hpa -n oficina
```

> **Observação:** durante o desenvolvimento com Docker Desktop pode ser necessário configurar o Metrics Server com o parâmetro `--kubelet-insecure-tls`, devido às características dos certificados TLS do ambiente local.

---

## 🗄️ Como acessar o banco local

### Windows com SSMS

Com os containers em execução, abra o **SQL Server Management Studio** e preencha a janela **Connect to Server** assim:

| Campo no SSMS | Valor |
| --- | --- |
| Server type | `Database Engine` |
| Server name | `localhost,14333` |
| Authentication | `SQL Server Authentication` |
| Login | `sa` |
| Password | Valor definido em `OFICINA_SQL_SA_PASSWORD` no arquivo `.env` |
| Trust server certificate | Marcado |

Depois de conectar:

1. Abra o painel **Object Explorer**.
2. Expanda **Databases**.
3. Selecione o banco **OficinaMecanicaDb**.
4. Expanda **Tables** para visualizar as tabelas criadas pelas migrations.

### Linux, macOS ou alternativa ao SSMS

Se não estiver no Windows, pode usar **Azure Data Studio**, **DBeaver** ou `sqlcmd` com os mesmos dados de conexão:

| Campo | Valor |
| --- | --- |
| Host | `localhost` |
| Porta | `14333` |
| Usuario | `sa` |
| Senha | Valor definido em `OFICINA_SQL_SA_PASSWORD` no arquivo `.env` |
| Banco | `OficinaMecanicaDb` |

Exemplo com `sqlcmd`:

```bash
sqlcmd -S localhost,14333 -U sa -P "<valor-de-OFICINA_SQL_SA_PASSWORD-no-.env>" -C -Q "SELECT name FROM sys.databases"
```

---

## 🔐 Autenticação

A autenticação é feita pelo endpoint:

```http
POST /api/v1/identidade/autenticacao/login
```

Use um dos usuários de demonstração abaixo para obter um token JWT.

| Perfil | Login | Senha | Uso principal |
| --- | --- | --- | --- |
| Administrador | `admin` | `admin123` | Acesso completo aos fluxos protegidos |
| Atendente | `atendente` | `atendente123` | Atendimento, clientes, veículos, abertura/andamento da OS e estoque |
| Mecânico | `mecanico` | `mecanico123` | Diagnóstico, definição de serviços, reserva, execução e finalização |
| Cliente | `cliente` | `cliente123` | Consulta de status da ordem de serviço |

### Autorizar no Swagger

1. Acesse `http://localhost:5093/swagger`.
2. Execute o endpoint de login.
3. Copie o valor retornado no campo `token`.
4. Clique em **Authorize**.
5. Cole apenas o token JWT, sem escrever `Bearer` manualmente.
6. Confirme em **Authorize**.

---

## 🧪 Como testar a API

O Swagger apresenta os contratos atualizados dos endpoints e deve ser usado para testes manuais rápidos.

### Fluxo feliz principal

<details>
<summary>Ver sequência completa de uso da API</summary>

| Ordem | Ação | Endpoint |
| --- | --- | --- |
| 1 | Autenticar usuário | `POST /api/v1/identidade/autenticacao/login` |
| 2 | Cadastrar cliente | `POST /api/v1/atendimento/clientes/cadastrar` |
| 3 | Cadastrar veículo | `POST /api/v1/atendimento/veiculos/cadastrar` |
| 4 | Cadastrar mecânico | `POST /api/v1/administrativo/mecanicos/cadastrar` |
| 5 | Cadastrar serviço do catálogo | `POST /api/v1/administrativo/servicos-catalogo/cadastrar` |
| 6 | Cadastrar peça/insumo do catálogo | `POST /api/v1/administrativo/pecas-insumos-catalogo/cadastrar` |
| 7 | Registrar entrada no estoque | `POST /api/v1/gestao-estoque/estoque/registrar-entrada` |
| 8 | Abrir ordem de serviço | `POST /api/v1/gestao-ordem-servico/ordens-servico/cadastrar` |
| 9 | Consultar status da OS | `GET /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/consultar-status` |
| 10 | Iniciar diagnóstico | `POST /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/iniciar-diagnostico` |
| 11 | Definir serviços | `PUT /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/definir-servicos` |
| 12 | Reservar peças e insumos | `POST /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/reservar-pecas-insumos` |
| 13 | Aguardar aprovação | `POST /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/aguardar-aprovacao` |
| 14 | Iniciar execução da OS | `POST /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/iniciar-execucao` |
| 15 | Iniciar execução do serviço | `POST /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/servicos/{servicoId}/iniciar-execucao` |
| 16 | Finalizar serviço | `POST /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/servicos/{servicoId}/finalizar` |
| 17 | Finalizar OS | `POST /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/finalizar` |
| 18 | Entregar veículo | `POST /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/entregar` |
| 19 | Consultar tempo médio | `GET /api/v1/gestao-ordem-servico/tempo-medio-servicos/consultar/{servicoCatalogoId}` |

</details>
<br>

O Swagger é a referência principal para demonstração manual dos endpoints. Uma collection do Postman pode ser criada como melhoria complementar, mas não é necessária para executar o fluxo de entrega.

---

## ✅ Testes automatizados

O projeto possui testes para domínio, aplicação e API integrada.

| Projeto de teste | Escopo |
| --- | --- |
| `OficinaMecanica.Domain.UnitTests` | Regras de domínio, agregados, entidades e objetos de valor |
| `OficinaMecanica.Application.UnitTests` | Casos de uso, validações, fluxos de sucesso e erro |
| `OficinaMecanica.API.IntegrationTests` | Endpoints, autenticação, autorização, persistência e cenários integrados |

Para rodar todos os testes:

```bash
dotnet test
```

Os testes integrados usam **Testcontainers** para subir um SQL Server temporário. Quando o Docker Desktop está em execução, eles rodam normalmente com `dotnet test`.

Se o Docker não estiver acessível no ambiente local, os testes integrados que dependem de container são marcados como `Skipped` em vez de quebrar a suíte inteira. Isso não altera a execução da API com Docker Compose; afeta apenas a experiência ao rodar testes automatizados.

Para pular intencionalmente os testes que dependem de Docker em uma execução local:

Windows PowerShell:

```powershell
$env:OFICINA_SKIP_DOCKER_TESTS = "true"
dotnet test
Remove-Item Env:OFICINA_SKIP_DOCKER_TESTS
```

Bash, macOS ou Linux:

```bash
export OFICINA_SKIP_DOCKER_TESTS=true
dotnet test
unset OFICINA_SKIP_DOCKER_TESTS
```

---

## Qualidade e evidências

Além dos testes automatizados, o projeto foi validado com evidências de cobertura, qualidade estática e análise dinâmica de segurança.

| Evidência | Ferramenta | Resultado validado |
| --- | --- | --- |
| Build e testes | `dotnet build`, `dotnet test` e Coverlet | Compilação sem erros e suíte automatizada aprovada |
| Qualidade estática | SonarQube | Quality Gate aprovado, sem bugs, vulnerabilidades, security hotspots ou code smells abertos |
| Segurança dinâmica | OWASP ZAP Baseline | Nenhuma falha crítica bloqueante; warnings residuais documentados como limitação do MVP |

Os relatórios gerados serão enviados junto ao PDF de entrega exigido no Tech Challenge.

Também foi validado o funcionamento do Horizontal Pod Autoscaler (HPA), comprovando o escalonamento automático da API durante testes de carga em ambiente Kubernetes local.

---

## 🧱 Banco de dados e seed

A aplicação usa **EF Core** com **SQL Server 2022**. A versão validada no Docker Compose e nos Testcontainers é `mcr.microsoft.com/mssql/server:2022-latest`.

| Configuração | Função |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | String de conexão principal |
| `Database:ApplyMigrationsOnStartup` | Aplica migrations automaticamente ao iniciar a API |
| `Database:SeedDemoData` | Carrega dados de demonstração |

Quando `Database:SeedDemoData` está habilitado, a aplicação carrega dados de demonstração para facilitar a avaliação local.

Na execução via Kubernetes, o SQL Server utiliza um `PersistentVolumeClaim` para garantir a persistência dos dados. Em um volume novo, a aplicação aplica automaticamente as migrations e cria o banco `OficinaMecanicaDb` durante a inicialização.

---

## 📖 Documentação complementar

A documentação arquitetural, decisões, diagramas, backlog técnico e acompanhamento do projeto estão centralizados na wiki do projeto no Notion:

[Sistema Integrado de Atendimento e Execução de Serviços em Oficina Mecânica - Wiki do Projeto](https://turquoise-syzygy-614.notion.site/Sistema-Integrado-de-Atendimento-e-Execu-o-de-Servi-os-em-Oficina-Mec-nica-Wiki-do-Projeto-329f64869829801cb424d26f19cf224f?source=copy_link)

---

## 📝 Observações

Este projeto foi desenvolvido para fins acadêmicos. Algumas decisões foram feitas para equilibrar prazo, escopo e clareza da entrega:

- Os usuários demo existem apenas para facilitar a avaliação local.
- O envio do orçamento ao cliente é representado como uma etapa externa ao sistema.
- A aprovação ou reprovação do cliente é refletida por ações feitas pelo atendente na API.
- A autenticação usa JWT com usuários demo, podendo evoluir para ASP.NET Identity ou provedor externo.
- A solução é um monólito, mas foi organizada internamente por contextos, camadas e responsabilidades.
- O Swagger documenta os contratos da API e auxilia na execução manual dos endpoints.
- O Swagger é a documentação principal de execução manual; uma collection do Postman fica como melhoria complementar pós-MVP.

### Limitações assumidas no MVP

| Item | Decisão para o MVP | Evolução recomendada |
| --- | --- | --- |
| Aprovação de orçamento | A chamada de iniciar execução representa que o orçamento foi aprovado fora do sistema | Criar endpoint/evento explícito de aprovação ou reprovação do cliente |
| Estoque insuficiente | A reserva retorna erro e bloqueia a operação | Avaliar cancelamento automático da OS quando essa regra for obrigatória |
| Consulta de status pelo cliente | O usuário demo `Cliente` consulta por ID da OS | Vincular usuário autenticado a `ClienteId` real e validar posse da OS |
| Numeração da OS | O número é gerado com base no maior número existente | Usar sequence/identity transacional para alta concorrência |
| Persistência | Repositórios simples ainda salvam diretamente; fluxos transacionais usam `IUnitOfWork` | Padronizar toda persistência em torno do Unit of Work |
| Login demo | Usuários e senhas existem para facilitar avaliação local | Evoluir para hash de senha, ASP.NET Identity ou provedor externo |
| Resposta de autenticação | Retorna token, login e perfil para deixar o demo mais explícito | Simplificar contrato conforme necessidade dos consumidores |
| Estoque global | O estoque é tratado como agregado único no MVP | Reavaliar modelagem por filial/localidade quando houver escala |
| Logging | Logs atuais cobrem startup, migrations e seed | Adicionar correlação por request e logging estruturado |
| OWASP ZAP | Baseline executado com `0` falhas e warnings residuais ligados principalmente ao Swagger UI | Endurecer CSP sem `unsafe-inline` e acompanhar atualização do Swagger UI/DOMPurify |

---

## 📄 Licença

Projeto desenvolvido para fins acadêmicos no contexto da **Pós Tech FIAP - Arquitetura de Software**.

Caso o repositório seja publicado como open source futuramente, recomenda-se adicionar um arquivo `LICENSE` com a licença escolhida.