variable "identifier" {
  description = "Identificador da instância RDS."
  type        = string
}

variable "username" {
  description = "Usuário administrador do banco."
  type        = string
}

variable "password" {
  description = "Senha do usuário administrador."
  type        = string
  sensitive   = true
}

variable "instance_class" {
  description = "Classe da instância RDS."
  type        = string
}

variable "allocated_storage" {
  description = "Espaço inicial em GB."
  type        = number
}

variable "engine_version" {
  description = "Versão do SQL Server."
  type        = string
}

variable "subnet_ids" {
  description = "Subnets privadas utilizadas pelo RDS."
  type        = list(string)
}

variable "allowed_cidr_blocks" {
  description = "CIDRs autorizados a acessar a porta 1433 do RDS."
  type        = list(string)
  default     = ["10.0.0.0/16"]
}

variable "multi_az" {
  description = "Habilita Multi-AZ para o RDS."
  type        = bool
  default     = false
}

variable "backup_retention_period" {
  description = "Retencao de backups automatizados em dias."
  type        = number
  default     = 0
}

variable "deletion_protection" {
  description = "Impede exclusao acidental da instancia RDS."
  type        = bool
  default     = false
}

variable "skip_final_snapshot" {
  description = "Ignora snapshot final ao destruir a instancia."
  type        = bool
  default     = true
}

variable "tags" {
  description = "Tags aplicadas aos recursos."
  type        = map(string)
}

variable "vpc_id" {
  description = "ID da VPC onde o banco será provisionado."
  type        = string
}
