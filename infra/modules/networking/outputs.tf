output "vpc_id" {
  description = "ID of the created VPC."
  value       = aws_vpc.this.id
}