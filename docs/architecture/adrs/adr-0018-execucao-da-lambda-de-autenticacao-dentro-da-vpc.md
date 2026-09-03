# ADR-0018 — Execução da Lambda de Autenticação dentro da VPC

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

A Lambda `oficina-mecanica-auth-lambda` será responsável por autenticar clientes por CPF. Para isso, ela precisará consultar os dados do cliente e seu status no RDS SQL Server.

O RDS será executado em sub-redes privadas e não terá acesso público. Uma Lambda executada fora da VPC não poderá acessar esse banco privado sem alterar a exposição de rede do RDS.

Era necessário definir como a Lambda acessará o banco de dados mantendo a segurança da infraestrutura.

## 2. Fatores Decisivos

- Permitir que a Lambda consulte clientes e seus status no RDS privado.
- Manter o RDS sem exposição pública.
- Restringir o acesso ao banco somente aos componentes autorizados.
- Reutilizar a VPC e as sub-redes privadas já previstas na arquitetura.
- Permitir que a Lambda acesse o AWS Secrets Manager e envie telemetria ao Datadog.
- Evitar a criação de acessos externos ao banco de dados.

## 3. Decisão

A Lambda de autenticação será anexada à VPC e executará em sub-redes privadas.

A Lambda utilizará um grupo de segurança próprio. O grupo de segurança do RDS permitirá conexão na porta do SQL Server somente a partir do grupo de segurança da Lambda e da API executada no Kubernetes.

A Lambda não receberá endereço IP público.

O acesso a serviços externos necessários, como AWS Secrets Manager e Datadog, ocorrerá pela conectividade de saída controlada da VPC, utilizando o NAT Gateway previsto para o ambiente.

O fluxo de autenticação será:

```text
Cliente
  → API Gateway
  → Lambda na VPC
  → RDS privado
```

## 4. Justificativa

A autenticação por CPF exige consultar a existência e o status do cliente no banco de dados. Como o RDS deve permanecer privado, a Lambda precisa estar na mesma VPC ou possuir conectividade privada equivalente.

Anexar a Lambda à VPC permite que ela acesse o RDS sem abrir o banco para a internet. O uso de grupos de segurança específicos restringe a comunicação ao menor conjunto necessário de recursos.

A solução também permite que a Lambda utilize o mesmo segredo de JWT armazenado no AWS Secrets Manager e envie logs e traces ao Datadog, mantendo a saída de rede controlada pela VPC.

## 5. Consequências

### Positivas

- A Lambda consegue consultar o RDS privado.
- O banco de dados permanece sem acesso público.
- A comunicação é restrita por grupos de segurança.
- A autenticação por CPF não exige expor dados de clientes para a internet.
- A Lambda pode acessar o AWS Secrets Manager de forma controlada.
- A arquitetura fica alinhada ao modelo de rede privada adotado para a solução.

### Negativas e riscos

- A Lambda passa a depender da VPC, das sub-redes privadas e dos grupos de segurança.
- Configurações incorretas de rede podem causar timeout na autenticação.
- A saída para serviços externos depende da conectividade da VPC, incluindo NAT Gateway.
- A inicialização da Lambda pode ter impacto adicional por utilizar recursos de rede na VPC.
- A destruição da VPC deverá ocorrer somente após remover a Lambda e seus recursos dependentes.

## 6. Referências

- RFC-0001 — Autenticação de Clientes por CPF com Função Serverless.
- RFC-0004 — Compartilhamento de Outputs entre Esteiras via AWS Systems Manager Parameter Store.
- ADR-0007 — Topologia de Rede AWS: VPC, 2 AZs e NAT Gateway Único.
- Tech Challenge FIAP — Fase 3.