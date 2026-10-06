variable "docker_host" {
  description = "Docker daemon endpoint."
  type        = string
  default     = "npipe:////./pipe/docker_engine"
}

variable "name_prefix" {
  description = "Prefix for all resource names so Terraform resources do not collide with docker compose ones."
  type        = string
  default     = "tf"
}

variable "api_image" {
  description = "Full API image reference, e.g. <dockerhub-username>/product-api:<tag>."
  type        = string
  default     = "local/product-api:latest"
}

variable "api_port" {
  description = "Host port for the API."
  type        = number
  default     = 8080
}

variable "postgres_port" {
  description = "Host port for PostgreSQL."
  type        = number
  default     = 5432
}

variable "postgres_db" {
  type    = string
  default = "devops_demo"
}

variable "postgres_user" {
  type    = string
  default = "postgres"
}

variable "postgres_password" {
  description = "Set via TF_VAR_postgres_password or a git-ignored terraform.tfvars."
  type        = string
  sensitive   = true
  default     = "postgres"
}
