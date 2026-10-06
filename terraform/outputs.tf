output "network_name" {
  value = docker_network.app.name
}

output "volume_name" {
  value = docker_volume.postgres_data.name
}

output "api_url" {
  value = "http://localhost:${var.api_port}"
}

output "health_url" {
  value = "http://localhost:${var.api_port}/health"
}

output "postgres_port" {
  value = var.postgres_port
}
