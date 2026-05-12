# SentinelX - Enterprise Security & API Management Platform

## Overview
SentinelX is a production-grade enterprise security platform built with ASP.NET Core microservices, Angular frontend, and comprehensive observability tools.

## Architecture

```
Angular Admin Dashboard
        ↓
  API Gateway (YARP)
        ↓
┌─────────────────────────────────────┐
│  Auth Service                       │
│  User Service                       │
│  Audit Service                      │
│  Notification Service               │
│  Security Engine                    │
└─────────────────────────────────────┘
        ↓
┌─────────────────────────────────────┐
│ PostgreSQL │ Redis │ RabbitMQ      │
└─────────────────────────────────────┘
        ↓
┌─────────────────────────────────────┐
│ OpenTelemetry → Prometheus → Grafana│
└─────────────────────────────────────┘
```

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8
- **Database**: PostgreSQL
- **Cache**: Redis
- **Message Queue**: RabbitMQ
- **Authentication**: JWT with AES Encryption
- **Architecture**: Clean Architecture + Repository Pattern

### Frontend
- **Framework**: Angular 18+
- **UI Library**: PrimeNG
- **State Management**: RxJS (optional NgRx)

### Infrastructure
- **Observability**: OpenTelemetry + Prometheus + Grafana + Loki
- **Containerization**: Docker & Docker Compose
- **API Gateway**: YARP

## Prerequisites

- .NET 8 SDK
- Docker & Docker Compose
- Node.js 18+ (for Angular)
- PostgreSQL 16+ (optional, runs in Docker)

## Quick Start

### 1. Clone Repository
```bash
git clone https://github.com/yourusername/sentinelx.git
cd SentiXPI
```

### 2. Start Infrastructure
```bash
docker-compose up -d
```

This starts:
- PostgreSQL (auth_db, user_db, audit_db, notification_db)
- Redis
- RabbitMQ
- Prometheus
- Grafana
- Loki
- OpenTelemetry Collector

### 3. Build & Run Backend Services

#### API Gateway
```bash
cd gateway
dotnet restore
dotnet run
```
Gateway runs on: `http://localhost:5000`

#### Auth Service
```bash
cd services/auth-service
dotnet restore
dotnet run
```
Auth Service runs on: `http://localhost:5001`

#### Other Services
Similar process for:
- User Service: `http://localhost:5002`
- Audit Service: `http://localhost:5003`
- Security Engine: `http://localhost:5004`
- Notification Service: `http://localhost:5005`

### 4. Frontend Setup

```bash
cd frontend
npm install
ng serve
```
Angular app runs on: `http://localhost:4200`

## Key Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/logout` - User logout
- `POST /api/auth/refresh` - Refresh JWT token
- `GET /api/auth/profile` - Get user profile (requires auth)

### Monitoring
- Grafana: `http://localhost:3000` (admin/Admin@123)
- Prometheus: `http://localhost:9090`
- RabbitMQ: `http://localhost:15672` (sentinelx/RabbitPass@123)

## Database Initialization

PostgreSQL automatically initializes with:
- User authentication tables
- Role & permission management
- Audit logging tables
- Security alerts tables
- Notification queues

See `infrastructure/postgres/init.sql` for schema.

## Configuration

