# Quick Swagger Access Guide

## 🔗 All Swagger URLs

```
Main Unified View:
http://localhost:5000/swagger/

Individual Auth Type Views:
http://localhost:5000/swagger/basicauth/
http://localhost:5000/swagger/bearer/
http://localhost:5000/swagger/jwt/
http://localhost:5000/swagger/apikey/
http://localhost:5000/swagger/oauth/
```

---

## 📊 What You'll See

### Main Unified View (`/swagger/`)
```
Dropdown Menu at Top:
┌─────────────────────────────────────────────────┐
│ 📋 All Endpoints (Unified View)              [v]│
│ 🔐 Basic Auth                                    │
│ 🎫 Bearer Token                                  │
│ 🔑 JWT Token                                     │
│ 🗝️ API Key                                       │
│ 🌐 OAuth 2.0                                     │
└─────────────────────────────────────────────────┘
```

Click any option to switch documentation instantly.

---

## 🎯 Individual Views

### Basic Auth View (`/swagger/basicauth/`)
- GET `/secure/basic-auth/protected` - Protected endpoint
- GET `/secure/basic-auth/credentials` - Get valid credentials
- Try It Out button to test endpoints
- Pre-filled security headers

### Bearer Token View (`/swagger/bearer/`)
- GET `/secure/bearer-token/protected` - Protected endpoint
- GET `/secure/bearer-token/token-info` - Get valid tokens
- Try It Out button to test endpoints

### JWT View (`/swagger/jwt/`)
- GET `/secure/jwt/protected` - Protected endpoint
- GET `/secure/jwt/decode` - Decode JWT info
- GET `/secure/jwt/generate` - Generate JWT token
- Try It Out button to test endpoints

### API Key View (`/swagger/apikey/`)
- GET `/secure/api-key/protected` - Protected endpoint
- GET `/secure/api-key/key-info` - Get valid API keys
- Try It Out button to test endpoints

### OAuth 2.0 View (`/swagger/oauth/`)
- POST `/secure/oauth/authorize` - Authorization endpoint
- POST `/secure/oauth/token` - Token endpoint
- GET `/secure/oauth/protected` - Protected endpoint
- GET `/secure/oauth/userinfo` - OAuth user info
- Try It Out button to test endpoints

---

## 🧪 Quick Testing Workflow

### For Basic Auth:
1. Go to `/swagger/basicauth/`
2. Click on `GET /secure/basic-auth/credentials`
3. Click "Try It Out" → "Execute"
4. Copy a credential (e.g., admin:password123)
5. Go to `GET /secure/basic-auth/protected`
6. Click "Try It Out"
7. In Authorization, select "BasicAuth"
8. Enter username and password
9. Click "Execute" to see protected data

### For Bearer Token:
1. Go to `/swagger/bearer/`
2. Click on `GET /secure/bearer-token/token-info`
3. Click "Try It Out" → "Execute"
4. Copy a token (e.g., bearer_token_admin_12345)
5. Go to `GET /secure/bearer-token/protected`
6. Click "Try It Out"
7. In Authorization, select "Bearer"
8. Paste token in the value field
9. Click "Execute" to see protected data

### For JWT:
1. Go to `/swagger/jwt/`
2. Click on `GET /secure/jwt/generate`
3. Set query param: `username=admin`
4. Click "Try It Out" → "Execute"
5. Copy the generated token
6. Go to `GET /secure/jwt/protected`
7. Click "Try It Out"
8. In Authorization, select "Bearer"
9. Paste token in the value field
10. Click "Execute" to see protected data

### For API Key:
1. Go to `/swagger/apikey/`
2. Click on `GET /secure/api-key/key-info`
3. Click "Try It Out" → "Execute"
4. Copy an API key (e.g., sk_live_admin_key_abc123def456)
5. Go to `GET /secure/api-key/protected`
6. Click "Try It Out"
7. In Authorization, select "ApiKey"
8. Paste key in the X-API-Key field
9. Click "Execute" to see protected data

