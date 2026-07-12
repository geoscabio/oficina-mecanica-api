resource "kubernetes_deployment_v1" "oficina_mecanica_api" {
  count = var.api_deploy_enabled ? 1 : 0

  wait_for_rollout = false

  metadata {
    name      = "oficina-mecanica-api"
    namespace = kubernetes_namespace_v1.oficina_mecanica[0].metadata[0].name
  }

  spec {
    replicas = 1

    selector {
      match_labels = local.api_labels
    }

    template {
      metadata {
        labels = local.api_labels
      }

      spec {
        container {
          name              = "oficina-mecanica-api"
          image             = var.api_image_uri
          image_pull_policy = "Always"

          port {
            container_port = 8080
          }

          resources {
            requests = {
              cpu    = "100m"
              memory = "256Mi"
            }

            limits = {
              cpu    = "500m"
              memory = "512Mi"
            }
          }

          env_from {
            config_map_ref {
              name = kubernetes_config_map_v1.oficina_mecanica_api[0].metadata[0].name
            }
          }

          env {
            name = "Jwt__Secret"

            value_from {
              secret_key_ref {
                name = kubernetes_secret_v1.oficina_mecanica_api[0].metadata[0].name
                key  = "Jwt__Secret"
              }
            }
          }

          env {
            name = "Integracoes__Orcamento__WebhookToken"

            value_from {
              secret_key_ref {
                name = kubernetes_secret_v1.oficina_mecanica_api[0].metadata[0].name
                key  = "Integracoes__Orcamento__WebhookToken"
              }
            }
          }

          env {
            name = "ConnectionStrings__DefaultConnection"

            value_from {
              secret_key_ref {
                name = kubernetes_secret_v1.oficina_mecanica_api[0].metadata[0].name
                key  = "ConnectionStrings__DefaultConnection"
              }
            }
          }

          startup_probe {
            http_get {
              path = "/api/health"
              port = 8080
            }

            failure_threshold = 30
            period_seconds    = 10
          }

          liveness_probe {
            http_get {
              path = "/api/health"
              port = 8080
            }

            initial_delay_seconds = 30
            period_seconds        = 15
            timeout_seconds       = 5
            failure_threshold     = 3
          }

          readiness_probe {
            http_get {
              path = "/api/health"
              port = 8080
            }

            initial_delay_seconds = 10
            period_seconds        = 10
            timeout_seconds       = 5
            failure_threshold     = 3
          }
        }
      }
    }
  }

  depends_on = [
    module.rds,
    module.ecr,
    kubernetes_config_map_v1.oficina_mecanica_api,
    kubernetes_secret_v1.oficina_mecanica_api
  ]
}
