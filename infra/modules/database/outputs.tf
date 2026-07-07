output "db_instance_id" {
  description = "ID da instância RDS."
  value       = aws_db_instance.this.id
}

output "db_instance_endpoint" {
  description = "Endpoint da instância RDS."
  value       = aws_db_instance.this.endpoint
}

output "db_instance_address" {
  description = "Endereço da instância RDS."
  value       = aws_db_instance.this.address
}

output "db_instance_port" {
  description = "Porta da instância RDS."
  value       = aws_db_instance.this.port
}

output "security_group_id" {
  description = "ID do Security Group do banco."
  value       = aws_security_group.this.id
}