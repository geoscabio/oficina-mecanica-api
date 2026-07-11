output "ecr_repository_url" {
  description = "URL do repositorio ECR da API."
  value       = module.registry.repository_url
}

output "eks_cluster_name" {
  description = "Nome do cluster EKS."
  value       = module.kubernetes.cluster_name
}

output "eks_cluster_endpoint" {
  description = "Endpoint do cluster EKS."
  value       = module.kubernetes.cluster_endpoint
}

output "rds_endpoint" {
  description = "Endpoint completo da instancia RDS."
  value       = module.database.db_instance_endpoint
}

output "rds_address" {
  description = "Endereco da instancia RDS."
  value       = module.database.db_instance_address
}

output "api_service_hostname" {
  description = "Hostname publico do Load Balancer da API, quando disponivel."
  value       = try(kubernetes_service_v1.oficina_api[0].status[0].load_balancer[0].ingress[0].hostname, null)
}
