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
