using Lupira.Auth.Jwt;
using Lupira.Hosting.Defaults;
using Lupira.Hosting.Health;
using Lupira.Hosting.LanEdge;
using Lupira.Hosting.Observability;
using Lupira.Hosting.OpenApi;
using Lupira.Hosting.Problems;
using Lupira.Identity.Marten.AspNetCore;
using Lupira.Mcp;
using Lupira.Postgres.Health;
using LupiraCareerApi.Endpoints;
using LupiraCareerApi.Handlers;
using LupiraCareerApi.Mcp;
using Marten;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Bounded context (data + transport-neutral services). Connection string is resolved lazily from
// configuration (ConnectionStrings:Postgres) inside AddCareerCore.
builder.Services.AddCareerCore();

// Host-only services: identity (claims -> PrincipalDirectory) + the thin REST handlers.
builder.Services.AddLupiraCurrentUser(o => o.StampProvenance = true);
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
builder.AddLupiraJwt();
var apiSchemes = LupiraJwtSchemes.Api(builder.Environment);

builder.Services.AddAuthorizationBuilder()
    .AddLupiraApiPolicy(apiSchemes)
    // Public portfolio surface: still requires a valid token (no anonymous reads), but the owner comes from the
    // route handle rather than the caller's principal. Separate from ApiPolicy so a scope/sub gate can be added
    // here later without touching the owner routes.
    .AddLupiraApiPolicy(apiSchemes, "PublicReadPolicy");

builder.AddLupiraTelemetry("lupira-career-api");

builder.Services.AddLupiraHealth().AddReadyCheck<DatabaseReadyCheck>("postgres");

// Enums serialize as their names on the wire (not ints), consistent with the Marten store.
builder.AddLupiraDefaults(o =>
{
    o.CaseInsensitiveProperties = true;
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
});

builder.Services.AddLupiraProblems();

builder.Services.AddOpenApi("v1", options => options.AddLupiraConventions(o =>
{
    o.Title = "Lupira Career API";
    o.Description =
        "Career and professional-history backend for Lupira. " +
        "Authenticate with a Bearer token issued by the OIDC provider (Authentik).";
}));

// MCP server for the agent, mounted at /mcp (LAN/WireGuard-only — not published through the tunnel).
builder.Services.AddLupiraMcp().WithTools<CareerTools>();

var app = builder.Build();

// Deliberate, one-shot schema apply (used as a deploy step: `dotnet LupiraCareerApi.dll --apply-schema`).
if (args.Contains("--apply-schema"))
{
    var store = app.Services.GetRequiredService<IDocumentStore>();
    await store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();
    Console.WriteLine("Schema applied.");
    return;
}

// LAN-only surfaces (/mcp + its discovery metadata): 404 anything arriving through the tunnel.
app.UseLanOnlySurfaces("/mcp", "/.well-known/oauth-protected-resource");

// Behind the Cloudflare Tunnel the public host differs from the container, so honor forwarded headers.
app.UseLupiraDefaults();
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapLupiraOpenApi(o => o.Title = "Lupira Career API");

app.MapLupiraHealth();

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

// Agent MCP transport (LAN/WireGuard-only; excluded from the Cloudflare Tunnel at the edge, with
// UseLanOnlySurfaces above as the in-process backstop).
// RFC 9728 metadata lets MCP clients discover the Authentik issuer from the 401 challenge.
app.MapMcpResourceMetadata(app.Configuration["Auth:Oidc:Authority"]);
app.MapLupiraMcp();

app.Run();

// Exposes the implicit Program entry point to the integration test assembly (WebApplicationFactory<Program>).
public partial class Program;
