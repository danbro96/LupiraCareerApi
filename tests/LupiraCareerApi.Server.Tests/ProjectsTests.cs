using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class ProjectsTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Create_with_non_owned_engagement_is_rejected()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var resp = await api.PostAsJsonAsync("/api/projects",
            new CreateProjectRequest { Kind = ProjectKind.Professional, Title = "X", Description = null, EngagementId = Guid.NewGuid(), Url = null, Start = null }, TestJson.Options);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Create_get_and_patch()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var project = await CreateProjectAsync(api, "Rebuild");

        var got = await api.GetFromJsonAsync<ProjectDto>($"/api/projects/{project.Id}", TestJson.Options);
        Assert.Equal("Rebuild", got!.Title);
        Assert.Equal(ProjectStatus.Active, got.Status);

        var resp = await api.PatchAsJsonAsync($"/api/projects/{project.Id}", new UpdateProjectRequest { Title = "Rebuilt", Description = "new desc", Url = "https://x" }, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        var updated = await resp.Content.ReadFromJsonAsync<ProjectDto>(TestJson.Options);
        Assert.Equal("Rebuilt", updated!.Title);
        Assert.Equal("new desc", updated.Description);
        Assert.Equal("https://x", updated.Url);
    }

    [Fact]
    public async Task Ship_shelve_and_archive_transition_status()
    {
        var api = Factory.ApiClient("anna@strivo.se");

        var shipped = await (await api.PostAsJsonAsync($"/api/projects/{(await CreateProjectAsync(api)).Id}/ship", new ShipProjectRequest { ShippedOn = new DateOnly(2021, 9, 1), Outcome = "launched" }, TestJson.Options)).Content.ReadFromJsonAsync<ProjectDto>(TestJson.Options);
        Assert.Equal(ProjectStatus.Shipped, shipped!.Status);
        Assert.Equal(new DateOnly(2021, 9, 1), shipped.End);
        Assert.Equal("launched", shipped.Outcome);

        var shelved = await (await api.PostAsJsonAsync($"/api/projects/{(await CreateProjectAsync(api)).Id}/shelve", "on hold", TestJson.Options)).Content.ReadFromJsonAsync<ProjectDto>(TestJson.Options);
        Assert.Equal(ProjectStatus.Shelved, shelved!.Status);

        var archived = await (await api.PostAsync($"/api/projects/{(await CreateProjectAsync(api)).Id}/archive", null)).Content.ReadFromJsonAsync<ProjectDto>(TestJson.Options);
        Assert.Equal(ProjectStatus.Archived, archived!.Status);
    }

    [Fact]
    public async Task Engagement_attach_and_detach()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api);
        var eng = await CreateEngagementAsync(api, org.Id);
        var project = await CreateProjectAsync(api);

        var attached = await (await api.PutAsJsonAsync($"/api/projects/{project.Id}/engagement", new AttachEngagementRequest { EngagementId = eng.Id }, TestJson.Options)).Content.ReadFromJsonAsync<ProjectDto>(TestJson.Options);
        Assert.Equal(eng.Id, attached!.EngagementId);

        var detached = await (await api.DeleteAsync($"/api/projects/{project.Id}/engagement")).Content.ReadFromJsonAsync<ProjectDto>(TestJson.Options);
        Assert.Null(detached!.EngagementId);
    }

    [Fact]
    public async Task Skill_attach_and_detach()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var project = await CreateProjectAsync(api);
        var skill = await RegisterSkillAsync(api);

        var attached = await (await api.PutAsync($"/api/projects/{project.Id}/skills/{skill.Id}", null)).Content.ReadFromJsonAsync<ProjectDto>(TestJson.Options);
        Assert.Contains(skill.Id, attached!.SkillIds);
        var detached = await (await api.DeleteAsync($"/api/projects/{project.Id}/skills/{skill.Id}")).Content.ReadFromJsonAsync<ProjectDto>(TestJson.Options);
        Assert.DoesNotContain(skill.Id, detached!.SkillIds);
    }

    [Fact]
    public async Task List_filtered_by_engagement()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api);
        var eng = await CreateEngagementAsync(api, org.Id);
        var underEng = await CreateProjectAsync(api, "Under", eng.Id);
        await CreateProjectAsync(api, "Standalone");

        var filtered = await api.GetFromJsonAsync<List<ProjectDto>>($"/api/projects?engagementId={eng.Id}", TestJson.Options);
        Assert.Single(filtered!);
        Assert.Equal(underEng.Id, filtered![0].Id);
    }

    [Fact]
    public async Task Reverse_lookups_return_linked_artifacts_and_media()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var project = await CreateProjectAsync(api);

        var artifact = await RegisterArtifactAsync(api);
        (await api.PutAsync($"/api/artifacts/{artifact.Id}/projects/{project.Id}", null)).EnsureSuccessStatusCode();

        var media = await RegisterMediaAsync(api);
        (await api.PutAsJsonAsync($"/api/media/{media.Id}/projects/{project.Id}", new MediaProjectRoleRequest { Role = MediaRole.Hero }, TestJson.Options)).EnsureSuccessStatusCode();

        var artifacts = await api.GetFromJsonAsync<List<ArtifactDto>>($"/api/projects/{project.Id}/artifacts", TestJson.Options);
        Assert.Contains(artifacts!, a => a.Id == artifact.Id);

        var mediaList = await api.GetFromJsonAsync<List<MediaDto>>($"/api/projects/{project.Id}/media", TestJson.Options);
        Assert.Contains(mediaList!, m => m.Id == media.Id);
    }

    [Fact]
    public async Task Operations_on_a_missing_project_are_404()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var missing = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.NotFound, (await api.GetAsync($"/api/projects/{missing}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.PatchAsJsonAsync($"/api/projects/{missing}", new UpdateProjectRequest { Title = "x", Description = null, Url = null }, TestJson.Options)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.PostAsync($"/api/projects/{missing}/archive", null)).StatusCode);
    }
}
