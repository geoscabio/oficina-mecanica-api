# Decisões

## Kubernetes Local

A pasta `k8s/` permanecerá dedicada exclusivamente à execução local do projeto.

Motivo:

A execução local utilizando Kubernetes é um dos entregáveis da Fase 2 do Tech Challenge.

---

## Deploy AWS

O deploy na AWS utilizará recursos específicos localizados em `infra/k8s/aws`.

Motivo:

Evitar impacto na execução local.

---

## AWS Academy

Todo provisionamento AWS deve considerar o orçamento limitado do Learner Lab.

Motivo:

Evitar consumo indevido de créditos. Nenhum `terraform apply` deve ser executado sem aprovação explícita e sem plano de `terraform destroy` ao final da sessão.
