module "ecr" {
  source = "../../modules/ecr"

  name = "oficina-api"

  tags = local.common_tags
}
