#!/bin/bash

set -euo pipefail

docker run \
  --volume=/:/rootfs:ro \
  --volume=/var/run:/var/run:ro \
  --volume=/sys:/sys:ro \
  --volume=/var/lib/docker/:/var/lib/docker:ro \
  --volume=/dev/disk/:/dev/disk:ro \
  --publish=8081:8080 \
  --detach=true \
  --rm \
  --name=cadvisor \
  gcr.io/cadvisor/cadvisor:latest

echo cAdvisor running on http://localhost:8081
