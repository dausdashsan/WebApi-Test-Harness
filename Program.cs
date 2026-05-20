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
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Web API Test Harness",
        Version = "v1",
        Description = "Comprehensive Web API for validating API Gateway (KrakenD) capabilities. All endpoints are UNSECURED by default."
    });
    c.SwaggerDoc("basic", new OpenApiInfo { Title = "Web API Test Harness - Basic Auth", Version = "v1" });
    c.SwaggerDoc("bearer", new OpenApiInfo { Title = "Web API Test Harness - Bearer Token", Version = "v1" });
    c.SwaggerDoc("apikey", new OpenApiInfo { Title = "Web API Test Harness - API Key", Version = "v1" });
    c.SwaggerDoc("combined", new OpenApiInfo { Title = "Web API Test Harness - Combined Auth", Version = "v1" });

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

    // Include all controllers in all docs
    c.DocInclusionPredicate((_, _) => true);
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
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Default (Unsecured)");
    c.SwaggerEndpoint("/swagger/basic/swagger.json", "Basic Auth Example");
    c.SwaggerEndpoint("/swagger/bearer/swagger.json", "Bearer Token Example");
    c.SwaggerEndpoint("/swagger/apikey/swagger.json", "API Key Example");
    c.SwaggerEndpoint("/swagger/combined/swagger.json", "Combined Auth Example");
    c.DocumentTitle = "Web API Test Harness";
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
