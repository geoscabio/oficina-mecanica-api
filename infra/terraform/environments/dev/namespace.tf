resource "kubernetes_namespace_v1" "oficina" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name = "oficina"
  }

  depends_on = [
    module.eks
  ]
}