### For OAuth 2.0:
1. Go to `/swagger/oauth/`
2. Click on `POST /secure/oauth/authorize`
3. Click "Try It Out"
4. Set parameters:
   - client_id: `client_123456`
   - redirect_uri: `http://localhost:3000/callback`
   - response_type: `code`
   - username: `admin`
5. Click "Execute"
6. Copy the `authorizationCode` from response
7. Click on `POST /secure/oauth/token`
8. Click "Try It Out"
9. Set form parameters:
   - client_id: `client_123456`
   - client_secret: `client_secret_abcdefghij1234567890`
   - code: {paste authorizationCode}
   - grant_type: `authorization_code`
   - redirect_uri: `http://localhost:3000/callback`
10. Click "Execute"
11. Copy the `access_token` from response
12. Go to `GET /secure/oauth/protected`
13. Click "Try It Out"
14. In Authorization, select "OAuth2" (or "Bearer")
15. Paste token in the value field
16. Click "Execute" to see protected data

---

## 🎨 Features Available in All Views

- 🔍 **Filter** - Search/filter endpoints by name
- 🔗 **Deep Linking** - Copy URL to specific endpoint
- ⏱️ **Request Duration** - See how long requests take
- 📋 **Try It Out** - Test endpoints directly in Swagger
- 🔐 **Security** - Pre-configured auth schemes

---

## 💡 Pro Tips

1. **Use the main view** (`/swagger/`) to explore all endpoints
2. **Use individual views** (`/swagger/{authtype}/`) to focus on specific auth type
3. **Try It Out feature** - Test directly from browser without curl
4. **Deep linking** - Share specific endpoint URLs with team members
5. **Filter feature** - Search for endpoints by keyword

---

## 🔄 Switching Between Views (In Main UI)

1. Look at the top of the page
2. Find the dropdown/selector showing current document
3. Click to see all available documents
4. Select the one you want
5. Page refreshes with new documentation

Or just navigate directly to the URL you want.

---

## 🚀 Live Testing

All endpoints can be tested directly from Swagger:

1. Click "Try It Out" button
2. Fill in required parameters
3. Add authorization headers (auto-filled if configured)
4. Click "Execute"
5. See response with status code and timing

---

## 📱 Mobile Access

Swagger UI is fully responsive and works on mobile devices:

```
Mobile URLs:
m.localhost:5000/swagger/
m.localhost:5000/swagger/jwt/
m.localhost:5000/swagger/oauth/
```

---

## 🔐 Security Note

- All Swagger endpoints are **public and unsecured**
- This is intentional for gateway testing
- Swagger itself requires no authentication
- Protected endpoints require valid credentials when called

---

## 🆘 Troubleshooting

**Q: Can't see the dropdown menu?**
A: Look at the top left area of the Swagger page. The document selector may be styled as tabs or a dropdown.

**Q: Swagger showing wrong endpoints?**
A: Clear browser cache (Ctrl+Shift+Del) and refresh.

**Q: Authorization not working in Try It Out?**
A: Make sure you select the right security scheme (BasicAuth, Bearer, ApiKey, or OAuth2) from the dropdown.

**Q: Can't find a specific endpoint?**
A: Use the filter/search box at the top to search by endpoint name or path.

---

## 📚 Related Files

- `SWAGGER_DOCUMENTATION.md` - Detailed Swagger setup explanation
- `SECURED_ENDPOINTS.md` - Full endpoint reference
- `CREDENTIAL_ENDPOINTS.md` - Unsecured credential endpoints
- `Program.cs` - Swagger configuration code

---

## 🎯 Quick Links (Localhost)

| View | URL |
|------|-----|
| Unified | http://localhost:5000/swagger/ |
| Basic Auth | http://localhost:5000/swagger/basicauth/ |
| Bearer Token | http://localhost:5000/swagger/bearer/ |
| JWT | http://localhost:5000/swagger/jwt/ |
| API Key | http://localhost:5000/swagger/apikey/ |
| OAuth 2.0 | http://localhost:5000/swagger/oauth/ |

---

**Enjoy your multi-document Swagger setup! 🎉**
