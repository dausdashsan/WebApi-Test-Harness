# Swagger Documentation - Multiple Separate Views

Your API now has **6 different Swagger documentation UIs** - one unified view plus 5 specialized views for each authentication type.

---

## 📋 Swagger URLs

### Main Unified View
**`GET /swagger/`** or **`GET /swagger/index.html`**
- Shows ALL endpoints from all 5 auth types
- Can switch between different auth type documentation
- Dropdown menu to select which document to view
- Best for getting an overview of the entire API

### Separate Auth-Type Documentation

**Basic Auth:**
- URL: `GET /swagger/basicauth/`
- Shows only `BasicAuthSecuredController` endpoints
- Includes all Basic Auth protected and credential endpoints

**Bearer Token:**
- URL: `GET /swagger/bearer/`
- Shows only `BearerTokenSecuredController` endpoints
- Includes all Bearer Token protected and credential endpoints

**JWT Token:**
- URL: `GET /swagger/jwt/`
- Shows only `JwtTokenSecuredController` endpoints
- Includes all JWT protected, generate, and decode endpoints

**API Key:**
- URL: `GET /swagger/apikey/`
- Shows only `ApiKeySecurityController` endpoints
- Includes all API Key protected and credential endpoints

**OAuth 2.0:**
- URL: `GET /swagger/oauth/`
- Shows only `OAuth2SecuredController` endpoints
- Includes all OAuth 2.0 authorize, token, protected, and userinfo endpoints

---

## 📐 Document Separation Logic

Each Swagger document is automatically populated based on the controller name:

| Document | Controllers Included | URL Pattern |
|----------|-------------------|------------|
| `v1` (Unified) | All controllers | `/swagger/` |
| `basicauth` | `BasicAuthSecuredController` | `/swagger/basicauth/` |
| `bearer` | `BearerTokenSecuredController` | `/swagger/bearer/` |
| `jwt` | `JwtTokenSecuredController` | `/swagger/jwt/` |
| `apikey` | `ApiKeySecurityController` | `/swagger/apikey/` |
| `oauth` | `OAuth2SecuredController` | `/swagger/oauth/` |

---

## 🔐 Security Definitions

Each Swagger document includes security definitions:

### Basic Auth
```
Type: HTTP
Scheme: basic
Header: Authorization: Basic {base64(username:password)}
```

### Bearer Token
```
Type: HTTP Bearer
Format: JWT
Header: Authorization: Bearer {token}
```

### API Key
```
Type: API Key
Location: Header
Parameter: X-API-Key
Header: X-API-Key: {key}
```

### OAuth 2.0
```
Type: OAuth 2.0
Flow: Authorization Code
Endpoints:
  - Authorization: POST /secure/oauth/authorize
  - Token: POST /secure/oauth/token
Scopes:
  - read: Read access
  - write: Write access
  - delete: Delete access
```

---

## 🎯 Use Cases

### I want to see everything
→ Go to `/swagger/` (unified view)
→ Use the dropdown menu to switch between auth types

### I only care about Basic Auth
→ Go to `/swagger/basicauth/`
→ See only Basic Auth endpoints

### I'm testing JWT implementation
→ Go to `/swagger/jwt/`
→ See JWT endpoints with dedicated documentation

### I'm integrating OAuth 2.0
→ Go to `/swagger/oauth/`
→ See full OAuth 2.0 flow and endpoints

### I'm documenting for specific teams
→ Share `/swagger/{authtype}/` URL directly
→ Example: `/swagger/oauth/` for OAuth team

---

## 📝 What's in Each Swagger Document

### Unified Document (`/swagger/`)
- ✅ All endpoints
- ✅ All security definitions
- ✅ All operation descriptions
- ✅ Switchable via dropdown (tabs at top)

### Individual Auth Type Documents
- ✅ Only relevant endpoints for that auth type
- ✅ Security definition for that auth type
- ✅ Focused documentation
- ✅ Cleaner, less cluttered view
- ✅ Direct access without switching

---

## 🚀 How Swagger Routing Works

The configuration in `Program.cs` sets up:

1. **Swagger JSON Endpoints** (OpenAPI specs)
   - `/swagger/v1/swagger.json` - All endpoints
   - `/swagger/basicauth/swagger.json` - Basic Auth only
   - `/swagger/bearer/swagger.json` - Bearer Token only
   - `/swagger/jwt/swagger.json` - JWT only
   - `/swagger/apikey/swagger.json` - API Key only
   - `/swagger/oauth/swagger.json` - OAuth 2.0 only

2. **Swagger UI Endpoints** (HTML documentation)
   - `/swagger/` - Unified view with all documents
   - `/swagger/basicauth/` - Basic Auth documentation
   - `/swagger/bearer/` - Bearer Token documentation
   - `/swagger/jwt/` - JWT documentation
   - `/swagger/apikey/` - API Key documentation
   - `/swagger/oauth/` - OAuth 2.0 documentation

