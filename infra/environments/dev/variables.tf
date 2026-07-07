variable "aws_region" {
  description = "AWS region where the infrastructure will be provisioned."
  type        = string
}

variable "db_password" {
  description = "Senha do usuário administrador do banco RDS."
  type        = string
  sensitive   = true
}