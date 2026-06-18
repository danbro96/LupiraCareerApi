using LupiraCareerApi.Dtos;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class ResumeTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Resume_composes_profile_engagements_projects_and_skills()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        await api.PutAsJsonAsync("/api/profile", new UpdateProfileRequest("Anna Dev", "Builder", null, null, null, null, null));
        var org = await CreateOrganizationAsync(api, "Strivo AB");
        await CreateEngagementAsync(api, org.Id);
        await CreateProjectAsync(api);
        await RegisterSkillAsync(api, "C#");

        var resume = await api.GetFromJsonAsync<ResumeDto>("/api/resume");
        Assert.Equal("Anna Dev", resume!.Profile.FullName);
        Assert.Single(resume.Engagements);
        Assert.Equal("Strivo AB", resume.Engagements[0].OrganizationName);
        Assert.Single(resume.Projects);
        Assert.Single(resume.Skills);
    }

    [Fact]
    public async Task Experience_timeline_includes_engagements_and_projects()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api, "Strivo AB");
        await CreateEngagementAsync(api, org.Id);
        await CreateProjectAsync(api, "Rebuild");

        var experience = await api.GetFromJsonAsync<List<ExperienceItemDto>>("/api/experience");
        Assert.Equal(2, experience!.Count);
        Assert.Contains(experience, x => x.OrganizationName == "Strivo AB");
        Assert.Contains(experience, x => x.Title == "Rebuild");
    }

    [Fact]
    public async Task Experience_reflects_a_shipped_projects_end_date_and_attached_skills()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var project = await CreateProjectAsync(api, "Rebuild");
        var skill = await RegisterSkillAsync(api);
        (await api.PutAsync($"/api/projects/{project.Id}/skills/{skill.Id}", null)).EnsureSuccessStatusCode();
        (await api.PostAsJsonAsync($"/api/projects/{project.Id}/ship", new ShipProjectRequest(new DateOnly(2021, 9, 1), "launched"))).EnsureSuccessStatusCode();

        var experience = await api.GetFromJsonAsync<List<ExperienceItemDto>>("/api/experience");
        var item = experience!.Single(x => x.Id == project.Id);
        Assert.Equal(new DateOnly(2021, 9, 1), item.EndDate);
        Assert.Contains(skill.Id, item.SkillIds);
    }
}
