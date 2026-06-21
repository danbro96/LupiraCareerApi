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
        var me = await api.GetFromJsonAsync<MeDto>("/api/me", TestJson.Options);
        return me!.Id;
    }

    protected static async Task<OrganizationDto> CreateOrganizationAsync(HttpClient api, string name = "Strivo")
    {
        var resp = await api.PostAsJsonAsync("/api/organizations", new CreateOrganizationRequest { Name = name, Kind = OrganizationKind.Company, Url = null, CalContactGroupRef = null }, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<OrganizationDto>(TestJson.Options))!;
    }

    protected static async Task<EngagementDto> CreateEngagementAsync(HttpClient api, Guid orgId, DateOnly? start = null)
    {
        var req = new CreateEngagementRequest { Kind = EngagementKind.Employment, OrganizationId = orgId, Start = start ?? new DateOnly(2020, 1, 1), Location = null, Summary = "Backend" };
        var resp = await api.PostAsJsonAsync("/api/engagements", req, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<EngagementDto>(TestJson.Options))!;
    }

    protected static async Task<ProjectDto> CreateProjectAsync(HttpClient api, string title = "Rebuild", Guid? engagementId = null)
    {
        var req = new CreateProjectRequest { Kind = ProjectKind.Professional, Title = title, Description = "desc", EngagementId = engagementId, Url = null, Start = new DateOnly(2021, 1, 1) };
        var resp = await api.PostAsJsonAsync("/api/projects", req, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<ProjectDto>(TestJson.Options))!;
    }

    protected static async Task<SkillDto> RegisterSkillAsync(HttpClient api, string name = "C#")
    {
        var req = new RegisterSkillRequest { Name = name, Category = SkillCategory.Language, Aliases = null, ParentSkillId = null };
        var resp = await api.PostAsJsonAsync("/api/skills", req, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<SkillDto>(TestJson.Options))!;
    }

    protected static async Task<GoalDto> CreateGoalAsync(HttpClient api, Guid? skillId = null, string motivation = "Get fluent")
    {
        var req = new SetGoalRequest { SkillId = skillId, TargetMaturity = Maturity.Fluent, Deadline = new DateOnly(2026, 12, 31), Motivation = motivation };
        var resp = await api.PostAsJsonAsync("/api/goals", req, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<GoalDto>(TestJson.Options))!;
    }

    protected static async Task<ArtifactDto> RegisterArtifactAsync(HttpClient api, string url = "https://github.com/x/y", string title = "Repo", ArtifactKind kind = ArtifactKind.Repo)
    {
        var req = new RegisterArtifactRequest { Kind = kind, Url = url, Title = title, Description = null, ProducedOn = null };
        var resp = await api.PostAsJsonAsync("/api/artifacts", req, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<ArtifactDto>(TestJson.Options))!;
    }

    protected static async Task<MediaDto> RegisterMediaAsync(HttpClient api, string blobRef = "career/hero.png", string mimeType = "image/png")
    {
        var req = new RegisterMediaRequest { BlobRef = blobRef, MimeType = mimeType, Width = 1200, Height = 630, AltText = "Hero", Caption = null };
        var resp = await api.PostAsJsonAsync("/api/media", req, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<MediaDto>(TestJson.Options))!;
    }
}
