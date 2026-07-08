module "kubernetes" {
  source = "../../modules/kubernetes"

  cluster_name    = "oficina-mecanica-eks-dev"
  cluster_version = "1.33"

  eks_cluster_role_name = "c213429a5396203l15787390t1w992306-LabEksClusterRole-Q53sTa5iyapx"
  eks_node_role_name    = "c213429a5396203l15787390t1w992306886-LabEksNodeRole-KA5FC0P6vb5f"

  vpc_id             = module.networking.vpc_id
  private_subnet_ids = module.networking.private_subnet_ids

  node_group_name = "oficina-node-group-dev"

  instance_types = ["t3.small"]

  desired_size = 1
  min_size     = 1
  max_size     = 2

  tags = local.common_tags
}