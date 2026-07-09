# Deploy na AWS Academy

> Obrigatorio: antes de qualquer criacao de recurso, ler `docs/deploy/aws-academy-guardrails.md`.

## Provisionamento da infraestrutura

- [ ] Iniciar o Learner Lab no AWS Academy
- [ ] Configurar credenciais temporarias da AWS Academy
- [ ] Configurar `TF_VAR_db_password` fora do repositorio
- [ ] Configurar `TF_VAR_eks_cluster_role_name` e `TF_VAR_eks_node_role_name` com roles existentes no lab
- [ ] Executar `terraform plan`
- [ ] Executar `terraform apply` somente com aprovacao explicita

As roles EKS variam entre labs/sessoes. Se o lab permitir consulta de IAM, liste candidatas com:

```powershell
aws iam list-roles --profile academy --query "Roles[?contains(RoleName, 'LabEks')].[RoleName]" --output table
```

Se nenhuma role EKS existir, nao executar `terraform apply` ate validar a estrategia de IAM do ambiente.

## Configuracao do cluster

- [ ] Atualizar o kubeconfig
- [ ] Validar acesso ao EKS com `kubectl get nodes`

## Implantacao da aplicacao

- [ ] Criar Namespace
- [ ] Criar ConfigMap
- [ ] Criar Secret manualmente no cluster
- [ ] Implantar Deployment
- [ ] Validar `/api/health`

## Exposicao da aplicacao

- [ ] Criar Service LoadBalancer somente durante a demonstracao
- [ ] Remover Service LoadBalancer ao finalizar o teste

## Encerramento obrigatorio

- [ ] Executar `kubectl delete -f infra/k8s/aws/`
- [ ] Executar `terraform destroy`
- [ ] Conferir que nao restaram EKS, EC2, RDS, NAT Gateway ou Load Balancer ativos

## Automacao futura

- [ ] GitHub Actions com etapa manual de aprovacao
- [ ] Job de destruicao documentado e validado
