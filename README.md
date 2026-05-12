# OficinaMecanica

API REST academica para gestao de atendimento e execucao de servicos em oficina mecanica.

## Arquitetura

O projeto segue a modelagem definida em `Contexto para IA`:

- Monolito modular
- Clean Architecture
- DDD tatico
- Application organizada por Use Cases
- API REST para uso via Swagger, Postman ou ferramentas HTTP

## Camadas

- `OficinaMecanica.Domain`: regras de negocio, agregados, entidades, value objects, enums, excecoes e contratos.
- `OficinaMecanica.Application`: use cases, DTOs, validators, mappings e orquestracao.
- `OficinaMecanica.Infrastructure`: EF Core, SQL Server, repositories, migrations e servicos de infraestrutura.
- `OficinaMecanica.API`: controllers, middlewares e configuracoes HTTP.

## Execucao local

Esta etapa inicial cria apenas a solution e os projetos base. As instrucoes completas de execucao, banco, Docker e Swagger serao preenchidas conforme as respectivas etapas forem implementadas.
