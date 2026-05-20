using Microsoft.OpenApi.Models;
using WebApiTestHarness.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web API Test Harness - Default", Version = "v1" });
    c.SwaggerDoc("basic", new OpenApiInfo { Title = "Web API Test Harness - Basic Auth", Version = "v1" });
    c.SwaggerDoc("bearer", new OpenApiInfo { Title = "Web API Test Harness - Bearer Token", Version = "v1" });
    c.SwaggerDoc("apikey", new OpenApiInfo { Title = "Web API Test Harness - API Key", Version = "v1" });
    c.SwaggerDoc("combined", new OpenApiInfo { Title = "Web API Test Harness - Combined", Version = "v1" });

    c.AddSecurityDefinition("BasicAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Basic Authentication"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Description = "API Key Authentication"
    });

    // Note: Applying security requirements globally for the documents that need them.
    // Swashbuckle usually applies these to all docs if added this way.
    // For a more granular control per document, one would typically use an IDocumentFilter.
});

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

app.UseWebSockets();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Default (Unsecured)");
    c.SwaggerEndpoint("/swagger/basic/swagger.json", "Basic Auth Example");
    c.SwaggerEndpoint("/swagger/bearer/swagger.json", "Bearer Token Example");
    c.SwaggerEndpoint("/swagger/apikey/swagger.json", "API Key Example");
    c.SwaggerEndpoint("/swagger/combined/swagger.json", "Combined Auth Example");
});

app.UseReDoc(c =>
{
    c.DocumentTitle = "Web API Test Harness - ReDoc";
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.RoutePrefix = "redoc";
});

app.MapControllers();
app.MapGraphQL();

app.Run();
