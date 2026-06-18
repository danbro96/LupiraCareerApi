using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class EngagementsTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Create_requires_an_owned_organization()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var resp = await api.PostAsJsonAsync("/api/engagements",
            new CreateEngagementRequest(EngagementKind.Employment, Guid.NewGuid(), new DateOnly(2020, 1, 1), null, null));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Create_and_get_resolves_organization_name()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api, "Strivo AB");
        var eng = await CreateEngagementAsync(api, org.Id);

        var got = await api.GetFromJsonAsync<EngagementDto>($"/api/engagements/{eng.Id}");
        Assert.Equal("Strivo AB", got!.OrganizationName);
        Assert.Equal(org.Id, got.OrganizationId);
    }

    [Fact]
    public async Task List_is_newest_first_by_start()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api);
        await CreateEngagementAsync(api, org.Id, new DateOnly(2018, 1, 1));
        await CreateEngagementAsync(api, org.Id, new DateOnly(2022, 1, 1));

        var list = await api.GetFromJsonAsync<List<EngagementDto>>("/api/engagements");
        Assert.Equal(new DateOnly(2022, 1, 1), list![0].Start);
        Assert.Equal(new DateOnly(2018, 1, 1), list[1].Start);
    }

    [Fact]
    public async Task Patch_revises_summary_kind_location_and_end()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api);
        var eng = await CreateEngagementAsync(api, org.Id);

        var resp = await api.PatchAsJsonAsync($"/api/engagements/{eng.Id}",
            new UpdateEngagementRequest("Lead role", EngagementKind.Volunteer, new Location(LocationKind.Office, "Stockholm", "SE"), new DateOnly(2023, 1, 1), "moved on"));
        resp.EnsureSuccessStatusCode();
        var updated = await resp.Content.ReadFromJsonAsync<EngagementDto>();
        Assert.Equal("Lead role", updated!.Summary);
        Assert.Equal(EngagementKind.Volunteer, updated.Kind);
        Assert.Equal("Stockholm", updated.Location?.City);
        Assert.Equal(new DateOnly(2023, 1, 1), updated.End);
    }

    [Fact]
    public async Task Titles_assume_then_patch_revise_and_retire()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api);
        var eng = await CreateEngagementAsync(api, org.Id);

        await api.PostAsJsonAsync($"/api/engagements/{eng.Id}/titles", new AssumeTitleRequest("Developer", new DateOnly(2020, 1, 1)));
        var afterSecond = await (await api.PostAsJsonAsync($"/api/engagements/{eng.Id}/titles", new AssumeTitleRequest("Senior Developer", new DateOnly(2022, 1, 1)))).Content.ReadFromJsonAsync<EngagementDto>();
        Assert.Equal("Senior Developer", afterSecond!.CurrentTitle);

        var seniorId = afterSecond.Titles.Single(t => t.Text == "Senior Developer").TitleId;
        var resp = await api.PatchAsJsonAsync($"/api/engagements/{eng.Id}/titles/{seniorId}", new UpdateTitleRequest("Staff Engineer", new DateOnly(2024, 1, 1)));
        resp.EnsureSuccessStatusCode();
        var patched = await resp.Content.ReadFromJsonAsync<EngagementDto>();
        var staff = patched!.Titles.Single(t => t.TitleId == seniorId);
        Assert.Equal("Staff Engineer", staff.Text);
        Assert.Equal(new DateOnly(2024, 1, 1), staff.To);
    }

    [Fact]
    public async Task Skill_attach_then_detach()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api);
        var eng = await CreateEngagementAsync(api, org.Id);
        var skill = await RegisterSkillAsync(api, "Kafka");

        var attached = await (await api.PutAsync($"/api/engagements/{eng.Id}/skills/{skill.Id}", null)).Content.ReadFromJsonAsync<EngagementDto>();
        Assert.Contains(skill.Id, attached!.SkillIds);

        var detached = await (await api.DeleteAsync($"/api/engagements/{eng.Id}/skills/{skill.Id}")).Content.ReadFromJsonAsync<EngagementDto>();
        Assert.DoesNotContain(skill.Id, detached!.SkillIds);
    }

    [Fact]
    public async Task Operations_on_a_missing_engagement_are_404()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var missing = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.NotFound, (await api.GetAsync($"/api/engagements/{missing}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.PatchAsJsonAsync($"/api/engagements/{missing}", new UpdateEngagementRequest("x", null, null, null, null))).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.PostAsJsonAsync($"/api/engagements/{missing}/titles", new AssumeTitleRequest("Dev", new DateOnly(2020, 1, 1)))).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.PutAsync($"/api/engagements/{missing}/skills/{Guid.NewGuid()}", null)).StatusCode);
    }
}
