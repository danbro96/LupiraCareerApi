using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

/// <summary>Base for integration tests: shares the container fixture, resets Marten data before each test, and
/// provides REST fixture helpers. Lives in the "integration" collection so tests run serially against the shared DB.</summary>
[Collection("integration")]
public abstract class IntegrationTest(CareerApiTestFactory factory) : IAsyncLifetime
{
    protected readonly CareerApiTestFactory Factory = factory;

    public async Task InitializeAsync() => await Factory.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    protected static async Task<Guid> GetMyIdAsync(HttpClient api)
    {
        var me = await api.GetFromJsonAsync<MeDto>("/api/me");
        return me!.Id;
    }

    protected static async Task<OrganizationDto> CreateOrganizationAsync(HttpClient api, string name = "Strivo")
    {
        var resp = await api.PostAsJsonAsync("/api/organizations", new CreateOrganizationRequest(name, OrganizationKind.Company, null, null));
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<OrganizationDto>())!;
    }

    protected static async Task<EngagementDto> CreateEngagementAsync(HttpClient api, Guid orgId, DateOnly? start = null)
    {
        var req = new CreateEngagementRequest(EngagementKind.Employment, orgId, start ?? new DateOnly(2020, 1, 1), null, "Backend");
        var resp = await api.PostAsJsonAsync("/api/engagements", req);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<EngagementDto>())!;
    }

    protected static async Task<ProjectDto> CreateProjectAsync(HttpClient api, string title = "Rebuild", Guid? engagementId = null)
    {
        var req = new CreateProjectRequest(ProjectKind.Professional, title, "desc", engagementId, null, new DateOnly(2021, 1, 1));
        var resp = await api.PostAsJsonAsync("/api/projects", req);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<ProjectDto>())!;
    }

    protected static async Task<SkillDto> RegisterSkillAsync(HttpClient api, string name = "C#")
    {
        var req = new RegisterSkillRequest(name, SkillCategory.Language, null, null);
        var resp = await api.PostAsJsonAsync("/api/skills", req);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<SkillDto>())!;
    }

    protected static async Task<GoalDto> CreateGoalAsync(HttpClient api, Guid? skillId = null, string motivation = "Get fluent")
    {
        var req = new SetGoalRequest(skillId, Maturity.Fluent, new DateOnly(2026, 12, 31), motivation);
        var resp = await api.PostAsJsonAsync("/api/goals", req);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<GoalDto>())!;
    }

    protected static async Task<ArtifactDto> RegisterArtifactAsync(HttpClient api, string url = "https://github.com/x/y", string title = "Repo", ArtifactKind kind = ArtifactKind.Repo)
    {
        var req = new RegisterArtifactRequest(kind, url, title, null, null);
        var resp = await api.PostAsJsonAsync("/api/artifacts", req);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<ArtifactDto>())!;
    }

    protected static async Task<MediaDto> RegisterMediaAsync(HttpClient api, string blobRef = "career/hero.png", string mimeType = "image/png")
    {
        var req = new RegisterMediaRequest(blobRef, mimeType, 1200, 630, "Hero", null);
        var resp = await api.PostAsJsonAsync("/api/media", req);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<MediaDto>())!;
    }
}
