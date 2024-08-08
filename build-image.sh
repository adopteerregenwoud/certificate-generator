#!/bin/bash

set -euo pipefail

docker build -t adopteerregenwoud/certificategenerator:0.01 -t adopteerregenwoud/certificategenerator:latest .
