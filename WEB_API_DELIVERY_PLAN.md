# Web API Test Harness - Endpoint Delivery Plan

**Objective:** Build a comprehensive Web API with complete endpoint coverage for validating API Gateway (KrakenD) capabilities across all HTTP methods, parameter types, and protocols.

**Tech Stack:** ASP.NET Core 9 | C# | Docker | Kubernetes

---

## Critical: All Endpoints Are UNSECURED By Default

✅ **Every endpoint listed below is accessible WITHOUT authentication**
- No login required
- No token needed
- No API key required
- All endpoints return 200 OK when called unsecured

**Why?** This API is designed to test gateway functionality. The gateway can add security layers independently.

---

## Swagger Documentation Variants

### Default Unsecured Documentation
- [ ] `GET /swagger/` - Swagger UI (unsecured)
- [ ] `GET /swagger/index.html` - Swagger UI alternative
- [ ] `GET /swagger/v1/openapi.json` - OpenAPI spec (unsecured)
- [ ] `GET /redoc/` - ReDoc documentation

### Security Example Variants (Same Endpoints, Different Docs)
These Swagger documentations show the SAME endpoints but with auth scheme examples for testing:

- [ ] `GET /swagger-secure/basic/` - Swagger with Basic Auth examples
  - Shows: `Authorization: Basic base64(username:password)`
  - Points to same unsecured endpoints
  - For testing: Does gateway forward Basic auth headers?

- [ ] `GET /swagger-secure/bearer/` - Swagger with Bearer Token examples
  - Shows: `Authorization: Bearer {jwt_token}`
  - Points to same unsecured endpoints
  - For testing: Does gateway validate/forward JWT tokens?

- [ ] `GET /swagger-secure/apikey/` - Swagger with API Key examples
  - Shows: `X-API-Key: {key}`
  - Points to same unsecured endpoints
  - For testing: Does gateway validate API keys?

- [ ] `GET /swagger-secure/combined/` - Swagger with all auth schemes
  - Shows: Basic + Bearer + API Key options
  - Points to same unsecured endpoints
  - For testing: Does gateway handle multiple auth schemes?

---

## Endpoints by Swagger Tags

### Tag: Health & Monitoring
All endpoints UNSECURED - No auth required

- [ ] `GET /health` - Basic health check
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "status": "healthy",
      "timestamp": "2024-01-15T10:30:00Z",
      "uptime": 86400,
      "version": "1.0.0"
    }
    ```

- [ ] `GET /health/detailed` - Detailed dependency health
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "status": "healthy",
      "timestamp": "2024-01-15T10:30:00Z",
      "services": {
        "database": { "status": "healthy", "responseTime": 5 },
        "cache": { "status": "healthy", "responseTime": 2 },
        "externalApi": { "status": "healthy", "responseTime": 150 }
      }
    }
    ```

- [ ] `GET /metrics` - System metrics
  - Query: `?period=1m|5m|1h`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "period": "1m",
      "timestamp": "2024-01-15T10:30:00Z",
      "requests": {
        "total": 1500,
        "successful": 1485,
        "failed": 15,
        "average_response_time_ms": 45
      },
      "errors": {
        "4xx": 10,
        "5xx": 5
      }
    }
    ```

- [ ] `GET /logs` - Application logs
  - Query: `?level=DEBUG|INFO|WARN|ERROR&limit=50&startDate=...&endDate=...`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "total": 50,
      "logs": [
        {
          "timestamp": "2024-01-15T10:30:00Z",
          "level": "INFO",
          "message": "Request received",
          "traceId": "0HN8P4J5K2M1L0N"
        }
      ]
    }
    ```

---

### Tag: Authentication & Security
All endpoints UNSECURED - No auth required to call them
(These endpoints PERFORM auth operations, they don't REQUIRE auth to call)

#### Basic Auth Support
- [ ] `POST /auth/basic/register` - Register user for Basic auth
  - Body: `{ "username": "john", "password": "securepass123" }`
  - Status: `201 Created`
  - Returns:
    ```json
    {
      "userId": "uuid",
      "username": "john",
      "createdAt": "2024-01-15T10:30:00Z",
      "permissions": ["read", "write"]
    }
    ```
  - Error `409 Conflict`: User already exists

- [ ] `POST /auth/basic/validate` - Validate Basic auth credentials
  - Headers: `Authorization: Basic base64(username:password)`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "valid": true,
      "userId": "uuid",
      "username": "john",
      "permissions": ["read", "write"]
    }
    ```
  - Error `401 Unauthorized`: Invalid credentials

#### JWT Bearer Token Support
- [ ] `POST /auth/login` - Get JWT token
  - Body: `{ "username": "admin", "password": "admin" }`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresIn": 3600,
      "refreshToken": "refresh_token_xyz",
      "tokenType": "Bearer"
    }
    ```
  - Error `401 Unauthorized`: Invalid credentials

- [ ] `POST /auth/refresh` - Refresh JWT token
  - Body: `{ "refreshToken": "refresh_token_xyz" }`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresIn": 3600,
      "refreshToken": "new_refresh_token_abc"
    }
    ```
  - Error `401 Unauthorized`: Invalid refresh token

- [ ] `GET /auth/validate` - Validate JWT token
  - Headers: `Authorization: Bearer {token}`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "valid": true,
      "user": {
        "userId": "uuid",
        "username": "admin",
        "roles": ["admin"]
      },
      "expiresIn": 3600,
      "expiresAt": "2024-01-15T11:30:00Z"
    }
    ```
  - Error `401 Unauthorized`: Invalid or expired token

- [ ] `GET /auth/introspect` - Get JWT claims
  - Headers: `Authorization: Bearer {token}`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "sub": "user_123",
      "username": "admin",
      "email": "admin@example.com",
      "roles": ["admin"],
      "scopes": ["read", "write", "delete"],
      "iat": 1705315200,
      "exp": 1705318800
    }
    ```
  - Error `401 Unauthorized`: Invalid token

- [ ] `POST /auth/logout` - Invalidate JWT token
  - Headers: `Authorization: Bearer {token}`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "success": true,
      "message": "Token invalidated",
      "loggedOutAt": "2024-01-15T10:30:00Z"
    }
    ```

