# System Architecture Diagram

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                   Angular Admin Dashboard                    │
│                    (Frontend - Port 4200)                    │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                    Nginx Reverse Proxy                       │
│        (SSL/TLS - Port 443, Rate Limiting, Security)        │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│              API Gateway (YARP/Ocelot)                       │
│         (Request Routing, JWT Validation - Port 8080)       │
└──┬──────────┬──────────┬──────────┬──────────┬──────────────┘
   │          │          │          │          │
   ▼          ▼          ▼          ▼          ▼
┌──────┐ ┌──────┐ ┌──────┐ ┌────────┐ ┌────────┐
│ Auth │ │ User │ │Audit │ │Notifi- │ │Security│
│ Svc  │ │ Svc  │ │ Svc  │ │cation  │ │ Engine │
│      │ │      │ │      │ │ Svc    │ │        │
│5001  │ │5002  │ │5003  │ │5004    │ │5005    │
└──┬───┘ └──┬───┘ └──┬───┘ └────┬───┘ └───┬────┘
   │        │        │         │          │
   └────────┼────────┼─────────┼──────────┘
            │        │         │
            ▼        ▼         ▼
      ┌──────────────────────────┐
      │   Data Layer             │
      ├──────────────────────────┤
      │ PostgreSQL (5432)        │
      │ Redis (6379)             │
      │ RabbitMQ (5672)          │
      └──────────────────────────┘
            │        │         │
            ▼        ▼         ▼
      ┌──────────────────────────┐
      │ Observability Stack      │
      ├──────────────────────────┤
      │ OpenTelemetry Collector  │
      │ Prometheus (9090)        │
      │ Loki (3100)              │
      │ Grafana (3000)           │
      └──────────────────────────┘
```

## Service Communication

```
Synchronous (REST)
├── Client → Gateway
├── Gateway → Services
└── Service → Service

Asynchronous (Message Queue)
├── Service → RabbitMQ
├── RabbitMQ → Service Consumers
└── Events → Audit/Notification Services

Distributed Cache
├── Service → Redis
├── Redis → Cache Store
└── Cache Invalidation
```

## Data Flow Example (Login)

```
1. Client sends credentials
   │
   └─→ POST /api/auth/login

2. Nginx handles SSL/TLS
   │
   └─→ Decrypt & validate request

3. API Gateway validates
   │
   └─→ Route to Auth Service

4. Auth Service processes
   │
   ├─→ Validate credentials
   ├─→ Generate JWT token
   ├─→ Create login history
   └─→ Publish LoginEvent

5. RabbitMQ distributes events
   │
   ├─→ Audit Service consumes
   └─→ Notification Service consumes

6. OpenTelemetry captures metrics
   │
   ├─→ Prometheus stores metrics
   ├─→ Loki stores logs
   └─→ Grafana visualizes

7. Response returned to client
   │
   └─→ JWT token + Refresh token
```

## Clean Architecture Layers

```
┌─────────────────────────────────────┐
│         API Layer                   │
│  Controllers, Middleware, Routes    │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│      Application Layer              │
│  Features, DTOs, Validators,        │
│  Services, Use Cases                │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│        Domain Layer                 │
│  Entities, Value Objects,           │
│  Domain Events, Interfaces          │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│      Infrastructure Layer           │
│  DbContext, Repositories,           │
│  External Services, Persistence     │
└─────────────────────────────────────┘
```
