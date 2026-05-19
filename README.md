# OficinaMecanica

API REST acadêmica para atendimento e gestão de ordens de serviço de uma oficina mecânica.

## Arquitetura

O projeto segue a modelagem definida no contexto acadêmico:

- Monólito modular
- Clean Architecture
- DDD tático
- Bounded contexts: Administrativo, Atendimento, Gestão de Estoque, Gestão de Ordem de Serviço e Identidade
- Domain rico
- Application organizada por Use Cases
- Infrastructure com EF Core, SQL Server, repositories, migrations e seed demo
- API REST com Swagger e autenticação JWT

## Pré-requisitos

- Docker Desktop instalado e em execução
- .NET SDK 10
- Visual Studio Community 2026, opcional para executar pela IDE
- SQL Server Management Studio, opcional para consultar o banco visualmente
- Portas `5093` e `14333` livres

## Credenciais Locais

O projeto não versiona senha de banco nem chave JWT. Os comandos abaixo usam valores prontos para avaliação local acadêmica, mas podem ser trocados por outros valores da máquina de quem estiver executando.

Valores usados nos exemplos:

- Senha local do SQL Server: `OficinaMecanicaDbLocal@2026`
- Chave local do JWT: `oficina-mecanica-jwt-secret-local-2026`

Regras obrigatórias:

- A senha do SQL Server deve atender à política de senha forte.
- A chave JWT deve ter mais de 32 caracteres.

## Usuários De Demonstração

Os usuários abaixo existem apenas para demonstração e avaliação acadêmica via Swagger/Postman. Eles não representam credenciais produtivas, não dão acesso a serviços externos e podem ser substituídos em uma evolução futura por ASP.NET Identity, provedor externo de identidade ou secret manager.

| Perfil | Login | Senha |
| --- | --- | --- |
| Administrador | `admin` | `admin123` |
| Atendente | `atendente` | `atendente123` |
| Mecânico | `mecanico` | `mecanico123` |
| Cliente | `cliente` | `cliente123` |

## Preparar A Pasta Do Projeto

Se o projeto ainda não foi clonado, use uma pasta simples como `C:\Projetos`:

```powershell
New-Item -ItemType Directory -Force C:\Projetos
cd C:\Projetos
git clone https://github.com/geoscabio/oficina_mecanica_api.git
cd C:\Projetos\oficina_mecanica_api
```

Se já clonou em outro lugar, entre na pasta raiz do repositório antes de executar os comandos seguintes.

## Fluxo 1: Linha De Comando Com Docker Compose

Este é o fluxo mais simples para avaliação do projeto. Ele sobe a API e o SQL Server juntos.

PowerShell:

```powershell
cd C:\Projetos\oficina_mecanica_api

$env:OFICINA_SQL_SA_PASSWORD = "OficinaMecanicaDbLocal@2026"
$env:OFICINA_JWT_SECRET = "oficina-mecanica-jwt-secret-local-2026"
$env:OFICINA_DEFAULT_CONNECTION = "Server=localhost,14333;Database=OficinaMecanicaDb;User Id=sa;Password=$env:OFICINA_SQL_SA_PASSWORD;TrustServerCertificate=True;"

docker compose up --build
```

CMD:

```cmd
cd C:\Projetos\oficina_mecanica_api

set OFICINA_SQL_SA_PASSWORD=OficinaMecanicaDbLocal@2026
set OFICINA_JWT_SECRET=oficina-mecanica-jwt-secret-local-2026
set OFICINA_DEFAULT_CONNECTION=Server=localhost,14333;Database=OficinaMecanicaDb;User Id=sa;Password=%OFICINA_SQL_SA_PASSWORD%;TrustServerCertificate=True;

docker compose up --build
```

Ao iniciar, a API aplica migrations e carrega dados demo automaticamente.

URLs principais:

- Swagger: `http://localhost:5093/swagger`
- SQL Server: `localhost,14333`
- Banco: `OficinaMecanicaDb`
- Usuário SQL Server: `sa`
- Senha SQL Server: `OficinaMecanicaDbLocal@2026`, se mantiver o exemplo acima

Para parar sem apagar dados:

```powershell
docker compose down
```

Para parar e recriar o banco do zero na próxima subida:

```powershell
docker compose down -v
```

## Fluxo 2: Visual Studio Com SQL Server No Docker

Este fluxo usa o SQL Server do Docker, mas executa a API pelo Visual Studio Community 2026.

PowerShell:

```powershell
cd C:\Projetos\oficina_mecanica_api

$env:OFICINA_SQL_SA_PASSWORD = "OficinaMecanicaDbLocal@2026"
$env:OFICINA_JWT_SECRET = "oficina-mecanica-jwt-secret-local-2026"
$env:OFICINA_DEFAULT_CONNECTION = "Server=localhost,14333;Database=OficinaMecanicaDb;User Id=sa;Password=$env:OFICINA_SQL_SA_PASSWORD;TrustServerCertificate=True;"

docker compose up -d sqlserver

$apiProject = "src\OficinaMecanica.API\OficinaMecanica.API.csproj"

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "$env:OFICINA_DEFAULT_CONNECTION" --project $apiProject
dotnet user-secrets set "Jwt:Secret" "$env:OFICINA_JWT_SECRET" --project $apiProject
```

CMD:

```cmd
cd C:\Projetos\oficina_mecanica_api

set OFICINA_SQL_SA_PASSWORD=OficinaMecanicaDbLocal@2026
set OFICINA_JWT_SECRET=oficina-mecanica-jwt-secret-local-2026
set OFICINA_DEFAULT_CONNECTION=Server=localhost,14333;Database=OficinaMecanicaDb;User Id=sa;Password=%OFICINA_SQL_SA_PASSWORD%;TrustServerCertificate=True;

docker compose up -d sqlserver

set apiProject=src\OficinaMecanica.API\OficinaMecanica.API.csproj

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "%OFICINA_DEFAULT_CONNECTION%" --project %apiProject%
dotnet user-secrets set "Jwt:Secret" "%OFICINA_JWT_SECRET%" --project %apiProject%
```

Depois dos comandos:

1. Abra a solution no Visual Studio Community 2026.
2. Defina `OficinaMecanica.API` como projeto de inicialização.
3. Selecione o profile `http`.
4. Execute com `F5` ou `Ctrl+F5`.
5. Acesse `http://localhost:5093/swagger`.

Nesse fluxo, o banco também recebe migrations e dados demo quando a API inicia.

## Consultar O Banco Pelo SSMS

No SQL Server Management Studio, conecte com:

```text
Server name: localhost,14333
Authentication: SQL Server Authentication
Login: sa
Password: OficinaMecanicaDbLocal@2026, se mantiver o exemplo acima
Trust server certificate: marcado
```

Depois de conectar, abra o banco `OficinaMecanicaDb`.

## Autenticação No Swagger

Use a rota de login:

```http
POST /api/v1/identidade/autenticacao/login
```

Body de exemplo:

```json
{
  "login": "admin",
  "senha": "admin123"
}
```

Copie apenas o valor de `token`, clique em `Authorize` no Swagger e cole o token sem escrever `Bearer`.

## Testes

Para rodar todos os testes:

```powershell
dotnet test
```

Resultado esperado:

```text
376 testes passando
```

Observação importante: os testes integrados usam Testcontainers e sobem um SQL Server temporário pelo Docker. Por isso, o Docker Desktop precisa estar aberto para `dotnet test` executar a suíte completa.

Os testes integrados não usam o banco do SSMS nem o banco do `docker compose`. Eles criam um container temporário, aplicam migrations, executam os cenários e descartam o container ao final.
