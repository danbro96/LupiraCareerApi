using Lupira.Testing.Postgres;
using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace LupiraCareerApi.IntegrationTests;

public sealed class CareerApiTestFactory : LupiraApiFactory<Program>
{
    public IDocumentStore Store => Services.GetRequiredService<IDocumentStore>();

    protected override string AuthentikSlug => "lupira-career";

    protected override Task ApplySchemaAsync() => Store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();

    protected override Task ResetDataAsync() => Store.Advanced.ResetAllData();
}
