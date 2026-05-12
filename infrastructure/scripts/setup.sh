#!/bin/bash

# SentinelX Setup Script

echo "Setting up SentinelX infrastructure..."

# Create .env file if it doesn't exist
if [ ! -f .env ]; then
  cat > .env << EOF
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=sentinelx
REDIS_PASSWORD=redis_password
RABBITMQ_USER=guest
RABBITMQ_PASS=guest
JWT_SECRET=your-secret-key-change-this-in-production
OTEL_EXPORTER_OTLP_ENDPOINT=http://otel-collector:4317
LOKI_URL=http://loki:3100
PROMETHEUS_URL=http://prometheus:9090
EOF
  echo ".env file created"
fi

# Create directories
mkdir -p data/postgres
mkdir -p data/redis
mkdir -p data/loki
mkdir -p logs

# Set permissions
chmod 700 data/postgres

echo "Setting up PostgreSQL database..."
docker-compose exec -T postgres psql -U postgres -f /docker-entrypoint-initdb.d/init.sql

echo "Creating Grafana dashboards..."
# Grafana provisioning is handled by docker-compose

echo "Starting services..."
docker-compose up -d

echo "Waiting for services to be ready..."
sleep 30

echo "Creating databases..."
docker-compose exec -T postgres createdb -U postgres sentinelx_auth 2>/dev/null || true
docker-compose exec -T postgres createdb -U postgres sentinelx_user 2>/dev/null || true
docker-compose exec -T postgres createdb -U postgres sentinelx_audit 2>/dev/null || true

echo "Setup complete!"
echo "Access points:"
echo "  - API Gateway: https://localhost"
echo "  - Grafana: https://localhost/grafana"
echo "  - Prometheus: https://localhost/metrics"
