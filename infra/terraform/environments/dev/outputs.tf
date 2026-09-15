output "ecr_repository_url" {
  description = "URL do repositorio ECR da API."
  value       = nonsensitive(data.aws_ssm_parameter.kubernetes_ecr_repository_url.value)
}

output "eks_cluster_name" {
  description = "Nome do cluster EKS."
  value       = nonsensitive(data.aws_ssm_parameter.kubernetes_cluster_name.value)
}

output "api_service_hostname" {
  description = "Hostname publico do Load Balancer da API, quando disponivel."
  value       = try(kubernetes_service_v1.oficina_mecanica_api[0].status[0].load_balancer[0].ingress[0].hostname, null)
}

output "api_internal_service_hostname" {
  description = "Hostname privado do NLB interno da API, quando disponível."
  value       = try(kubernetes_service_v1.oficina_mecanica_api_internal[0].status[0].load_balancer[0].ingress[0].hostname, null)
}
