# Swagger Structure Visualization

## 🗂️ Swagger Document Organization

```
┌─────────────────────────────────────────────────────────────────┐
│                    SWAGGER INFRASTRUCTURE                       │
└─────────────────────────────────────────────────────────────────┘

                         Program.cs Configuration
                                 │
                    ┌────────────┴────────────┐
                    │                         │
            SwaggerGen Setup          SwaggerUI Setup
                    │                         │
        ┌───────────┼───────────┐    ┌──────┴──────┐
        │           │           │    │             │
    Register     Security     Filter  Main UI    Individual UIs
    Documents   Definitions   Rules   (dropdown)  (6 routes)
    (6 docs)    (5 types)


┌─────────────────────────────────────────────────────────────────┐
│                      6 SWAGGER DOCUMENTS                        │
├──────────┬──────────┬──────────┬──────────┬──────────┬──────────┤
│    v1    │basicauth │ bearer   │   jwt    │ apikey   │  oauth   │
│ Unified  │BasicAuth │ Bearer   │   JWT    │  API Key │ OAuth 2.0│
│  View    │ Secured  │ Secured  │ Secured  │ Secured  │ Secured  │
└──────────┴──────────┴──────────┴──────────┴──────────┴──────────┘
```

---

## 🌐 Access Points

```
┌───────────────────────────────────────────────────────────┐
│         SWAGGER USER INTERFACE ACCESS ROUTES              │
├───────────────────────────────────────────────────────────┤
│                                                           │
│  /swagger/                    /swagger/basicauth/        │
│  ├─ Unified View             ├─ Basic Auth Only         │
│  ├─ Dropdown Menu             ├─ 2 Endpoints           │
│  ├─ All 10 Endpoints          └─ Focused Docs          │
│  └─ Switch Between Docs                                  │
│                               /swagger/bearer/           │
│  /swagger/index.html         ├─ Bearer Token Only       │
│  └─ Same as /swagger/        ├─ 2 Endpoints            │
│                              └─ Focused Docs            │
│                                                          │
│                               /swagger/jwt/             │
│                               ├─ JWT Only               │
│                               ├─ 3 Endpoints            │
│                               └─ Focused Docs           │
│                                                          │
│                               /swagger/apikey/          │
│                               ├─ API Key Only           │
│                               ├─ 2 Endpoints            │
│                               └─ Focused Docs           │
│                                                          │
│                               /swagger/oauth/           │
│                               ├─ OAuth 2.0 Only         │
│                               ├─ 4 Endpoints            │
│                               └─ Focused Docs           │
└───────────────────────────────────────────────────────────┘
```

---

## 📄 JSON Specification Endpoints

```
┌────────────────────────────────────────────────────────────┐
│        OPENAPI SPECIFICATION (JSON) ENDPOINTS              │
├──────────────────────┬──────────────────┬─────────────────┤
│   Document ID        │   JSON Endpoint  │   Document Name │
├──────────────────────┼──────────────────┼─────────────────┤
│   v1                 │ /swagger/v1/     │ All Endpoints   │
│   basicauth          │ /swagger/        │ Basic Auth      │
│                      │   basicauth/     │                 │
│   bearer             │ /swagger/bearer/ │ Bearer Token    │
│   jwt                │ /swagger/jwt/    │ JWT Token       │
│   apikey             │ /swagger/        │ API Key         │
│                      │   apikey/        │                 │
│   oauth              │ /swagger/oauth/  │ OAuth 2.0       │
└──────────────────────┴──────────────────┴─────────────────┘
```

---

## 🎛️ Main Swagger UI Dropdown

```
┌──────────────────────────────────────┐
│  📋 All Endpoints (Unified View) ▼   │
├──────────────────────────────────────┤
│ ✓ 📋 All Endpoints (Unified View)    │
│   🔐 Basic Auth                      │
│   🎫 Bearer Token                    │
│   🔑 JWT Token                       │
│   🗝️ API Key                         │
│   🌐 OAuth 2.0                       │
└──────────────────────────────────────┘

Click any option to switch documentation instantly
```

