# API Documentation

## Base URL
```
https://api.sentinelx.com/api
```

## Authentication
All endpoints (except `/auth/register` and `/auth/login`) require JWT token in the Authorization header:
```
Authorization: Bearer <access_token>
```

## Auth Service API

### Login
```http
POST /auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}

Response:
{
  "success": true,
  "data": {
    "userId": 1,
    "email": "user@example.com",
    "accessToken": "eyJ0eXAiOiJKV1QiLCJhbGc...",
    "refreshToken": "base64encodedrefreshtoken",
    "expiresIn": 1800
  }
}
```

### Register
```http
POST /auth/register
Content-Type: application/json

{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "SecurePass123",
  "firstName": "John",
  "lastName": "Doe"
}

Response:
{
  "success": true,
  "data": {
    "userId": 2,
    "username": "newuser",
    "email": "newuser@example.com",
    "createdAt": "2024-01-15T10:30:00Z"
  }
}
```

### Refresh Token
```http
POST /auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "base64encodedrefreshtoken"
}

Response:
{
  "success": true,
  "data": {
    "accessToken": "eyJ0eXAiOiJKV1QiLCJhbGc...",
    "refreshToken": "newbase64encodedrefreshtoken",
    "expiresIn": 1800
  }
}
```

## User Service API

### Get User Profile
```http
GET /users/{userId}
Authorization: Bearer <access_token>

Response:
{
  "success": true,
  "data": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+1234567890",
    "country": "USA"
  }
}
```

### Update User Profile
```http
PUT /users/{userId}
Authorization: Bearer <access_token>
Content-Type: application/json

{
  "phoneNumber": "+1234567890",
  "country": "USA",
  "city": "New York"
}

Response:
{
  "success": true,
  "message": "User updated successfully"
}
```

## Audit Service API

### Get Audit Logs
```http
GET /audit/logs?userId=1&limit=100&offset=0
Authorization: Bearer <access_token>

Response:
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "userId": 1,
        "action": "LOGIN",
        "entityType": "User",
        "timestamp": "2024-01-15T10:30:00Z"
      }
    ],
    "totalCount": 150,
    "pageNumber": 1,
    "pageSize": 100,
    "hasNextPage": true
  }
}
```

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Invalid request",
  "errorCode": "VALIDATION_FAILED"
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Unauthorized access",
  "errorCode": "UNAUTHORIZED"
}
```

### 403 Forbidden
```json
{
  "success": false,
  "message": "Forbidden access",
  "errorCode": "FORBIDDEN"
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Resource not found",
  "errorCode": "NOT_FOUND"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "Internal server error",
  "errorCode": "INTERNAL_ERROR"
}
```
