resource "kubernetes_namespace_v1" "oficina_mecanica" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name = "oficina-mecanica"
  }

  depends_on = [
    module.eks
  ]
}
