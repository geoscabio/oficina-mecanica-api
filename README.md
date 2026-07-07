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
| Execucao local | Docker, Docker Compose, Kubernetes (Docker Desktop), NGINX Ingress Controller, Metrics Server, Horizontal Pod Autoscaler (HPA) e .NET SDK |

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

O Docker Compose lê o arquivo `.env` automaticamente. Não é necessário criar o arquivo na mão. Se quiser trocar a senha do SQL Server, o segredo JWT ou o token do webhook de orçamento, edite os valores do `.env` antes de subir os containers.

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
- Deployment da API com Startup Probe, Liveness Probe e Readiness Probe.
- Deployment do SQL Server.
- Services para comunicação entre os Pods.
- ConfigMap para configurações da aplicação.
- Secrets para informações sensíveis.
- PersistentVolumeClaim para persistência dos dados do SQL Server.
- Horizontal Pod Autoscaler (HPA) baseado em utilização de CPU e memória.

### Pré-requisitos

- Docker Desktop com Kubernetes habilitado.
- `kubectl` configurado para acessar o cluster local.
- Metrics Server instalado.
> **Observação:** as probes da aplicação utilizam o endpoint `/api/health`, responsável por informar ao Kubernetes quando a aplicação está inicializada, saudável e pronta para receber requisições. O endpoint será implementado pela aplicação antes do deploy em ambiente AWS.

### Padronização das tags da infraestrutura

Os módulos Terraform utilizam um conjunto de tags compartilhadas, centralizadas no arquivo:

```text
infra/environments/dev/locals.tf
```

Essas tags são repassadas para todos os módulos por meio da variável `tags`, garantindo padronização na identificação dos recursos provisionados e evitando duplicação de configuração entre os módulos de infraestrutura.



### Rede privada (Amazon VPC)

A infraestrutura Terraform provisiona uma Virtual Private Cloud (VPC) composta por sub-redes públicas e privadas distribuídas em múltiplas Availability Zones.

Além da conectividade pública por meio do Internet Gateway, a infraestrutura utiliza um NAT Gateway associado a um Elastic IP para fornecer acesso de saída à Internet aos recursos implantados nas sub-redes privadas.

A configuração implementada contempla:

- Virtual Private Cloud (VPC);
- Internet Gateway para acesso das sub-redes públicas;
- Duas sub-redes públicas;
- Duas sub-redes privadas;
- NAT Gateway associado a um Elastic IP;
- Route Table pública com rota para o Internet Gateway;
- Route Table privada com rota padrão (`0.0.0.0/0`) apontando para o NAT Gateway;
- Aplicação das tags compartilhadas da infraestrutura para padronização dos recursos provisionados.

A utilização do NAT Gateway permite que recursos executados nas sub-redes privadas, como os nós do Amazon EKS e a instância do Amazon RDS, realizem conexões de saída para serviços da AWS e para a Internet sem exposição direta por endereço IP público.

A infraestrutura de rede é provisionada pelo módulo Terraform localizado em:

```text
infra/modules/networking
```

### Registro de imagens (Amazon ECR)

A infraestrutura Terraform cria automaticamente um repositório privado no Amazon Elastic Container Registry (ECR), responsável por armazenar as imagens Docker da aplicação que serão utilizadas durante o provisionamento do ambiente na AWS.

A configuração do repositório contempla:

- Tags de imagem imutáveis (`IMMUTABLE`), evitando a sobrescrita de versões já publicadas;
- Verificação automática de vulnerabilidades (`scan_on_push`) a cada envio de imagem;
- Criptografia padrão da AWS (`AES-256`);
- Aplicação das tags compartilhadas da infraestrutura para padronização dos recursos provisionados.

O repositório é provisionado pelo módulo Terraform `registry`, localizado em:

```text
infra/modules/registry
```

Sua utilização será integrada ao pipeline de CI/CD e ao provisionamento do cluster Amazon EKS nas próximas etapas do projeto.

### Banco de dados gerenciado (Amazon RDS)

