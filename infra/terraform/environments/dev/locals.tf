locals {
  ssm_base_path = "/oficina-mecanica/${var.ssm_environment_name}"
  common_tags = {
    Project     = "OficinaMecanica"
    Environment = "Development"
    ManagedBy   = "Terraform"
  }

  api_labels = {
    app = "oficina-mecanica-api"
  }

  api_version = coalesce(element(reverse(split(":", var.api_image_uri)), 0), "unknown")

  api_datadog_labels = {
    "tags.datadoghq.com/env"     = "development"
    "tags.datadoghq.com/service" = "oficina-mecanica-api"
    "tags.datadoghq.com/version" = local.api_version
  }

}
