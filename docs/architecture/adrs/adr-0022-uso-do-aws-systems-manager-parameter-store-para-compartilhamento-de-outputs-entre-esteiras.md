# ADR-0022 — Uso do AWS Systems Manager Parameter Store para Compartilhamento de Outputs entre Esteiras

## Status

**Status:** Aceito  
**Data:** 31/08/2026  
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

A Fase 3 organiza a solução em múltiplos repositórios e esteiras de CI/CD, separando responsabilidades entre infraestrutura de rede, banco de dados, Kubernetes, Lambda, aplicação principal e API Gateway.

Essa separação faz com que determinados recursos sejam criados por uma esteira e utilizados posteriormente por outra. Por exemplo, a infraestrutura da VPC fornece informações necessárias para a criação do RDS e do Kubernetes, enquanto a infraestrutura do Kubernetes e da aplicação fornece informações necessárias para a configuração do API Gateway.

Era necessário definir uma estratégia para compartilhar essas informações entre as diferentes esteiras sem criar acoplamento direto entre os repositórios ou depender de valores fixos armazenados no código.

Além dos outputs dos recursos, também era necessário possuir uma forma padronizada de registrar o estado das esteiras e dos recursos provisionados.

## 2. Fatores Decisivos

- Permitir o compartilhamento de outputs entre diferentes repositórios.
- Evitar acoplamento direto entre os estados Terraform das diferentes esteiras.
- Evitar armazenar identificadores de recursos diretamente no código.
- Utilizar um serviço gerenciado pela AWS.
- Permitir que as esteiras de CI/CD consultem informações produzidas por outras esteiras.
- Manter uma convenção de nomes consistente entre ambientes e recursos.
- Permitir o armazenamento de informações como IDs, endpoints, ARNs e nomes de recursos.
- Permitir o registro do status de cada recurso provisionado.
- Facilitar a operação e a reconstrução do ambiente.
- Manter a solução simples e compatível com a arquitetura proposta para a Fase 3.

## 3. Decisão

Será utilizado o **AWS Systems Manager Parameter Store (SSM Parameter Store)** como mecanismo padrão para compartilhamento de outputs e informações de estado entre as esteiras da Fase 3.

Cada esteira será responsável por publicar no Parameter Store os outputs necessários para que outras esteiras possam consumir seus recursos.

Será adotada a seguinte convenção:

```text
/oficina-mecanica/{ambiente}/{recurso}/{output}
```

Exemplos:

```text
/oficina-mecanica/development/vpc/vpc_id
/oficina-mecanica/development/vpc/private_subnet_ids
/oficina-mecanica/development/rds/endpoint
/oficina-mecanica/development/rds/security_group_id
/oficina-mecanica/development/kubernetes/cluster_name
/oficina-mecanica/development/kubernetes/ecr_repository_url
/oficina-mecanica/development/auth-lambda/function_arn
/oficina-mecanica/development/auth-lambda/jwt_secret_arn
/oficina-mecanica/development/api/nlb_dns_name
/oficina-mecanica/development/api/nlb_listener_arn
```

Os parâmetros serão organizados de acordo com o ambiente e o recurso responsável pela publicação.

Além dos outputs, cada esteira deverá publicar um marcador de status no seguinte padrão:

```text
/oficina-mecanica/{ambiente}/status/{recurso}
```

O valor do marcador representará o estado esperado do recurso após a execução da esteira.

O SSM será utilizado como mecanismo de **compartilhamento de informações**, e não como substituto do estado do Terraform.

Cada esteira continuará mantendo seu próprio estado Terraform de forma independente.

Antes de executar operações que dependam de outro recurso, a esteira deverá consultar os parâmetros necessários no SSM.

Durante operações de `destroy`, os parâmetros do SSM não serão considerados como única fonte de verdade. A existência dos recursos deverá ser validada diretamente na AWS antes de permitir a destruição.

Caso o `LabRole` do ambiente acadêmico não permita a utilização de `ssm:PutParameter`, poderá ser utilizado como fallback o armazenamento dos outputs em **GitHub Repository Variables**, conforme previsto no plano de execução.

O SSM Parameter Store permanece, entretanto, como a estratégia principal da arquitetura.

## 4. Justificativa

A separação da infraestrutura em diferentes repositórios exige um mecanismo de comunicação entre as esteiras.

Compartilhar diretamente estados Terraform entre os repositórios criaria um acoplamento desnecessário entre as infraestruturas. Cada esteira deixaria de possuir uma responsabilidade claramente isolada e passaria a depender da estrutura interna do estado de outra esteira.

O Parameter Store permite que cada recurso publique somente as informações necessárias para os consumidores, mantendo a separação entre as responsabilidades.

A convenção de parâmetros baseada em ambiente e recurso também permite identificar facilmente a origem e o contexto de cada informação.

A utilização do SSM é adequada ao ambiente AWS porque é um serviço gerenciado e pode ser acessado pelas esteiras por meio das permissões IAM disponíveis.

A separação entre outputs e estado Terraform também é importante para a segurança e confiabilidade da solução. O parâmetro representa uma informação publicada pelo recurso, mas não substitui a validação do estado real da infraestrutura.

Essa distinção é especialmente importante durante operações de `destroy`, nas quais o plano determina que a existência dos recursos seja confirmada diretamente na AWS, evitando confiar exclusivamente nos marcadores armazenados no SSM.

A decisão também está alinhada ao plano de execução da Fase 3, que define uma convenção centralizada para os parâmetros e estabelece o SSM como mecanismo principal de compartilhamento entre as esteiras.

## 5. Consequências

### Positivas

- As esteiras podem compartilhar outputs sem compartilhar diretamente seus estados Terraform.
- Os repositórios permanecem desacoplados.
- Cada recurso possui uma responsabilidade clara sobre os outputs que publica.
- Os parâmetros possuem uma convenção de nomenclatura padronizada.
- Os valores podem ser separados por ambiente.
- Outras esteiras conseguem descobrir recursos necessários de forma padronizada.
- A operação de deploy fica menos dependente de valores fixos no código.
- O status dos recursos pode ser registrado em um local centralizado.
- A solução utiliza um serviço gerenciado da AWS.
- A estratégia facilita a reconstrução do ambiente seguindo a ordem de dependências definida na arquitetura.

### Negativas e riscos

- As esteiras passam a depender da disponibilidade e das permissões de acesso ao SSM Parameter Store.
- Parâmetros incorretos ou desatualizados podem causar falhas em esteiras dependentes.
- A remoção ou alteração indevida de parâmetros pode quebrar integrações entre recursos.
- É necessário controlar corretamente as permissões IAM para leitura e escrita dos parâmetros.
- O SSM não representa o estado real da infraestrutura e não pode ser utilizado isoladamente para validar a existência dos recursos.
- O ambiente acadêmico pode possuir restrições de permissão no `LabRole`, exigindo utilização do mecanismo de fallback previsto.
- A convenção de parâmetros precisa ser mantida durante a evolução dos repositórios para evitar inconsistências.

## 6. Referências

- Tech Challenge FIAP — Fase 3.
- Plano Final de Execução — Fase 3.
- RFC-0004 — Compartilhamento de Outputs entre Esteiras via AWS Systems Manager Parameter Store.
- ADR-0018 — Execução da Lambda de Autenticação dentro da VPC.
- ADR-0021 — Datadog como Solução de Observabilidade.
