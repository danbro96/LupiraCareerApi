using LupiraCareerApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class ProfileTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Get_returns_empty_shell_before_any_upsert()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var profile = await api.GetFromJsonAsync<ProfileDto>("/api/profile");
        Assert.Equal("", profile!.FullName);
        Assert.Null(profile.Tagline);
    }

    [Fact]
    public async Task Upsert_with_blank_full_name_is_rejected()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var resp = await api.PutAsJsonAsync("/api/profile", new UpdateProfileRequest("  ", null, null, null, null, null, null));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Upsert_then_get_roundtrips_all_fields()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var put = await api.PutAsJsonAsync("/api/profile",
            new UpdateProfileRequest("Anna Dev", "Builder", "Bio text", "Stockholm", "https://gh/anna", "https://li/anna", "https://anna.dev"));
        put.EnsureSuccessStatusCode();

        var got = await api.GetFromJsonAsync<ProfileDto>("/api/profile");
        Assert.Equal("Anna Dev", got!.FullName);
        Assert.Equal("Builder", got.Tagline);
        Assert.Equal("Stockholm", got.Location);
        Assert.Equal("https://gh/anna", got.GithubUrl);
        Assert.Equal("https://anna.dev", got.WebsiteUrl);
    }
}
