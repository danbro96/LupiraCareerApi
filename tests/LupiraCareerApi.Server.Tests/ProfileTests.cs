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
        var profile = await api.GetFromJsonAsync<ProfileDto>("/profile", TestJson.Options);
        Assert.Equal("", profile!.FullName);
        Assert.Null(profile.Tagline);
    }

    [Fact]
    public async Task Upsert_with_blank_full_name_is_rejected()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var resp = await api.PutAsJsonAsync("/profile", new UpdateProfileRequest { FullName = "  ", Tagline = null, Bio = null, Location = null, GithubUrl = null, LinkedInUrl = null, WebsiteUrl = null }, TestJson.Options);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Upsert_then_get_roundtrips_all_fields()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var put = await api.PutAsJsonAsync("/profile",
            new UpdateProfileRequest { FullName = "Anna Dev", Tagline = "Builder", Bio = "Bio text", Location = "Stockholm", GithubUrl = "https://gh/anna", LinkedInUrl = "https://li/anna", WebsiteUrl = "https://anna.dev" }, TestJson.Options);
        put.EnsureSuccessStatusCode();

        var got = await api.GetFromJsonAsync<ProfileDto>("/profile", TestJson.Options);
        Assert.Equal("Anna Dev", got!.FullName);
        Assert.Equal("Builder", got.Tagline);
        Assert.Equal("Stockholm", got.Location);
        Assert.Equal("https://gh/anna", got.GithubUrl);
        Assert.Equal("https://anna.dev", got.WebsiteUrl);
    }
}
