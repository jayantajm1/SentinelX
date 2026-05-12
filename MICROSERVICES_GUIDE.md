# Microservices Development Guide

## Service Communication Pattern

```
Client (Angular)
    ↓
API Gateway (YARP) - Port 5000
    ↓
Microservices:
- Auth Service (5001)
- User Service (5002)
- Audit Service (5003)
- Security Engine (5004)
- Notification Service (5005)
    ↓
Databases:
- PostgreSQL (5432)
- Redis (6379)
- RabbitMQ (5672)
```

## Authentication Flow

1. **User Login**: `POST /api/auth/login`
   - Email + Password
   - Returns JWT Access Token + Refresh Token

2. **JWT Validation**: On every request
   - API Gateway validates JWT
   - Claims contain UserId, Role, Permissions
   - Expires in 30 minutes

3. **Token Refresh**: `POST /api/auth/refresh`
   - Refresh token valid for 7 days
   - Returns new JWT Access Token

4. **Logout**: `POST /api/auth/logout`
   - Token added to Redis blacklist
   - Session invalidated

## Adding a New Service

### Step 1: Create Project Structure
```bash
mkdir services/new-service
cd services/new-service
dotnet new webapi -name SentinelX.NewService
```

### Step 2: Create Files
- `SentinelX.NewService.csproj` - Project file
- `Program.cs` - Configuration
- `appsettings.json` - Settings
- `Data/` - Database context
- `Controllers/` - REST endpoints
- `Services/` - Business logic
- `DTOs/` - Data transfer objects

### Step 3: Update docker-compose.yml
Add service and dependencies

### Step 4: Register in API Gateway
Update `gateway/appsettings.json` routes

### Step 5: Database (if needed)
Add PostgreSQL tables to `infrastructure/postgres/init.sql`

## Testing Services

### Test Auth Service
```bash
# Register
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email":"test@sentinelx.com",
    "username":"testuser",
    "password":"Test@123"
  }'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email":"test@sentinelx.com",
    "password":"Test@123"
  }'

# With JWT Token
curl -H "Authorization: Bearer <TOKEN>" \
  http://localhost:5000/api/auth/profile
```

### Test Other Services
```bash
# User Service
curl -H "Authorization: Bearer <TOKEN>" \
  http://localhost:5000/api/users/1

# Audit Service
curl -H "Authorization: Bearer <TOKEN>" \
  http://localhost:5000/api/audit/logs

# Security Service
curl http://localhost:5000/api/security/rate-limit/user123

# Notification Service
curl -H "Authorization: Bearer <TOKEN>" \
  http://localhost:5000/api/notifications/user/1
```

## Database Queries

### Connect to PostgreSQL
```bash
docker exec -it sentinelx-postgres psql -U sentinelx_admin -d auth_db
```

### Common Queries
```sql
-- View users
SELECT * FROM users;

-- View audit logs
SELECT * FROM audit_logs ORDER BY created_at DESC;

-- Check rates
SELECT * FROM rate_limit:* (from Redis CLI)
```

### Connect to Redis
```bash
docker exec -it sentinelx-redis redis-cli
AUTH RedisPass@123
KEYS *
GET some-key
```

## Logging & Monitoring

### View Logs
```bash
# Auth Service
docker logs sentinelx-auth-service

# Gateway
docker logs sentinelx-api-gateway
```

### Monitoring Dashboards
- **Grafana**: http://localhost:3000
- **Prometheus**: http://localhost:9090
- **Loki**: http://localhost:3100

## Common Tasks

### View Service Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f auth-service
```

### Restart Service
```bash
docker-compose restart auth-service
```

### Rebuild Service
```bash
docker-compose build auth-service
```

### Check Health
```bash
curl http://localhost:5001/health/ready
```

## Architecture Patterns

### Repository Pattern
```csharp
public interface IUserRepository
{
    Task<User> GetByIdAsync(long id);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
}
```

### Dependency Injection
```csharp
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

### Middleware Pattern
```csharp
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<AesDecryptionMiddleware>();
```

### Service Communication
- Synchronous: HTTP REST via API Gateway
- Asynchronous: RabbitMQ message queues

## Environment Variables

Create `.env` or use `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;...",
    "Redis": "redis-password=XXX:6379"
  },
  "Jwt": {
    "SecretKey": "Your-Secret-Key"
  }
}
```

## Performance Tips

1. **Caching**: Use Redis for frequently accessed data
2. **Database**: Use indexes on frequently queried columns
3. **Async/Await**: Always use async for I/O operations
4. **Batch Operations**: Group database operations
5. **Rate Limiting**: Use Redis token bucket algorithm
6. **Correlation IDs**: Trace requests across services

## Security Checklist

- [ ] Never expose Redis to public network
- [ ] Use HTTPS in production
- [ ] Rotate JWT secrets regularly
- [ ] Validate all DTOs
- [ ] Use SQL parameterized queries
- [ ] Implement CORS properly
- [ ] Use secure password hashing (BCrypt)
- [ ] Implement rate limiting
- [ ] Log security events
- [ ] Monitor for suspicious activity
