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
  name = module.kubernetes.cluster_name

  depends_on = [
    module.kubernetes
  ]
}

data "aws_eks_cluster_auth" "this" {
  name = module.kubernetes.cluster_name

  depends_on = [
    module.kubernetes
  ]
}

provider "kubernetes" {
  host                   = data.aws_eks_cluster.this.endpoint
  cluster_ca_certificate = base64decode(data.aws_eks_cluster.this.certificate_authority[0].data)
  token                  = data.aws_eks_cluster_auth.this.token
}
