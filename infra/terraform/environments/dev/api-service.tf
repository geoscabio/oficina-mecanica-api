resource "kubernetes_service_v1" "oficina_mecanica_api" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name      = "oficina-mecanica-api"
    namespace = kubernetes_namespace_v1.oficina_mecanica[0].metadata[0].name
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
    kubernetes_deployment_v1.oficina_mecanica_api
  ]
}

resource "kubernetes_service_v1" "oficina_mecanica_api_internal" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name      = "oficina-mecanica-api-internal"
    namespace = kubernetes_namespace_v1.oficina_mecanica[0].metadata[0].name

    annotations = {
      "service.beta.kubernetes.io/aws-load-balancer-type"                     = "nlb"
      "service.beta.kubernetes.io/aws-load-balancer-internal"                 = "true"
      "service.beta.kubernetes.io/aws-load-balancer-additional-resource-tags" = "Project=OficinaMecanica,Environment=Development,Component=ApiInternalNlb"
    }
  }

  spec {
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
    kubernetes_deployment_v1.oficina_mecanica_api
  ]
}
