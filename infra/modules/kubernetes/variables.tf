variable "eks_cluster_role_name" {
  description = "Nome da IAM Role utilizada pelo cluster EKS."
  type        = string
}

variable "eks_node_role_name" {
  description = "Nome da IAM Role utilizada pelo Managed Node Group."
  type        = string
}

variable "vpc_id" {
  description = "ID da VPC onde o cluster EKS será criado."
  type        = string
}

variable "private_subnet_ids" {
  description = "Lista de IDs das subnets privadas utilizadas pelo EKS."
  type        = list(string)
}

variable "tags" {
  description = "Tags comuns aplicadas a todos os recursos."
  type        = map(string)
  default     = {}
}

variable "cluster_name" {
  description = "Nome do cluster EKS."
  type        = string
}

variable "cluster_version" {
  description = "Versão do Kubernetes do cluster."
  type        = string
}

variable "node_group_name" {
  description = "Nome do Managed Node Group."
  type        = string
}

variable "instance_types" {
  description = "Tipos de instâncias EC2 do Node Group."
  type        = list(string)
}

variable "desired_size" {
  description = "Quantidade desejada de nós."
  type        = number
}

variable "min_size" {
  description = "Quantidade mínima de nós."
  type        = number
}

variable "max_size" {
  description = "Quantidade máxima de nós."
  type        = number
}