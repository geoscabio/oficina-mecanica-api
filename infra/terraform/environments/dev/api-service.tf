resource "kubernetes_service_v1" "oficina_api" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name      = "oficina-api"
    namespace = kubernetes_namespace_v1.oficina[0].metadata[0].name
  }

  spec {
    # EKS/AWS cria o Load Balancer externo automaticamente a partir deste Service.
    # O recurso AWS não aparece como aws_lb porque é gerenciado pelo controller cloud-provider do Kubernetes.
    type     = "LoadBalancer"
    selector = local.api_labels

    port {
      name        = "http"
      port        = 80
      target_port = 8080
      protocol    = "TCP"
    }
  }

  depends_on = [
    kubernetes_deployment_v1.oficina_api
  ]
}
