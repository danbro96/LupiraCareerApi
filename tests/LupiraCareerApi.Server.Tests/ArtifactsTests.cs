using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class ArtifactsTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Register_with_blank_url_or_title_is_rejected()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        Assert.Equal(HttpStatusCode.BadRequest, (await api.PostAsJsonAsync("/api/artifacts", new RegisterArtifactRequest { Kind = ArtifactKind.Repo, Url = "  ", Title = "Title", Description = null, ProducedOn = null }, TestJson.Options)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await api.PostAsJsonAsync("/api/artifacts", new RegisterArtifactRequest { Kind = ArtifactKind.Repo, Url = "https://x", Title = "  ", Description = null, ProducedOn = null }, TestJson.Options)).StatusCode);
    }

    [Fact]
    public async Task Register_get_list_and_patch()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var artifact = await RegisterArtifactAsync(api, "https://github.com/x/y", "Repo");

        var got = await api.GetFromJsonAsync<ArtifactDto>($"/api/artifacts/{artifact.Id}", TestJson.Options);
        Assert.Equal("Repo", got!.Title);

        var list = await api.GetFromJsonAsync<List<ArtifactDto>>("/api/artifacts", TestJson.Options);
        Assert.Contains(list!, a => a.Id == artifact.Id);

        var resp = await api.PatchAsJsonAsync($"/api/artifacts/{artifact.Id}", new UpdateArtifactRequest { Url = "https://github.com/x/z", Title = "Renamed", Description = "desc" }, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        var updated = await resp.Content.ReadFromJsonAsync<ArtifactDto>(TestJson.Options);
        Assert.Equal("Renamed", updated!.Title);
        Assert.Equal("https://github.com/x/z", updated.Url);
        Assert.Equal("desc", updated.Description);
    }

    [Fact]
    public async Task Link_and_unlink_a_project()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var project = await CreateProjectAsync(api);
        var artifact = await RegisterArtifactAsync(api);

        var linked = await (await api.PutAsync($"/api/artifacts/{artifact.Id}/projects/{project.Id}", null)).Content.ReadFromJsonAsync<ArtifactDto>(TestJson.Options);
        Assert.Contains(project.Id, linked!.LinkedProjectIds);

        var unlinked = await (await api.DeleteAsync($"/api/artifacts/{artifact.Id}/projects/{project.Id}")).Content.ReadFromJsonAsync<ArtifactDto>(TestJson.Options);
        Assert.DoesNotContain(project.Id, unlinked!.LinkedProjectIds);
    }

    [Fact]
    public async Task Link_and_unlink_a_skill_with_role()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var skill = await RegisterSkillAsync(api);
        var artifact = await RegisterArtifactAsync(api);

        var linked = await (await api.PutAsJsonAsync($"/api/artifacts/{artifact.Id}/skills/{skill.Id}", new ArtifactSkillRoleRequest { Role = ArtifactRole.Output }, TestJson.Options)).Content.ReadFromJsonAsync<ArtifactDto>(TestJson.Options);
        Assert.Contains(linked!.LinkedSkills, l => l.SkillId == skill.Id && l.Role == ArtifactRole.Output);

        var unlinked = await (await api.DeleteAsync($"/api/artifacts/{artifact.Id}/skills/{skill.Id}")).Content.ReadFromJsonAsync<ArtifactDto>(TestJson.Options);
        Assert.DoesNotContain(unlinked!.LinkedSkills, l => l.SkillId == skill.Id);
    }

    [Fact]
    public async Task Link_and_unlink_an_engagement()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api);
        var eng = await CreateEngagementAsync(api, org.Id);
        var artifact = await RegisterArtifactAsync(api);

        var linked = await (await api.PutAsync($"/api/artifacts/{artifact.Id}/engagements/{eng.Id}", null)).Content.ReadFromJsonAsync<ArtifactDto>(TestJson.Options);
        Assert.Contains(eng.Id, linked!.LinkedEngagementIds);

        var unlinked = await (await api.DeleteAsync($"/api/artifacts/{artifact.Id}/engagements/{eng.Id}")).Content.ReadFromJsonAsync<ArtifactDto>(TestJson.Options);
        Assert.DoesNotContain(eng.Id, unlinked!.LinkedEngagementIds);
    }

    [Fact]
    public async Task Delete_archives_the_artifact()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var artifact = await RegisterArtifactAsync(api);
        var archived = await (await api.DeleteAsync($"/api/artifacts/{artifact.Id}")).Content.ReadFromJsonAsync<ArtifactDto>(TestJson.Options);
        Assert.True(archived!.Archived);
        var got = await api.GetFromJsonAsync<ArtifactDto>($"/api/artifacts/{artifact.Id}", TestJson.Options);
        Assert.True(got!.Archived);
    }

    [Fact]
    public async Task Operations_on_a_missing_artifact_are_404()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var missing = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.NotFound, (await api.GetAsync($"/api/artifacts/{missing}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.PutAsync($"/api/artifacts/{missing}/projects/{Guid.NewGuid()}", null)).StatusCode);
    }
}
