# Secured Endpoints Documentation

This document provides a quick reference for all the secured endpoints with valid credentials and usage examples.

---

## 1. Basic Auth Secured Endpoints

**Controller:** `BasicAuthSecuredController`  
**Route:** `/secure/basic-auth`

### Endpoints

#### GET /secure/basic-auth/protected
Protected resource requiring Basic Authentication.

**Headers:**
```
Authorization: Basic base64(username:password)
```

**Valid Credentials:**
- `admin:password123`
- `user:user@123`
- `test:test@123`

**Example Request:**
```bash
curl -H "Authorization: Basic YWRtaW46cGFzc3dvcmQxMjM=" http://localhost:5000/secure/basic-auth/protected
```

**Example Response (200 OK):**
```json
{
  "success": true,
  "message": "Access granted",
  "user": "admin",
  "resource": {
    "id": "uuid",
    "title": "Confidential Document",
    "content": "This is protected data only accessible with valid Basic Auth credentials",
    "accessedAt": "2024-01-01T12:00:00Z"
  }
}
```

---

#### GET /secure/basic-auth/credentials
Returns credential information for the authenticated user.

**Headers:**
```
Authorization: Basic base64(username:password)
```

**Example Response (200 OK):**
```json
{
  "authenticated": true,
  "username": "admin",
  "userId": "uuid",
  "authType": "Basic Auth",
  "permissions": ["read", "write"],
  "roles": ["admin"],
  "createdAt": "2024-01-01T12:00:00Z",
  "expiresAt": "2025-01-01T12:00:00Z"
}
```

---

## 2. Bearer Token Secured Endpoints

**Controller:** `BearerTokenSecuredController`  
**Route:** `/secure/bearer-token`

### Endpoints

#### GET /secure/bearer-token/protected
Protected resource requiring Bearer Token authentication.

**Headers:**
```
Authorization: Bearer {token}
```

**Valid Tokens:**
- `bearer_token_admin_12345`
- `bearer_token_user_67890`
- `bearer_token_guest_11111`

**Example Request:**
```bash
curl -H "Authorization: Bearer bearer_token_admin_12345" http://localhost:5000/secure/bearer-token/protected
```

**Example Response (200 OK):**
```json
{
  "success": true,
  "message": "Access granted",
  "user": "admin",
  "resource": {
    "id": "uuid",
    "title": "User Dashboard Data",
    "content": "This is protected data accessible with a valid Bearer Token",
    "userStats": {
      "loginCount": 42,
      "lastLogin": "2024-01-01T10:00:00Z",
      "totalRequests": 1250
    },
    "accessedAt": "2024-01-01T12:00:00Z"
  }
}
```

---

#### GET /secure/bearer-token/token-info
Returns Bearer Token information for the authenticated user.

**Headers:**
```
Authorization: Bearer {token}
```

**Example Response (200 OK):**
```json
{
  "authenticated": true,
  "token": "****12345",
  "username": "admin",
  "userId": "user_001",
  "authType": "Bearer Token",
  "permissions": ["read", "write", "delete"],
  "roles": ["admin"],
  "issuedAt": "2024-01-01T11:00:00Z",
  "expiresAt": "2024-01-02T11:00:00Z",
  "expiresIn": 3600
}
```

---

## 3. JWT Token Secured Endpoints

**Controller:** `JwtTokenSecuredController`  
**Route:** `/secure/jwt`

### Endpoints

#### GET /secure/jwt/generate
Generates a JWT token for testing.

**Query Parameters:**
- `username` (optional, default: "user") - Values: `admin`, `user`, `guest`

**Example Request:**
```bash
curl "http://localhost:5000/secure/jwt/generate?username=admin"
```

