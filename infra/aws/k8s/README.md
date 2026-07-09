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

Os workflows de CD aplicam estes manifests automaticamente quando:

- a branch é `develop`, `release`/`release/**` ou `main`;
- a validação de build, testes, cobertura, imagem Docker e manifests passa;
- a variável `AWS_DEPLOY_ENABLED=true` está configurada no repositório;
- o GitHub Environment correspondente possui secrets e variables configurados.

Mapeamento:

| Branch | Environment |
| --- | --- |
| `develop` | `development` |
| `release` ou `release/**` | `homologation` |
| `main` | `production` |

O campo `image` de `api-deployment.yaml` usa um placeholder. No deploy pelo GitHub Actions ele é substituído dinamicamente pela imagem enviada ao ECR.

## Cleanup

Para remover os recursos Kubernetes da demonstração, rode `Actions > AWS Cleanup > Run workflow` com:

- `target_environment` apontando para o ambiente correto

Depois disso, execute `terraform destroy` localmente usando o mesmo estado criado pelo `terraform apply`. Não deixe EKS, RDS, ECR, Load Balancer ou NAT ativos após a demonstração.

## Observações

- O Secret não é versionado no repositório.
- O Secret pode ser criado pelo GitHub Actions a partir dos secrets do environment.
- O Service `LoadBalancer` cria recurso cobrado na AWS; não deixar ativo após a demonstração.
