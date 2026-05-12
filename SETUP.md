# Quick Setup Guide

## 1. Prerequisites Check
```bash
dotnet --version          # Should be 8.0+
docker --version          # Any recent version
docker-compose --version  # Version 2.0+
node --version           # Should be 18+
```

## 2. Start Docker Infrastructure
```bash
cd d:\SentiXPI
docker-compose up -d
```

Wait for all containers to be healthy (2-3 minutes):
```bash
docker-compose ps
```

## 3. Database Migration
PostgreSQL auto-initializes, but verify:
```bash
docker exec sentinelx-postgres psql -U sentinelx_admin -d auth_db -c "\dt"
```

## 4. Build Solution
```bash
# Navigate to each service and run:
cd gateway
dotnet build
cd ../services/auth-service
dotnet build
# ... repeat for other services
```

## 5. Run Services (separate terminals)

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

## 6. Test API Gateway
```bash
curl http://localhost:5000/health/ready
```

## 7. Test Authentication
```bash
# Register
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email":"admin@sentinelx.com",
    "username":"admin",
    "password":"Admin@123",
    "firstName":"Admin",
    "lastName":"User"
  }'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email":"admin@sentinelx.com",
    "password":"Admin@123"
  }'
```

## 8. Access Dashboards

- Grafana: http://localhost:3000 (admin / Admin@123)
- Prometheus: http://localhost:9090
- RabbitMQ: http://localhost:15672 (sentinelx / RabbitPass@123)

## 9. Setup Frontend (optional)
```bash
cd d:\SentiXPI\frontend
npm install
ng serve
# Access at http://localhost:4200
```

## Environment Variables

Create `.env` if needed:
```bash
ASPNETCORE_ENVIRONMENT=Development
PostgreSQL_Password=SecurePass@123
Redis_Password=RedisPass@123
RabbitMQ_Password=RabbitPass@123
JWT_SecretKey=YourSecureKeyWithMinimum32Characters
```

## Troubleshooting

**Port conflicts:**
```bash
netstat -ano | findstr :5000  # Find process
taskkill /PID <PID> /F        # Kill it
```

**Docker issues:**
```bash
docker-compose down
docker volume prune
docker-compose up -d
```

**Database issues:**
```bash
docker exec sentinelx-postgres psql -U sentinelx_admin -l
# Check if databases exist
```

## Next Steps

1. Implement User Service
2. Setup Audit logging via RabbitMQ
3. Add Security Engine rate limiting
4. Build Angular dashboard
5. Configure OpenTelemetry instrumentation
