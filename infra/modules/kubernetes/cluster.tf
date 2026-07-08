resource "aws_eks_cluster" "this" {
  name = var.cluster_name
  version = var.cluster_version
  
  role_arn = data.aws_iam_role.eks_cluster.arn

  access_config {
    authentication_mode                         = "API"
    bootstrap_cluster_creator_admin_permissions = true
  }

  vpc_config {
    subnet_ids = var.private_subnet_ids

    endpoint_private_access = true
    endpoint_public_access  = true
    
    public_access_cidrs = ["0.0.0.0/0"]
  }

  tags = merge(
    var.tags,
    {
      Name = "oficina-mecanica-eks"
    }
  )
}