#### API Key Support
- [ ] `POST /api-keys/generate` - Generate API key
  - Status: `201 Created`
  - Returns:
    ```json
    {
      "apiKey": "sk_test_4eC39HqLyjWDarhtT1ZdV7DO",
      "keyId": "key_123",
      "createdAt": "2024-01-15T10:30:00Z",
      "expiresAt": "2025-01-15T10:30:00Z",
      "scopes": ["read", "write"]
    }
    ```

- [ ] `GET /api-keys` - List API keys
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "total": 3,
      "keys": [
        {
          "keyId": "key_123",
          "createdAt": "2024-01-15T10:30:00Z",
          "expiresAt": "2025-01-15T10:30:00Z",
          "lastUsed": "2024-01-15T09:00:00Z",
          "scopes": ["read", "write"]
        }
      ]
    }
    ```

- [ ] `GET /api-keys/{keyId}` - Get API key details
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "keyId": "key_123",
      "createdAt": "2024-01-15T10:30:00Z",
      "expiresAt": "2025-01-15T10:30:00Z",
      "lastUsed": "2024-01-15T09:00:00Z",
      "scopes": ["read", "write"],
      "permissions": ["GET", "POST"]
    }
    ```
  - Error `404 Not Found`: Key not found

- [ ] `DELETE /api-keys/{keyId}` - Revoke API key
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "success": true,
      "keyId": "key_123",
      "revokedAt": "2024-01-15T10:30:00Z",
      "message": "API key revoked"
    }
    ```
  - Error `404 Not Found`: Key not found

- [ ] `POST /api-keys/{keyId}/rotate` - Rotate API key
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "oldKeyId": "key_123",
      "newApiKey": "sk_test_newkey789",
      "newKeyId": "key_456",
      "expiresAt": "2025-01-15T10:30:00Z",
      "message": "API key rotated, old key remains valid for 24 hours"
    }
    ```

#### Security Validation
- [ ] `GET /security/validate` - Validate any auth scheme
  - Status: `200 OK` / `401 Unauthorized`
  - Returns (success):
    ```json
    {
      "authenticated": true,
      "user": "admin",
      "userId": "uuid",
      "permissions": ["read", "write", "delete"],
      "scopes": ["*"],
      "authType": "bearer"
    }
    ```
  - Returns (failure):
    ```json
    {
      "authenticated": false,
      "message": "Invalid or missing authentication"
    }
    ```

- [ ] `GET /security/permissions` - Get user permissions
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "user": "admin",
      "userId": "uuid",
      "permissions": ["read", "write", "delete", "admin"],
      "resourcePermissions": {
        "users": ["read", "write", "delete"],
        "files": ["read", "write"],
        "settings": ["read", "write", "admin"]
      }
    }
    ```

- [ ] `GET /security/roles` - Get user roles
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "user": "admin",
      "userId": "uuid",
      "roles": ["admin", "user"],
      "roleDetails": [
        {
          "role": "admin",
          "permissions": ["*"],
          "assignedAt": "2024-01-01T00:00:00Z"
        }
      ]
    }
    ```

---

### Tag: HTTP Methods - GET
All endpoints UNSECURED

- [ ] `GET /test/simple` - Simple GET test
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "method": "GET",
      "timestamp": "2024-01-15T10:30:00Z",
      "message": "GET request successful"
    }
    ```

- [ ] `GET /test/path/{id}` - GET with single path param
  - Path: `/test/path/123`
  - Status: `200 OK` / `404 Not Found`
  - Returns (success):
    ```json
    {
      "id": 123,
      "name": "Item 123",
      "description": "Details for item",
      "createdAt": "2024-01-15T10:30:00Z"
    }
    ```
  - Returns (not found):
    ```json
    {
      "statusCode": 404,
      "message": "Resource not found",
      "error": "RESOURCE_NOT_FOUND"
    }
    ```

- [ ] `GET /test/path/{uuid}` - GET with UUID path param
  - Path: `/test/path/550e8400-e29b-41d4-a716-446655440000`
  - Status: `200 OK` / `400 Bad Request` / `404 Not Found`
  - Returns (success):
    ```json
    {
      "uuid": "550e8400-e29b-41d4-a716-446655440000",
      "data": {...}
    }
    ```
  - Returns (invalid UUID):
    ```json
    {
      "statusCode": 400,
      "message": "Invalid UUID format",
      "error": "INVALID_FORMAT"
    }
    ```

- [ ] `GET /test/path/{category}/{subcategory}/{itemId}` - GET with multiple path params
  - Path: `/test/path/electronics/phones/iphone14`
  - Status: `200 OK` / `404 Not Found`
  - Returns:
    ```json
    {
      "category": "electronics",
      "subcategory": "phones",
      "itemId": "iphone14",
      "name": "iPhone 14",
      "price": 999.99
    }
    ```

- [ ] `GET /test/path/{slug}/details` - GET with slug + suffix
  - Path: `/test/path/my-product/details`
  - Status: `200 OK` / `404 Not Found`
  - Returns:
    ```json
    {
      "slug": "my-product",
      "title": "My Product",
      "details": {
        "description": "...",
        "specifications": {...}
      }
    }
    ```

- [ ] `GET /test/path/{id}/combined?expand=full&fields=id,name` - Path + query params
  - Path: `/test/path/123/combined?expand=full&fields=id,name`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "id": 123,
      "name": "Item Name",
      "expanded": {
        "details": {...},
        "metadata": {...}
      }
    }
    ```

### Tag: HTTP Methods - POST
All endpoints UNSECURED

- [ ] `POST /test/simple` - Simple POST
  - Body: `{ "message": "hello world" }`
  - Status: `201 Created` / `400 Bad Request`
  - Returns (success):
    ```json
    {
      "id": "uuid",
      "message": "hello world",
      "status": "created",
      "createdAt": "2024-01-15T10:30:00Z"
    }
    ```
  - Returns (validation error):
    ```json
    {
      "statusCode": 400,
      "message": "Bad Request",
      "error": "VALIDATION_ERROR",
      "details": {
        "field": "message",
        "issue": "Message is required"
      }
    }
    ```

