locals {
  common_tags = {
    Project     = "OficinaMecanica"
    Environment = "Development"
    ManagedBy   = "Terraform"
  }

  api_labels = {
    app = "oficina-mecanica-api"
  }

}
