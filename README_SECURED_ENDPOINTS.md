# Web API Test Harness - Secured Endpoints

Complete authentication testing harness with **5 authentication types** and **multiple Swagger documentation views**.

---

## 🚀 Quick Start

### Start the Application
```bash
cd d:\Workspace\webapi-test-harness
dotnet run
```

### Access Swagger
```
Main (All Endpoints):  http://localhost:5000/swagger/
Basic Auth Only:       http://localhost:5000/swagger/basicauth/
Bearer Token Only:     http://localhost:5000/swagger/bearer/
JWT Only:              http://localhost:5000/swagger/jwt/
API Key Only:          http://localhost:5000/swagger/apikey/
OAuth 2.0 Only:        http://localhost:5000/swagger/oauth/
```

---

## 📋 What's Included

### 5 Authentication Types with Secured Endpoints

1. **HTTP Basic Auth**
   - Protected endpoint: `GET /secure/basic-auth/protected`
   - Credential info: `GET /secure/basic-auth/credentials`
   - Valid users: admin, user, test

2. **Bearer Token**
   - Protected endpoint: `GET /secure/bearer-token/protected`
   - Token info: `GET /secure/bearer-token/token-info`
   - Valid tokens included

3. **JWT Token**
   - Protected endpoint: `GET /secure/jwt/protected`
   - Token info: `GET /secure/jwt/decode`
   - Generate token: `GET /secure/jwt/generate`

4. **API Key**
   - Protected endpoint: `GET /secure/api-key/protected`
   - Key info: `GET /secure/api-key/key-info`
   - Valid API keys included

5. **OAuth 2.0**
   - Authorization: `POST /secure/oauth/authorize`
   - Token exchange: `POST /secure/oauth/token`
   - Protected endpoint: `GET /secure/oauth/protected`
   - User info: `GET /secure/oauth/userinfo`

---

## 🎯 Key Features

✅ **Real credential validation** - Not just mock responses  
✅ **Hardcoded test data** - No database needed  
✅ **Multiple Swagger UIs** - Unified view + 5 separate focused views  
✅ **OAuth 2.0 full flow** - Complete authorization code implementation  
✅ **Try It Out** - Test endpoints directly from Swagger  
✅ **Comprehensive docs** - 6 documentation files  

---

## 📚 Documentation Files

### Getting Started
- **README_SECURED_ENDPOINTS.md** (this file) - Quick overview
- **SWAGGER_ACCESS_GUIDE.md** - How to use each Swagger UI with examples

### API Reference
- **SECURED_ENDPOINTS.md** - Complete endpoint documentation with examples
- **CREDENTIAL_ENDPOINTS.md** - How to get credentials for testing

### Technical Details
- **SWAGGER_DOCUMENTATION.md** - How multiple Swagger documents work
- **SWAGGER_STRUCTURE.md** - Visual diagrams of the architecture
- **IMPLEMENTATION_SUMMARY.md** - What was implemented and why

---

## 🔐 Test Credentials

### Basic Auth
```
Username: admin     Password: password123
Username: user      Password: user@123
Username: test      Password: test@123
```

### Bearer Tokens
```
bearer_token_admin_12345
bearer_token_user_67890
bearer_token_guest_11111
```

### JWT
```
Generate via: GET /secure/jwt/generate?username=admin|user|guest
Pre-generated tokens available in SECURED_ENDPOINTS.md
```

### API Keys
```
sk_live_admin_key_abc123def456
sk_live_user_key_xyz789uvw012
sk_test_limited_key_pqr345stu678
```

### OAuth 2.0
```
Client ID:     client_123456
Client Secret: client_secret_abcdefghij1234567890
Users:         admin, user, guest
```

---

## 🧪 Testing Example (JWT)

```bash
# 1. Generate a JWT token
curl http://localhost:5000/secure/jwt/generate?username=admin

# Response includes: token, username, role, permissions, expiresAt

# 2. Use the token to access protected endpoint
curl -H "Authorization: Bearer {token}" \
     http://localhost:5000/secure/jwt/protected

# Response: Protected data with user information
```

---

## 📊 Swagger Architecture

### Unified View
- **URL:** `/swagger/`
- **Shows:** All 10+ endpoints from all 5 auth types
- **Features:** Dropdown menu to switch between different auth type views

### Individual Focused Views
- **Basic Auth:** `/swagger/basicauth/` - 2 endpoints
- **Bearer Token:** `/swagger/bearer/` - 2 endpoints
- **JWT:** `/swagger/jwt/` - 3 endpoints
- **API Key:** `/swagger/apikey/` - 2 endpoints
- **OAuth 2.0:** `/swagger/oauth/` - 4 endpoints

---

## 🔄 Usage Pattern

```
1. Choose your auth type
   ↓
2. Get credentials/token (credential endpoint)
   ↓
3. Use credentials to test protected endpoint
   ↓
4. Success! See protected response
```

---

## 🎛️ How It Works

### Real Credential Validation
Each authentication type validates credentials like a real API:
- **Basic Auth:** Decodes and validates username:password
- **Bearer Token:** Checks token against valid list
- **JWT:** Validates structure and expiry
- **API Key:** Checks against valid keys
- **OAuth 2.0:** Full authorization flow with code/token exchange

