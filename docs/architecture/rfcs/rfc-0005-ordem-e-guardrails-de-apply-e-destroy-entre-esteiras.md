# RFC-0005 — Ordem e Guardrails de Apply e Destroy entre Esteiras

## Status

Aceita para implementação na Fase 3.

## Contexto

A solução será composta por seis esteiras com recursos dependentes entre si. Criar ou destruir recursos fora da ordem correta pode causar falhas de provisionamento, recursos órfãos, custos desnecessários ou indisponibilidade do ambiente.

Exemplos:

- O RDS e o EKS dependem da VPC.
- A Lambda depende da rede e do RDS.
- A API depende do EKS, do ECR e do RDS.
- O API Gateway depende da Lambda e do NLB interno publicado pela API.

O ambiente será utilizado no AWS Academy Learner Lab, onde os recursos podem ser temporários e precisam ser destruídos de forma controlada após demonstrações ou testes.

## Decisão

Cada esteira de infraestrutura utilizará um arquivo versionado chamado `infra-action.env` para declarar a ação desejada:

```text
TERRAFORM_ACTION=apply
```

As únicas ações permitidas serão:

```text
TERRAFORM_ACTION=apply
TERRAFORM_ACTION=destroy
```

A alteração de `apply` para `destroy` deverá ocorrer somente em uma branch e Pull Request dedicados à destruição do ambiente.

A pipeline deverá validar a ação antes de executar qualquer alteração na AWS. Valores diferentes de `apply` ou `destroy` deverão interromper a execução.

## Ordem de apply

O provisionamento deverá respeitar a seguinte ordem:

```text
1. oficina-mecanica-infra-vpc
2. oficina-mecanica-infra-kubernetes e oficina-mecanica-infra-rds
3. oficina-mecanica-auth-lambda
4. oficina-mecanica-api
5. oficina-mecanica-infra-api-gateway
```

As esteiras de Kubernetes e RDS poderão ser executadas em paralelo após a criação bem-sucedida da VPC.

Nenhuma esteira poderá executar apply quando seus parâmetros obrigatórios ou recursos dependentes ainda não estiverem disponíveis.

## Ordem de destroy

A destruição deverá seguir a ordem inversa:

```text
1. oficina-mecanica-infra-api-gateway
2. oficina-mecanica-api
3. oficina-mecanica-auth-lambda
4. oficina-mecanica-infra-kubernetes e oficina-mecanica-infra-rds
5. oficina-mecanica-infra-vpc
```

Uma esteira não poderá executar destroy caso existam recursos dependentes ativos.

Exemplos:

- A VPC não poderá ser destruída enquanto existirem EKS, RDS, Lambda ou VPC Link dependentes.
- O RDS não poderá ser destruído enquanto a Lambda ou a API ainda dependerem dele.
- O NLB não poderá ser removido enquanto o API Gateway possuir uma integração ativa por VPC Link.
- A Lambda não poderá ser removida enquanto o API Gateway ainda possuir a rota `POST /auth/cpf`.

## Guardrails obrigatórios

Toda pipeline de infraestrutura deverá implementar os seguintes controles:

1. Validar o valor de `TERRAFORM_ACTION`.
2. Executar `terraform fmt` e `terraform validate`.
3. Gerar um plano Terraform antes de aplicar ou destruir recursos.
4. Publicar o plano como artefato da pipeline.
5. Exigir Pull Request dedicado para `destroy`.
6. Exigir aprovação do GitHub Environment antes da destruição.
7. Validar dependências antes de executar apply.
8. Validar dependentes antes de executar destroy.
9. Confirmar o estado real dos recursos na AWS antes e depois da execução.
10. Publicar ou atualizar os outputs e marcadores de status no Parameter Store.

Os marcadores de status no Parameter Store não serão a única fonte de verdade. Eles auxiliam a coordenação entre esteiras, mas a pipeline deverá consultar a AWS para confirmar a existência real dos recursos.

## Validações de estado real

As validações deverão utilizar os serviços adequados para cada recurso.

Exemplos:

| Recurso | Validação sugerida |
| --- | --- |
| VPC | `aws ec2 describe-vpcs` |
| EKS | `aws eks describe-cluster` |
| RDS | `aws rds describe-db-instances` |
| Lambda | `aws lambda get-function` |
| API Gateway | `aws apigatewayv2 get-api` |
| Serviço da API | `kubectl get svc` |
| Deploy da API | `kubectl get deployment` |

O objetivo dessas validações é impedir que uma esteira dependa apenas de um state local, cache ou marcador de status desatualizado.

## Reativação após destroy

Após uma destruição bem-sucedida, o controle da esteira deverá retornar para:

```text
TERRAFORM_ACTION=apply
```

Esse retorno servirá apenas para rearmar a esteira para uma futura criação controlada. A pipeline não deverá recriar recursos automaticamente apenas porque o arquivo voltou para `apply`.

Um novo apply deverá ocorrer somente quando houver uma alteração elegível para deploy e todas as dependências estiverem disponíveis.

## Consequências

### Positivas

- Reduz o risco de destruição acidental de recursos dependentes.
- Mantém as decisões de apply e destroy rastreáveis no histórico do Git.
- Permite revisar o plano Terraform antes de uma ação sensível.
- Evita depender exclusivamente de state local ou cache de pipeline.
- Facilita a recriação controlada do ambiente de demonstração.
- Ajuda a evitar custos com recursos esquecidos no AWS Academy.

### Trade-offs e cuidados

- A destruição passa a exigir mais etapas e aprovação explícita.
- As pipelines precisam consultar recursos reais na AWS, aumentando o tempo de execução.
- A equipe deverá respeitar a ordem definida entre as esteiras.
- Falhas parciais podem exigir diagnóstico antes de repetir uma operação.
- O mecanismo de reativação para `apply` deve ser implementado com cuidado para não recriar o ambiente involuntariamente.

## Fora de escopo

- Definir todos os detalhes do backend de state do Terraform.
- Executar a implementação das pipelines.
- Criar recursos AWS.
- Escrever o runbook operacional completo de reconstrução do ambiente.
- Implementar alertas e dashboards de observabilidade.

## Critérios de aceite

- Cada esteira possui um arquivo `infra-action.env` com ação válida.
- Ações de destroy exigem Pull Request dedicado e aprovação de ambiente.
- Toda execução publica um plano Terraform antes da ação.
- A ordem de apply respeita as dependências entre esteiras.
- A ordem de destroy é a inversa da ordem de apply.
- A pipeline bloqueia destroy quando recursos dependentes ainda existem.
- O estado real dos recursos é validado na AWS antes e depois das operações.
- O retorno para `TERRAFORM_ACTION=apply` não recria recursos automaticamente.