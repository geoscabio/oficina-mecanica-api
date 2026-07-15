resource "kubernetes_config_map_v1" "oficina_mecanica_api" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name      = "oficina-mecanica-api-config"
    namespace = kubernetes_namespace_v1.oficina_mecanica[0].metadata[0].name
  }

  data = {
    ASPNETCORE_ENVIRONMENT             = "Staging"
    ASPNETCORE_URLS                    = "http://+:8080"
    Database__ApplyMigrationsOnStartup = "true"
    Database__SeedDemoData             = "true"
  }
}
