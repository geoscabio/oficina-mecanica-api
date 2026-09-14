provider "aws" {
  region = var.aws_region

  default_tags {
    tags = {
      Project     = "OficinaMecanica"
      Environment = "Development"
      ManagedBy   = "Terraform"
    }
  }
}

data "aws_eks_cluster" "this" {
  name = data.aws_ssm_parameter.kubernetes_cluster_name.value

  depends_on = [
    terraform_data.kubernetes_ready
  ]
}

data "aws_eks_cluster_auth" "this" {
  name = data.aws_ssm_parameter.kubernetes_cluster_name.value

  depends_on = [
    terraform_data.kubernetes_ready
  ]
}

provider "kubernetes" {
  host                   = data.aws_eks_cluster.this.endpoint
  cluster_ca_certificate = base64decode(data.aws_eks_cluster.this.certificate_authority[0].data)
  token                  = data.aws_eks_cluster_auth.this.token
}