---

## 🔀 Document Inclusion Logic

```
┌─────────────────────────────────────────────────────────┐
│         DOCUMENT FILTERING BY CONTROLLER NAME           │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  if docName == "v1":                                    │
│      return true  ✓ (Include all controllers)          │
│                                                         │
│  if docName == "basicauth":                            │
│      return controllerName.Contains("BasicAuth")       │
│      ✓ BasicAuthSecuredController                      │
│      ✗ All others                                      │
│                                                         │
│  if docName == "bearer":                               │
│      return controllerName.Contains("BearerToken")     │
│      ✓ BearerTokenSecuredController                    │
│      ✗ All others                                      │
│                                                         │
│  if docName == "jwt":                                  │
│      return controllerName.Contains("JwtToken")        │
│      ✓ JwtTokenSecuredController                       │
│      ✗ All others                                      │
│                                                         │
│  if docName == "apikey":                               │
│      return controllerName.Contains("ApiKey")          │
│      ✓ ApiKeySecurityController                        │
│      ✗ All others                                      │
│                                                         │
│  if docName == "oauth":                                │
│      return controllerName.Contains("OAuth")           │
│      ✓ OAuth2SecuredController                         │
│      ✗ All others                                      │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## 📦 Controller Mapping

```
┌──────────────────────────────────────────────────────────┐
│            CONTROLLERS → SWAGGER DOCUMENTS               │
├──────────────────────┬─────────────────────────────────┤
│     Controller       │     Appears In Documents        │
├──────────────────────┼─────────────────────────────────┤
│ BasicAuthSecured     │ ✓ v1 (unified)                  │
│                      │ ✓ basicauth                     │
│                      │ ✗ bearer, jwt, apikey, oauth    │
│                      │                                 │
│ BearerTokenSecured   │ ✓ v1 (unified)                  │
│                      │ ✓ bearer                        │
│                      │ ✗ basicauth, jwt, apikey, oauth │
│                      │                                 │
│ JwtTokenSecured      │ ✓ v1 (unified)                  │
│                      │ ✓ jwt                           │
│                      │ ✗ basicauth, bearer, apikey,oauth
│                      │                                 │
│ ApiKeySecured        │ ✓ v1 (unified)                  │
│                      │ ✓ apikey                        │
│                      │ ✗ basicauth, bearer, jwt, oauth │
│                      │                                 │
│ OAuth2Secured        │ ✓ v1 (unified)                  │
│                      │ ✓ oauth                         │
│                      │ ✗ basicauth, bearer, jwt, apikey
└──────────────────────┴─────────────────────────────────┘
```

---

## 🔐 Security Definitions

```
┌───────────────────────────────────────────────────────────┐
│           SECURITY SCHEMES IN SWAGGER DOCUMENTS           │
├─────────────────────────────┬─────────────────────────────┤
│     Security Type           │    Swagger Definition       │
├─────────────────────────────┼─────────────────────────────┤
│ HTTP Basic Auth             │ Type: Http                  │
│                             │ Scheme: basic               │
│                             │ Header: Authorization       │
│                             │                             │
│ Bearer Token                │ Type: Http                  │
│                             │ Scheme: bearer              │
│                             │ Format: JWT                 │
│                             │ Header: Authorization       │
│                             │                             │
│ API Key                     │ Type: ApiKey                │
│                             │ Name: X-API-Key             │
│                             │ In: Header                  │
│                             │                             │
│ OAuth 2.0                   │ Type: OAuth2                │
│                             │ Flow: Authorization Code    │
│                             │ Auth URL: /authorize        │
│                             │ Token URL: /token           │
│                             │ Scopes: read, write, delete │
└─────────────────────────────┴─────────────────────────────┘
```

---

## 🌳 Complete Endpoint Tree

```
API Endpoints
│
├── /secure/basic-auth
│   ├── GET /protected (requires auth)
│   └── GET /credentials (no auth)
│
├── /secure/bearer-token
│   ├── GET /protected (requires auth)
│   └── GET /token-info (no auth)
│
├── /secure/jwt
│   ├── GET /protected (requires auth)
│   ├── GET /decode (no auth)
│   └── GET /generate (no auth)
│
├── /secure/api-key
│   ├── GET /protected (requires auth)
│   └── GET /key-info (no auth)
│
└── /secure/oauth
    ├── POST /authorize (no auth)
    ├── POST /token (no auth)
    ├── GET /protected (requires auth)
    └── GET /userinfo (no auth)


