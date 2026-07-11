# Decisões

## Kubernetes Local

A pasta `k8s/` permanecerá dedicada exclusivamente à execução local do projeto.

Motivo:

A execução local utilizando Kubernetes é um dos entregáveis da Fase 2 do Tech Challenge.

---

## Deploy AWS

O deploy na AWS utilizará recursos específicos localizados em `infra/aws/k8s`.

Motivo:

Evitar impacto na execução local.

---

## AWS temporária

Todo provisionamento AWS deve considerar o orçamento limitado do Learner Lab.

Motivo:

Evitar consumo indevido de créditos. Nenhum `terraform apply` manual ou via CD deve ocorrer sem aprovação explícita; no Git Flow, essa aprovação é o merge revisado para `develop`, sempre com plano de `terraform destroy` ao final da sessão.

---

## Ambiente AWS de demonstração

O ambiente publicado na AWS roda com `ASPNETCORE_ENVIRONMENT=Staging`. Swagger e usuários demo são habilitados explicitamente via `appsettings.Staging.json` para permitir a avaliação do Tech Challenge.

Motivo:

Evitar que a exposição de Swagger e credenciais demo pareça vazamento acidental de `Development`. Este padrão é aceitável apenas para demonstração acadêmica e não deve ser replicado em produção real.
