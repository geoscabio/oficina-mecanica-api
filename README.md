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
- [Como acessar o banco pelo SSMS](#-como-acessar-o-banco-pelo-ssms)
- [Autenticação](#-autenticação)
- [Como testar a API](#-como-testar-a-api)
- [Testes automatizados](#-testes-automatizados)
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
| **Gestão de OS** | Abertura, diagnóstico, orçamento, aprovação, execução, finalização, entrega e cancelamento |
| **Indicadores** | Consulta de tempo médio de execução dos serviços |

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
| Banco de dados | SQL Server |
| ORM | Entity Framework Core |
| Documentação da API | Swagger e Postman |
| Segurança | JWT Bearer Authentication e Authorization |
| Validação e mapeamento | FluentValidation e AutoMapper |
| Testes | xUnit, FluentAssertions, Moq, Testcontainers, Respawn e Coverlet |
| Execução local | Docker e Docker Compose |

---

## 📁 Estrutura do repositório

```text
.
├── Dockerfile
├── docker-compose.yml
├── OficinaMecanica.sln
├── README.md
├── src
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

| Item | Obrigatório? | Observação |
| --- | --- | --- |
| Docker Desktop | Sim | Necessário para subir API, SQL Server e testes integrados |
| .NET SDK 10 | Sim | Necessário para executar testes ou rodar a API fora do container |
| SQL Server Management Studio | Opcional | Usado apenas para visualizar o banco localmente |
| Porta `5093` livre | Sim | Porta da API |
| Porta `14333` livre | Sim | Porta local do SQL Server |

### 1. Subir API e banco

Na raiz do repositório, execute no **PowerShell**:

```powershell
Copy-Item .env.example .env
```

Depois exporte as variaveis (ou ajuste os valores direto no arquivo `.env`) e suba o compose:

```powershell
$env:OFICINA_SQL_SA_PASSWORD = "OficinaMecanicaDbLocal@2026";
$env:OFICINA_JWT_SECRET = "oficina-mecanica-jwt-secret-local-2026";
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

<details>
<summary>🧑‍💻 Execução alternativa pela IDE ou dotnet CLI</summary>

Este fluxo é opcional e serve para quando você quiser executar ou depurar a API diretamente pelo **Visual Studio** ou pelo **terminal**, mantendo apenas o SQL Server no Docker.

O fluxo recomendado para avaliação continua sendo o Docker Compose completo, porque exige menos configuração manual.

| Componente | Onde executa |
| --- | --- |
| SQL Server | Docker |
| API | Visual Studio ou `dotnet run` |

#### 1. Subir apenas o SQL Server

Na raiz do repositório, execute no **PowerShell**:

```powershell
$env:OFICINA_SQL_SA_PASSWORD = "OficinaMecanicaDbLocal@2026";
$env:OFICINA_JWT_SECRET = "oficina-mecanica-jwt-secret-local-2026";
$env:OFICINA_DEFAULT_CONNECTION = "Server=localhost,14333;Database=OficinaMecanicaDb;User Id=sa;Password=$env:OFICINA_SQL_SA_PASSWORD;TrustServerCertificate=True;";
docker compose up -d sqlserver
```

#### 2. Configurar os User Secrets da API

Ainda no **PowerShell**, execute:

```powershell
$apiProject = "src\OficinaMecanica.API\OficinaMecanica.API.csproj";
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "$env:OFICINA_DEFAULT_CONNECTION" --project $apiProject;
dotnet user-secrets set "Jwt:Secret" "$env:OFICINA_JWT_SECRET" --project $apiProject;
```

Essas configurações ficam salvas localmente no ambiente de desenvolvimento e não são versionadas no repositório.

#### 3. Executar pelo Visual Studio

1. Abra a solução `OficinaMecanica.sln`.
2. Defina `OficinaMecanica.API` como projeto de inicialização.
3. Selecione o profile `http`.
4. Execute com `F5` ou `Ctrl + F5`.
5. Acesse o Swagger em `http://localhost:5093/swagger`.

#### 4. Executar pelo terminal

```powershell
dotnet restore;
dotnet run --project src\OficinaMecanica.API\OficinaMecanica.API.csproj --launch-profile http
```

Depois acesse:

```text
http://localhost:5093/swagger
```

</details>
<br>

---

## 🗄️ Como acessar o banco pelo SSMS

Com os containers em execução, abra o **SQL Server Management Studio** e preencha a janela **Connect to Server** assim:

| Campo no SSMS | Valor |
| --- | --- |
| Server type | `Database Engine` |
| Server name | `localhost,14333` |
| Authentication | `SQL Server Authentication` |
| Login | `sa` |
| Password | Valor definido em `$env:OFICINA_SQL_SA_PASSWORD` |
| Trust server certificate | Marcado |

Depois de conectar:

1. Abra o painel **Object Explorer**.
2. Expanda **Databases**.
3. Selecione o banco **OficinaMecanicaDb**.
4. Expanda **Tables** para visualizar as tabelas criadas pelas migrations.

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

A collection completa do Postman será adicionada posteriormente em `docs/postman` para facilitar a demonstração do fluxo completo.

---

## ✅ Testes automatizados

O projeto possui testes para domínio, aplicação e API integrada.

| Projeto de teste | Escopo |
| --- | --- |
| `OficinaMecanica.Domain.UnitTests` | Regras de domínio, agregados, entidades e objetos de valor |
| `OficinaMecanica.Application.UnitTests` | Casos de uso, validações, fluxos de sucesso e erro |
| `OficinaMecanica.API.IntegrationTests` | Endpoints, autenticação, autorização, persistência e cenários integrados |

Para rodar todos os testes:

```powershell
dotnet test
```

Para rodar por projeto:

```powershell
dotnet test tests\OficinaMecanica.Domain.UnitTests\OficinaMecanica.Domain.UnitTests.csproj
dotnet test tests\OficinaMecanica.Application.UnitTests\OficinaMecanica.Application.UnitTests.csproj
dotnet test tests\OficinaMecanica.API.IntegrationTests\OficinaMecanica.API.IntegrationTests.csproj
```

Os testes integrados usam **Testcontainers** para subir um SQL Server temporário. Por isso, o Docker Desktop precisa estar em execução.

---

## 🧱 Banco de dados e seed

A aplicação usa **EF Core** com **SQL Server**.

| Configuração | Função |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | String de conexão principal |
| `Database:ApplyMigrationsOnStartup` | Aplica migrations automaticamente ao iniciar a API |
| `Database:SeedDemoData` | Carrega dados de demonstração |

Quando `Database:SeedDemoData` está habilitado, a aplicação carrega dados de demonstração para facilitar a avaliação local.

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
- A collection do Postman será adicionada como documentação complementar para facilitar a demonstração do fluxo completo.

---

## 📄 Licença

Projeto desenvolvido para fins acadêmicos no contexto da **Pós Tech FIAP - Arquitetura de Software**.

Caso o repositório seja publicado como open source futuramente, recomenda-se adicionar um arquivo `LICENSE` com a licença escolhida.