**Example Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "userId": "user_001",
  "role": "admin",
  "permissions": ["read", "write", "delete"],
  "expiresIn": 86400,
  "expiresAt": "2024-01-02T12:00:00Z",
  "tokenType": "Bearer"
}
```

---

#### GET /secure/jwt/protected
Protected resource requiring JWT Bearer Token.

**Headers:**
```
Authorization: Bearer {jwt_token}
```

**Valid Pre-generated Tokens:**
- `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwidXNlcm5hbWUiOiJhZG1pbiIsInJvbGUiOiJhZG1pbiIsImlhdCI6MTcwNDAwMDAwMCwiZXhwIjoxNzA1MjA5NjAwfQ.admin_jwt_signature` (admin)
- `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIyIiwidXNlcm5hbWUiOiJ1c2VyIiwicm9sZSI6InVzZXIiLCJpYXQiOjE3MDQwMDAwMDAsImV4cCI6MTcwNDAwMzYwMH0.user_jwt_signature` (user)
- `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzIiwidXNlcm5hbWUiOiJndWVzdCIsInJvbGUiOiJndWVzdCIsImlhdCI6MTcwNDAwMDAwMCwiZXhwIjoxNzA0MDAwMzYwfQ.guest_jwt_signature` (guest)

**Example Request:**
```bash
curl -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." http://localhost:5000/secure/jwt/protected
```

**Example Response (200 OK):**
```json
{
  "success": true,
  "message": "JWT token validated successfully",
  "user": "admin",
  "resource": {
    "id": "uuid",
    "title": "JWT Protected Resource",
    "content": "This data is protected by JWT authentication",
    "data": {
      "userId": "user_001",
      "role": "admin",
      "permissions": ["read", "write", "delete"],
      "lastAccessed": "2024-01-01T12:00:00Z"
    }
  },
  "accessedAt": "2024-01-01T12:00:00Z"
}
```

---

#### GET /secure/jwt/decode
Decodes and returns JWT token claims.

**Headers:**
```
Authorization: Bearer {jwt_token}
```

**Example Response (200 OK):**
```json
{
  "authenticated": true,
  "token": "****signature",
  "claims": {
    "sub": "user_001",
    "username": "admin",
    "role": "admin",
    "permissions": ["read", "write", "delete"]
  },
  "authType": "JWT Bearer Token",
  "issuedAt": "2024-01-01T11:00:00Z",
  "expiresAt": "2024-01-02T11:00:00Z",
  "expiresIn": 3600,
  "isExpired": false
}
```

---

## 4. API Key Secured Endpoints

**Controller:** `ApiKeySecurityController`  
**Route:** `/secure/api-key`

### Endpoints

#### GET /secure/api-key/protected
Protected resource requiring API Key authentication.

**Headers:**
```
X-API-Key: {api_key}
```

**Valid API Keys:**
- `sk_live_admin_key_abc123def456` (admin)
- `sk_live_user_key_xyz789uvw012` (user)
- `sk_test_limited_key_pqr345stu678` (test/guest)

**Example Request:**
```bash
curl -H "X-API-Key: sk_live_admin_key_abc123def456" http://localhost:5000/secure/api-key/protected
```

**Example Response (200 OK):**
```json
{
  "success": true,
  "message": "API Key authenticated successfully",
  "user": "admin",
  "resource": {
    "id": "uuid",
    "title": "API Key Protected Data",
    "content": "This resource is protected by API Key authentication",
    "apiUsage": {
      "requestsThisMonth": 1523,
      "requestsRemaining": 8477,
      "quotaLimit": 10000
    },
    "accessedAt": "2024-01-01T12:00:00Z"
  }
}
```

---

#### GET /secure/api-key/key-info
Returns API Key information.

**Headers:**
```
X-API-Key: {api_key}
```

**Example Response (200 OK):**
```json
{
  "authenticated": true,
  "keyId": "key_001",
  "key": "sk_live****def456",
  "name": "Admin API Key",
  "username": "admin",
  "userId": "user_001",
  "authType": "API Key",
  "permissions": ["read", "write", "delete"],
  "role": "admin",
  "createdAt": "2023-12-01T12:00:00Z",
  "expiresAt": "2025-01-01T12:00:00Z",
  "daysUntilExpiry": 365,
  "status": "active",
  "lastUsed": "2024-01-01T10:00:00Z",
  "usageStats": {
    "totalRequests": 3456,
    "requestsThisMonth": 1523,
    "averageResponseTime": 245
  }
}
```

---

## 5. OAuth 2.0 Secured Endpoints

**Controller:** `OAuth2SecuredController`  
**Route:** `/secure/oauth`

### OAuth 2.0 Configuration

- **Client ID:** `client_123456`
- **Client Secret:** `client_secret_abcdefghij1234567890`
- **Redirect URI:** `http://localhost:3000/callback`
- **Authorization Code Expiry:** 10 minutes
- **Access Token Expiry:** 1 hour

### Endpoints

#### POST /secure/oauth/authorize
Authorization endpoint for OAuth 2.0 flow.

**Query Parameters:**
- `client_id` (required) - Must be `client_123456`
- `redirect_uri` (required) - Must be `http://localhost:3000/callback`
- `response_type` (required) - Must be `code`
- `username` (optional, default: "user") - Values: `admin`, `user`, `guest`
- `state` (optional) - CSRF protection state parameter

**Example Request:**
```bash
curl "http://localhost:5000/secure/oauth/authorize?client_id=client_123456&redirect_uri=http://localhost:3000/callback&response_type=code&username=admin"
```

**Example Response (200 OK):**
```json
{
  "authorizationCode": "auth_code_abc123def456",
  "redirectUrl": "http://localhost:3000/callback?code=auth_code_abc123def456",
  "expiresIn": 600,
  "message": "Authorization code generated. Redirect to the URL above with the authorization code."
}
```

