using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

/// <summary>Multi-principal isolation: one principal cannot see or write another's career graph. A non-owned id
/// reads as 404 (its existence is not leaked) and never appears in the other principal's listings.</summary>
public class IsolationTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    private HttpClient Anna => Factory.ApiClient("anna@strivo.se");
    private HttpClient Bjorn => Factory.ApiClient("bjorn@strivo.se");

    [Fact]
    public async Task Other_principal_cannot_read_or_list_my_organization()
    {
        var org = await CreateOrganizationAsync(Anna, "Strivo AB");

        Assert.Empty((await Bjorn.GetFromJsonAsync<List<OrganizationDto>>("/api/organizations"))!);
        Assert.Equal(HttpStatusCode.NotFound, (await Bjorn.GetAsync($"/api/organizations/{org.Id}")).StatusCode);
    }

    [Fact]
    public async Task Other_principal_cannot_reference_my_organization_in_an_engagement()
    {
        var org = await CreateOrganizationAsync(Anna, "Strivo AB");
        var resp = await Bjorn.PostAsJsonAsync("/api/engagements",
            new CreateEngagementRequest(EngagementKind.Employment, org.Id, new DateOnly(2020, 1, 1), null, null));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Other_principal_cannot_read_my_engagement()
    {
        var org = await CreateOrganizationAsync(Anna);
        var eng = await CreateEngagementAsync(Anna, org.Id);
        Assert.Equal(HttpStatusCode.NotFound, (await Bjorn.GetAsync($"/api/engagements/{eng.Id}")).StatusCode);
        Assert.Empty((await Bjorn.GetFromJsonAsync<List<EngagementDto>>("/api/engagements"))!);
    }

    [Fact]
    public async Task Other_principal_cannot_read_or_mutate_my_project()
    {
        var project = await CreateProjectAsync(Anna);
        Assert.Equal(HttpStatusCode.NotFound, (await Bjorn.GetAsync($"/api/projects/{project.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Bjorn.PostAsync($"/api/projects/{project.Id}/archive", null)).StatusCode);
        Assert.Empty((await Bjorn.GetFromJsonAsync<List<ProjectDto>>("/api/projects"))!);
    }

    [Fact]
    public async Task Other_principal_cannot_read_my_skill()
    {
        var skill = await RegisterSkillAsync(Anna);
        Assert.Equal(HttpStatusCode.NotFound, (await Bjorn.GetAsync($"/api/skills/{skill.Id}")).StatusCode);
        Assert.Empty((await Bjorn.GetFromJsonAsync<List<SkillDto>>("/api/skills"))!);
    }

    [Fact]
    public async Task Other_principal_cannot_read_my_goal()
    {
        var goal = await CreateGoalAsync(Anna);
        Assert.Equal(HttpStatusCode.NotFound, (await Bjorn.GetAsync($"/api/goals/{goal.Id}")).StatusCode);
        Assert.Empty((await Bjorn.GetFromJsonAsync<List<GoalDto>>("/api/goals"))!);
    }

    [Fact]
    public async Task Other_principal_cannot_read_my_artifact()
    {
        var artifact = await RegisterArtifactAsync(Anna);
        Assert.Equal(HttpStatusCode.NotFound, (await Bjorn.GetAsync($"/api/artifacts/{artifact.Id}")).StatusCode);
        Assert.Empty((await Bjorn.GetFromJsonAsync<List<ArtifactDto>>("/api/artifacts"))!);
    }

    [Fact]
    public async Task Other_principal_cannot_read_my_media()
    {
        var media = await RegisterMediaAsync(Anna);
        Assert.Equal(HttpStatusCode.NotFound, (await Bjorn.GetAsync($"/api/media/{media.Id}")).StatusCode);
        Assert.Empty((await Bjorn.GetFromJsonAsync<List<MediaDto>>("/api/media"))!);
    }
}