Swagger Documents:
├── v1 (unified) → All 10+ endpoints
├── basicauth → 2 Basic Auth endpoints
├── bearer → 2 Bearer Token endpoints
├── jwt → 3 JWT endpoints
├── apikey → 2 API Key endpoints
└── oauth → 4 OAuth 2.0 endpoints
```

---

## 📊 Data Flow Diagram

```
┌──────────────────────────────────────────────────────────┐
│                   USER INTERACTION FLOW                  │
└──────────────────────────────────────────────────────────┘

User Browser
    │
    ├─→ GET /swagger/
    │   │
    │   └─→ Server Serves HTML
    │       │
    │       ├─→ Loads v1 Swagger JSON
    │       ├─→ References other JSON endpoints
    │       └─→ Renders Dropdown Menu
    │
    ├─→ User Clicks Dropdown
    │   │
    │   └─→ Selects "Bearer Token"
    │       │
    │       └─→ Browser Loads /swagger/bearer/swagger.json
    │           │
    │           └─→ Gets Bearer Token endpoints only
    │
    ├─→ User Clicks "Try It Out"
    │   │
    │   └─→ Enters Authorization Credentials
    │       │
    │       └─→ Clicks Execute
    │           │
    │           └─→ API Validates & Returns Response


┌──────────────────────────────────────────────────────────┐
│              BACKEND PROCESSING FLOW                     │
└──────────────────────────────────────────────────────────┘

