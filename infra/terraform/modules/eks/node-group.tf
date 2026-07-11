resource "aws_eks_node_group" "this" {
  cluster_name    = aws_eks_cluster.this.name
  node_group_name = var.node_group_name
  node_role_arn   = data.aws_iam_role.eks_node.arn

  subnet_ids = var.private_subnet_ids

  instance_types = var.instance_types

  scaling_config {
    desired_size = var.desired_size
    min_size     = var.min_size
    max_size     = var.max_size
  }

  capacity_type = "ON_DEMAND"

  update_config {
    max_unavailable = 1
  }

  tags = merge(
    var.tags,
    {
      Name = var.node_group_name
    }
  )

  depends_on = [
    aws_eks_cluster.this
  ]
}