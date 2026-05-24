# Unsecured Credential Information Endpoints

All credential information endpoints are now **unsecured** and don't require authentication. You can access them freely to get credential details for testing the protected endpoints.

---

## 1. Basic Auth - Credentials Endpoint

**GET /secure/basic-auth/credentials**

Returns all valid Basic Auth credentials without requiring authentication.

**Example Request:**
```bash
curl http://localhost:5000/secure/basic-auth/credentials
```

**Example Response (200 OK):**
```json
{
  "authType": "Basic Auth",
  "description": "HTTP Basic Authentication using Base64 encoded username:password",
  "validCredentials": [
    {
      "username": "admin",
      "password": "password123",
      "role": "admin",
      "permissions": ["read", "write", "delete"]
    },
    {
      "username": "user",
      "password": "user@123",
      "role": "user",
      "permissions": ["read", "write"]
    },
    {
      "username": "test",
      "password": "test@123",
      "role": "guest",
      "permissions": ["read"]
    }
  ],
  "headerFormat": "Authorization: Basic base64(username:password)",
  "exampleHeader": "Authorization: Basic YWRtaW46cGFzc3dvcmQxMjM=",
  "protectedEndpoint": "GET /secure/basic-auth/protected"
}
```

---

## 2. Bearer Token - Token Info Endpoint

**GET /secure/bearer-token/token-info**

Returns all valid Bearer tokens without requiring authentication.

**Example Request:**
```bash
curl http://localhost:5000/secure/bearer-token/token-info
```

**Example Response (200 OK):**
```json
{
  "authType": "Bearer Token",
  "description": "Bearer Token authentication using a simple token string",
  "validTokens": [
    {
      "token": "bearer_token_admin_12345",
      "username": "admin",
      "role": "admin",
      "permissions": ["read", "write", "delete"]
    },
    {
      "token": "bearer_token_user_67890",
      "username": "user",
      "role": "user",
      "permissions": ["read", "write"]
    },
    {
      "token": "bearer_token_guest_11111",
      "username": "guest",
      "role": "guest",
      "permissions": ["read"]
    }
  ],
  "headerFormat": "Authorization: Bearer {token}",
  "exampleHeader": "Authorization: Bearer bearer_token_admin_12345",
  "protectedEndpoint": "GET /secure/bearer-token/protected"
}
```

---

## 3. JWT Token - Decode Information Endpoint

**GET /secure/jwt/decode**

Returns JWT authentication information without requiring a valid token.

**Example Request:**
```bash
curl http://localhost:5000/secure/jwt/decode
```

**Example Response (200 OK):**
```json
{
  "authType": "JWT Bearer Token",
  "description": "JWT (JSON Web Token) authentication using signed tokens",
  "validTokens": [
    {
      "username": "admin",
      "role": "admin",
      "permissions": ["read", "write", "delete"]
    },
    {
      "username": "user",
      "role": "user",
      "permissions": ["read", "write"]
    },
    {
      "username": "guest",
      "role": "guest",
      "permissions": ["read"]
    }
  ],
  "headerFormat": "Authorization: Bearer {jwt_token}",
  "exampleHeader": "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "generateEndpoint": "GET /secure/jwt/generate?username=admin",
  "protectedEndpoint": "GET /secure/jwt/protected",
  "notes": "Use the /generate endpoint to create a valid JWT token for testing"
}
```

---

## 4. API Key - Key Info Endpoint

**GET /secure/api-key/key-info**

Returns all valid API keys without requiring authentication.

**Example Request:**
```bash
curl http://localhost:5000/secure/api-key/key-info
```

**Example Response (200 OK):**
```json
{
  "authType": "API Key",
  "description": "API Key authentication using X-API-Key header",
  "validKeys": [
    {
      "key": "sk_live_admin_key_abc123def456",
      "username": "admin",
      "role": "admin",
      "permissions": ["read", "write", "delete"]
    },
    {
      "key": "sk_live_user_key_xyz789uvw012",
      "username": "user",
      "role": "user",
      "permissions": ["read", "write"]
    },
    {
      "key": "sk_test_limited_key_pqr345stu678",
      "username": "test",
      "role": "guest",
      "permissions": ["read"]
    }
  ],
  "headerFormat": "X-API-Key: {api_key}",
  "exampleHeader": "X-API-Key: sk_live_admin_key_abc123def456",
  "protectedEndpoint": "GET /secure/api-key/protected"
}
```

---

## 5. OAuth 2.0 - UserInfo Endpoint

**GET /secure/oauth/userinfo**

