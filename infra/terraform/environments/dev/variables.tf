variable "aws_region" {
  description = "AWS region where the infrastructure will be provisioned."
  type        = string
}

variable "db_password" {
  description = "Senha do usuário administrador do banco RDS."
  type        = string
  sensitive   = true
}

variable "eks_cluster_role_name" {
  description = "Nome da IAM Role existente que sera usada pelo cluster EKS no AWS Academy."
  type        = string
}

variable "eks_node_role_name" {
  description = "Nome da IAM Role existente que sera usada pelo Managed Node Group no AWS Academy."
  type        = string
}

variable "api_deploy_enabled" {
  description = "Habilita o deploy da API no EKS gerenciado pelo Terraform."
  type        = bool
  default     = false
}

variable "api_image_uri" {
  description = "Imagem Docker completa da API publicada no ECR."
  type        = string
  default     = ""
}

variable "jwt_secret" {
  description = "Chave JWT usada pela API."
  type        = string
  sensitive   = true
  default     = ""
}

variable "webhook_token" {
  description = "Token do webhook de orcamento."
  type        = string
  sensitive   = true
  default     = ""
}