Request → Route Handler
    │
    ├─→ Check Authorization Header
    │   │
    │   ├─→ Valid? → Process & Return 200
    │   └─→ Invalid? → Return 401
    │
    ├─→ For Credential Endpoints:
    │   │
    │   └─→ No Auth Check → Return Hardcoded Data
    │
    └─→ Swagger Intercepts (for /swagger/*)
        │
        ├─→ Get Document ID from Route
        │
        ├─→ Apply Inclusion Predicate
        │   │
        │   ├─→ If v1: Include all endpoints
        │   └─→ If specific: Filter by controller
        │
        └─→ Return Filtered OpenAPI JSON
```

---

## 🔄 Request Processing Example (JWT)

```
1. User visits /swagger/jwt/
   ↓
2. Browser loads HTML for /swagger/jwt
   ↓
3. HTML references /swagger/jwt/swagger.json
   ↓
4. Server generates Swagger v2
   ↓
5. SwaggerGen runs DocInclusionPredicate:
   - docName = "jwt"
   - Scans all controllers
   - Finds JwtTokenSecuredController
   - Includes only its endpoints
   ↓
6. Excludes: BasicAuth, Bearer, ApiKey, OAuth controllers
   ↓
7. Returns JSON with:
   - 3 JWT endpoints (/protected, /decode, /generate)
   - Bearer security definition
   - JWT-specific descriptions
   ↓
8. Browser renders Swagger UI
   ↓
9. User sees only JWT endpoints in dropdown + Try It Out
```

---

## 🎯 Unified View vs Individual Views

```
UNIFIED VIEW (/swagger/)
┌─────────────────────────────────────┐
│ Swagger Document Selector           │
├─────────────────────────────────────┤
│ ▼ All Endpoints (Unified View)      │
│   └─ 10+ endpoints from all types   │
│                                     │
│ > Basic Auth                        │
│ > Bearer Token                      │
│ > JWT Token                         │
│ > API Key                           │
│ > OAuth 2.0                         │
└─────────────────────────────────────┘
        (Click to switch)


INDIVIDUAL VIEW (/swagger/jwt/)
┌─────────────────────────────────────┐
│ JWT Token Secured Endpoints         │
├─────────────────────────────────────┤
│ GET /secure/jwt/protected           │
│ GET /secure/jwt/decode              │
│ GET /secure/jwt/generate            │
│                                     │
│ (Only 3 endpoints, focused view)    │
└─────────────────────────────────────┘
```

---

## 🔗 URL Navigation Map

```
http://localhost:5000/
    │
    ├─→ /swagger/
    │   ├─→ index.html (Unified UI with dropdown)
    │   ├─→ v1/swagger.json (All endpoints)
    │   ├─→ basicauth/swagger.json (Basic Auth only)
    │   ├─→ bearer/swagger.json (Bearer only)
    │   ├─→ jwt/swagger.json (JWT only)
    │   ├─→ apikey/swagger.json (API Key only)
    │   └─→ oauth/swagger.json (OAuth only)
    │
    ├─→ /swagger/basicauth/
    │   ├─→ index.html (Basic Auth UI)
    │   └─→ swagger.json (Basic Auth spec)
    │
    ├─→ /swagger/bearer/
    │   ├─→ index.html (Bearer Token UI)
    │   └─→ swagger.json (Bearer Token spec)
    │
    ├─→ /swagger/jwt/
    │   ├─→ index.html (JWT UI)
    │   └─→ swagger.json (JWT spec)
    │
    ├─→ /swagger/apikey/
    │   ├─→ index.html (API Key UI)
    │   └─→ swagger.json (API Key spec)
    │
    └─→ /swagger/oauth/
        ├─→ index.html (OAuth UI)
        └─→ swagger.json (OAuth spec)
```

---

## 📝 Configuration Summary

```
Program.cs Configuration
│
├─ AddSwaggerGen(c =>)
│  ├─ Register 6 Documents (v1, basicauth, bearer, jwt, apikey, oauth)
│  ├─ Register 5 Security Definitions (BasicAuth, Bearer, ApiKey, OAuth2)
│  └─ Set DocInclusionPredicate (filter by controller)
│
└─ UseSwaggerUI(c =>)
   ├─ Main UI at /swagger/
   │  └─ Shows dropdown with all 6 documents
   │
   ├─ Individual UI at /swagger/basicauth/
   ├─ Individual UI at /swagger/bearer/
   ├─ Individual UI at /swagger/jwt/
   ├─ Individual UI at /swagger/apikey/
   └─ Individual UI at /swagger/oauth/
```

---

## ✨ Key Benefits Visualization

```
WITHOUT MULTIPLE DOCUMENTS:
┌──────────────────────────────┐
│  /swagger/                   │
├──────────────────────────────┤
│ 50+ Endpoints (Messy)        │
│ - Basic Auth endpoints       │
│ - Bearer Token endpoints     │
│ - JWT endpoints              │
│ - API Key endpoints          │
│ - OAuth endpoints            │
│ - Other endpoints            │
│ - Health endpoints           │
│ - etc...                     │
│ (Hard to find what you need) │
└──────────────────────────────┘


WITH MULTIPLE DOCUMENTS:
┌──────────────────┐  ┌──────────────────┐
│  /swagger/       │  │ /swagger/jwt/    │
├──────────────────┤  ├──────────────────┤
│ All Endpoints    │  │ 3 JWT Endpoints  │
│ (Dropdown Menu)  │  │ (Focused)        │
│                  │  │ - /protected     │
│ + View each type │  │ - /decode        │
│   separately     │  │ - /generate      │
│                  │  │ (Easy to find)   │
└──────────────────┘  └──────────────────┘

Plus:
┌──────────────────┐  ┌──────────────────┐
│ /swagger/bearer/ │  │ /swagger/oauth/  │
├──────────────────┤  ├──────────────────┤
│ 2 Endpoints      │  │ 4 Endpoints      │
│ (Bearer Token)   │  │ (OAuth 2.0)      │
└──────────────────┘  └──────────────────┘
```

---

**This structure provides the perfect balance between unified overview and focused documentation!** 🎉
