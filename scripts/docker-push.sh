#!/usr/bin/env bash
# Manual Docker Hub push. Usage: DOCKERHUB_USERNAME=me ./scripts/docker-push.sh [version]
# Run `docker login` first (use an access token, never commit it).
set -euo pipefail
: "${DOCKERHUB_USERNAME:?set DOCKERHUB_USERNAME}"
SHA=$(git rev-parse --short HEAD)
VERSION=${1:-$SHA}
IMAGE="$DOCKERHUB_USERNAME/product-api"

docker build -f docker/Dockerfile -t "$IMAGE:$VERSION" -t "$IMAGE:$SHA" -t "$IMAGE:latest" .
docker push "$IMAGE:$VERSION"
docker push "$IMAGE:$SHA"
docker push "$IMAGE:latest"
