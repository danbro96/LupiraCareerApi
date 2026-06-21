using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class MediaTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Register_with_blank_blobref_or_mimetype_is_rejected()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        Assert.Equal(HttpStatusCode.BadRequest, (await api.PostAsJsonAsync("/media", new RegisterMediaRequest { BlobRef = "  ", MimeType = "image/png", Width = null, Height = null, AltText = "alt", Caption = null }, TestJson.Options)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await api.PostAsJsonAsync("/media", new RegisterMediaRequest { BlobRef = "career/x.png", MimeType = "  ", Width = null, Height = null, AltText = "alt", Caption = null }, TestJson.Options)).StatusCode);
    }

    [Fact]
    public async Task Register_get_and_list()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var media = await RegisterMediaAsync(api, "career/hero.png", "image/png");

        var got = await api.GetFromJsonAsync<MediaDto>($"/media/{media.Id}", TestJson.Options);
        Assert.Equal("career/hero.png", got!.BlobRef);

        var list = await api.GetFromJsonAsync<List<MediaDto>>("/media", TestJson.Options);
        Assert.Contains(list!, m => m.Id == media.Id);
    }

    [Fact]
    public async Task Link_and_unlink_a_project_with_role()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var project = await CreateProjectAsync(api);
        var media = await RegisterMediaAsync(api);

        var linked = await (await api.PutAsJsonAsync($"/media/{media.Id}/projects/{project.Id}", new MediaProjectRoleRequest { Role = MediaRole.Hero }, TestJson.Options)).Content.ReadFromJsonAsync<MediaDto>(TestJson.Options);
        Assert.Contains(linked!.LinkedProjects, p => p.ProjectId == project.Id && p.Role == MediaRole.Hero);

        var unlinked = await (await api.DeleteAsync($"/media/{media.Id}/projects/{project.Id}")).Content.ReadFromJsonAsync<MediaDto>(TestJson.Options);
        Assert.DoesNotContain(unlinked!.LinkedProjects, p => p.ProjectId == project.Id);
    }

    [Fact]
    public async Task Link_and_unlink_a_skill()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var skill = await RegisterSkillAsync(api);
        var media = await RegisterMediaAsync(api);

        var linked = await (await api.PutAsync($"/media/{media.Id}/skills/{skill.Id}", null)).Content.ReadFromJsonAsync<MediaDto>(TestJson.Options);
        Assert.Contains(skill.Id, linked!.LinkedSkillIds);

        var unlinked = await (await api.DeleteAsync($"/media/{media.Id}/skills/{skill.Id}")).Content.ReadFromJsonAsync<MediaDto>(TestJson.Options);
        Assert.DoesNotContain(skill.Id, unlinked!.LinkedSkillIds);
    }

    [Fact]
    public async Task Delete_archives_the_media()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var media = await RegisterMediaAsync(api);
        var archived = await (await api.DeleteAsync($"/media/{media.Id}")).Content.ReadFromJsonAsync<MediaDto>(TestJson.Options);
        Assert.True(archived!.Archived);
    }

    [Fact]
    public async Task Operations_on_a_missing_media_are_404()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var missing = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.NotFound, (await api.GetAsync($"/media/{missing}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.PutAsync($"/media/{missing}/skills/{Guid.NewGuid()}", null)).StatusCode);
    }
}