### Tag: HTTP Methods - PUT
All endpoints UNSECURED

- [ ] `PUT /test/simple/{id}` - Full resource replacement
  - Path: `{id}`
  - Body: `{ "message": "updated message" }`
  - Status: `200 OK` / `201 Created` / `400 Bad Request` / `404 Not Found`
  - Returns:
    ```json
    {
      "id": "123",
      "message": "updated message",
      "status": "updated",
      "updatedAt": "2024-01-15T10:30:00Z"
    }
    ```

- [ ] `PUT /test/simple/{id}?force=true` - PUT with query override
  - Path: `{id}`
  - Query: `force=true|false`
  - Body: JSON object
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "id": "123",
      "data": {...},
      "forced": true,
      "message": "Updated with force override"
    }
    ```

### Tag: HTTP Methods - PATCH
All endpoints UNSECURED

- [ ] `PATCH /test/simple/{id}` - Partial update
  - Path: `{id}`
  - Body: `{ "message": "updated" }` (only changed fields)
  - Status: `200 OK` / `400 Bad Request` / `404 Not Found`
  - Returns:
    ```json
    {
      "id": "123",
      "message": "updated",
      "partialUpdate": true,
      "patchedFields": ["message"],
      "updatedAt": "2024-01-15T10:30:00Z"
    }
    ```

- [ ] `PATCH /test/simple/{id}/{field}` - Single field update
  - Path: `{id}/{field}`
  - Body: `"new value"`
  - Status: `200 OK` / `404 Not Found`
  - Returns:
    ```json
    {
      "id": "123",
      "field": "name",
      "value": "new value",
      "updatedAt": "2024-01-15T10:30:00Z"
    }
    ```

### Tag: HTTP Methods - DELETE
All endpoints UNSECURED

- [ ] `DELETE /test/simple/{id}` - Simple delete
  - Path: `{id}`
  - Status: `200 OK` / `204 No Content` / `404 Not Found`
  - Returns:
    ```json
    {
      "success": true,
      "id": "123",
      "deletedAt": "2024-01-15T10:30:00Z",
      "message": "Resource deleted"
    }
    ```

- [ ] `DELETE /test/simple/{id}?hard=true` - Soft/hard delete option
  - Path: `{id}`
  - Query: `hard=true|false`
  - Status: `200 OK` / `404 Not Found`
  - Returns:
    ```json
    {
      "success": true,
      "id": "123",
      "deleteType": "hard",
      "deletedAt": "2024-01-15T10:30:00Z"
    }
    ```

---

### Tag: Query Parameters
All endpoints UNSECURED

- [ ] `GET /test/querystring?page=1&limit=10&filter=active` - Pagination + filtering
  - Query: page, limit, filter
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "page": 1,
      "limit": 10,
      "total": 150,
      "totalPages": 15,
      "data": [
        { "id": 1, "name": "Item 1", "status": "active" }
      ]
    }
    ```

- [ ] `GET /test/querystring/multiple?tag=node&tag=api&tag=gateway` - Multiple values
  - Query: `tag[]` array
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "tags": ["node", "api", "gateway"],
      "results": [
        { "id": 1, "title": "Node.js Guide", "tags": ["node", "api"] }
      ],
      "count": 5
    }
    ```

- [ ] `GET /test/querystring/complex?filter[status]=active&filter[type]=api&sort=-created` - Nested params
  - Query: `filter[field]`, `sort`, `fields`
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "filters": {
        "status": "active",
        "type": "api"
      },
      "sortBy": "-created",
      "results": [
        { "id": 1, "status": "active", "type": "api", "created": "2024-01-15T00:00:00Z" }
      ]
    }
    ```

- [ ] `GET /test/querystring/optional?q=search&limit=20` - Optional with defaults
  - Query: All optional
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "query": "search",
      "limit": 20,
      "offset": 0,
      "results": [
        { "id": 1, "name": "Search Result 1" }
      ],
      "total": 45
    }
    ```

- [ ] `GET /test/search?q=keyword&limit=50&offset=0&sort=-date` - Search with sorting
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "query": "keyword",
      "limit": 50,
      "offset": 0,
      "sort": "-date",
      "results": [
        { "id": 1, "title": "Result 1", "date": "2024-01-15T00:00:00Z" }
      ],
      "total": 123
    }
    ```

- [ ] `GET /test/items?fields=id,name,description&expand=full` - Field selection
  - Query: `fields`, `expand`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "items": [
        {
          "id": 1,
          "name": "Item 1",
          "description": "Description text",
          "expanded": {
            "details": {...},
            "metadata": {...}
          }
        }
      ]
    }
    ```

- [ ] `GET /test/logs?startDate=2024-01-01&endDate=2024-12-31&level=ERROR` - Date range filtering
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "startDate": "2024-01-01",
      "endDate": "2024-12-31",
      "level": "ERROR",
      "total": 234,
      "logs": [
        {
          "timestamp": "2024-01-15T10:30:00Z",
          "level": "ERROR",
          "message": "Error occurred"
        }
      ]
    }
    ```

---

### Tag: Path Parameters
All endpoints UNSECURED

- [ ] `GET /test/path/{id}` - Integer ID
  - Path: `/test/path/123`
  - Status: `200 OK` / `400 Bad Request` / `404 Not Found`
  - Returns (success):
    ```json
    {
      "id": 123,
      "name": "Item 123",
      "status": "active"
    }
    ```
  - Returns (invalid ID):
    ```json
    {
      "statusCode": 400,
      "message": "ID must be an integer",
      "error": "INVALID_ID_FORMAT"
    }
    ```
  - Returns (not found):
    ```json
    {
      "statusCode": 404,
      "message": "Resource not found",
      "error": "NOT_FOUND"
    }
    ```

- [ ] `GET /test/path/{uuid}` - UUID format
  - Status: `200 OK` / `400 Bad Request` / `404 Not Found`
  - Returns:
    ```json
    {
      "uuid": "550e8400-e29b-41d4-a716-446655440000",
      "name": "Resource Name"
    }
    ```

