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
        Assert.Equal(HttpStatusCode.BadRequest, (await api.PostAsJsonAsync("/api/media", new RegisterMediaRequest("  ", "image/png", null, null, "alt", null))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await api.PostAsJsonAsync("/api/media", new RegisterMediaRequest("career/x.png", "  ", null, null, "alt", null))).StatusCode);
    }

    [Fact]
    public async Task Register_get_and_list()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var media = await RegisterMediaAsync(api, "career/hero.png", "image/png");

        var got = await api.GetFromJsonAsync<MediaDto>($"/api/media/{media.Id}");
        Assert.Equal("career/hero.png", got!.BlobRef);

        var list = await api.GetFromJsonAsync<List<MediaDto>>("/api/media");
        Assert.Contains(list!, m => m.Id == media.Id);
    }

    [Fact]
    public async Task Link_and_unlink_a_project_with_role()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var project = await CreateProjectAsync(api);
        var media = await RegisterMediaAsync(api);

        var linked = await (await api.PutAsJsonAsync($"/api/media/{media.Id}/projects/{project.Id}", new MediaProjectRoleRequest(MediaRole.Hero))).Content.ReadFromJsonAsync<MediaDto>();
        Assert.Contains(linked!.LinkedProjects, p => p.ProjectId == project.Id && p.Role == MediaRole.Hero);

        var unlinked = await (await api.DeleteAsync($"/api/media/{media.Id}/projects/{project.Id}")).Content.ReadFromJsonAsync<MediaDto>();
        Assert.DoesNotContain(unlinked!.LinkedProjects, p => p.ProjectId == project.Id);
    }

    [Fact]
    public async Task Link_and_unlink_a_skill()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var skill = await RegisterSkillAsync(api);
        var media = await RegisterMediaAsync(api);

        var linked = await (await api.PutAsync($"/api/media/{media.Id}/skills/{skill.Id}", null)).Content.ReadFromJsonAsync<MediaDto>();
        Assert.Contains(skill.Id, linked!.LinkedSkillIds);

        var unlinked = await (await api.DeleteAsync($"/api/media/{media.Id}/skills/{skill.Id}")).Content.ReadFromJsonAsync<MediaDto>();
        Assert.DoesNotContain(skill.Id, unlinked!.LinkedSkillIds);
    }

    [Fact]
    public async Task Delete_archives_the_media()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var media = await RegisterMediaAsync(api);
        var archived = await (await api.DeleteAsync($"/api/media/{media.Id}")).Content.ReadFromJsonAsync<MediaDto>();
        Assert.True(archived!.Archived);
    }

    [Fact]
    public async Task Operations_on_a_missing_media_are_404()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var missing = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.NotFound, (await api.GetAsync($"/api/media/{missing}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.PutAsync($"/api/media/{missing}/skills/{Guid.NewGuid()}", null)).StatusCode);
    }
}
