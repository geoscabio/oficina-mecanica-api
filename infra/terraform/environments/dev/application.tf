locals {
  api_labels = {
    app = "oficina-api"
  }

  api_rds_connection_string = "Server=tcp:${module.database.db_instance_address},1433;Database=OficinaMecanicaDb;User Id=adminoficina;Password=${var.db_password};TrustServerCertificate=True;"
}

resource "kubernetes_namespace_v1" "oficina" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name = "oficina"
  }

  depends_on = [
    module.kubernetes
  ]
}

resource "kubernetes_config_map_v1" "oficina_api" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name      = "oficina-api-config"
    namespace = kubernetes_namespace_v1.oficina[0].metadata[0].name
  }

  data = {
    ASPNETCORE_ENVIRONMENT             = "Development"
    ASPNETCORE_URLS                    = "http://+:8080"
    Database__ApplyMigrationsOnStartup = "true"
    Database__SeedDemoData             = "true"
  }
}

resource "kubernetes_secret_v1" "oficina_api" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name      = "oficina-api-secret"
    namespace = kubernetes_namespace_v1.oficina[0].metadata[0].name
  }

  type = "Opaque"

  data = {
    Jwt__Secret                          = var.jwt_secret
    Integracoes__Orcamento__WebhookToken = var.webhook_token
    ConnectionStrings__DefaultConnection = local.api_rds_connection_string
  }
}

resource "kubernetes_deployment_v1" "oficina_api" {
  count = var.api_deploy_enabled ? 1 : 0

  wait_for_rollout = false

  metadata {
    name      = "oficina-api"
    namespace = kubernetes_namespace_v1.oficina[0].metadata[0].name
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
          name              = "oficina-api"
          image             = var.api_image_uri
          image_pull_policy = "Always"

          port {
            container_port = 8080
          }

          resources {
            requests = {
              cpu    = "100m"
              memory = "128Mi"
            }

            limits = {
              cpu    = "500m"
              memory = "512Mi"
            }
          }

          env_from {
            config_map_ref {
              name = kubernetes_config_map_v1.oficina_api[0].metadata[0].name
            }
          }

          env {
            name = "Jwt__Secret"

            value_from {
              secret_key_ref {
                name = kubernetes_secret_v1.oficina_api[0].metadata[0].name
                key  = "Jwt__Secret"
              }
            }
          }

          env {
            name = "Integracoes__Orcamento__WebhookToken"

            value_from {
              secret_key_ref {
                name = kubernetes_secret_v1.oficina_api[0].metadata[0].name
                key  = "Integracoes__Orcamento__WebhookToken"
              }
            }
          }

          env {
            name = "ConnectionStrings__DefaultConnection"

            value_from {
              secret_key_ref {
                name = kubernetes_secret_v1.oficina_api[0].metadata[0].name
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
    module.database,
    module.registry,
    kubernetes_config_map_v1.oficina_api,
    kubernetes_secret_v1.oficina_api
  ]
}

resource "kubernetes_service_v1" "oficina_api" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name      = "oficina-api"
    namespace = kubernetes_namespace_v1.oficina[0].metadata[0].name
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
    kubernetes_deployment_v1.oficina_api
  ]
}

resource "kubernetes_horizontal_pod_autoscaler_v2" "oficina_api" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name      = "oficina-api-hpa"
    namespace = kubernetes_namespace_v1.oficina[0].metadata[0].name
  }

  spec {
    min_replicas = 1
    max_replicas = 5

    scale_target_ref {
      api_version = "apps/v1"
      kind        = "Deployment"
      name        = kubernetes_deployment_v1.oficina_api[0].metadata[0].name
    }

    metric {
      type = "Resource"

      resource {
        name = "cpu"

        target {
          type                = "Utilization"
          average_utilization = 70
        }
      }
    }

    metric {
      type = "Resource"

      resource {
        name = "memory"

        target {
          type                = "Utilization"
          average_utilization = 80
        }
      }
    }
  }

  depends_on = [
    kubernetes_deployment_v1.oficina_api
  ]
}