- [ ] `GET /test/path/{category}/{subcategory}/{itemId}` - Multiple params
  - Path: `/test/path/electronics/phones/iphone14`
  - Status: `200 OK` / `404 Not Found`
  - Returns:
    ```json
    {
      "category": "electronics",
      "subcategory": "phones",
      "itemId": "iphone14",
      "product": {
        "name": "iPhone 14",
        "price": 999.99
      }
    }
    ```

- [ ] `GET /test/path/{slug}` - URL-safe string
  - Path: `/test/path/my-product`
  - Status: `200 OK` / `404 Not Found`
  - Returns:
    ```json
    {
      "slug": "my-product",
      "title": "My Product",
      "description": "Product description"
    }
    ```

- [ ] `GET /test/path/{slug}/details` - Param with suffix
  - Path: `/test/path/my-product/details`
  - Status: `200 OK` / `404 Not Found`
  - Returns:
    ```json
    {
      "slug": "my-product",
      "details": {
        "description": "...",
        "specifications": {...},
        "reviews": {...}
      }
    }
    ```

- [ ] `GET /test/path/{id}/combined?expand=full&fields=id,name` - Path + query combo
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "id": 123,
      "name": "Item Name",
      "expand": "full",
      "fields": ["id", "name"],
      "data": {...}
    }
    ```

---

### Tag: Body - JSON
All endpoints UNSECURED

- [ ] `POST /test/body/json` - Simple JSON
  - Content-Type: `application/json`
  - Body: `{ "name": "John", "email": "john@example.com" }`
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "received": {
        "name": "John",
        "email": "john@example.com"
      },
      "processed": true,
      "message": "JSON body received and processed"
    }
    ```

- [ ] `POST /test/body/json/complex` - Nested JSON
  - Body: Complex nested objects and arrays
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "received": {
        "user": {
          "name": "John",
          "contacts": [...]
        }
      },
      "depth": 3,
      "arrayCount": 5
    }
    ```

- [ ] `POST /test/body/json/large` - Large payload
  - Status: `200 OK` / `413 Payload Too Large`
  - Returns:
    ```json
    {
      "size": 1048576,
      "items": 10000,
      "processed": true,
      "message": "Large JSON payload processed"
    }
    ```

### Tag: Body - XML
All endpoints UNSECURED

- [ ] `POST /test/body/xml` - Simple XML
  - Content-Type: `application/xml`
  - Body: XML document
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "received": {
        "name": "John",
        "email": "john@example.com"
      },
      "format": "xml",
      "parsed": true
    }
    ```

- [ ] `POST /test/body/xml/complex` - Nested XML
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "elements": 15,
      "attributes": 8,
      "depth": 4,
      "parsed": true
    }
    ```

### Tag: Body - Form Data
All endpoints UNSECURED

- [ ] `POST /test/body/formdata` - Simple form fields
  - Content-Type: `multipart/form-data`
  - Body: name, email, description
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "fields": {
        "name": "John",
        "email": "john@example.com",
        "description": "User description"
      },
      "fieldCount": 3
    }
    ```

- [ ] `POST /test/body/formdata/file` - Form + single file
  - Status: `200 OK` / `400 Bad Request` / `413 Payload Too Large`
  - Returns:
    ```json
    {
      "fields": {
        "name": "John",
        "email": "john@example.com"
      },
      "file": {
        "filename": "document.pdf",
        "size": 2048,
        "contentType": "application/pdf",
        "uploadedAt": "2024-01-15T10:30:00Z"
      }
    }
    ```

- [ ] `POST /test/body/formdata/multiple-files` - Form + multiple files
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "fields": {
        "batchName": "batch-2024-01"
      },
      "files": [
        {
          "filename": "file1.pdf",
          "size": 2048,
          "contentType": "application/pdf"
        },
        {
          "filename": "file2.pdf",
          "size": 3072,
          "contentType": "application/pdf"
        }
      ],
      "fileCount": 2
    }
    ```

### Tag: Body - Form URL Encoded
All endpoints UNSECURED

- [ ] `POST /test/body/formurlencoded` - Simple form-urlencoded
  - Content-Type: `application/x-www-form-urlencoded`
  - Body: name=John&email=john@example.com
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "fields": {
        "name": "John",
        "email": "john@example.com"
      },
      "format": "urlencoded"
    }
    ```

- [ ] `POST /test/body/formurlencoded/multiple` - Multiple values
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "fields": {
        "tags": ["node", "api", "gateway"]
      },
      "arrayFields": ["tags"]
    }
    ```

### Tag: Body - Plain Text
All endpoints UNSECURED

- [ ] `POST /test/body/plain-text` - Plain text body
  - Content-Type: `text/plain`
  - Body: Raw text string
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "length": 123,
      "text": "Plain text content...",
      "lines": 5,
      "words": 25
    }
    ```

- [ ] `POST /test/body/octet-stream` - Binary data
  - Content-Type: `application/octet-stream`
  - Body: Binary file data
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "size": 1024,
      "hash": "sha256_hash_value",
      "contentType": "application/octet-stream",
      "received": true
    }
    ```

---

### Tag: Headers
All endpoints UNSECURED

- [ ] `GET /test/headers/required` - Required header validation
  - Headers: `X-Api-Key`, `X-Request-ID` (required)
  - Status: `200 OK` / `400 Bad Request`
  - Returns (success):
    ```json
    {
      "receivedHeaders": {
        "X-Api-Key": "key-value",
        "X-Request-ID": "550e8400-e29b-41d4-a716-446655440000"
      },
      "validated": true
    }
    ```
  - Returns (missing header):
    ```json
    {
      "statusCode": 400,
      "message": "Bad Request",
      "error": "MISSING_HEADER",
      "details": "X-Request-ID header is required"
    }
    ```

- [ ] `GET /test/headers/optional` - Optional headers with defaults
  - Headers: `X-Custom-Header`, `Accept-Language` (optional)
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "receivedHeaders": {
        "X-Custom-Header": "custom-value",
        "Accept-Language": "en-US"
      },
      "defaults": {
        "Accept-Language": "en-US"
      }
    }
    ```

