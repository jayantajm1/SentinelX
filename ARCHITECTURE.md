# SentinelX Architecture Documentation

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    Client Layer                         │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Angular Admin Dashboard (Port 4200)             │   │
│  │  ├─ Authentication Module                        │   │
│  │  ├─ User Management Module                       │   │
│  │  ├─ Security Monitoring Module                   │   │
│  │  ├─ Audit Logs Module                            │   │
│  │  └─ Dashboard/Analytics Module                   │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                         ↓ HTTPS/JWT
┌─────────────────────────────────────────────────────────┐
│                 API Gateway Layer                       │
│  ┌──────────────────────────────────────────────────┐   │
│  │  YARP Reverse Proxy (Port 5000)                  │   │
│  │  ├─ Route Aggregation                            │   │
│  │  ├─ JWT Validation                               │   │
│  │  ├─ Rate Limiting                                │   │
│  │  ├─ Request/Response Logging                     │   │
│  │  └─ Correlation ID Propagation                   │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                ↓      ↓      ↓      ↓      ↓
┌─────────────────────────────────────────────────────────┐
│              Microservices Layer                         │
│                                                          │
│  ┌──────────────┐  ┌──────────────┐  ┌────────────┐    │
│  │  Auth        │  │  User        │  │  Audit     │    │
│  │  Service     │  │  Service     │  │  Service   │    │
│  │  (5001)      │  │  (5002)      │  │  (5003)    │    │
│  └──────────────┘  └──────────────┘  └────────────┘    │
│                                                          │
│  ┌──────────────┐  ┌──────────────┐                     │
│  │  Security    │  │  Notification│                     │
│  │  Engine      │  │  Service     │                     │
│  │  (5004)      │  │  (5005)      │                     │
│  └──────────────┘  └──────────────┘                     │
│                                                          │
│  Each Service Contains:                                 │
│  ├─ Controllers (REST endpoints)                        │
│  ├─ Services (Business Logic)                           │
│  ├─ Repositories (Data Access)                          │
│  ├─ DTOs (Data Transfer Objects)                        │
│  ├─ Entities (Database Models)                          │
│  ├─ Middleware (Cross-cutting Concerns)                 │
│  └─ Configuration (Dependency Injection)                │
└─────────────────────────────────────────────────────────┘
                ↓              ↓              ↓
┌─────────────────────────────────────────────────────────┐
│            Persistence & Communication Layer             │
│                                                          │
│  ┌─────────────┐  ┌─────────────┐  ┌────────────────┐  │
│  │ PostgreSQL  │  │    Redis    │  │   RabbitMQ     │  │
│  │  (5432)     │  │   (6379)    │  │   (5672)       │  │
│  │             │  │             │  │                │  │
│  │ ├─auth_db   │  │ ├─ Token    │  │ ├─ audit-log   │  │
│  │ ├─user_db   │  │ │ Blacklist │  │ ├─ email-notif │  │
│  │ ├─audit_db  │  │ ├─ Refresh  │  │ ├─ suspicious  │  │
│  │ └─notif_db  │  │ │ Tokens    │  │ ├─ user-created│  │
│  │             │  │ ├─ Rate     │  │ └─ login-events│  │
│  │             │  │ │ Limits    │  │                │  │
│  │             │  │ └─ Cache    │  │                │  │
│  └─────────────┘  └─────────────┘  └────────────────┘  │
└─────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│          Observability & Monitoring Layer               │
│                                                          │
│  ┌────────────┐  ┌────────────┐  ┌────────────────┐   │
│  │ Prometheus │  │   Grafana  │  │     Loki       │   │
│  │  (9090)    │  │   (3000)   │  │   (3100)       │   │
│  │            │  │            │  │                │   │
│  │ ├─Metrics  │  │ ├─Dashboards│  │ ├─ Log Streams│   │
│  │ ├─Alerts   │  │ ├─Alerts   │  │ ├─ Filtering  │   │
│  │ └─Recording│  │ └─Datasrc  │  │ └─ Aggregation│   │
│  └────────────┘  └────────────┘  └────────────────┘   │
│         ↑                ↑                 ↑             │
│         └────────────────┴─────────────────┘             │
│            OpenTelemetry Collector (4317)               │
└─────────────────────────────────────────────────────────┘
```

## Component Responsibilities

### API Gateway (YARP)
- Route requests to appropriate microservices
- JWT token validation
- CORS handling
- Rate limiting enforcement
- Correlation ID injection
- Request/Response logging

### Auth Service
- User authentication (login/register)
- JWT token generation
- Refresh token management
- Password hashing (BCrypt)
- Device tracking
- Login history

### User Service
- User profile management
- User CRUD operations
- Role assignment
- Permission management
- Activity tracking
- Two-factor authentication

### Audit Service
- Audit log persistence
- Security alert logging
- Event tracking
- Compliance reporting
- Historical data analysis

### Security Engine
- Rate limiting (token bucket algorithm)
- Brute force detection
- IP-based blocking
- Suspicious activity detection
- Threat intelligence
- Security policy enforcement

### Notification Service
- Email sending
- In-app notifications
- SMS alerts (future)
- Security event notifications
- OTP delivery

## Data Flow

### Login Flow
```
1. Client: POST /api/auth/login
   ↓
