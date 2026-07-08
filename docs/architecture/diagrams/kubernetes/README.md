# Kubernetes - Diagrama do cluster local

# Objetivo

Gerar no Eraser um diagrama Kubernetes profissional com os recursos realmente existentes nos manifests em `k8s/`.

# Escopo

Cluster Kubernetes local ou compatível, namespace `oficina`, API ASP.NET Core e SQL Server local em contêiner.

# Recursos identificados no projeto

- Namespace `oficina`.
- Ingress `oficina-api` com `ingressClassName: nginx`.
- Host `oficina.local`.
- Service `oficina-api` do tipo `ClusterIP`, porta `8080`.
- Deployment `oficina-api`, imagem `gbsousadev/oficina-api:1.0`.
- Container `oficina-api`, porta `8080`.
- ConfigMap `oficina-api-config`.
- Secret `oficina-api-secret`.
- HPA `oficina-api-hpa` com 1 a 5 réplicas.
- Probes da API:
  - startup probe em `/api/health`.
  - liveness probe em `/api/health`.
  - readiness probe em `/api/health`.
- Deployment `sqlserver`, imagem `mcr.microsoft.com/mssql/server:2022-latest`.
- Service `sqlserver`, porta `1433`.
- Secret `sqlserver-secret`.
- PVC `sqlserver-pvc` com `5Gi`.

# Recursos planejados

Não representar recursos de cloud gerenciados como existentes. EKS, RDS, ALB e external secrets não aparecem nos manifests atuais.

# Recursos que não devem aparecer

- Amazon EKS.
- Amazon RDS.
- AWS Load Balancer Controller.
- External Secrets Operator.
- Cert-manager.
- Argo CD.
- Prometheus/Grafana.
- Docker Compose.

# Layout recomendado

Use uma moldura externa "Cluster Kubernetes". Dentro dela, use uma moldura "Namespace oficina".

Fluxo da esquerda para a direita:

`Cliente/Browser` -> `Ingress nginx oficina-api` -> `Service oficina-api` -> `Deployment oficina-api` -> `Pods oficina-api` -> `Service sqlserver` -> `Deployment sqlserver` -> `PVC sqlserver-pvc`.

Coloque `ConfigMap oficina-api-config` e `Secret oficina-api-secret` acima do Deployment da API, conectados ao Pod da API. Coloque `Secret sqlserver-secret` acima do Deployment do SQL Server. Coloque o HPA abaixo ou ao lado do Deployment da API, apontando para ele.

# Hierarquia visual

- Nível 1: Cluster Kubernetes.
- Nível 2: Namespace `oficina`.
- Nível 3: Entrada HTTP, workload da API, autoscaling, configuração, banco local e volume.
- Nível 4: Relações de leitura de ConfigMap/Secret e persistência em PVC.

# Fluxos

- O tráfego HTTP chega pelo Ingress `oficina-api` com host `oficina.local`.
- O Ingress encaminha para o Service `oficina-api` na porta 8080.
- O Service roteia para Pods gerenciados pelo Deployment `oficina-api`.
- A API lê variáveis do ConfigMap e segredos do Secret.
- A API acessa o Service `sqlserver` na porta 1433.
- O Service `sqlserver` encaminha para Pods do Deployment `sqlserver`.
- O SQL Server persiste dados no PVC `sqlserver-pvc`.
- O HPA escala o Deployment `oficina-api` de 1 a 5 réplicas por CPU e memória.

# Prompt final para o Eraser

Crie um Kubernetes Architecture Diagram no Eraser usando ícones oficiais Kubernetes. Mostre uma moldura externa "Cluster Kubernetes" e dentro dela uma moldura "Namespace oficina". À esquerda, coloque um usuário ou browser acessando o Ingress "oficina-api" com ingressClassName nginx e host oficina.local. Conecte o Ingress ao Service "oficina-api" do tipo ClusterIP na porta 8080. Conecte o Service ao Deployment "oficina-api", que cria Pods com container "oficina-api", imagem gbsousadev/oficina-api:1.0, porta 8080 e probes em /api/health. Acima do Deployment da API, coloque ConfigMap "oficina-api-config" e Secret "oficina-api-secret" conectados aos Pods da API como fontes de variáveis de ambiente. Ao lado do Deployment da API, coloque HPA "oficina-api-hpa" com minReplicas 1, maxReplicas 5, CPU 70% e memória 80%, apontando para o Deployment. À direita, mostre Service "sqlserver" porta 1433 conectado ao Deployment "sqlserver", com container SQL Server 2022 usando imagem mcr.microsoft.com/mssql/server:2022-latest. Acima do SQL Server, coloque Secret "sqlserver-secret". Abaixo, coloque PVC "sqlserver-pvc" com 5Gi conectado ao Deployment SQL Server para persistência em /var/opt/mssql. Não desenhe EKS, RDS, ALB, AWS Load Balancer Controller, External Secrets, cert-manager, Argo CD, Prometheus ou Docker Compose. Use fundo claro, hierarquia visual limpa e ícones oficiais Kubernetes.
