# Kubernetes - Ambiente AWS

Esta pasta contém os manifestos Kubernetes utilizados para implantação da aplicação no Amazon EKS.

Os manifestos presentes neste diretório são exclusivos para o ambiente AWS e não substituem os manifestos da pasta `k8s/`, utilizados para execução local do projeto.

## Ordem de implantação

1. Namespace
2. ConfigMap
3. Secret
4. Deployment
5. Service

## Observações

- O Secret não é versionado no repositório.
- O Secret é criado manualmente (ou futuramente via GitHub Actions) antes da implantação da aplicação.
- AWS Academy possui orçamento limitado: aplicar estes manifestos somente durante testes/demonstração e remover com `kubectl delete -f infra/k8s/aws/` ao finalizar.
- O Service `LoadBalancer` cria recurso cobrado na AWS; não deixar ativo após a demonstração.
