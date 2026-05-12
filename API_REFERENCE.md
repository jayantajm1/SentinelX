# SentinelX API Reference

## Base URL
```
http://localhost:5000/api
```

All requests require JWT authentication in the `Authorization` header:
```
Authorization: Bearer <access_token>
```

## Authentication Endpoints

### 1. User Registration
```
POST /auth/register
Content-Type: application/json

Request Body:
{
  "email": "user@example.com",
  "username": "username",
  "password": "SecurePass@123",
  "firstName": "John",
  "lastName": "Doe"
}

Response (200):
{
  "success": true,
  "message": "Registration successful",
  "data": {
    "userId": 1,
    "email": "user@example.com",
    "username": "username",
    "accessToken": "eyJhbGc...",
    "refreshToken": "...",
    "expiresAt": "2024-01-15T10:30:00Z"
  }
}

Error Response (400):
{
  "success": false,
  "message": "User with this email already exists",
  "errors": ["email_exists"]
}
```

### 2. User Login
```
POST /auth/login
Content-Type: application/json

Request Body:
{
  "email": "user@example.com",
  "password": "SecurePass@123",
  "deviceId": "device-uuid-1234",
  "deviceName": "Chrome on Windows"
}

Response (200):
{
  "success": true,
  "message": "Login successful",
  "data": {
    "userId": 1,
    "email": "user@example.com",
    "username": "username",
    "accessToken": "eyJhbGc...",
    "refreshToken": "...",
    "expiresAt": "2024-01-15T10:30:00Z"
  }
}

Error Response (401):
{
  "success": false,
  "message": "Invalid credentials"
}
```

### 3. Refresh Token
```
POST /auth/refresh
Content-Type: application/json

Request Body:
{
  "refreshToken": "refresh-token-string"
}

Response (200):
{
  "success": true,
  "message": "Token refreshed successfully",
  "data": {
    "accessToken": "eyJhbGc...",
    "expiresAt": "2024-01-15T10:30:00Z"
  }
}
```

### 4. Logout
```
POST /auth/logout
Authorization: Bearer <access_token>

Response (200):
{
  "success": true,
  "message": "Logout successful"
}
```

### 5. Get User Profile
```
GET /auth/profile
Authorization: Bearer <access_token>

Response (200):
{
  "success": true,
  "message": "Profile retrieved successfully",
  "data": {
    "id": 1,
    "email": "user@example.com",
    "username": "username",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["User"],
    "permissions": ["user.read", "audit.view"],
    "twoFactorEnabled": false,
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

## User Service Endpoints

### 1. Get User Profile
```
GET /users/{userId}
Authorization: Bearer <access_token>

Response (200):
{
  "success": true,
  "data": {
    "userId": 1,
    "phoneNumber": "+1234567890",
    "address": "123 Main St",
    "city": "New York",
    "country": "USA",
    "isVerified": true,
    "twoFactorEnabled": false
  }
}
```

### 2. Update User Profile
```
PUT /users/{userId}
Authorization: Bearer <access_token>
Content-Type: application/json

Request Body:
{
  "phoneNumber": "+1234567890",
  "address": "456 Oak Ave",
  "city": "Los Angeles",
  "country": "USA",
  "postalCode": "90001",
  "bio": "Security engineer"
}

Response (200):
{
  "success": true,
  "message": "Profile updated successfully",
  "data": { ... }
}
```

### 3. Enable Two-Factor Authentication
```
POST /users/{userId}/two-factor
Authorization: Bearer <access_token>

Response (200):
{
  "success": true,
  "message": "Two-factor enabled successfully"
}
```

## Audit Service Endpoints

### 1. Get Audit Logs
```
GET /audit/logs?page=1&pageSize=50
Authorization: Bearer <access_token>

Response (200):
{
  "success": true,
  "data": [
    {
      "userId": 1,
      "action": "USER_LOGIN",
      "service": "AuthService",
      "ipAddress": "192.168.1.1",
      "correlationId": "guid-123",
      "createdAt": "2024-01-15T10:20:00Z"
    }
  ]
}
```

### 2. Get Security Alerts
```
GET /audit/alerts?page=1&pageSize=50
Authorization: Bearer <access_token>

Response (200):
{
  "success": true,
  "data": [
    {
      "userId": 1,
      "alertType": "BRUTE_FORCE_ATTEMPT",
      "severity": "High",
      "description": "Multiple failed login attempts detected",
      "createdAt": "2024-01-15T10:15:00Z"
    }
  ]
}
```

## Security Service Endpoints

### 1. Check Rate Limit
```
GET /security/rate-limit/{userId}

Response (200):
{
  "success": true,
  "data": {
    "allowed": true,
    "remaining": 87
  }
}
```

### 2. Check Suspicious Activity
```
POST /security/check-suspicious/{userId}?ipAddress=192.168.1.1
Authorization: Bearer <access_token>

