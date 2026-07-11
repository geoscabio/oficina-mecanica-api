module "eks" {
  source = "../../modules/eks"

  cluster_name    = "oficina-mecanica-eks-dev"
  cluster_version = "1.33"

  eks_cluster_role_name = var.eks_cluster_role_name
  eks_node_role_name    = var.eks_node_role_name

  vpc_id             = module.vpc.vpc_id
  private_subnet_ids = module.vpc.private_subnet_ids

  # AWS Academy: restringir para o IP publico autorizado quando possivel.
  cluster_endpoint_public_access_cidrs = ["0.0.0.0/0"]

  node_group_name = "oficina-node-group-dev"

  instance_types = ["t3.small"]

  desired_size = 1
  min_size     = 1
  max_size     = 1

  tags = local.common_tags
}
