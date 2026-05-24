using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using WebApiTestHarness.GraphQL;
using WebApiTestHarness.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Bind to 0.0.0.0 on the PORT env var (Render sets this); fallback to 10000
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();

// Forward proxy headers so Swagger UI links work behind Render's reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    // Main unified document
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Web API Test Harness",
        Version = "v1",
        Description = "Comprehensive Web API for validating API Gateway (KrakenD) capabilities. All endpoints are UNSECURED by default."
    });

    // Separate documents for each auth type
    c.SwaggerDoc("basicauth", new OpenApiInfo
    {
        Title = "Basic Auth Secured Endpoints",
        Version = "v1",
        Description = "API endpoints secured with HTTP Basic Authentication"
    });
    c.SwaggerDoc("bearer", new OpenApiInfo
    {
        Title = "Bearer Token Secured Endpoints",
        Version = "v1",
        Description = "API endpoints secured with Bearer Token authentication"
    });
    c.SwaggerDoc("jwt", new OpenApiInfo
    {
        Title = "JWT Token Secured Endpoints",
        Version = "v1",
        Description = "API endpoints secured with JWT Bearer Token authentication"
    });
    c.SwaggerDoc("apikey", new OpenApiInfo
    {
        Title = "API Key Secured Endpoints",
        Version = "v1",
        Description = "API endpoints secured with API Key authentication"
    });
    c.SwaggerDoc("oauth", new OpenApiInfo
    {
        Title = "OAuth 2.0 Secured Endpoints",
        Version = "v1",
        Description = "API endpoints secured with OAuth 2.0 authentication"
    });

    c.AddSecurityDefinition("BasicAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Basic Authentication — Authorization: Basic base64(username:password)"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Bearer — Authorization: Bearer {token}"
    });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Description = "API Key Authentication — X-API-Key: {key}"
    });
    c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("http://localhost:5000/secure/oauth/authorize"),
                TokenUrl = new Uri("http://localhost:5000/secure/oauth/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "read", "Read access" },
                    { "write", "Write access" },
                    { "delete", "Delete access" }
                }
            }
        },
        Description = "OAuth 2.0 Authentication"
    });

    // Document inclusion predicate to separate by document
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (docName == "v1")
        {
            // Main document includes everything
            return true;
        }

        var controllerName = apiDesc.ActionDescriptor?.RouteValues?["controller"]?.ToString() ?? "";

        return docName switch
        {
            "basicauth" => controllerName.Contains("BasicAuth", StringComparison.OrdinalIgnoreCase),
            "bearer" => controllerName.Contains("BearerToken", StringComparison.OrdinalIgnoreCase),
            "jwt" => controllerName.Contains("JwtToken", StringComparison.OrdinalIgnoreCase),
            "apikey" => controllerName.Contains("ApiKey", StringComparison.OrdinalIgnoreCase),
            "oauth" => controllerName.Contains("OAuth", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    });
});

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

// Must be first — handle forwarded headers from Render's proxy
app.UseForwardedHeaders();

// WebSocket support
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30)
});

// Route WebSocket connections
app.Map("/ws/connect", async context =>
{
    await WebSocketHandler.HandleAsync(context);
});

// Swagger — always enabled (needed for gateway testing, no env check)
app.UseSwagger();

// Main Swagger UI (unified view with all documents)
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "📋 All Endpoints (Unified View)");
    c.SwaggerEndpoint("/swagger/basicauth/swagger.json", "🔐 Basic Auth");
    c.SwaggerEndpoint("/swagger/bearer/swagger.json", "🎫 Bearer Token");
    c.SwaggerEndpoint("/swagger/jwt/swagger.json", "🔑 JWT Token");
    c.SwaggerEndpoint("/swagger/apikey/swagger.json", "🗝️ API Key");
    c.SwaggerEndpoint("/swagger/oauth/swagger.json", "🌐 OAuth 2.0");
    c.DocumentTitle = "Web API Test Harness - Complete Documentation";
    c.DisplayRequestDuration();
    c.EnableFilter();
    c.EnableDeepLinking();
});

// Individual Swagger UIs for each auth type
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger/basicauth";
    c.SwaggerEndpoint("/swagger/basicauth/swagger.json", "Basic Auth Secured Endpoints");
    c.DocumentTitle = "Basic Auth - Web API Test Harness";
    c.DisplayRequestDuration();
    c.EnableFilter();
    c.EnableDeepLinking();
});

app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger/bearer";
    c.SwaggerEndpoint("/swagger/bearer/swagger.json", "Bearer Token Secured Endpoints");
    c.DocumentTitle = "Bearer Token - Web API Test Harness";
    c.DisplayRequestDuration();
    c.EnableFilter();
    c.EnableDeepLinking();
});

app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger/jwt";
    c.SwaggerEndpoint("/swagger/jwt/swagger.json", "JWT Token Secured Endpoints");
    c.DocumentTitle = "JWT Token - Web API Test Harness";
    c.DisplayRequestDuration();
    c.EnableFilter();
    c.EnableDeepLinking();
});

app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger/apikey";
    c.SwaggerEndpoint("/swagger/apikey/swagger.json", "API Key Secured Endpoints");
    c.DocumentTitle = "API Key - Web API Test Harness";
    c.DisplayRequestDuration();
    c.EnableFilter();
    c.EnableDeepLinking();
});

app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger/oauth";
    c.SwaggerEndpoint("/swagger/oauth/swagger.json", "OAuth 2.0 Secured Endpoints");
    c.DocumentTitle = "OAuth 2.0 - Web API Test Harness";
    c.DisplayRequestDuration();
    c.EnableFilter();
    c.EnableDeepLinking();
});

app.UseReDoc(c =>
{
    c.DocumentTitle = "Web API Test Harness - ReDoc";
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.RoutePrefix = "redoc";
});

app.MapControllers();
app.MapGraphQL();

// Root redirect to Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();