- [ ] `GET /test/headers/echo` - Echo all received headers
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "Host": "localhost:5000",
      "User-Agent": "Mozilla/5.0...",
      "Accept": "application/json",
      "Authorization": "Bearer token",
      "X-Trace-ID": "abc123",
      "X-Custom-Header": "value"
    }
    ```

- [ ] `GET /test/headers/custom` - Custom header forwarding
  - Headers: `X-Trace-ID`, `X-User-ID`, `X-Tenant-ID`
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "traceId": "trace-id-value",
      "userId": "user-id-value",
      "tenantId": "tenant-id-value",
      "headerVerification": {
        "X-Trace-ID": "forwarded",
        "X-User-ID": "forwarded",
        "X-Tenant-ID": "forwarded"
      }
    }
    ```

---

### Tag: File Upload
All endpoints UNSECURED

- [ ] `POST /files/upload` - Single file upload
  - Content-Type: `multipart/form-data`
  - Form: file, description, tags
  - Status: `201 Created` / `400 Bad Request` / `413 Payload Too Large`
  - Returns (success):
    ```json
    {
      "fileId": "f550e840-0e29-41d4-a716-446655440000",
      "fileName": "document.pdf",
      "fileSize": 2048,
      "contentType": "application/pdf",
      "uploadedAt": "2024-01-15T10:30:00Z",
      "url": "/api/files/f550e840-0e29-41d4-a716-446655440000/download"
    }
    ```
  - Returns (file too large):
    ```json
    {
      "statusCode": 413,
      "message": "Payload Too Large",
      "error": "FILE_TOO_LARGE",
      "details": "File size exceeds maximum allowed: 10MB"
    }
    ```

- [ ] `POST /files/upload/batch` - Multiple files
  - Form: files[], batchName
  - Status: `201 Created` / `400 Bad Request`
  - Returns:
    ```json
    {
      "batchId": "batch-550e8400-0e29-41d4-a716",
      "batchName": "batch-2024-01",
      "filesCount": 5,
      "successCount": 5,
      "failureCount": 0,
      "uploadedAt": "2024-01-15T10:30:00Z",
      "files": [
        {
          "fileId": "file-1",
          "fileName": "doc1.pdf",
          "fileSize": 2048,
          "status": "success"
        }
      ]
    }
    ```

- [ ] `POST /files/upload/chunked` - Resumable chunked upload
  - Query: uploadId, chunkNumber, totalChunks
  - Body: Binary chunk data
  - Status: `202 Accepted` (uploading) / `201 Created` (complete) / `400 Bad Request`
  - Returns (uploading):
    ```json
    {
      "uploadId": "upload-uuid",
      "chunkNumber": 1,
      "totalChunks": 10,
      "status": "uploading",
      "nextChunkUrl": "/files/upload/chunked?uploadId=...&chunkNumber=2"
    }
    ```
  - Returns (complete):
    ```json
    {
      "uploadId": "upload-uuid",
      "fileId": "file-uuid",
      "fileName": "largefile.zip",
      "fileSize": 104857600,
      "status": "complete",
      "url": "/api/files/file-uuid/download"
    }
    ```

- [ ] `POST /files/upload/with-validation` - File validation
  - Query: maxSize, allowedTypes
  - Status: `201 Created` / `400 Bad Request` / `413 Payload Too Large`
  - Returns (success):
    ```json
    {
      "fileId": "file-uuid",
      "fileName": "image.jpg",
      "validated": true,
      "validation": {
        "size": "pass",
        "type": "pass",
        "virusScan": "pass"
      }
    }
    ```
  - Returns (validation failed):
    ```json
    {
      "statusCode": 400,
      "message": "Bad Request",
      "error": "VALIDATION_FAILED",
      "details": "File type not allowed. Allowed types: image/jpeg, image/png"
    }
    ```

---

### Tag: File Download
All endpoints UNSECURED

- [ ] `GET /files/{fileId}/download` - Single file download
  - Query: `inline=true|false`, `disposition=attachment|inline|preview`
  - Status: `200 OK` / `404 Not Found`
  - Returns (success):
    ```
    HTTP/1.1 200 OK
    Content-Type: application/pdf
    Content-Length: 2048
    Content-Disposition: attachment; filename="document.pdf"
    ETag: "abc123def456"
    Last-Modified: Mon, 15 Jan 2024 10:30:00 GMT
    
    [Binary file content]
    ```
  - Returns (not found):
    ```json
    {
      "statusCode": 404,
      "message": "Not Found",
      "error": "FILE_NOT_FOUND"
    }
    ```

- [ ] `GET /files/{fileId}/download-stream` - Streaming download
  - Supports: HTTP 206 Range requests
  - Status: `200 OK` / `206 Partial Content` / `404 Not Found`
  - Returns (full content):
    ```
    HTTP/1.1 200 OK
    Content-Type: application/octet-stream
    Content-Length: 104857600
    Accept-Ranges: bytes
    
    [Streamed file content]
    ```
  - Returns (partial - range request):
    ```
    HTTP/1.1 206 Partial Content
    Content-Type: application/octet-stream
    Content-Length: 1048576
    Content-Range: bytes 0-1048575/104857600
    
    [Partial file content]
    ```

- [ ] `GET /files/{fileId}/download-conditional` - Conditional download
  - Headers: `If-Modified-Since`, `If-None-Match`
  - Status: `200 OK` / `304 Not Modified` / `404 Not Found`
  - Returns (modified):
    ```
    HTTP/1.1 200 OK
    Content-Type: application/pdf
    Content-Length: 2048
    ETag: "new_etag_value"
    Last-Modified: Mon, 15 Jan 2024 10:30:00 GMT
    
    [File content]
    ```
  - Returns (not modified):
    ```
    HTTP/1.1 304 Not Modified
    ETag: "same_etag_value"
    ```

