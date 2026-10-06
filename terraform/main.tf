# Infrastructure only: network, volume, containers. No application logic here.

resource "docker_network" "app" {
  name = "${var.name_prefix}-product-net"
}

resource "docker_volume" "postgres_data" {
  name = "${var.name_prefix}-postgres-data"
}

resource "docker_image" "postgres" {
  name         = "postgres:16-alpine"
  keep_locally = true
}

# The API image comes from Docker Hub (or a local build). keep_locally avoids deleting it on destroy.
resource "docker_image" "api" {
  name         = var.api_image
  keep_locally = true
}

resource "docker_container" "postgres" {
  name    = "${var.name_prefix}-product-postgres"
  image   = docker_image.postgres.image_id
  restart = "unless-stopped"

  env = [
    "POSTGRES_DB=${var.postgres_db}",
    "POSTGRES_USER=${var.postgres_user}",
    "POSTGRES_PASSWORD=${var.postgres_password}",
  ]

  ports {
    internal = 5432
    external = var.postgres_port
  }

  volumes {
    volume_name    = docker_volume.postgres_data.name
    container_path = "/var/lib/postgresql/data"
  }

  networks_advanced {
    name    = docker_network.app.name
    aliases = ["postgres"]
  }

  healthcheck {
    test     = ["CMD-SHELL", "pg_isready -U ${var.postgres_user} -d ${var.postgres_db}"]
    interval = "5s"
    timeout  = "5s"
    retries  = 10
  }

  # Wait until the container reports healthy before creating the API.
  wait = true
}

resource "docker_container" "api" {
  name    = "${var.name_prefix}-product-api"
  image   = docker_image.api.image_id
  restart = "unless-stopped"

  env = [
    "ASPNETCORE_ENVIRONMENT=Production",
    "ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=${var.postgres_db};Username=${var.postgres_user};Password=${var.postgres_password}",
  ]

  ports {
    internal = 8080
    external = var.api_port
  }

  networks_advanced {
    name = docker_network.app.name
  }

  depends_on = [docker_container.postgres]
}
