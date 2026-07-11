# EKS Module

## Objetivo

Este modulo provisiona o cluster Amazon EKS e o Managed Node Group usados para executar a API no ambiente AWS Academy.

## Recursos criados

- Amazon EKS Cluster
- Amazon EKS Managed Node Group
- Referencias a IAM Roles existentes no laboratorio

As roles nao sao criadas por este modulo. Elas devem existir no AWS Academy e ser informadas pelo ambiente:

```powershell
$env:TF_VAR_eks_cluster_role_name = "<LabEksClusterRole-...>"
$env:TF_VAR_eks_node_role_name = "<LabEksNodeRole-...>"
```

## Decisoes para AWS Academy

- Os nos usam subnets privadas.
- O endpoint privado do cluster fica habilitado.
- O endpoint publico fica habilitado para facilitar a operacao no laboratorio.
- O ambiente `dev` limita o Node Group a `desired_size = 1`, `min_size = 1` e `max_size = 1` para evitar crescimento de custo.

## Endurecimento recomendado

Antes de aplicar em AWS, restrinja `cluster_endpoint_public_access_cidrs` para o IP publico autorizado quando possivel:

```hcl
cluster_endpoint_public_access_cidrs = ["203.0.113.10/32"]
```

Ao final de qualquer teste, executar `terraform destroy` para encerrar os recursos cobrados.