---

#### POST /secure/oauth/token
Token endpoint for exchanging authorization code for access token.

**Form Parameters (application/x-www-form-urlencoded):**
- `client_id` (required) - Must be `client_123456`
- `client_secret` (required) - Must be `client_secret_abcdefghij1234567890`
- `code` (required) - Authorization code from `/authorize` endpoint
- `grant_type` (required) - Must be `authorization_code`
- `redirect_uri` (required) - Must be `http://localhost:3000/callback`

**Example Request:**
```bash
curl -X POST "http://localhost:5000/secure/oauth/token" \
  -d "client_id=client_123456" \
  -d "client_secret=client_secret_abcdefghij1234567890" \
  -d "code=auth_code_abc123def456" \
  -d "grant_type=authorization_code" \
  -d "redirect_uri=http://localhost:3000/callback"
```

**Example Response (200 OK):**
```json
{
  "access_token": "access_token_abc123def456xyz",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "refresh_token_xyz789",
  "scope": "read write delete"
}
```

---

#### GET /secure/oauth/protected
Protected resource requiring OAuth 2.0 access token.

**Headers:**
```
Authorization: Bearer {access_token}
```

**Example Request:**
```bash
curl -H "Authorization: Bearer access_token_abc123def456xyz" http://localhost:5000/secure/oauth/protected
```

**Example Response (200 OK):**
```json
{
  "success": true,
  "message": "OAuth 2.0 token validated successfully",
  "user": "admin",
  "resource": {
    "id": "uuid",
    "title": "OAuth Protected Resource",
    "content": "This data is protected by OAuth 2.0 authentication",
    "userData": {
      "username": "admin",
      "email": "admin@example.com",
      "role": "admin",
      "permissions": ["read", "write", "delete"]
    }
  },
  "accessedAt": "2024-01-01T12:00:00Z"
}
```

---

#### GET /secure/oauth/userinfo
Returns authenticated user information (OAuth 2.0 UserInfo endpoint).

**Headers:**
```
Authorization: Bearer {access_token}
```

**Example Response (200 OK):**
```json
{
  "authenticated": true,
  "sub": "user_001",
  "username": "admin",
  "email": "admin@example.com",
  "role": "admin",
  "permissions": ["read", "write", "delete"],
  "authType": "OAuth 2.0",
  "tokenIssuedAt": "2024-01-01T11:00:00Z",
  "tokenExpiresAt": "2024-01-01T12:00:00Z",
  "expiresIn": 3600
}
```

---

## Testing the OAuth 2.0 Flow

### Step 1: Get Authorization Code
```bash
curl "http://localhost:5000/secure/oauth/authorize?client_id=client_123456&redirect_uri=http://localhost:3000/callback&response_type=code&username=admin"
```

### Step 2: Exchange Code for Access Token
```bash
curl -X POST "http://localhost:5000/secure/oauth/token" \
  -d "client_id=client_123456" \
  -d "client_secret=client_secret_abcdefghij1234567890" \
  -d "code=auth_code_abc123def456" \
  -d "grant_type=authorization_code" \
  -d "redirect_uri=http://localhost:3000/callback"
```

### Step 3: Access Protected Resource
```bash
curl -H "Authorization: Bearer access_token_abc123def456xyz" http://localhost:5000/secure/oauth/protected
```

### Step 4: Get User Info
```bash
curl -H "Authorization: Bearer access_token_abc123def456xyz" http://localhost:5000/secure/oauth/userinfo
```

---

## Error Responses

### 401 Unauthorized
```json
{
  "error": "Invalid or missing credentials"
}
```

### 400 Bad Request
```json
{
  "error": "invalid_request",
  "error_description": "Description of what went wrong"
}
```

---

## Summary Table

| Auth Type | Protected Endpoint | Info Endpoint | Header/Query Parameter |
|-----------|-------------------|---------------|------------------------|
| Basic Auth | GET /secure/basic-auth/protected | GET /secure/basic-auth/credentials | Authorization: Basic |
| Bearer Token | GET /secure/bearer-token/protected | GET /secure/bearer-token/token-info | Authorization: Bearer |
| JWT | GET /secure/jwt/protected | GET /secure/jwt/decode | Authorization: Bearer |
| API Key | GET /secure/api-key/protected | GET /secure/api-key/key-info | X-API-Key |
| OAuth 2.0 | GET /secure/oauth/protected | GET /secure/oauth/userinfo | Authorization: Bearer |

---

## Notes

- All credentials and tokens are hardcoded for testing purposes
- Timestamps use ISO 8601 format with UTC timezone
- Each authentication method returns sample data in protected resources
- For OAuth 2.0, authorization codes expire after 10 minutes
- For OAuth 2.0, access tokens expire after 1 hour
