module "ecr" {
  source = "../../modules/ecr"

  name = "oficina-mecanica-api"

  tags = local.common_tags
}
