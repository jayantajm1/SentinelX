# Development Scripts

## Prerequisites
- .NET 8 SDK installed
- Docker & Docker Compose installed
- Node.js 18+ installed (for Angular)

## Scripts

### Start Infrastructure
```bash
# Start all containers
./start-infrastructure.sh

# Stop all containers
./stop-infrastructure.sh

# Reset everything
./reset-infrastructure.sh
```

### Build Services
```bash
# Build all services
./build-all-services.sh

# Build specific service
./build-service.sh auth-service
```

### Run Services
```bash
# Run specific service (requires building first)
./run-service.sh auth-service

# Run all services
./run-all-services.sh
```

### Database Migrations
```bash
# Create migration for auth-service
./create-migration.sh auth-service AddAuditLogging

# Apply migrations
./apply-migrations.sh
```

### Testing
```bash
# Test API endpoints
./test-api.sh

# Test load
./load-test.sh

# Test security
./security-test.sh
```

## Manual Commands

### Start Infrastructure
```bash
cd d:\SentiXPI
docker-compose up -d
```

### Build All Services
```bash
cd d:\SentiXPI

# Auth Service
cd gateway && dotnet build && cd ..
cd services/auth-service && dotnet build && cd ../..
cd services/user-service && dotnet build && cd ../..
cd services/audit-service && dotnet build && cd ../..
cd services/notification-service && dotnet build && cd ../..
cd services/security-engine && dotnet build && cd ../..
```

### Run Services (Separate Terminals)

Terminal 1 - API Gateway:
```bash
cd d:\SentiXPI\gateway
dotnet run
```

Terminal 2 - Auth Service:
```bash
cd d:\SentiXPI\services\auth-service
dotnet run
```

Terminal 3 - Other Services:
```bash
cd d:\SentiXPI\services\user-service
dotnet run
```

### Run Angular Frontend
```bash
cd d:\SentiXPI\frontend
npm install
ng serve
# Access at http://localhost:4200
```

## Docker Operations

### View Container Status
```bash
docker-compose ps
```

### View Container Logs
```bash
# All containers
docker-compose logs -f

# Specific container
docker-compose logs -f postgres
```

### Execute Commands in Container
```bash
# PostgreSQL
docker exec -it sentinelx-postgres psql -U sentinelx_admin

# Redis CLI
docker exec -it sentinelx-redis redis-cli

# RabbitMQ Management
# Open http://localhost:15672
```

### Restart Services
```bash
# Single service
docker-compose restart postgres

# All services
docker-compose restart
```

### Remove All Data
```bash
docker-compose down -v
docker-compose up -d
```

## API Gateway Port Forwarding

The API Gateway at `http://localhost:5000` forwards to:
- `/api/auth/*` → Auth Service (5001)
- `/api/users/*` → User Service (5002)
- `/api/audit/*` → Audit Service (5003)
- `/api/security/*` → Security Engine (5004)
- `/api/notifications/*` → Notification Service (5005)

## Useful Ports

- **Frontend**: 4200 (Angular)
- **API Gateway**: 5000
- **Auth Service**: 5001
- **User Service**: 5002
- **Audit Service**: 5003
- **Security Engine**: 5004
- **Notification Service**: 5005
- **PostgreSQL**: 5432
- **Redis**: 6379
- **RabbitMQ**: 5672 (message broker), 15672 (management)
- **Prometheus**: 9090
- **Grafana**: 3000
- **Loki**: 3100
- **OpenTelemetry**: 4317 (gRPC), 4318 (HTTP)

## Troubleshooting

### Port Already in Use
```powershell
# Find process using port
netstat -ano | findstr :5000

# Kill process
taskkill /PID <PID> /F
```

### Docker Compose Issues
```bash
# Remove all containers
docker-compose down

# Rebuild images
docker-compose build

# Start fresh
docker-compose up -d
```

### Database Connection Issues
```bash
# Check PostgreSQL is running
docker exec sentinelx-postgres pg_isready

# Check Redis
docker exec sentinelx-redis redis-cli ping

# Check RabbitMQ
docker exec sentinelx-rabbitmq rabbitmq-diagnostics ping
```

### Service Build Errors
```bash
# Clean build
cd services/auth-service
dotnet clean
dotnet restore
dotnet build
```
