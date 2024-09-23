#/bin/bash

set -euo pipefail

for i in {1..100}; do
    curl -v -X GET "http://localhost:8080/certificate?name=JohnDoe&squareMeters=100&year=2024&month=9&day=23&language=dutch" \
        -H "X-API-TOKEN: secret" --output /dev/null
done
