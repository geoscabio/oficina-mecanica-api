locals {
  common_tags = {
    Project     = "OficinaMecanica"
    Environment = "Development"
    ManagedBy   = "Terraform"
  }

  api_labels = {
    app = "oficina-api"
  }

  api_rds_connection_string = "Server=tcp:${module.rds.db_instance_address},1433;Database=OficinaMecanicaDb;User Id=adminoficina;Password=${var.db_password};TrustServerCertificate=True;"
}