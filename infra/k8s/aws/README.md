# Kubernetes - Ambiente AWS

Esta pasta contem os manifestos Kubernetes usados para implantar a API no Amazon EKS.

Os manifestos deste diretorio sao exclusivos para AWS e nao substituem os manifestos da pasta `k8s/`, usados na execucao local com Docker Desktop.

## Ordem de implantacao

1. Namespace
2. ConfigMap
3. Secret
4. Deployment
5. Service

## Deploy via GitHub Actions

O caminho recomendado para demonstracao na AWS Academy e o workflow `CI/CD`, executado manualmente por `workflow_dispatch` com a opcao `aws-academy-deploy`.

Esse caminho:

1. aguarda aprovacao no environment `aws-academy`;
2. autentica na AWS Academy com secrets temporarios;
3. faz build da imagem Docker;
4. envia a imagem para o ECR criado pelo Terraform;
5. configura o kubeconfig do EKS;
6. cria ou atualiza o Secret da API sem versionar valores reais;
7. aplica os manifestos desta pasta;
8. imprime o endpoint do Service `LoadBalancer`.

O campo `image` de `api-deployment.yaml` usa um placeholder. No deploy pelo GitHub Actions ele e substituido dinamicamente pela imagem enviada ao ECR.

## Cleanup

Para remover os recursos Kubernetes da demonstracao, rode o mesmo workflow com a opcao `aws-academy-destroy-k8s`.

Depois disso, execute `terraform destroy` localmente usando o mesmo estado criado pelo `terraform apply`. Nao deixar EKS, RDS, ECR, Load Balancer ou NAT ativos apos a demonstracao na AWS Academy.

## Observacoes

- O Secret nao e versionado no repositorio.
- O Secret pode ser criado manualmente ou pelo GitHub Actions a partir dos secrets do environment `aws-academy`.
- AWS Academy possui orcamento limitado: aplicar estes manifestos somente durante testes/demonstracao.
- O Service `LoadBalancer` cria recurso cobrado na AWS; nao deixar ativo apos a demonstracao.
