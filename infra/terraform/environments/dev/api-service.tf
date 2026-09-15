resource "kubernetes_service_v1" "oficina_mecanica_api" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name      = "oficina-mecanica-api"
    namespace = kubernetes_namespace_v1.oficina_mecanica[0].metadata[0].name
  }

  spec {
    type     = "NodePort"
    selector = local.api_labels

    port {
      name        = "http"
      port        = 80
      target_port = 8080
      protocol    = "TCP"
      node_port   = tonumber(data.aws_ssm_parameter.kubernetes_api_internal_node_port.value)
    }
  }

  depends_on = [
    kubernetes_deployment_v1.oficina_mecanica_api
  ]
}
