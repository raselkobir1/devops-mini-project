terraform {
  required_version = ">= 1.5.0"

  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "~> 3.0"
    }
  }
}

# Windows (Docker Desktop): npipe:////./pipe/docker_engine
# Linux / macOS / remote later: unix:///var/run/docker.sock  (or ssh://user@host)
provider "docker" {
  host = var.docker_host
}