- [ ] `POST /files/download/batch` - Batch as ZIP
  - Body: `{ "fileIds": [...], "archiveName": "archive" }`
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```
    HTTP/1.1 200 OK
    Content-Type: application/zip
    Content-Length: 5242880
    Content-Disposition: attachment; filename="archive.zip"
    
    [ZIP file content]
    ```
  - Returns (invalid IDs):
    ```json
    {
      "statusCode": 400,
      "message": "Bad Request",
      "error": "INVALID_FILE_IDS",
      "details": "Some file IDs not found: file-404, file-deleted"
    }
    ```

---

### Tag: GraphQL
All endpoints UNSECURED

- [ ] `POST /graphql` - Query/mutation execution
  - Content-Type: `application/json`
  - Body: `{ "query": "{ users(limit: 10) { id name } }", "variables": {...} }`
  - Status: `200 OK` / `400 Bad Request`
  - Returns (success):
    ```json
    {
      "data": {
        "users": [
          {
            "id": "user-1",
            "name": "John Doe"
          }
        ]
      }
    }
    ```
  - Returns (with errors):
    ```json
    {
      "data": null,
      "errors": [
        {
          "message": "Cannot query field 'invalidField' on type 'User'",
          "locations": [{"line": 1, "column": 10}],
          "extensions": {
            "code": "GRAPHQL_PARSE_FAILED"
          }
        }
      ]
    }
    ```

- [ ] `GET /graphql/schema` - Schema introspection
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "__schema": {
        "types": [
          {
            "kind": "OBJECT",
            "name": "User",
            "fields": [
              {
                "name": "id",
                "type": {"kind": "SCALAR", "name": "String"}
              }
            ]
          }
        ],
        "queryType": {"name": "Query"},
        "mutationType": {"name": "Mutation"}
      }
    }
    ```

#### GraphQL Queries (via POST /graphql)
- [ ] `{ users(limit: 10) { id name email } }` - User query
  - Returns: User list with specified fields
- [ ] `{ posts(userId: "123") { id title content } }` - Posts query
  - Returns: Posts for specific user
- [ ] `{ items(filter: {status: "active"}) { id name } }` - Filtered query
  - Returns: Filtered items
- [ ] `{ admins: users(role: "admin") { id name } }` - Query with aliases
  - Returns: Aliased query results

#### GraphQL Mutations (via POST /graphql)
- [ ] `mutation { createUser(input: {name: "...", email: "..."}) { id } }` - Create
  - Status: `200 OK` / `400 Bad Request`
  - Returns: Created user with ID
- [ ] `mutation { updateUser(id: "123", input: {name: "..."}) { id name } }` - Update
  - Returns: Updated user
- [ ] `mutation { deleteUser(id: "123") }` - Delete
  - Returns: `{ "success": true }`
- [ ] `mutation { batchCreate(items: [...]) { id name } }` - Batch operation
  - Returns: Array of created items

---

### Tag: WebSocket
All endpoints UNSECURED

- [ ] `GET /ws/connect` - WebSocket upgrade
  - Query: token, clientId, subscriptions
  - Status: `101 Switching Protocols` / `400 Bad Request`
  - Upgrade successful:
    ```
    HTTP/1.1 101 Switching Protocols
    Upgrade: websocket
    Connection: Upgrade
    Sec-WebSocket-Accept: [key]
    ```
  - Connection message (server → client):
    ```json
    {
      "type": "connected",
      "clientId": "client-uuid",
      "timestamp": "2024-01-15T10:30:00Z",
      "subscriptions": ["events", "logs"]
    }
    ```

#### WebSocket Subscriptions
- [ ] Subscribe to events: `{ "type": "subscribe", "channel": "events" }`
  - Status: `ack`
  - Receive messages:
    ```json
    {
      "type": "message",
      "channel": "events",
      "data": {
        "id": "event-1",
        "type": "api.request",
        "timestamp": "2024-01-15T10:30:00Z",
        "metadata": {...}
      }
    }
    ```

- [ ] Subscribe to logs: `{ "type": "subscribe", "channel": "logs" }`
  - Receive:
    ```json
    {
      "type": "message",
      "channel": "logs",
      "data": {
        "timestamp": "2024-01-15T10:30:00Z",
        "level": "INFO",
        "message": "Request received"
      }
    }
    ```

- [ ] Subscribe to metrics: `{ "type": "subscribe", "channel": "metrics" }`
  - Receive:
    ```json
    {
      "type": "message",
      "channel": "metrics",
      "data": {
        "timestamp": "2024-01-15T10:30:00Z",
        "throughput": 1500,
        "avgLatency": 45,
        "errorRate": 0.01
      }
    }
    ```

- [ ] Subscribe to notifications: `{ "type": "subscribe", "channel": "notifications" }`
  - Receive:
    ```json
    {
      "type": "message",
      "channel": "notifications",
      "data": {
        "id": "notif-1",
        "title": "New message",
        "message": "You have 1 new notification"
      }
    }
    ```

- [ ] Subscribe to broadcast: `{ "type": "subscribe", "channel": "broadcast" }`
  - Receive:
    ```json
    {
      "type": "message",
      "channel": "broadcast",
      "data": {
        "id": "broadcast-1",
        "title": "System Announcement",
        "message": "Scheduled maintenance in 1 hour"
      }
    }
    ```

#### WebSocket Management
- [ ] Unsubscribe: `{ "type": "unsubscribe", "channel": "events" }`
  - Response:
    ```json
    {
      "type": "unsubscribed",
      "channel": "events",
      "timestamp": "2024-01-15T10:30:00Z"
    }
    ```

- [ ] Heartbeat: Server sends `{ "type": "ping", "timestamp": "..." }`
  - Client responds: `{ "type": "pong", "timestamp": "..." }`

- [ ] Connection close
  - Status: `1000 Normal Closure`
  - Message:
    ```json
    {
      "type": "disconnected",
      "clientId": "client-uuid",
      "reason": "Client closed connection"
    }
    ```

---

### Tag: Error Handling
All endpoints UNSECURED

- [ ] `GET /test/error/400` - Bad Request (400)
  - Status: `400 Bad Request`
  - Returns:
    ```json
    {
      "statusCode": 400,
      "message": "Bad Request",
      "error": "INVALID_PARAMETER",
      "details": {
        "field": "email",
        "issue": "Invalid email format"
      },
      "traceId": "0HN8P4J5K2M1L0N"
    }
    ```

- [ ] `GET /test/error/401` - Unauthorized (401)
  - Status: `401 Unauthorized`
  - Returns:
    ```json
    {
      "statusCode": 401,
      "message": "Unauthorized",
      "error": "INVALID_CREDENTIALS",
      "details": "Invalid username or password",
      "traceId": "0HN8P4J5K2M1L0N"
    }
    ```

- [ ] `GET /test/error/403` - Forbidden (403)
  - Status: `403 Forbidden`
  - Returns:
    ```json
    {
      "statusCode": 403,
      "message": "Forbidden",
      "error": "INSUFFICIENT_PERMISSIONS",
      "details": "User lacks 'admin' permission for this resource",
      "traceId": "0HN8P4J5K2M1L0N"
    }
    ```

- [ ] `GET /test/error/404` - Not Found (404)
  - Status: `404 Not Found`
  - Returns:
    ```json
    {
      "statusCode": 404,
      "message": "Not Found",
      "error": "RESOURCE_NOT_FOUND",
      "details": "Resource with ID 12345 does not exist",
      "traceId": "0HN8P4J5K2M1L0N"
    }
    ```

- [ ] `GET /test/error/500` - Internal Server Error (500)
  - Status: `500 Internal Server Error`
  - Returns:
    ```json
    {
      "statusCode": 500,
      "message": "Internal Server Error",
      "error": "INTERNAL_ERROR",
      "details": "An unexpected error occurred while processing your request",
      "traceId": "0HN8P4J5K2M1L0N"
    }
    ```

- [ ] `GET /test/error/503` - Service Unavailable (503)
  - Status: `503 Service Unavailable`
  - Returns:
    ```json
    {
      "statusCode": 503,
      "message": "Service Unavailable",
      "error": "SERVICE_UNAVAILABLE",
      "details": "The service is temporarily unavailable. Please try again later",
      "retryAfter": 60,
      "traceId": "0HN8P4J5K2M1L0N"
    }
    ```

- [ ] `GET /test/error/timeout?delay=30000` - Timeout simulation
  - Query: delay in milliseconds (30000 = 30 seconds)
  - Status: `408 Request Timeout`
  - Returns:
    ```json
    {
      "statusCode": 408,
      "message": "Request Timeout",
      "error": "REQUEST_TIMEOUT",
      "details": "Request did not complete within 30 seconds",
      "traceId": "0HN8P4J5K2M1L0N"
    }
    ```

---

### Tag: Advanced Features
All endpoints UNSECURED

#### Content Transformation
- [ ] `POST /transform/uppercase` - Text to uppercase
  - Body: `{ "text": "hello world" }`
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "original": "hello world",
      "result": "HELLO WORLD",
      "transformation": "uppercase"
    }
    ```

