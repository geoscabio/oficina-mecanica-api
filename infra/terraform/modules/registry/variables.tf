variable "name" {
  description = "Nome do repositório ECR."
  type        = string
}

variable "image_tag_mutability" {
  description = "Define se as tags das imagens podem ser sobrescritas."
  type        = string
  default     = "IMMUTABLE"
}

variable "force_delete" {
  description = "Permite remover o repositorio mesmo com imagens durante terraform destroy."
  type        = bool
  default     = true
}

variable "scan_on_push" {
  description = "Habilita a varredura automática de vulnerabilidades após o push da imagem."
  type        = bool
  default     = true
}

variable "tags" {
  description = "Tags aplicadas aos recursos."
  type        = map(string)
}