2. API Gateway: Route to Auth Service
   ↓
3. Auth Service:
   a. Validate credentials
   b. Check account status
   c. Generate JWT token
   d. Create refresh token in Redis
   e. Log login event
   ↓
4. Response: {accessToken, refreshToken, expiresAt}
   ↓
5. Client: Store JWT in localStorage
```

### Request Flow
```
1. Client: GET /api/users/123
   Header: Authorization: Bearer <JWT>
   ↓
2. API Gateway:
   a. Extract JWT from header
   b. Validate JWT signature
   c. Check token expiration
   d. Check token blacklist (Redis)
   e. Inject claims into context
   ↓
3. User Service:
   a. Extract userId from claims
   b. Fetch user data
   c. Apply security rules
   ↓
4. Database: Query user from PostgreSQL
   ↓
5. Response: User data (with caching in Redis)
```

### Audit Logging Flow
```
1. User Action in any Service
   ↓
2. Service publishes AuditLogEvent to RabbitMQ
   ↓
3. Audit Service consumes from queue
   ↓
4. Save to PostgreSQL audit_logs table
   ↓
5. Publish to Prometheus/Loki
   ↓
6. Display in Grafana dashboard
```

## Security Architecture

### Authentication
- JWT tokens with 30-minute expiration
- Refresh tokens with 7-day expiration
- Token blacklist in Redis on logout
- Secure storage (HTTPS only in production)

### Authorization
- Claim-based access control
- Role-based access control (RBAC)
- Dynamic permission evaluation
- Scope-based authorization

### Encryption
- AES-256 middleware encryption for sensitive payloads
- BCrypt for password hashing
- HTTPS for all communications
- Encrypted fields in database

### Rate Limiting
- Token bucket algorithm
- Per-user limits
- Per-endpoint limits
- IP-based limits
- Configurable policies

### Threat Detection
- Brute force detection (5 failed attempts → lock)
- Suspicious activity monitoring
- Impossible travel detection
- Abnormal behavior analysis
- IP reputation tracking

## Database Schema Overview

### Auth Service (auth_db)
- Users
- Roles
- Permissions
- UserRoles
- RolePermissions
- RefreshTokens
- UserDevices
- LoginHistory

### User Service (user_db)
- UserProfiles
- UserActivity

### Audit Service (audit_db)
- AuditLogs
- SecurityAlerts

### Notification Service (notification_db)
- Notifications
- EmailQueue

## Caching Strategy

### Redis Keys
```
token-blacklist:{tokenId}
refresh-token:{userId}
user-permissions:{userId}
rate-limit:{userId}:{endpoint}
blocked-ip:{ipAddress}
user-activity:{userId}
session:{sessionId}
```

### Cache Invalidation
- TTL-based expiration
- Manual invalidation on updates
- Cascade invalidation on entity changes

## Message Queue Events

### RabbitMQ Exchanges
- `audit-exchange` → audit-log-queue
- `notification-exchange` → email-notification-queue
- `security-exchange` → suspicious-activity-queue, security-alert-queue

### Event Types
- UserCreated
- UserUpdated
- LoginAttempted
- FailedAuthentication
- RateLimitExceeded
- SuspiciousActivityDetected
- IPBlocked

## Observability

### Metrics Collected
- Request count per endpoint
- Failed authentication attempts
- Rate limited requests
- DB query latency
- Cache hit/miss ratio
- Message queue depth
- Service health

### Logs Aggregated
- Application logs (Serilog → Loki)
- Request/Response logs
- Security events
- Performance metrics

### Traces
- Distributed tracing via OpenTelemetry
- Correlation IDs across services
- End-to-end request visualization

## Scalability Considerations

### Horizontal Scaling
- Stateless services allow multiple instances
- Load balancing at gateway level
- Session storage in Redis (not in-memory)

### Vertical Scaling
- Database indexing for query performance
- Connection pooling
- Redis as distributed cache

### Performance Optimization
- Eager loading in repository patterns
- Select-only necessary columns
- Batch operations where possible
- Async/await throughout
- Caching frequently accessed data

## Disaster Recovery

### Backup Strategy
- PostgreSQL daily backups
- Redis persistence enabled
- Configuration as code

### High Availability
- Docker restart policies
- Health checks on all services
- Service discovery via DNS
- Database replication (future)

## Deployment Strategies

### Development
- Docker Compose local setup
- Hot-reload enabled
- Mock services for testing

### Production
- Kubernetes orchestration (future)
- Blue-green deployments
- Rolling updates
- Database migrations before deployment

## Integration Points

### Third-Party Services
- Email service (MailKit)
- SMS service (Twilio - future)
- LDAP/Active Directory (future)
- OAuth providers (future)
