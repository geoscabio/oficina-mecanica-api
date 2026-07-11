module "rds" {
  source = "../../modules/rds"

  identifier = "oficina-db-dev"

  username = "adminoficina"
  password = var.db_password

  instance_class    = "db.t3.micro"
  allocated_storage = 20

  engine_version = "16.00.4185.3.v1"

  # AWS Academy: reduzir custo e facilitar destroy apos demonstracao.
  backup_retention_period = 0
  deletion_protection     = false
  skip_final_snapshot     = true

  vpc_id              = module.vpc.vpc_id
  subnet_ids          = module.vpc.private_subnet_ids
  allowed_cidr_blocks = module.vpc.private_subnet_cidrs

  tags = local.common_tags
}
