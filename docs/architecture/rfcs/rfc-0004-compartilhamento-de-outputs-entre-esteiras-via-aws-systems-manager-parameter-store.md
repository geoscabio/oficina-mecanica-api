# RFC-0004 — Compartilhamento de Outputs entre Esteiras via AWS Systems Manager Parameter Store

## Status

Aceita para implementação na Fase 3.

## Contexto

A arquitetura da Fase 3 será dividida em seis esteiras independentes. Cada repositório terá seu próprio ciclo de CI/CD, mas alguns recursos dependerão de informações produzidas por outras esteiras.

Exemplos:

- A infraestrutura Kubernetes precisa conhecer a VPC e as sub-redes privadas.
- A Lambda de autenticação precisa conhecer o endpoint do RDS.
- A API precisa conhecer o cluster Kubernetes, o repositório ECR e o RDS.
- O API Gateway precisa conhecer o ARN da Lambda e o listener do NLB interno.

Copiar esses valores manualmente entre repositórios, arquivos de configuração ou variáveis de pipeline aumenta o risco de erro, dificulta a rastreabilidade e torna a recriação do ambiente menos confiável.

## Decisão

O AWS Systems Manager Parameter Store será utilizado como mecanismo padrão para publicar e consumir outputs entre as esteiras.

Cada recurso publicará apenas os parâmetros necessários para os repositórios dependentes. As esteiras consumidoras deverão ler os valores diretamente do Parameter Store durante a execução de suas pipelines ou do Terraform.

A convenção de nomes será:

```text
/oficina-mecanica/{ambiente}/{recurso}/{output}
```

Para status operacional, será utilizada a convenção:

```text
/oficina-mecanica/{ambiente}/status/{recurso}
```

Exemplo para o ambiente de desenvolvimento:

```text
/oficina-mecanica/development/vpc/vpc_id
/oficina-mecanica/development/rds/endpoint
/oficina-mecanica/development/kubernetes/cluster_name
/oficina-mecanica/development/auth-lambda/function_arn
/oficina-mecanica/development/api/nlb_listener_arn
/oficina-mecanica/development/status/vpc
/oficina-mecanica/development/status/rds
```

## Parâmetros previstos

| Parâmetro | Produzido por | Consumido por |
| --- | --- | --- |
| `/oficina-mecanica/development/vpc/vpc_id` | `infra-vpc` | `infra-rds`, `infra-kubernetes`, `auth-lambda`, `infra-api-gateway` |
| `/oficina-mecanica/development/vpc/private_subnet_ids` | `infra-vpc` | `infra-rds`, `infra-kubernetes`, `auth-lambda`, `infra-api-gateway` |
| `/oficina-mecanica/development/rds/endpoint` | `infra-rds` | `auth-lambda`, `api` |
| `/oficina-mecanica/development/rds/security_group_id` | `infra-rds` | `auth-lambda`, `api` |
| `/oficina-mecanica/development/kubernetes/cluster_name` | `infra-kubernetes` | `api` |
| `/oficina-mecanica/development/kubernetes/ecr_repository_url` | `infra-kubernetes` | `api` |
| `/oficina-mecanica/development/auth-lambda/function_arn` | `auth-lambda` | `infra-api-gateway` |
| `/oficina-mecanica/development/auth-lambda/jwt_secret_arn` | `auth-lambda` | `api` e `infra-api-gateway`, quando necessário |
| `/oficina-mecanica/development/api/nlb_dns_name` | `api` | `infra-api-gateway` |
| `/oficina-mecanica/development/api/nlb_listener_arn` | `api` | `infra-api-gateway` |

## Regras de segurança

O Parameter Store será usado para compartilhar referências e outputs de infraestrutura, não valores secretos.

Os seguintes dados não poderão ser armazenados como texto simples no Parameter Store:

- Senha do banco de dados.
- Segredo de assinatura do JWT.
- Tokens de integração.
- Credenciais AWS.
- Chaves privadas.

Os segredos permanecerão no AWS Secrets Manager. Quando outra esteira precisar conhecê-los, será publicado somente o ARN do segredo, nunca seu valor.

Parâmetros de infraestrutura, como identificadores, endpoints, nomes e ARNs, poderão ser armazenados como `String`.

Cada pipeline deverá possuir apenas as permissões necessárias para ler ou publicar os parâmetros do seu próprio ambiente.

## Publicação e consumo

Após criar ou atualizar um recurso, a esteira produtora deverá:

1. Validar se o recurso existe na AWS.
2. Publicar seus outputs no Parameter Store.
3. Atualizar seu marcador de status.
4. Registrar a execução no log da pipeline.

Antes de executar uma ação dependente, a esteira consumidora deverá:

1. Ler os parâmetros necessários.
2. Validar se os valores obrigatórios existem.
3. Validar o status publicado pela esteira produtora.
4. Confirmar, quando necessário, o estado real do recurso na AWS.

O marcador no Parameter Store não deverá ser considerado como única fonte de verdade. Ele é um mecanismo de coordenação; a existência real do recurso deverá ser validada na AWS quando a operação for sensível, especialmente em operações de destroy.

## Fallback para ambiente acadêmico

O fluxo principal utilizará o AWS Systems Manager Parameter Store.

Caso as permissões do AWS Academy Learner Lab impeçam a publicação de parâmetros, o fallback será o uso temporário de GitHub Repository Variables ou GitHub Environment Variables.

Esse fallback deverá:

- Ser documentado no README da esteira afetada.
- Manter os mesmos nomes lógicos dos parâmetros.
- Não armazenar segredos como variáveis comuns.
- Ser removido quando o acesso ao Parameter Store estiver disponível.

## Consequências

### Positivas

- Elimina a cópia manual de outputs entre repositórios.
- Torna as dependências entre esteiras explícitas e rastreáveis.
- Permite recriar o ambiente com menos intervenção manual.
- Mantém informações de infraestrutura organizadas por ambiente e recurso.
- Evita a exposição de segredos em arquivos versionados ou logs de pipeline.

### Trade-offs e cuidados

- As esteiras passam a depender de permissões de leitura e escrita no Parameter Store.
- Um parâmetro desatualizado pode causar falhas em recursos dependentes.
- A equipe deve manter nomes consistentes para evitar divergência entre repositórios.
- O fallback por variáveis do GitHub reduz a automação e exige atualização manual controlada.
- O Parameter Store não substitui a validação real do estado dos recursos na AWS.

## Fora de escopo

- Armazenar senhas, tokens ou segredos no Parameter Store.
- Implementar um mecanismo de descoberta automática de recursos fora da convenção definida.
- Substituir o Terraform state.
- Implementar os guardrails detalhados de apply e destroy.

## Critérios de aceite

- Cada esteira publica seus outputs necessários seguindo a convenção de nomes definida.
- Nenhuma esteira depende de copiar manualmente identificadores de infraestrutura.
- Parâmetros são separados por ambiente.
- Segredos permanecem armazenados no AWS Secrets Manager.
- A Lambda, a API e o API Gateway conseguem consumir os outputs necessários das esteiras anteriores.
- A ausência de parâmetro obrigatório interrompe a pipeline com mensagem clara.
- O fallback por GitHub Variables é utilizado somente se o AWS Academy bloquear o Parameter Store.