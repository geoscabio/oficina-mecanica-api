# Kubernetes - Ambiente AWS

Esta pasta contém os manifests Kubernetes de referência para implantar a API no Amazon EKS.

Os manifests deste diretório são exclusivos para AWS e não substituem os manifests da pasta `deploy/kubernetes/local/`, usados na execução local com Docker Desktop.

## Ordem de implantação

1. Namespace
2. ConfigMap
3. Secret
4. Deployment
5. Service
6. HorizontalPodAutoscaler

## Deploy via GitHub Actions

O workflow `CD Development` gerencia os mesmos recursos Kubernetes via Terraform, para que `terraform destroy` remova também Service/Load Balancer, Deployment, ConfigMap, Secret e Namespace.

Estes arquivos continuam versionados como referência operacional e para validação client-side no CI.

O ambiente publicado na AWS roda com `ASPNETCORE_ENVIRONMENT=Staging`. Swagger e usuários demo são habilitados explicitamente via `appsettings.Staging.json` para permitir a avaliação do Tech Challenge; nunca replicar este padrão, com credenciais fixas e Swagger público, em um ambiente de produção real.

Mapeamento:

| Branch | Environment |
| --- | --- |
| `develop` | `development` |

O campo `image` de `api-deployment.yaml` usa um placeholder porque o deploy real recebe a imagem gerada no ECR pela esteira.

O HPA da API tambem e gerenciado pelo Terraform no deploy real e depende do Metrics Server instalado no EKS para coletar metricas de CPU e memoria.

## Destroy

Ao encerrar a demonstração, executar `terraform destroy` usando o mesmo backend/state da esteira.

Não deixe EKS, RDS, ECR, Load Balancer, NAT Gateway ou EC2 ativos após a demonstração.

## Observações

- O Secret não é versionado no repositório.
- O Secret é criado pelo Terraform a partir dos secrets do GitHub Environment.
- O Service `LoadBalancer` cria recurso cobrado na AWS; remover com `terraform destroy` ao finalizar.
- O HPA escala somente a aplicacao `oficina-api`; o banco permanece no Amazon RDS.