- [ ] `POST /transform/encrypt` - Encrypt data
  - Body: `{ "data": "secret message" }`
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "original": "secret message",
      "encrypted": "u2FsdGVkX1...",
      "algorithm": "AES-256",
      "encoding": "base64"
    }
    ```

- [ ] `POST /transform/decompress` - Decompress data
  - Body: Gzip/deflate compressed data
  - Status: `200 OK` / `400 Bad Request`
  - Returns:
    ```json
    {
      "decompressed": "original content...",
      "originalSize": 1024,
      "compressedSize": 512,
      "algorithm": "gzip"
    }
    ```

#### Compression Testing
- [ ] `GET /test/compression?algorithm=gzip|deflate` - Response compression
  - Query: algorithm type
  - Status: `200 OK`
  - Returns (response header):
    ```
    HTTP/1.1 200 OK
    Content-Encoding: gzip
    Content-Type: application/json
    Vary: Accept-Encoding
    ```
  - Returns (body):
    ```json
    {
      "message": "Response compressed with gzip",
      "originalSize": 1024,
      "compressedSize": 256,
      "compressionRatio": 0.25
    }
    ```

#### Metadata & Tracing
- [ ] `GET /test/metadata` - Request/response metadata
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "request": {
        "method": "GET",
        "path": "/test/metadata",
        "headers": {...},
        "timestamp": "2024-01-15T10:30:00Z"
      },
      "response": {
        "statusCode": 200,
        "processingTime": 45,
        "timestamp": "2024-01-15T10:30:00.045Z"
      }
    }
    ```

