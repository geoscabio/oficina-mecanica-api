# Database Module

## Objetivo

Este módulo é responsável por provisionar a infraestrutura de banco de dados da aplicação utilizando o Amazon Relational Database Service (RDS), disponibilizando uma instância gerenciada do SQL Server Express em ambiente AWS.

## Recursos criados

- Amazon RDS DB Subnet Group
- Amazon RDS for SQL Server Express
- Security Group dedicado ao banco de dados

## Funcionalidades

- Criação de DB Subnet Group utilizando as subnets privadas da VPC;
- Provisionamento de instância Amazon RDS SQL Server Express;
- Criação de Security Group específico para acesso ao banco;
- Restrição de acesso à porta **1433** aos CIDRs privados informados pelo ambiente;
- Banco configurado como privado (`publicly_accessible = false`);
- Aplicação das tags compartilhadas da infraestrutura.

## Variáveis de entrada

| Variável | Tipo | Descrição |
|----------|------|-----------|
| `identifier` | `string` | Identificador da instância RDS. |
| `username` | `string` | Usuário administrador da instância. |
| `password` | `string` | Senha do usuário administrador. |
| `instance_class` | `string` | Classe da instância RDS. |
| `allocated_storage` | `number` | Espaço inicial de armazenamento (GB). |
| `engine_version` | `string` | Versão do SQL Server utilizada pela instância. |
| `vpc_id` | `string` | ID da VPC onde o banco será provisionado. |
| `subnet_ids` | `list(string)` | Lista de subnets privadas utilizadas pelo DB Subnet Group. |
| `allowed_cidr_blocks` | `list(string)` | CIDRs autorizados a acessar a porta 1433 do RDS. |
| `multi_az` | `bool` | Habilita Multi-AZ. No AWS Academy permanece `false` para reduzir custo. |
| `backup_retention_period` | `number` | Retenção de backups automáticos. No AWS Academy permanece `0`. |
| `skip_final_snapshot` | `bool` | Define se o snapshot final será ignorado no destroy. No AWS Academy permanece `true`. |
| `tags` | `map(string)` | Tags aplicadas aos recursos. |

## Outputs

| Output | Descrição |
|--------|-----------|
| `db_instance_id` | ID da instância RDS. |
| `db_instance_endpoint` | Endpoint completo da instância. |
| `db_instance_address` | Endereço da instância RDS. |
| `db_instance_port` | Porta de acesso ao banco de dados. |
| `security_group_id` | ID do Security Group associado ao banco. |

## Observações

- O Amazon RDS para SQL Server Express não permite informar o parâmetro `db_name` durante a criação da instância. O banco da aplicação deverá ser criado posteriormente por meio de migrations ou scripts de inicialização.
- A versão da engine (`engine_version`) deve corresponder a uma versão suportada pela região AWS utilizada. Recomenda-se consultar as versões disponíveis antes do provisionamento utilizando:

```bash
aws rds describe-db-engine-versions \
  --engine sqlserver-ex \
  --region us-east-1
```