Response (200):
{
  "success": true,
  "data": {
    "suspicious": false
  }
}
```

### 3. Block IP Address
```
POST /security/block-ip/{ipAddress}
Authorization: Bearer <access_token>

Response (200):
{
  "success": true,
  "message": "IP blocked successfully"
}
```

## Notification Service Endpoints

### 1. Get User Notifications
```
GET /notifications/user/{userId}
Authorization: Bearer <access_token>

Response (200):
{
  "success": true,
  "data": [
    {
      "id": 1,
      "type": "security_alert",
      "title": "Suspicious Login Attempt",
      "message": "Login from new device detected",
      "isRead": false,
      "createdAt": "2024-01-15T10:10:00Z"
    }
  ]
}
```

### 2. Mark Notification as Read
```
POST /notifications/{notificationId}/read
Authorization: Bearer <access_token>

Response (200):
{
  "success": true,
  "message": "Notification marked as read"
}
```

## Response Format

### Success Response
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { /* actual data */ },
  "correlationId": "guid-123",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error description",
  "errors": ["error1", "error2"],
  "correlationId": "guid-123",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 | OK - Request successful |
| 201 | Created - Resource created |
| 400 | Bad Request - Invalid input |
| 401 | Unauthorized - Missing/invalid JWT |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource not found |
| 429 | Too Many Requests - Rate limit exceeded |
| 500 | Internal Server Error |

## Common Request Headers

```
Authorization: Bearer <jwt_token>
Content-Type: application/json
X-Device-ID: device-uuid
X-Correlation-ID: correlation-guid (optional, generated by gateway)
X-Content-Encryption: AES-256-CBC (optional, for encrypted payloads)
```

## Common Response Headers

```
X-Correlation-ID: correlation-guid
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 87
X-RateLimit-Reset: 1705315800
```

## JWT Token Structure

JWT tokens have three parts separated by dots:

```
Header.Payload.Signature
```

Decoded payload example:
```json
{
  "sub": "1",
  "email": "user@example.com",
  "name": "username",
  "role": "User",
  "permissions": "user.read,audit.view",
  "iat": 1705308600,
  "exp": 1705310400
}
```

Claims:
- `sub` - User ID
- `email` - User email
- `name` - Username
- `role` - User role
- `permissions` - Comma-separated permissions
- `iat` - Issued at (Unix timestamp)
- `exp` - Expiration (Unix timestamp)

## Rate Limiting

Rate limits are enforced at multiple levels:

### Global Rate Limits
- 100 requests per minute per user
- 1000 requests per minute per IP

### Endpoint-specific Rate Limits
- `/auth/login` - 5 attempts per minute
- `/auth/register` - 1 per minute
- Other endpoints - 100 per minute

Rate limit info available in response headers:
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 87
X-RateLimit-Reset: 1705315800
```

## Error Handling

### Error Response Example
```json
{
  "success": false,
  "message": "Invalid credentials",
  "errors": [
    "email_not_found",
    "password_incorrect"
  ],
  "correlationId": "guid-123",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Common Error Codes
- `invalid_credentials` - Email/password incorrect
- `account_locked` - Account locked after failed attempts
- `account_inactive` - Account not active
- `token_expired` - JWT token expired
- `token_invalid` - JWT token invalid
- `permission_denied` - Insufficient permissions
- `rate_limit_exceeded` - Rate limit exceeded
- `email_exists` - Email already registered
- `validation_error` - Input validation failed

## Testing with cURL

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@sentinelx.com",
    "password": "Admin@123"
  }'
```

### Get Profile with JWT
```bash
curl -H "Authorization: Bearer <token>" \
  http://localhost:5000/api/auth/profile
```

### Get Audit Logs
```bash
curl -H "Authorization: Bearer <token>" \
  http://localhost:5000/api/audit/logs?page=1&pageSize=10
```

## Pagination

For endpoints that return lists, pagination is supported:

```
?page=1&pageSize=50
```

Response includes pagination info:
```json
{
  "data": [...],
  "totalCount": 250,
  "page": 1,
  "pageSize": 50,
  "totalPages": 5
}
```

## API Gateway Routing

The API Gateway at `http://localhost:5000` forwards requests to:

| Path Prefix | Service | Port |
|---|---|---|
| `/api/auth/*` | Auth Service | 5001 |
| `/api/users/*` | User Service | 5002 |
| `/api/audit/*` | Audit Service | 5003 |
| `/api/security/*` | Security Engine | 5004 |
| `/api/notifications/*` | Notification Service | 5005 |

## WebSocket Endpoints (Future)

- `ws://localhost:5000/ws/notifications` - Real-time notifications
- `ws://localhost:5000/ws/security-alerts` - Real-time security alerts
- `ws://localhost:5000/ws/audit-events` - Real-time audit events

---

For more examples and detailed documentation, refer to:
- [Swagger/OpenAPI](http://localhost:5001/swagger) (when implemented)
- [Postman Collection](./postman-collection.json) (when created)
- [Architecture Guide](./ARCHITECTURE.md)
