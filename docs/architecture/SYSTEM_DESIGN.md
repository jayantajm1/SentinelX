# SentinelX Architecture

## System Overview

SentinelX is an enterprise security platform built with a microservices architecture, featuring:

- **API Gateway**: Central entry point using YARP/Ocelot
- **Microservices**: Auth, User, Audit, Notification, Security Engine
- **Data Layer**: PostgreSQL, Redis, RabbitMQ
- **Observability**: OpenTelemetry, Prometheus, Grafana, Loki
- **Frontend**: Angular Admin Dashboard

## Clean Architecture Layers

### 1. API Layer (Presentation)
- Controllers
- Middleware
- Extensions
- Request/Response handling

### 2. Application Layer
- Features (Commands/Queries)
- DTOs
- Validators
- Service implementations

### 3. Domain Layer
- Entities
- Value Objects
- Domain Events
- Interfaces (Repository contracts)

### 4. Infrastructure Layer
- DbContext
- Repository implementations
- External service integrations
- Data persistence

## Microservices

### Auth Service
- Login/Register
- JWT + Refresh Token
- Role-Based Access Control
- Device tracking
- Token blacklist

### User Service
- User management
- Profile management
- User preferences
- Demographics

### Audit Service
- Audit logging
- Security events
- API logs
- Activity tracking

### Notification Service
- Email notifications
- SMS notifications
- Push notifications
- Notification history

### Security Engine
- Threat detection
- Anomaly detection
- Rate limiting
- IP reputation
- OWASP compliance checks

## Data Flow

```
Client → Nginx (SSL/TLS) → API Gateway → Services → PostgreSQL/Redis
                                ↓
                        OpenTelemetry Collector
                                ↓
                        Prometheus/Loki/Grafana
```

## Inter-Service Communication

- **Synchronous**: REST API calls
- **Asynchronous**: RabbitMQ event bus
- **Caching**: Redis for distributed cache
- **Logging**: Loki for centralized logging

## Security Features

- HTTPS/SSL enforced
- JWT with AES encryption
- Rate limiting
- CORS configuration
- Security headers (HSTS, CSP, X-Frame-Options)
- Input validation with FluentValidation
- OWASP protection
