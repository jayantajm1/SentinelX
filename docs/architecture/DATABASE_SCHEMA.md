# Database Schema

## Auth Service Database

### Users Table
```sql
CREATE TABLE Users (
  Id BIGSERIAL PRIMARY KEY,
  Username VARCHAR(50) UNIQUE NOT NULL,
  Email VARCHAR(255) UNIQUE NOT NULL,
  PasswordHash VARCHAR(255) NOT NULL,
  FirstName VARCHAR(100),
  LastName VARCHAR(100),
  IsActive BOOLEAN DEFAULT true,
  EmailConfirmed BOOLEAN DEFAULT false,
  CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  UpdatedAt TIMESTAMP,
  LastLoginAt TIMESTAMP
);
```

### Roles Table
```sql
CREATE TABLE Roles (
  Id SERIAL PRIMARY KEY,
  Name VARCHAR(100) UNIQUE NOT NULL,
  Description TEXT,
  IsActive BOOLEAN DEFAULT true
);
```

### Permissions Table
```sql
CREATE TABLE Permissions (
  Id SERIAL PRIMARY KEY,
  Name VARCHAR(100) UNIQUE NOT NULL,
  Description TEXT
);
```

### UserRoles Junction Table
```sql
CREATE TABLE UserRoles (
  UserId BIGINT NOT NULL,
  RoleId INT NOT NULL,
  AssignedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (UserId, RoleId),
  FOREIGN KEY (UserId) REFERENCES Users(Id),
  FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
```

### RolePermissions Junction Table
```sql
CREATE TABLE RolePermissions (
  RoleId INT NOT NULL,
  PermissionId INT NOT NULL,
  PRIMARY KEY (RoleId, PermissionId),
  FOREIGN KEY (RoleId) REFERENCES Roles(Id),
  FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
);
```

### RefreshTokens Table
```sql
CREATE TABLE RefreshTokens (
  Id SERIAL PRIMARY KEY,
  UserId BIGINT NOT NULL,
  Token TEXT NOT NULL UNIQUE,
  ExpiryDate TIMESTAMP NOT NULL,
  IsRevoked BOOLEAN DEFAULT false,
  CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

### LoginHistory Table
```sql
CREATE TABLE LoginHistory (
  Id BIGSERIAL PRIMARY KEY,
  UserId BIGINT NOT NULL,
  IpAddress VARCHAR(45),
  UserAgent TEXT,
  IsSuccessful BOOLEAN,
  LoginAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

## User Service Database

### Users (Extended) Table
```sql
CREATE TABLE user_profiles (
  id BIGSERIAL PRIMARY KEY,
  auth_user_id BIGINT NOT NULL,
  phone_number VARCHAR(20),
  date_of_birth DATE,
  country VARCHAR(100),
  state VARCHAR(100),
  city VARCHAR(100),
  postal_code VARCHAR(20),
  avatar_url TEXT,
  bio TEXT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP
);
```

## Audit Service Database

### AuditLogs Table
```sql
CREATE TABLE audit_logs (
  id BIGSERIAL PRIMARY KEY,
  user_id BIGINT,
  action VARCHAR(100) NOT NULL,
  entity_type VARCHAR(100),
  entity_id BIGINT,
  old_values JSONB,
  new_values JSONB,
  timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### SecurityEvents Table
```sql
CREATE TABLE security_events (
  id BIGSERIAL PRIMARY KEY,
  event_type VARCHAR(100) NOT NULL,
  user_id BIGINT,
  ip_address VARCHAR(45),
  description TEXT,
  severity VARCHAR(20),
  timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```
