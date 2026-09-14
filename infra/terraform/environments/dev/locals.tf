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

}
