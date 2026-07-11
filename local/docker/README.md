# Docker local

Este diretório contém o Docker Compose usado para desenvolvimento local.

## Subir ambiente

Execute a partir da raiz do repositório:

```powershell
Copy-Item .env.example .env
docker compose --env-file .env -f local/docker/docker-compose.yml up -d --build
```

## Parar ambiente

```powershell
docker compose --env-file .env -f local/docker/docker-compose.yml down
```

Para recriar o volume do SQL Server:

```powershell
docker compose --env-file .env -f local/docker/docker-compose.yml down -v
docker compose --env-file .env -f local/docker/docker-compose.yml up -d --build
```

## Observação

O `Dockerfile` permanece na raiz para manter o contexto de build simples para CI/CD, Docker Compose e publicação da imagem no ECR.
