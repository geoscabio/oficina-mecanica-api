# Decisões

## Kubernetes local

A pasta `k8s/` permanece dedicada exclusivamente à execução local do projeto.

Motivo:

A execução local utilizando Kubernetes é um dos entregáveis da Fase 2 do Tech Challenge.

---

## Deploy AWS

O deploy na AWS utilizar? recursos espec?ficos gerenciados pelo Terraform em `infra/terraform/environments/dev/`. N?o h? manifests YAML AWS duplicados no reposit?rio: o workload Kubernetes da API na AWS fica em `api-workload.tf`.

Motivo:

Evitar impacto na execução local.

---

## AWS temporária

Todo provisionamento AWS deve considerar o orçamento limitado do Learner Lab.

Motivo:

Evitar consumo indevido de créditos. Nenhum `terraform apply` manual ou via CD deve ocorrer sem aprovação explícita; no Git Flow, essa aprovação é o merge revisado para `develop`, sempre com plano de `terraform destroy` ao final da sessão.

---

## Trade-offs AWS Academy

O ambiente AWS foi desenhado para demonstração acadêmica e para caber nas limitações do AWS Academy Learner Lab. Por isso, algumas escolhas reduzem custo e risco operacional, mas não representam um desenho produtivo completo:

- O node group do EKS permanece com 1 nó para reduzir consumo de EC2.
- O RDS roda sem Multi-AZ, sem retenção de backup e com `skip_final_snapshot=true`.
- A rede utiliza um único NAT Gateway para equilibrar custo e simplicidade.
- As migrations e o seed inicial continuam no startup da API para simplificar a entrega.

Motivo:

Essas decisões deixam a solução executável no laboratório, mantêm o desenho rastreável por Terraform e evitam recursos permanentes caros. Em produção real, o baseline deveria evoluir para alta disponibilidade, backups, snapshots finais, rotação de segredos e execução controlada de migrations fora do ciclo de inicialização da API.

---

## Ambiente AWS de demonstração

O ambiente publicado na AWS roda com `ASPNETCORE_ENVIRONMENT=Staging`. Swagger e usuários demo são habilitados explicitamente via `appsettings.Staging.json` para permitir a avaliação do Tech Challenge.

Motivo:

Evitar que a exposição de Swagger e credenciais demo pareça vazamento acidental de `Development`. Este padrão é aceitável apenas para demonstração acadêmica e não deve ser replicado em produção real.
