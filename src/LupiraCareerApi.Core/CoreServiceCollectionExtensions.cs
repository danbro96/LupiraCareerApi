using JasperFx;
using LupiraCareerApi;
using LupiraCareerApi.Application;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Registers the LupiraCareerApi bounded context (Marten event store + document store + transport-neutral
/// services) into the host's DI container.</summary>
public static class CoreServiceCollectionExtensions
{
    public const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=lupira_career;Username=lupira_career_user;Password=devpassword";

    public static IServiceCollection AddCareerCore(this IServiceCollection services)
    {
        // Resolve the connection string lazily from IConfiguration so test hosts (WebApplicationFactory) can
        // override ConnectionStrings:Postgres before the store is built.
        services.AddMarten(sp =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Postgres") ?? DefaultConnectionString;
            var env = sp.GetRequiredService<IHostEnvironment>();
            var opts = new StoreOptions();
            opts.Connection(connectionString);
            // Prod never mutates schema on boot; DDL is a deliberate --apply-schema step. Dev auto-creates
            // so `dotnet run` and the integration tests self-provision.
            opts.AutoCreateSchemaObjects = env.IsDevelopment() ? AutoCreate.CreateOrUpdate : AutoCreate.None;
            opts.UseLupiraCareer();
            return opts;
        }).UseLightweightSessions();

        services.AddScoped<PrincipalDirectory>();
        services.AddScoped<ProfileService>();
        services.AddScoped<OrganizationService>();
        services.AddScoped<EngagementService>();
        services.AddScoped<ProjectService>();
        services.AddScoped<SkillService>();
        services.AddScoped<GoalService>();
        services.AddScoped<ArtifactService>();
        services.AddScoped<MediaService>();
        services.AddScoped<ResumeService>();
        services.AddScoped<PublicPortfolioService>();
        return services;
    }
}