3. **Document Inclusion Predicate**
   - Automatically filters endpoints by controller name
   - Each document only shows its relevant endpoints

---

## 💡 Example: Testing Different Auth Types

### Step 1: Choose your auth type
```bash
# Go to one of these
http://localhost:5000/swagger/              # All endpoints
http://localhost:5000/swagger/basicauth/    # Basic Auth only
http://localhost:5000/swagger/bearer/       # Bearer Token only
http://localhost:5000/swagger/jwt/          # JWT only
http://localhost:5000/swagger/apikey/       # API Key only
http://localhost:5000/swagger/oauth/        # OAuth 2.0 only
```

### Step 2: Get credentials/tokens
- Click on credential endpoint (e.g., `GET /secure/basicauth/credentials`)
- Click "Try it out"
- See the response with valid credentials

### Step 3: Test protected endpoint
- Scroll to protected endpoint (e.g., `GET /secure/basicauth/protected`)
- Click "Try it out"
- Add the credential from step 2 in the Authorization header
- Click "Execute"
- See the protected response

---

## 🔧 Configuration Details

### Location: `Program.cs`

#### 1. Register Multiple Swagger Documents
```csharp
c.SwaggerDoc("v1", new OpenApiInfo { ... });           // Main
c.SwaggerDoc("basicauth", new OpenApiInfo { ... });    // Basic Auth
c.SwaggerDoc("bearer", new OpenApiInfo { ... });       // Bearer Token
c.SwaggerDoc("jwt", new OpenApiInfo { ... });          // JWT
c.SwaggerDoc("apikey", new OpenApiInfo { ... });       // API Key
c.SwaggerDoc("oauth", new OpenApiInfo { ... });        // OAuth 2.0
```

#### 2. Document Inclusion Predicate
```csharp
c.DocInclusionPredicate((docName, apiDesc) =>
{
    if (docName == "v1") return true; // Include everything
    
    var controllerName = apiDesc.ActionDescriptor?.RouteValues?["controller"];
    
    return docName switch
    {
        "basicauth" => controllerName.Contains("BasicAuth"),
        "bearer" => controllerName.Contains("BearerToken"),
        "jwt" => controllerName.Contains("JwtToken"),
        "apikey" => controllerName.Contains("ApiKey"),
        "oauth" => controllerName.Contains("OAuth"),
        _ => false
    };
});
```

#### 3. Swagger UI Middleware
```csharp
// Main UI with dropdown
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "All Endpoints");
    c.SwaggerEndpoint("/swagger/basicauth/swagger.json", "Basic Auth");
    // ... more endpoints
});

// Individual UIs
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger/basicauth";
    c.SwaggerEndpoint("/swagger/basicauth/swagger.json", "Basic Auth");
});
// ... more UI configurations
```

---

## 🎨 Swagger UI Features Enabled

For all Swagger UIs:
- ✅ `DisplayRequestDuration()` - Shows how long requests took
- ✅ `EnableFilter()` - Filter endpoints by tag
- ✅ `EnableDeepLinking()` - Share links to specific endpoints
- ✅ `EnableAnnotations()` - Show Swagger annotations

---

## 📚 Related Documentation

- See `SECURED_ENDPOINTS.md` for endpoint details
- See `CREDENTIAL_ENDPOINTS.md` for unsecured credential endpoints
- See `Program.cs` for Swagger configuration

---

## 🔄 Switching Between Views

### From Main View (`/swagger/`)
1. Look for the dropdown menu at the top
2. Select the auth type you want
3. Documentation updates instantly

### From Individual Views
1. Go directly to `/swagger/{authtype}/`
2. Or go back to `/swagger/` and select from dropdown

---

## ✨ Benefits of This Setup

1. **Flexibility** - Users can see everything or focus on one auth type
2. **Documentation Clarity** - Less visual clutter for specific auth types
3. **Developer Experience** - Easy to share specific auth type docs
4. **Testing** - Dedicated Swagger UI for each authentication method
5. **Professional** - Looks polished with organized documentation

---

## 🚨 Important Notes

- All Swagger endpoints are public (no authentication required)
- Swagger is always enabled for gateway testing
- Each endpoint appears in exactly one individual document (or all in v1)
- Documentation is auto-generated from controller names
- If you add new controllers, follow the naming pattern to include them in the right document

---

## 📞 Quick Reference

| Need | Go To |
|------|-------|
| Overview of all endpoints | `/swagger/` |
| Basic Auth documentation | `/swagger/basicauth/` |
| Bearer Token documentation | `/swagger/bearer/` |
| JWT documentation | `/swagger/jwt/` |
| API Key documentation | `/swagger/apikey/` |
| OAuth 2.0 documentation | `/swagger/oauth/` |
| Unsecured credential info | `/secure/{authtype}/credentials` or equivalent |
| Full endpoint details | See `SECURED_ENDPOINTS.md` |

---

This setup provides the **best of both worlds**: a unified view for overview and detailed, focused views for specific authentication types.
