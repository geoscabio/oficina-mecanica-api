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

variable "tags" {
  description = "Tags aplicadas aos recursos."
  type        = map(string)
}

variable "vpc_id" {
  description = "ID da VPC onde o banco será provisionado."
  type        = string
}