using LupiraCareerApi.Dtos;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class MeTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Me_returns_jit_provisioned_identity()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var me = await api.GetFromJsonAsync<MeDto>("/me", TestJson.Options);
        Assert.Equal("anna@strivo.se", me!.Email);
        Assert.NotEqual(Guid.Empty, me.Id);
    }

    [Fact]
    public async Task Same_email_resolves_to_the_same_principal()
    {
        var first = await GetMyIdAsync(Factory.ApiClient("anna@strivo.se"));
        var second = await GetMyIdAsync(Factory.ApiClient("anna@strivo.se"));
        Assert.Equal(first, second);
    }
}
