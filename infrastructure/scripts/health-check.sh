#!/bin/bash

echo "Checking SentinelX services health..."

services=("gateway" "auth-service" "user-service" "postgres" "redis" "rabbitmq" "prometheus" "loki" "grafana")

for service in "${services[@]}"; do
  if docker-compose ps $service | grep -q "Up"; then
    echo "✓ $service is running"
  else
    echo "✗ $service is not running"
  fi
done

echo ""
echo "Checking port availability..."
netstat -tuln | grep -E ':(8080|5001|5002|5432|6379|15672|9090|3000)' || echo "No services responding on expected ports"
