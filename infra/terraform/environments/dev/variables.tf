variable "aws_region" {
  description = "AWS region where the infrastructure will be provisioned."
  type        = string
}

variable "ssm_environment_name" {
  description = "Nome do ambiente usado nos paths do SSM."
  type        = string
  default     = "development"
}

variable "db_connection_string" {
  description = "Connection string SQL Server da API montada pela esteira a partir do RDS compartilhado."
  type        = string
  sensitive   = true
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