### JWT Settings
Edit `services/auth-service/appsettings.json`:
```json
{
  "Jwt": {
    "SecretKey": "Your-Secret-Key-Min-32-Chars",
    "Issuer": "https://sentinelx.com",
    "Audience": "SentinelXApi",
    "ExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Redis Connection
Edit connection strings in service appsettings:
```json
{
  "ConnectionStrings": {
    "Redis": "redis-password=RedisPass@123:6379"
  }
}
```

### RabbitMQ Connection
```json
{
  "RabbitMq": {
    "Host": "rabbitmq",
    "Username": "sentinelx",
    "Password": "RabbitPass@123",
    "VirtualHost": "/sentinelx"
  }
}
```

## Development Features

### Phase 1: Foundation ✅
- [x] Solution structure
- [x] Docker Compose setup
- [x] PostgreSQL schema
- [x] API Gateway (YARP)
- [x] Redis configuration
- [x] RabbitMQ configuration

### Phase 2: Authentication ✅
- [x] Auth Service structure
- [x] JWT token generation
- [x] User login/register
- [x] Role-based authorization
- [ ] Middleware encryption
- [ ] Device tracking

### Phase 3: User Service (In Progress)
- [ ] User CRUD operations
- [ ] Profile management
- [ ] Role assignment
- [ ] Activity tracking

### Phase 4: Security Engine
- [ ] Rate limiting
- [ ] Brute force detection
- [ ] Suspicious activity detection
- [ ] IDS implementation

### Phase 5: Audit Service
- [ ] Audit log storage
- [ ] Event tracking
- [ ] RabbitMQ integration

### Phase 6: Notification Service
- [ ] Email notifications
- [ ] Security alerts
- [ ] OTP system

### Phase 7: Angular Dashboard
- [ ] Authentication UI
- [ ] Dashboard
- [ ] User management
- [ ] Security monitoring

### Phase 8: Observability
- [ ] OpenTelemetry instrumentation
- [ ] Prometheus metrics
- [ ] Grafana dashboards
- [ ] Loki log aggregation

### Phase 9: Docker Deployment
- [ ] Service Dockerfiles
- [ ] Container networking
- [ ] Health checks

### Phase 10: Advanced Features
- [ ] API key management
- [ ] WebSocket alerts
- [ ] Distributed tracing
- [ ] Analytics engine

## Security Best Practices

✓ Never expose Redis/RabbitMQ to public network
✓ Use HTTPS in production
✓ Rotate JWT secrets regularly
✓ Implement rate limiting
✓ Use correlation IDs for tracing
✓ Encrypt sensitive data
✓ Validate all DTOs
✓ Implement structured logging
✓ Use security headers (CSP, HSTS)
✓ Enable two-factor authentication

## Troubleshooting

### Port Already in Use
```bash
# Find process on port
netstat -ano | findstr :5000
# Kill process
taskkill /PID <PID> /F
```

### Database Connection Issues
```bash
# Check PostgreSQL container
docker logs sentinelx-postgres
# Verify connection string in appsettings.json
```

### RabbitMQ Connection Issues
```bash
# Access RabbitMQ management
http://localhost:15672
# Default credentials: sentinelx / RabbitPass@123
```

## Project Structure

```
SentiXPI/
├── gateway/                          # API Gateway (YARP)
├── services/
│   ├── auth-service/                # Authentication & Authorization
│   ├── user-service/                # User Management
│   ├── audit-service/               # Audit Logging
│   ├── notification-service/        # Notifications & Alerts
│   └── security-engine/             # Threat Detection & Rate Limiting
├── shared/                          # Shared DTOs, Models, Constants
├── frontend/                        # Angular Admin Dashboard
├── infrastructure/                  # Docker, configs, dashboards
│   ├── postgres/                    # PostgreSQL initialization
│   ├── prometheus/                  # Prometheus config
│   ├── loki/                        # Loki config
│   ├── otel/                        # OpenTelemetry config
│   ├── grafana/                     # Grafana dashboards
│   └── rabbitmq/                    # RabbitMQ config
└── docker-compose.yml               # Infrastructure orchestration
```

## Contributing

1. Create a feature branch
2. Commit changes with clear messages
3. Push to branch
4. Create Pull Request

## Learning Outcomes

Upon completing this project, you'll understand:

- ✓ Enterprise ASP.NET Core architecture
- ✓ Microservices communication patterns
- ✓ Security & authentication design
- ✓ Distributed systems principles
- ✓ Observability implementation
- ✓ Event-driven architecture
- ✓ DevOps with Docker
- ✓ Advanced API Gateway patterns

## License

MIT License

## Support

For issues and questions:
- GitHub Issues: [Project Issues](https://github.com/yourusername/sentinelx/issues)
- Documentation: [Project Wiki](https://github.com/yourusername/sentinelx/wiki)

---

**Built with ❤️ for Enterprise Security**