A infraestrutura Terraform também provisiona uma instância gerenciada do Amazon Relational Database Service (RDS), utilizada como banco de dados da aplicação em ambiente AWS.

A configuração implementada contempla:

- Engine **Amazon RDS for SQL Server Express**;
- Implantação em sub-redes privadas da VPC por meio de um **DB Subnet Group**;
- Security Group dedicado permitindo acesso somente pela porta **1433** dentro da rede privada da aplicação;
- Banco não exposto à Internet (`publicly_accessible = false`);
- Configuração preparada para ambientes de desenvolvimento e laboratório, com `skip_final_snapshot = true` e `backup_retention_period = 0`;
- Aplicação das tags compartilhadas da infraestrutura para padronização dos recursos provisionados.

O banco é provisionado pelo módulo Terraform `database`, localizado em:

```text
infra/modules/database
```

A aplicação utilizará o endpoint gerado pelo Amazon RDS para estabelecer a conexão com o banco de dados durante a execução no cluster Amazon EKS.

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
kubectl describe hpa api-hpa -n oficina
```

### Acesso via Ingress

Após aplicar os manifestos Kubernetes, a API também pode ser acessada através do NGINX Ingress Controller.

Aplicação do manifesto:

```bash
kubectl apply -f k8s/api-ingress.yaml
```

Validação em ambiente local:

```bash
kubectl port-forward -n ingress-nginx service/ingress-nginx-controller 8081:80
```

Em outro terminal:

```powershell
Invoke-WebRequest -Headers @{Host="oficina.local"} http://localhost:8081
```

Resultado esperado:

```text
StatusCode : 200
```

Esse procedimento confirma que o NGINX Ingress Controller está roteando corretamente as requisições para a API.

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
| Cliente | `cliente` | `cliente123` | Usuário demo mantido para avaliação local; a consulta de status da OS é pública por ID |

### Autorizar no Swagger

1. Acesse `http://localhost:5093/swagger`.
2. Execute o endpoint de login.
3. Copie o valor retornado no campo `token`.
4. Clique em **Authorize**.
5. Cole apenas o token JWT, sem escrever `Bearer` manualmente.
6. Confirme em **Authorize**.

### Endpoint externo de orçamento

O recebimento da aprovacao ou reprovacao do orcamento do cliente e feito por endpoint externo dedicado:

`POST /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/orcamento/notificacoes`

Esse endpoint nao usa JWT de perfis internos. Para simular a integracao externa com seguranca simples, envie o header `X-Webhook-Token` com o mesmo valor configurado em `Integracoes:Orcamento:WebhookToken`.

O segredo nao fica em `appsettings.json`: no Docker Compose ele vem da variavel `OFICINA_ORCAMENTO_WEBHOOK_TOKEN` do arquivo `.env`, mapeada para `Integracoes__Orcamento__WebhookToken`; no Kubernetes ele vem do Secret `oficina-api-secret`.

Assim como o `Jwt__Secret`, a API valida essa configuracao na inicializacao e nao sobe se o token estiver ausente ou tiver menos de 32 caracteres. Para uso local sem Docker, configure a variavel de ambiente `Integracoes__Orcamento__WebhookToken` antes de executar a aplicacao.

### Consulta pública de status

A consulta de status da OS e publica por ID para representar o acompanhamento do cliente sem login obrigatorio:

`GET /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/consultar-status`

### Contrato oficial de abertura da OS

O endpoint oficial da Fase 2 para abrir Ordem de Servico permanece:

`POST /api/v1/gestao-ordem-servico/ordens-servico/cadastrar`

O fluxo completo da aplicação prevê que cliente e veículo sejam previamente identificados ou cadastrados por meio dos endpoints específicos de Atendimento. Dessa forma, a abertura da Ordem de Serviço referencia cliente, veículo e mecânico por seus identificadores (ou documento do cliente, quando aplicável), preservando a consistência transacional e evitando duplicidade de cadastros.

