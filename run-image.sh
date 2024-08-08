#!/bin/bash

set -euo pipefail

docker run -d -p 8080:8080 --name certificategenerator --rm \
    -e ASPNETCORE_ENVIRONMENT=Development \
    -e ApiConfiguration__TemplateDirectory=/app/templates \
    -e ApiConfiguration__ApiToken=secret \
    -v ./bestanden/templates:/app/templates:ro \
    adopteerregenwoud/certificategenerator
