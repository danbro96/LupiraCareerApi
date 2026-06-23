using LupiraCareerApi.Auth;
using LupiraCareerApi.Domain;
using LupiraCareerApi.Endpoints;
using LupiraCareerApi.Handlers;
using LupiraCareerApi.Health;
using LupiraCareerApi.Mcp;
using Marten;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Bounded context (data + transport-neutral services). Connection string is resolved lazily from
// configuration (ConnectionStrings:Postgres) inside AddCareerCore.
builder.Services.AddCareerCore();

// Host-only services: identity (claims -> Core PrincipalDirectory) + the thin REST handlers.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();
builder.Services.AddScoped<MeHandler>();
builder.Services.AddScoped<ProfileHandler>();
builder.Services.AddScoped<OrganizationsHandler>();
builder.Services.AddScoped<EngagementsHandler>();
builder.Services.AddScoped<ProjectsHandler>();
builder.Services.AddScoped<SkillsHandler>();
builder.Services.AddScoped<GoalsHandler>();
builder.Services.AddScoped<ArtifactsHandler>();
builder.Services.AddScoped<MediaHandler>();
builder.Services.AddScoped<ResumeHandler>();
builder.Services.AddScoped<PublicPortfolioHandler>();

// Auth: OIDC JWT for the owner surface (at root). Every endpoint requires an authenticated principal.
var authBuilder = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

// Development-only: allow X-Dev-User header auth so the API can be exercised without Authentik.
if (builder.Environment.IsDevelopment())
    authBuilder.AddScheme<AuthenticationSchemeOptions, DevAuthHandler>(DevAuthHandler.SchemeName, _ => { });

string[] apiSchemes = builder.Environment.IsDevelopment()
    ? [JwtBearerDefaults.AuthenticationScheme, DevAuthHandler.SchemeName]
    : [JwtBearerDefaults.AuthenticationScheme];

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ApiPolicy", p => p.AddAuthenticationSchemes(apiSchemes).RequireAuthenticatedUser())
    // Public portfolio surface: still requires a valid token (no anonymous reads), but the owner comes from the
    // route handle rather than the caller's principal. Separate from ApiPolicy so a scope/sub gate can be added
    // here later without touching the owner routes.
    .AddPolicy("PublicReadPolicy", p => p.AddAuthenticationSchemes(apiSchemes).RequireAuthenticatedUser());

// Observability: OpenTelemetry -> OpenObserve, env-gated. The OTLP exporter reads OTEL_EXPORTER_OTLP_*
// automatically (protocol + Basic auth header set in compose).
var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("lupira-career-api"))
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation();
        t.AddHttpClientInstrumentation();
        t.AddSource(Telemetry.ActivitySourceName);
        if (!string.IsNullOrWhiteSpace(otlpEndpoint)) t.AddOtlpExporter();
    })
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation();
        m.AddHttpClientInstrumentation();
        m.AddRuntimeInstrumentation();
        if (!string.IsNullOrWhiteSpace(otlpEndpoint)) m.AddOtlpExporter();
    });

// Logs -> OpenObserve via OTLP, same env gate as traces/metrics.
builder.Logging.AddOpenTelemetry(o =>
{
    o.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("lupira-career-api"));
    o.IncludeScopes = true;
    o.IncludeFormattedMessage = true;
    if (!string.IsNullOrWhiteSpace(otlpEndpoint)) o.AddOtlpExporter();
});

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseReadyCheck>("postgres", tags: ["ready"]);

// Enums serialize as their names on the wire (not ints), consistent with the Marten store.
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

// MCP server for the agent, mounted at /mcp (LAN/WireGuard-only — not published through the tunnel).
builder.Services.AddMcpServer().WithHttpTransport().WithTools<CareerTools>();

var app = builder.Build();

// Deliberate, one-shot schema apply (used as a deploy step: `dotnet LupiraCareerApi.dll --apply-schema`).
if (args.Contains("--apply-schema"))
{
    var store = app.Services.GetRequiredService<IDocumentStore>();
    await store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();
    Console.WriteLine("Schema applied.");
    return;
}

// Behind the Cloudflare Tunnel the public host differs from the container, so honor forwarded headers.
var forwarded = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
};
forwarded.KnownIPNetworks.Clear();
forwarded.KnownProxies.Clear();
app.UseForwardedHeaders(forwarded);

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();              // /openapi/v1.json
app.MapScalarApiReference();   // /scalar/v1

// Health probes: /livez = liveness (no dependency checks); /readyz = readiness (Postgres reachable).
app.MapHealthChecks("/livez", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/readyz", new HealthCheckOptions { Predicate = c => c.Tags.Contains("ready") })
    .DisableHttpMetrics();

// Owner write/read surface (at root), one MapXxx per resource.
app.MapMe();
app.MapProfile();
app.MapOrganizations();
app.MapEngagements();
app.MapProjects();
app.MapSkills();
app.MapGoals();
app.MapArtifacts();
app.MapMedia();
app.MapResume();

// Public, handle-addressed read surface (at /public/{handle}); gated by a valid token, not owner-scoped.
app.MapPublicPortfolio();

// Agent MCP transport (LAN/WireGuard-only; excluded from the Cloudflare Tunnel at the edge).
app.MapMcp("/mcp").RequireAuthorization("ApiPolicy");

app.Run();

// Exposes the implicit Program entry point to the integration test assembly (WebApplicationFactory<Program>).
public partial class Program;
