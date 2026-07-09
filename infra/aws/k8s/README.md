# Kubernetes - Ambiente AWS

Esta pasta contém os manifests Kubernetes usados para implantar a API no Amazon EKS.

Os manifests deste diretório são exclusivos para AWS e não substituem os manifests da pasta `k8s/`, usados na execução local com Docker Desktop.

## Ordem de implantação

1. Namespace
2. ConfigMap
3. Secret
4. Deployment
5. Service

## Deploy via GitHub Actions

O workflow `CD Development` aplica estes manifests automaticamente quando:

- a branch é `develop`;
- o GitHub Environment correspondente possui secrets e variables configurados.

Mapeamento:

| Branch | Environment |
| --- | --- |
| `develop` | `development` |

O campo `image` de `api-deployment.yaml` usa um placeholder. No deploy pelo GitHub Actions ele é substituído dinamicamente pela imagem enviada ao ECR.

## Cleanup

Cleanup e destroy não ficam na esteira. Antes de encerrar a demonstração, alterar `infra/aws/lifecycle.yml` para `destroy: true` como sinal operacional do time.

Depois, use `kubectl delete` localmente com as credenciais AWS ativas e execute `terraform destroy` usando o mesmo estado criado pelo `terraform apply`.

Não deixe EKS, RDS, ECR, Load Balancer ou NAT ativos após a demonstração.

## Observações

- O Secret não é versionado no repositório.
- O Secret pode ser criado pelo GitHub Actions a partir dos secrets do environment.
- O Service `LoadBalancer` cria recurso cobrado na AWS; não deixar ativo após a demonstração.
