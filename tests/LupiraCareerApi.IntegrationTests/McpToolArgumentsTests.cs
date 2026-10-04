using Lupira.Testing.Mcp;
using Lupira.Testing.Postgres;
using Xunit;

namespace LupiraCareerApi.IntegrationTests;

[Collection("integration")]
public sealed class McpToolArgumentsTests(CareerApiTestFactory factory) : McpStrictArgumentsTests
{
    protected override HttpClient CreateAuthenticatedClient() => factory.ApiClient("alice@x.test");

    protected override string DeclaredToolName => "list_skills";

    public override Task InitializeAsync() => factory.ResetAsync();
}