### Document Separation
Swagger automatically filters endpoints by controller:
- BasicAuthSecuredController → basicauth document
- BearerTokenSecuredController → bearer document
- JwtTokenSecuredController → jwt document
- ApiKeySecurityController → apikey document
- OAuth2SecuredController → oauth document

### Try It Out Feature
Test endpoints directly from Swagger:
1. Click endpoint
2. Click "Try It Out"
3. Add credentials/headers
4. Click "Execute"
5. See response

---

## 🚀 Common Tasks

### I want to test Basic Auth
→ Go to `/swagger/basicauth/`  
→ Get credentials from credential endpoint  
→ Use in protected endpoint  

### I want to see all endpoints
→ Go to `/swagger/`  
→ Use dropdown to switch between types  

### I want to test OAuth 2.0 flow
→ Go to `/swagger/oauth/`  
→ Follow the flow: authorize → token → protected  

### I want to share JWT documentation with a team
→ Share URL: `http://your-host/swagger/jwt/`  
→ They see only JWT endpoints  

### I want to understand the architecture
→ Read `SWAGGER_STRUCTURE.md` for visual diagrams  
→ Read `IMPLEMENTATION_SUMMARY.md` for details  

---

## 🔍 Finding What You Need

| Need | File | URL |
|------|------|-----|
| Quick overview | This file | README_SECURED_ENDPOINTS.md |
| How to use Swagger | SWAGGER_ACCESS_GUIDE.md | See docs folder |
| Endpoint details & examples | SECURED_ENDPOINTS.md | See docs folder |
| How to get credentials | CREDENTIAL_ENDPOINTS.md | See docs folder |
| Technical architecture | SWAGGER_DOCUMENTATION.md | See docs folder |
| Visual diagrams | SWAGGER_STRUCTURE.md | See docs folder |
| What was built & why | IMPLEMENTATION_SUMMARY.md | See docs folder |
| Swagger UI | /swagger/ | Browser |
| JWT endpoints | /swagger/jwt/ | Browser |
| OAuth endpoints | /swagger/oauth/ | Browser |

---

## 💡 Tips & Tricks

1. **Use the dropdown in `/swagger/`** to quickly switch between auth type views
2. **Copy the exact URL** for a specific auth type to share with teammates
3. **Swagger Try It Out** is easiest for testing - no curl needed
4. **Get credentials first** by calling the credential endpoint
5. **Check the response** to see permissions and role information
6. **Test expired scenarios** by checking expiry times in responses

---

## 🎓 Learning Paths

### For Beginners
1. Start at `/swagger/`
2. Read SWAGGER_ACCESS_GUIDE.md for walkthrough
3. Test Basic Auth first (simplest)
4. Graduate to Bearer Token
5. Try JWT with generate feature
6. Test OAuth 2.0 flow

### For Gateway Integration
1. Read SECURED_ENDPOINTS.md for full endpoint reference
2. Review IMPLEMENTATION_SUMMARY.md for architecture
3. Use individual Swagger UIs for documentation
4. Test each auth type with your gateway

### For Developers
1. Review SWAGGER_DOCUMENTATION.md for setup details
2. Check SWAGGER_STRUCTURE.md for architecture
3. Look at Program.cs for configuration code
4. Review controller code for validation logic

---

## 🔧 Build & Run

### Prerequisites
- .NET 8.0 or later
- Windows/Linux/Mac

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Access
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger/

---

## 📋 Project Structure

```
Controllers/
├── BasicAuthSecuredController.cs
├── BearerTokenSecuredController.cs
├── JwtTokenSecuredController.cs
├── ApiKeySecurityController.cs
└── OAuth2SecuredController.cs

Program.cs (Swagger configuration)

Documentation/
├── README_SECURED_ENDPOINTS.md (this file)
├── SWAGGER_ACCESS_GUIDE.md
├── SECURED_ENDPOINTS.md
├── CREDENTIAL_ENDPOINTS.md
├── SWAGGER_DOCUMENTATION.md
├── SWAGGER_STRUCTURE.md
└── IMPLEMENTATION_SUMMARY.md
```

---

## ✅ What's Tested

- ✅ Basic Auth credential validation
- ✅ Bearer Token validation
- ✅ JWT token generation and validation
- ✅ API Key validation
- ✅ OAuth 2.0 authorization code flow
- ✅ Swagger document separation
- ✅ Multiple Swagger UIs
- ✅ Try It Out functionality
- ✅ Credential endpoints

---

## 🎉 You're All Set!

Everything is configured and ready to test:

1. ✅ 5 authentication types implemented
2. ✅ Real credential validation
3. ✅ Multiple Swagger documentation views
4. ✅ Comprehensive documentation
5. ✅ Ready for API gateway testing

**Start with `/swagger/` and explore!**

---

## 📞 Quick Links

**Swagger UIs:**
- All: http://localhost:5000/swagger/
- JWT: http://localhost:5000/swagger/jwt/
- OAuth: http://localhost:5000/swagger/oauth/
- API Key: http://localhost:5000/swagger/apikey/
- Bearer: http://localhost:5000/swagger/bearer/
- Basic Auth: http://localhost:5000/swagger/basicauth/

**Documentation:**
- Getting started: SWAGGER_ACCESS_GUIDE.md
- Full reference: SECURED_ENDPOINTS.md
- How it works: SWAGGER_DOCUMENTATION.md
- Architecture: SWAGGER_STRUCTURE.md

---

**Happy testing! 🚀**
