resource "aws_db_instance" "this" {
  identifier = var.identifier

  engine         = "sqlserver-ex"
  engine_version = var.engine_version

  instance_class    = var.instance_class
  allocated_storage = var.allocated_storage

  username = var.username
  password = var.password

  db_subnet_group_name   = aws_db_subnet_group.this.name
  vpc_security_group_ids = [aws_security_group.this.id]

  publicly_accessible = false
  multi_az            = var.multi_az

  backup_retention_period = var.backup_retention_period

  deletion_protection = var.deletion_protection
  skip_final_snapshot = var.skip_final_snapshot

  tags = merge(
    var.tags,
    {
      Name = var.identifier
    }
  )
}
