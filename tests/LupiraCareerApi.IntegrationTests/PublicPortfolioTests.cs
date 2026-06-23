using LupiraCareerApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.IntegrationTests;

/// <summary>The public, handle-addressed read surface: owner resolved from a published handle (not the caller),
/// goals never exposed, archived/retired items filtered, unpublished/unknown handles indistinguishable (404).</summary>
public class PublicPortfolioTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Published_handle_is_served_to_any_authenticated_caller()
    {
        var owner = Factory.ApiClient("anna@strivo.se");
        await PublishAsync(owner, "anna");
        var org = await CreateOrganizationAsync(owner, "Strivo AB");
        await CreateEngagementAsync(owner, org.Id);
        await CreateProjectAsync(owner, "Rebuild");
        await RegisterSkillAsync(owner, "C#");

        // A different principal entirely — the public surface is gated, not owner-scoped.
        var caller = Factory.ApiClient("svc@lupira.local");
        var portfolio = await caller.GetFromJsonAsync<PublicPortfolioDto>("/public/anna", TestJson.Options);

        Assert.Equal("Anna Dev", portfolio!.Profile.FullName);
        Assert.Single(portfolio.Engagements);
        Assert.Single(portfolio.Projects);
        Assert.Single(portfolio.Skills);
        Assert.Contains(portfolio.Experience, x => x.Title == "Rebuild");

        var profile = await caller.GetFromJsonAsync<ProfileDto>("/public/anna/profile", TestJson.Options);
        Assert.Equal("Anna Dev", profile!.FullName);
    }

    [Fact]
    public async Task Handle_is_resolved_case_insensitively()
    {
        var owner = Factory.ApiClient("anna@strivo.se");
        await PublishAsync(owner, "anna");

        var resp = await Factory.ApiClient("svc@lupira.local").GetAsync("/public/ANNA");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task Unknown_handle_is_404()
    {
        var resp = await Factory.ApiClient("svc@lupira.local").GetAsync("/public/nobody");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task Unpublished_handle_is_404()
    {
        var owner = Factory.ApiClient("anna@strivo.se");
        await owner.PutAsJsonAsync("/profile", Profile("anna", published: false), TestJson.Options);

        var resp = await Factory.ApiClient("svc@lupira.local").GetAsync("/public/anna");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task Anonymous_request_is_rejected()
    {
        var resp = await Factory.CreateClient().GetAsync("/public/anna");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Archived_projects_and_retired_skills_are_filtered_out()
    {
        var owner = Factory.ApiClient("anna@strivo.se");
        await PublishAsync(owner, "anna");

        var live = await CreateProjectAsync(owner, "Live");
        var archived = await CreateProjectAsync(owner, "Old");
        (await owner.PostAsync($"/projects/{archived.Id}/archive", null)).EnsureSuccessStatusCode();

        var liveSkill = await RegisterSkillAsync(owner, "C#");
        var retired = await RegisterSkillAsync(owner, "VB6");
        (await owner.PostAsync($"/skills/{retired.Id}/retire", null)).EnsureSuccessStatusCode();

        var caller = Factory.ApiClient("svc@lupira.local");
        var portfolio = await caller.GetFromJsonAsync<PublicPortfolioDto>("/public/anna", TestJson.Options);

        Assert.Equal([live.Id], portfolio!.Projects.Select(p => p.Id));
        Assert.Equal([liveSkill.Id], portfolio.Skills.Select(s => s.Id));
        Assert.DoesNotContain(portfolio.Experience, x => x.Id == archived.Id);

        // Detail routes hide the filtered items too, but still serve the live ones.
        Assert.Equal(HttpStatusCode.NotFound, (await caller.GetAsync($"/public/anna/projects/{archived.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await caller.GetAsync($"/public/anna/projects/{live.Id}")).StatusCode);
    }

    [Fact]
    public async Task Publishing_requires_a_handle()
    {
        var owner = Factory.ApiClient("anna@strivo.se");
        var resp = await owner.PutAsJsonAsync("/profile",
            new UpdateProfileRequest { FullName = "Anna Dev", PublicHandle = null, IsPublished = true }, TestJson.Options);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task A_handle_cannot_be_taken_by_two_owners()
    {
        await PublishAsync(Factory.ApiClient("anna@strivo.se"), "shared");

        var bob = Factory.ApiClient("bob@strivo.se");
        var resp = await bob.PutAsJsonAsync("/profile", Profile("shared"), TestJson.Options);
        Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
    }

    private static async Task PublishAsync(HttpClient api, string handle)
    {
        var resp = await api.PutAsJsonAsync("/profile", Profile(handle), TestJson.Options);
        resp.EnsureSuccessStatusCode();
    }

    private static UpdateProfileRequest Profile(string handle, bool published = true) => new()
    {
        FullName = "Anna Dev",
        Tagline = "Builder",
        PublicHandle = handle,
        IsPublished = published,
    };
}