Returns OAuth 2.0 configuration and valid users without requiring authentication.

**Example Request:**
```bash
curl http://localhost:5000/secure/oauth/userinfo
```

**Example Response (200 OK):**
```json
{
  "authType": "OAuth 2.0",
  "description": "OAuth 2.0 Authorization Code Flow",
  "clientId": "client_123456",
  "clientSecret": "client_secret_abcdefghij1234567890",
  "redirectUri": "http://localhost:3000/callback",
  "validUsers": [
    {
      "username": "admin",
      "email": "admin@example.com",
      "role": "admin",
      "permissions": ["read", "write", "delete"]
    },
    {
      "username": "user",
      "email": "user@example.com",
      "role": "user",
      "permissions": ["read", "write"]
    },
    {
      "username": "guest",
      "email": "guest@example.com",
      "role": "guest",
      "permissions": ["read"]
    }
  ],
  "flow": {
    "step1": "POST /secure/oauth/authorize?client_id=client_123456&redirect_uri=http://localhost:3000/callback&response_type=code&username=admin",
    "step2": "POST /secure/oauth/token (with authorization code)",
    "step3": "GET /secure/oauth/protected (with access token)"
  },
  "authorizationCodeExpiry": 600,
  "accessTokenExpiry": 3600
}
```

---

## Summary Table

| Auth Type | Unsecured Endpoint | Purpose |
|-----------|------------------|---------|
| Basic Auth | GET /secure/basic-auth/credentials | Get valid credentials |
| Bearer Token | GET /secure/bearer-token/token-info | Get valid tokens |
| JWT | GET /secure/jwt/decode | Get JWT info (use /generate to create token) |
| API Key | GET /secure/api-key/key-info | Get valid API keys |
| OAuth 2.0 | GET /secure/oauth/userinfo | Get OAuth config and valid users |

---

## Usage Pattern

1. **Call the unsecured credential endpoint** to get credentials/tokens
2. **Use the credentials/tokens** to call the protected endpoint
3. **Get protected resource** if credentials are valid

### Example for Basic Auth:

```bash
# Step 1: Get credentials
curl http://localhost:5000/secure/basic-auth/credentials

# Step 2: Use credentials to access protected endpoint
curl -H "Authorization: Basic YWRtaW46cGFzc3dvcmQxMjM=" http://localhost:5000/secure/basic-auth/protected

# Response: Protected data
```

### Example for Bearer Token:

```bash
# Step 1: Get valid tokens
curl http://localhost:5000/secure/bearer-token/token-info

# Step 2: Use token to access protected endpoint
curl -H "Authorization: Bearer bearer_token_admin_12345" http://localhost:5000/secure/bearer-token/protected

# Response: Protected data
```

### Example for JWT:

```bash
# Step 1: Generate a JWT token
curl "http://localhost:5000/secure/jwt/generate?username=admin"

# Step 2: Use the generated token to access protected endpoint
curl -H "Authorization: Bearer {generated_token}" http://localhost:5000/secure/jwt/protected

# Response: Protected data
```

### Example for API Key:

```bash
# Step 1: Get valid API keys
curl http://localhost:5000/secure/api-key/key-info

# Step 2: Use API key to access protected endpoint
curl -H "X-API-Key: sk_live_admin_key_abc123def456" http://localhost:5000/secure/api-key/protected

# Response: Protected data
```

### Example for OAuth 2.0:

```bash
# Step 1: Get OAuth configuration
curl http://localhost:5000/secure/oauth/userinfo

# Step 2: Get authorization code
curl "http://localhost:5000/secure/oauth/authorize?client_id=client_123456&redirect_uri=http://localhost:3000/callback&response_type=code&username=admin"

# Step 3: Exchange code for access token
curl -X POST "http://localhost:5000/secure/oauth/token" \
  -d "client_id=client_123456" \
  -d "client_secret=client_secret_abcdefghij1234567890" \
  -d "code={authorization_code}" \
  -d "grant_type=authorization_code" \
  -d "redirect_uri=http://localhost:3000/callback"

# Step 4: Use access token to access protected endpoint
curl -H "Authorization: Bearer {access_token}" http://localhost:5000/secure/oauth/protected

# Response: Protected data
```

---

## Key Changes Made

✅ All credential/info endpoints now return **hardcoded credential information**  
✅ No authentication required on credential endpoints - accessible freely  
✅ Protected endpoints still require valid credentials  
✅ Perfect for testing authentication flows without needing a database  
✅ All credentials and tokens are visible in the unsecured endpoints for convenience