- [ ] `GET /test/tracing` - Distributed trace info
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "traceId": "0HN8P4J5K2M1L0N",
      "spans": [
        {
          "spanId": "span-1",
          "name": "http-request",
          "startTime": "2024-01-15T10:30:00Z",
          "duration": 45,
          "tags": {"http.method": "GET", "http.path": "/test/tracing"}
        }
      ]
    }
    ```

#### Rate Limiting Headers
- [ ] `GET /test/rate-limited` - Rate limited (10 req/min)
  - Status: `200 OK` / `429 Too Many Requests`
  - Returns (success):
    ```json
    {
      "message": "Request successful",
      "rateLimit": {
        "limit": 10,
        "remaining": 5,
        "reset": "2024-01-15T11:30:00Z"
      }
    }
    ```
  - Headers:
    ```
    X-RateLimit-Limit: 10
    X-RateLimit-Remaining: 5
    X-RateLimit-Reset: 1705318800
    ```
  - Returns (rate limited):
    ```json
    {
      "statusCode": 429,
      "message": "Too Many Requests",
      "error": "RATE_LIMIT_EXCEEDED",
      "retryAfter": 60
    }
    ```

- [ ] `GET /test/rate-limited/high` - High limit (100 req/min)
  - Status: `200 OK` / `429 Too Many Requests`
  - Same format as above, limit: 100

- [ ] `GET /test/rate-limited/low` - Low limit (1 req/min)
  - Status: `200 OK` / `429 Too Many Requests`
  - Same format as above, limit: 1

#### Webhooks
- [ ] `POST /webhooks/register` - Register webhook
  - Body: `{ "url": "https://example.com/webhook", "events": ["file.uploaded", "user.created"] }`
  - Status: `201 Created` / `400 Bad Request`
  - Returns:
    ```json
    {
      "webhookId": "webhook-uuid",
      "url": "https://example.com/webhook",
      "events": ["file.uploaded", "user.created"],
      "secret": "whsec_test1234567890",
      "active": true,
      "createdAt": "2024-01-15T10:30:00Z"
    }
    ```

- [ ] `POST /webhooks/{webhookId}/test` - Send test webhook
  - Status: `200 OK` / `404 Not Found`
  - Returns:
    ```json
    {
      "webhookId": "webhook-uuid",
      "testId": "test-uuid",
      "status": "success",
      "responseStatus": 200,
      "responseTime": 250,
      "payload": {...}
    }
    ```

#### Versioning
- [ ] `GET /api/v1/test/simple` - API v1 endpoint
  - Status: `200 OK` / `410 Gone` (if deprecated)
  - Returns:
    ```json
    {
      "version": "1.0.0",
      "message": "API v1 endpoint",
      "deprecation": "v1 will be deprecated on 2024-12-31"
    }
    ```

- [ ] `GET /api/v2/test/simple` - API v2 endpoint
  - Status: `200 OK`
  - Returns:
    ```json
    {
      "version": "2.0.0",
      "message": "API v2 endpoint"
    }
    ```

- [ ] `GET /test/simple?api-version=1.0` - Header-based versioning
  - Query: `api-version=1.0|2.0`
  - Status: `200 OK`
  - Returns version-specific response

#### Idempotency
- [ ] `POST /test/idempotent` - Idempotent request with deduplication
  - Header: `Idempotency-Key: {uuid}`
  - Status: `200 OK` / `409 Conflict`
  - Returns (first request):
    ```json
    {
      "id": "request-uuid",
      "idempotencyKey": "idempotency-key-value",
      "status": "created",
      "result": {...},
      "createdAt": "2024-01-15T10:30:00Z"
    }
    ```
  - Returns (duplicate with same key):
    ```json
    {
      "id": "request-uuid",
      "idempotencyKey": "idempotency-key-value",
      "status": "duplicate",
      "result": {...},
      "originalTimestamp": "2024-01-15T10:30:00Z"
    }
    ```

---

## Endpoint Summary by Tag

| Tag | Count | Unsecured? |
|-----|-------|-----------|
| Health & Monitoring | 4 | ✅ All |
| Authentication & Security | 13 | ✅ All |
| HTTP Methods (GET/POST/PUT/PATCH/DELETE) | 11 | ✅ All |
| Query Parameters | 7 | ✅ All |
| Path Parameters | 6 | ✅ All |
| Body - JSON | 3 | ✅ All |
| Body - XML | 2 | ✅ All |
| Body - Form Data | 3 | ✅ All |
| Body - Form URL Encoded | 2 | ✅ All |
| Body - Plain Text | 2 | ✅ All |
| Headers | 4 | ✅ All |
| File Upload | 4 | ✅ All |
| File Download | 4 | ✅ All |
| GraphQL | 10+ | ✅ All |
| WebSocket | 8+ | ✅ All |
| Error Handling | 7 | ✅ All |
| Advanced Features | 12+ | ✅ All |
| **TOTAL** | **~100+** | **✅ ALL** |

---

## Gateway Testing Scenarios

### Routing Tests
- [ ] Simple path routing: `/test/simple` → backend
- [ ] Path prefix stripping: `/api/v1/...` strip `/api`
- [ ] Multiple route patterns with same endpoint
- [ ] Route matching priority (exact > prefix > wildcard)

### Parameter Tests
- [ ] Query string preservation through gateway
- [ ] Path parameter extraction and forwarding
- [ ] Body parameter forwarding (all content-types: JSON/XML/Form/etc)
- [ ] Header forwarding through gateway
- [ ] Parameter validation error propagation

### Content Type Tests
- [ ] JSON content-type handling
- [ ] XML content-type handling
- [ ] Form data (multipart) handling
- [ ] Form URL encoded handling
- [ ] Plain text handling
- [ ] Binary (octet-stream) handling

### Protocol Tests
- [ ] HTTP method preservation (GET/POST/PUT/PATCH/DELETE)
- [ ] WebSocket upgrade through gateway
- [ ] GraphQL execution through gateway
- [ ] File streaming through gateway

### Security Tests
- [ ] Basic auth header forwarding
- [ ] Bearer token forwarding
- [ ] API key header forwarding
- [ ] Custom header preservation
- [ ] 401/403 error responses

### Performance Tests
- [ ] Concurrent request handling
- [ ] Large payload processing
- [ ] File streaming (range requests)
- [ ] Response compression
- [ ] Request timeout handling

---

## Success Criteria

✅ **All Endpoints Implemented**
- [ ] ~100+ endpoints across 17 tags
- [ ] All HTTP methods (GET, POST, PUT, PATCH, DELETE)
- [ ] All parameter types (path, query, body all formats, headers)
- [ ] File operations (upload/download with all variants)
- [ ] GraphQL (queries, mutations, introspection, subscriptions)
- [ ] WebSocket (connection, channels, messaging)
- [ ] Authentication utilities (login, token refresh, key generation)
- [ ] Error responses (all HTTP status codes)
- [ ] Advanced features (compression, versioning, webhooks, idempotency)

✅ **Swagger Documentation**
- [ ] Default unsecured Swagger UI
- [ ] 4 security variant Swagger UIs (Basic/Bearer/API-Key/Combined)
- [ ] OpenAPI JSON specs for each variant
- [ ] ReDoc documentation
- [ ] All endpoints properly tagged

✅ **Gateway Validation Ready**
- [ ] All endpoints accessible through gateway
- [ ] Parameters preserved through gateway
- [ ] Headers forwarded correctly
- [ ] File uploads/downloads streaming
- [ ] GraphQL execution working
- [ ] WebSocket upgrade successful
- [ ] Error responses propagated

✅ **Deliverables**
- [ ] Running API service on port 5000
- [ ] Docker image buildable
- [ ] Kubernetes manifests available
- [ ] Complete Swagger/OpenAPI specs
- [ ] Sample requests (Postman collection)
- [ ] API endpoint documentation
