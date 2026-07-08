# Runtime - Execução local com Docker

# Objetivo

Gerar no Eraser um diagrama de runtime local mostrando como a API e o SQL Server sobem via Docker Compose.

# Escopo

`Dockerfile` e `docker-compose.yml` existentes na raiz do projeto.

# Recursos identificados no projeto

- Dockerfile multi-stage:
  - build com `mcr.microsoft.com/dotnet/sdk:10.0`.
  - runtime com `mcr.microsoft.com/dotnet/aspnet:10.0`.
  - publish do projeto `src/OficinaMecanica.API/OficinaMecanica.API.csproj`.
  - entrypoint `dotnet OficinaMecanica.API.dll`.
  - porta interna `8080`.
- Docker Compose:
  - service `api`.
  - service `sqlserver`.
  - volume `oficina-mecanica-sqlserver-data`.
- SQL Server:
  - imagem `mcr.microsoft.com/mssql/server:2022-latest`.
  - porta local `14333` para container `1433`.
  - healthcheck com `sqlcmd`.
- API:
  - build local via Dockerfile.
  - porta local `5093` para container `8080`.
  - depende do SQL Server saudável.
  - aplica migrations e seed demo no startup.

# Recursos planejados

Não representar Kubernetes ou AWS neste diagrama.

# Recursos que não devem aparecer

- Kubernetes.
- AWS.
- ECR.
- Ingress.
- HPA.
- GitHub Actions.

# Layout recomendado

Use uma moldura "Developer workstation". Dentro dela, use uma moldura "Docker Compose".

Fluxo da esquerda para a direita:

`Desenvolvedor` -> `api service` -> `sqlserver service` -> `volume oficina-mecanica-sqlserver-data`.

Acima do `api service`, mostrar o Dockerfile multi-stage como origem da imagem local.

# Hierarquia visual

- Nível 1: Developer workstation.
- Nível 2: Docker Compose.
- Nível 3: Services.
- Nível 4: Container, portas, variáveis e volume.

# Fluxos

- Desenvolvedor acessa API em `localhost:5093`.
- Compose publica a API em `5093:8080`.
- API conecta no SQL Server pelo hostname `sqlserver` e porta `1433`.
- SQL Server persiste dados no volume Docker.
- API aguarda healthcheck do SQL Server antes de iniciar.

# Prompt final para o Eraser

Crie um runtime architecture diagram no Eraser para execução local com Docker Compose. Use uma moldura externa "Developer workstation" e dentro dela uma moldura "Docker Compose". Mostre o service "api" construído a partir do Dockerfile multi-stage: build com mcr.microsoft.com/dotnet/sdk:10.0, runtime com mcr.microsoft.com/dotnet/aspnet:10.0, entrypoint dotnet OficinaMecanica.API.dll e porta interna 8080. Mostre a porta publicada localhost:5093 -> api:8080. Mostre que a API usa ASPNETCORE_ENVIRONMENT=Development, aplica migrations no startup e carrega seed demo. Mostre o service "sqlserver" com imagem mcr.microsoft.com/mssql/server:2022-latest, porta localhost:14333 -> sqlserver:1433, healthcheck com sqlcmd e volume "oficina-mecanica-sqlserver-data" montado em /var/opt/mssql. Conecte api -> sqlserver com ConnectionStrings__DefaultConnection usando hostname sqlserver e porta 1433. Mostre que api depende de sqlserver saudável. Não desenhe Kubernetes, AWS, ECR, Ingress, HPA ou GitHub Actions.