Os serviços e peças/insumos podem ser enviados já na abertura da Ordem de Serviço, quando conhecidos, ou definidos posteriormente durante a etapa de diagnóstico pelo mecânico, conforme o fluxo de negócio modelado no Domain Storytelling e Event Storming do projeto.

Quando servicos ou pecas/insumos ainda nao se aplicarem na abertura, as listas devem ser enviadas vazias (`[]`). Quando forem enviadas, a OS ja registra esses itens, calcula o orcamento inicial e reserva estoque para pecas/insumos.

Exemplo:

```json
{
  "clienteId": "00000000-0000-0000-0000-000000000000",
  "documentoCliente": null,
  "veiculoId": "00000000-0000-0000-0000-000000000000",
  "mecanicoId": "00000000-0000-0000-0000-000000000000",
  "servicosCatalogoIds": [],
  "pecasInsumos": []
}
```

Exemplo com orcamento inicial:

```json
{
  "clienteId": "00000000-0000-0000-0000-000000000000",
  "documentoCliente": null,
  "veiculoId": "00000000-0000-0000-0000-000000000000",
  "mecanicoId": "00000000-0000-0000-0000-000000000000",
  "servicosCatalogoIds": [
    "00000000-0000-0000-0000-000000000000"
  ],
  "pecasInsumos": [
    {
      "pecaInsumoCatalogoId": "00000000-0000-0000-0000-000000000000",
      "quantidade": 1
    }
  ]
}
```

---

## 📚 Documentação da API (Swagger/OpenAPI)

A documentação oficial da API é disponibilizada através do Swagger/OpenAPI da própria aplicação.

Após executar o projeto localmente, toda a documentação dos endpoints poderá ser acessada em:

`http://localhost:5093/swagger`

O Swagger é a fonte oficial da documentação da API deste projeto e atende ao requisito do Tech Challenge.

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
| 14 | Notificar aprovação do orçamento externo | `POST /api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/orcamento/notificacoes` |
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

A aplicação usa **EF Core** com **SQL Server 2022**.

### Justificativa da escolha do SQL Server

O SQL Server foi escolhido por sua aderência ao domínio relacional da aplicação, que exige integridade transacional entre Ordens de Serviço, estoque, clientes, veículos e serviços. A solução possui integração nativa com o Entity Framework Core, funciona de forma consistente tanto no Docker Compose quanto nos Testcontainers utilizados pelos testes automatizados e simplifica a execução e avaliação local durante o Tech Challenge.

Esta é uma decisão arquitetural voltada ao contexto acadêmico do projeto e poderá evoluir conforme necessidades futuras da solução. A versão validada no Docker Compose e nos Testcontainers é `mcr.microsoft.com/mssql/server:2022-latest`.

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
- A aprovação ou reprovação do cliente é recebida por endpoint externo de notificação de orçamento, protegido por `X-Webhook-Token`.
- A consulta de status da OS é pública por ID para representar o acompanhamento do cliente sem login obrigatório.
- A autenticação usa JWT com usuários demo, podendo evoluir para ASP.NET Identity ou provedor externo.
- A solução é um monólito, mas foi organizada internamente por contextos, camadas e responsabilidades.
- O Swagger documenta os contratos da API e auxilia na execução manual dos endpoints.
- O Swagger é a documentação principal de execução manual; uma collection do Postman fica como melhoria complementar pós-MVP.

### Decisões técnicas e evolução recomendada

| Item | Decisão para o MVP | Evolução recomendada |
| --- | --- | --- |
| Webhook de orçamento | A decisão externa usa endpoint dedicado com header `X-Webhook-Token`; o segredo vem de `.env`/Secret e deve ter pelo menos 32 caracteres | Evoluir para assinatura HMAC, idempotência e trilha de auditoria |
| Estoque insuficiente | A reserva retorna erro e bloqueia a operação | Avaliar cancelamento automático da OS quando essa regra for obrigatória |
| Consulta pública de status | O cliente acompanha a OS por ID sem login, conforme rota de acompanhamento prevista no desafio | Evoluir para código público de acompanhamento quando houver portal real |
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
