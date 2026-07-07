module "registry" {
  source = "../../modules/registry"

  name = "oficina-api"

  tags = local.common_tags
}