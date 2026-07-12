# 🐳 ECR Module

## 🎯 Objetivo

Este módulo é responsável por provisionar um repositório privado no Amazon Elastic Container Registry (ECR), utilizado para armazenar as imagens Docker da aplicação.

## 📦 Recursos criados

- Amazon ECR Repository

## ⚙️ Funcionalidades

- Criação de repositório privado;
- Imutabilidade das tags de imagem (`IMMUTABLE`);
- Verificação automática de vulnerabilidades (`scan_on_push`);
- Aplicação das tags compartilhadas da infraestrutura.

## 🧩 Variáveis de entrada

| Variável | Tipo | Descrição |
|----------|------|-----------|
| `name` | `string` | Nome do repositório ECR. |
| `image_tag_mutability` | `string` | Define se as tags das imagens podem ser sobrescritas. |
| `scan_on_push` | `bool` | Habilita a verificação automática de vulnerabilidades após o envio da imagem. |
| `tags` | `map(string)` | Tags aplicadas ao recurso. |

## 📤 Outputs

| Output | Descrição |
|--------|-----------|
| `repository_name` | Nome do repositório. |
| `repository_url` | URL do repositório ECR. |
| `repository_arn` | ARN do repositório. |
