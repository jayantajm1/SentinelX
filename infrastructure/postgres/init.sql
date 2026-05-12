-- Create databases for each service
CREATE DATABASE auth_db;
CREATE DATABASE user_db;
CREATE DATABASE audit_db;
CREATE DATABASE notification_db;

-- Connect to auth_db and create tables
\c auth_db;

-- Users table
CREATE TABLE users (
    id BIGSERIAL PRIMARY KEY,
    email VARCHAR(255) NOT NULL UNIQUE,
    username VARCHAR(128) NOT NULL UNIQUE,
    password_hash VARCHAR(512) NOT NULL,
    first_name VARCHAR(128),
    last_name VARCHAR(128),
    is_active BOOLEAN DEFAULT true,
    is_locked BOOLEAN DEFAULT false,
    failed_login_attempts INT DEFAULT 0,
    last_login_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Roles table
CREATE TABLE roles (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(128) NOT NULL UNIQUE,
    description TEXT,
    is_system_role BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Permissions table
CREATE TABLE permissions (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- UserRoles mapping
CREATE TABLE user_roles (
    user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role_id BIGINT NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, role_id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- RolePermissions mapping
CREATE TABLE role_permissions (
    role_id BIGINT NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    permission_id BIGINT NOT NULL REFERENCES permissions(id) ON DELETE CASCADE,
    PRIMARY KEY (role_id, permission_id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- RefreshTokens table
CREATE TABLE refresh_tokens (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token_hash VARCHAR(512) NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    revoked_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- UserDevices table
CREATE TABLE user_devices (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    device_id VARCHAR(255) NOT NULL,
    device_name VARCHAR(255),
    device_type VARCHAR(128),
    ip_address VARCHAR(45),
    user_agent TEXT,
    last_used_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- LoginHistory table
CREATE TABLE login_history (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT REFERENCES users(id) ON DELETE SET NULL,
    ip_address VARCHAR(45),
    device_id VARCHAR(255),
    success BOOLEAN,
    failure_reason VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_login_history_user_id ON login_history(user_id);
CREATE INDEX idx_login_history_created_at ON login_history(created_at);

-- Insert default roles
INSERT INTO roles (name, description, is_system_role) VALUES
    ('SuperAdmin', 'System administrator with full access', true),
    ('Admin', 'Administrator with management capabilities', true),
    ('User', 'Standard user with limited access', true),
    ('SecurityOfficer', 'Security monitoring and incident response', false);

-- Insert default permissions
INSERT INTO permissions (name, description) VALUES
    ('user.create', 'Create new users'),
    ('user.read', 'View user information'),
    ('user.update', 'Update user details'),
    ('user.delete', 'Delete users'),
    ('role.manage', 'Manage roles and permissions'),
    ('audit.view', 'View audit logs'),
    ('security.manage', 'Manage security policies'),
    ('notification.send', 'Send notifications');

-- Grant SuperAdmin all permissions
INSERT INTO role_permissions (role_id, permission_id)
SELECT (SELECT id FROM roles WHERE name = 'SuperAdmin'), id FROM permissions;

-- Grant Admin most permissions
INSERT INTO role_permissions (role_id, permission_id)
SELECT (SELECT id FROM roles WHERE name = 'Admin'), id FROM permissions
WHERE name NOT IN ('role.manage');

-- Grant User basic permissions
INSERT INTO role_permissions (role_id, permission_id)
SELECT (SELECT id FROM roles WHERE name = 'User'), id FROM permissions
WHERE name IN ('user.read', 'audit.view');

-- Connect to user_db and create tables
\c user_db;

CREATE TABLE user_profiles (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL,
    phone_number VARCHAR(20),
    address TEXT,
    city VARCHAR(128),
    country VARCHAR(128),
    postal_code VARCHAR(20),
    avatar_url TEXT,
    bio TEXT,
    is_verified BOOLEAN DEFAULT false,
    two_factor_enabled BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE user_activity (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL,
    activity_type VARCHAR(255),
    description TEXT,
    ip_address VARCHAR(45),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_user_profiles_user_id ON user_profiles(user_id);
CREATE INDEX idx_user_activity_user_id ON user_activity(user_id);

-- Connect to audit_db and create tables
\c audit_db;

CREATE TABLE audit_logs (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT,
    action VARCHAR(255) NOT NULL,
    service VARCHAR(128) NOT NULL,
    resource_type VARCHAR(128),
    resource_id VARCHAR(255),
    ip_address VARCHAR(45),
    correlation_id UUID,
    status VARCHAR(50),
    details JSONB,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_audit_logs_user_id ON audit_logs(user_id);
CREATE INDEX idx_audit_logs_action ON audit_logs(action);
CREATE INDEX idx_audit_logs_created_at ON audit_logs(created_at);
CREATE INDEX idx_audit_logs_correlation_id ON audit_logs(correlation_id);

CREATE TABLE security_alerts (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT,
    alert_type VARCHAR(255) NOT NULL,
    severity VARCHAR(50),
    ip_address VARCHAR(45),
    description TEXT,
    resolved BOOLEAN DEFAULT false,
    resolved_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_security_alerts_severity ON security_alerts(severity);
CREATE INDEX idx_security_alerts_user_id ON security_alerts(user_id);

-- Connect to notification_db and create tables
\c notification_db;

CREATE TABLE notifications (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL,
    type VARCHAR(128) NOT NULL,
    title VARCHAR(255),
    message TEXT,
    is_read BOOLEAN DEFAULT false,
    read_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE email_queue (
    id BIGSERIAL PRIMARY KEY,
    recipient_email VARCHAR(255) NOT NULL,
    subject VARCHAR(255) NOT NULL,
    body TEXT NOT NULL,
    status VARCHAR(50) DEFAULT 'pending',
    retry_count INT DEFAULT 0,
    sent_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_notifications_user_id ON notifications(user_id);
CREATE INDEX idx_notifications_is_read ON notifications(is_read);
CREATE INDEX idx_email_queue_status ON email_queue(status);
