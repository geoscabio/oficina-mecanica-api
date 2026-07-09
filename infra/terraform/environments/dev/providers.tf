provider "aws" {
  region = var.aws_region

  default_tags {
    tags = {
      Project     = "OficinaMecanica"
      Environment = "Development"
      ManagedBy   = "Terraform"
    }
  }
}