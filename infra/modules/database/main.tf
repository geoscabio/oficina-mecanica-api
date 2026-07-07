resource "aws_db_subnet_group" "this" {
  name       = "${var.identifier}-subnet-group"
  subnet_ids = var.subnet_ids

  tags = merge(
    var.tags,
    {
      Name = "${var.identifier}-subnet-group"
    }
  )
}

resource "aws_security_group" "this" {
  name        = "${var.identifier}-sg"
  description = "Security Group do Amazon RDS"
  vpc_id      = var.vpc_id

  ingress {
    description = "SQL Server"
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = ["10.0.0.0/16"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = merge(
    var.tags,
    {
      Name = "${var.identifier}-sg"
    }
  )
}

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
  multi_az            = false

  backup_retention_period = 0

  deletion_protection = false
  skip_final_snapshot = true

  tags = merge(
    var.tags,
    {
      Name = var.identifier
    }
  )
}