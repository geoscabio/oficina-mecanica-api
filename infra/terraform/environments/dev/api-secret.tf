resource "kubernetes_secret_v1" "oficina_mecanica_api" {
  count = var.api_deploy_enabled ? 1 : 0

  metadata {
    name      = "oficina-mecanica-api-secret"
    namespace = kubernetes_namespace_v1.oficina_mecanica[0].metadata[0].name
  }

  type = "Opaque"

  data = {
    Jwt__Secret                          = var.jwt_secret
    Integracoes__Orcamento__WebhookToken = var.webhook_token
    ConnectionStrings__DefaultConnection = var.db_connection_string
  }

  depends_on = [
    terraform_data.rds_ready
  ]
}
