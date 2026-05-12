# Deployment Guide

## Prerequisites
- Docker & Docker Compose
- .NET 8.0 SDK
- Node.js 20+
- PostgreSQL 16
- Redis 7+
- RabbitMQ 3.12+

## Local Development Setup

1. Clone the repository
```bash
git clone https://github.com/yourusername/SentinelX.git
cd SentinelX
```

2. Setup environment variables
```bash
cp .env.example .env
# Edit .env with your values
```

3. Start infrastructure
```bash
cd infrastructure/scripts
chmod +x setup.sh
./setup.sh
```

4. Run migrations
```bash
dotnet ef database update --project services/auth-service-clean/SentinelX.Auth.Infrastructure
dotnet ef database update --project services/user-service-clean/SentinelX.User.Infrastructure
```

5. Build and run services
```bash
dotnet build
dotnet run --project gateway/SentinelX.Gateway.csproj
```

## Docker Compose Deployment

```bash
docker-compose up -d
```

This will start all services:
- API Gateway (port 8080)
- Auth Service (port 5001)
- User Service (port 5002)
- PostgreSQL (port 5432)
- Redis (port 6379)
- RabbitMQ (ports 5672, 15672)
- Prometheus (port 9090)
- Grafana (port 3000)
- Loki (port 3100)

## Production Deployment

### Using Kubernetes

1. Create namespace
```bash
kubectl create namespace sentinelx
```

2. Apply ConfigMaps
```bash
kubectl apply -f k8s/configmaps.yaml -n sentinelx
```

3. Apply Secrets
```bash
kubectl apply -f k8s/secrets.yaml -n sentinelx
```

4. Deploy services
```bash
kubectl apply -f k8s/deployments/ -n sentinelx
kubectl apply -f k8s/services/ -n sentinelx
```

5. Setup Ingress
```bash
kubectl apply -f k8s/ingress.yaml -n sentinelx
```

### Using Docker Swarm

```bash
docker swarm init
docker stack deploy -c docker-compose.prod.yml sentinelx
```

## Monitoring & Logging

### Access Points
- Grafana Dashboard: https://yourdomain.com/grafana
- Prometheus Metrics: https://yourdomain.com/metrics
- Kibana (if using): https://yourdomain.com/kibana

### Default Credentials
- Grafana: admin/admin
- RabbitMQ: guest/guest

## Security Checklist

- [ ] Enable HTTPS/SSL
- [ ] Configure CORS
- [ ] Setup WAF (Web Application Firewall)
- [ ] Enable authentication
- [ ] Configure rate limiting
- [ ] Setup security headers
- [ ] Enable logging and monitoring
- [ ] Rotate secrets regularly
- [ ] Enable backup strategy
- [ ] Configure DDoS protection
