using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class GoalsTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Create_with_blank_motivation_is_rejected()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var resp = await api.PostAsJsonAsync("/api/goals", new SetGoalRequest { SkillId = null, TargetMaturity = Maturity.Fluent, Deadline = null, Motivation = "  " }, TestJson.Options);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Create_get_and_list()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var skill = await RegisterSkillAsync(api);
        var goal = await CreateGoalAsync(api, skill.Id, "Reach fluency");

        var got = await api.GetFromJsonAsync<GoalDto>($"/api/goals/{goal.Id}", TestJson.Options);
        Assert.Equal("Reach fluency", got!.Motivation);
        Assert.Equal(GoalStatus.Active, got.Status);

        var list = await api.GetFromJsonAsync<List<GoalDto>>("/api/goals", TestJson.Options);
        Assert.Contains(list!, g => g.Id == goal.Id);
    }

    [Fact]
    public async Task Patch_rescopes_target_and_deadline()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var goal = await CreateGoalAsync(api);
        var resp = await api.PatchAsJsonAsync($"/api/goals/{goal.Id}", new RescopeGoalRequest { TargetMaturity = Maturity.Expert, Deadline = new DateOnly(2027, 1, 1) }, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        var updated = await resp.Content.ReadFromJsonAsync<GoalDto>(TestJson.Options);
        Assert.Equal(Maturity.Expert, updated!.TargetMaturity);
        Assert.Equal(new DateOnly(2027, 1, 1), updated.Deadline);
    }

    [Fact]
    public async Task Progress_appends_an_entry()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var goal = await CreateGoalAsync(api);
        var resp = await api.PostAsJsonAsync($"/api/goals/{goal.Id}/progress", new RecordProgressRequest { Note = "Did a course", LinkedEventId = null }, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        var updated = await resp.Content.ReadFromJsonAsync<GoalDto>(TestJson.Options);
        Assert.Single(updated!.Progress);
        Assert.Equal("Did a course", updated.Progress[0].Note);
    }

    [Fact]
    public async Task Achieve_resolves_with_evidence()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var goal = await CreateGoalAsync(api);
        var artifact = await RegisterArtifactAsync(api);

        var resp = await api.PostAsJsonAsync($"/api/goals/{goal.Id}/achieve", new AchieveGoalRequest { AchievedOn = new DateOnly(2026, 6, 1), EvidenceArtifactId = artifact.Id }, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        var achieved = await resp.Content.ReadFromJsonAsync<GoalDto>(TestJson.Options);
        Assert.Equal(GoalStatus.Achieved, achieved!.Status);
        Assert.NotNull(achieved.ResolvedAt);
        Assert.Equal(artifact.Id, achieved.EvidenceArtifactId);
    }

    [Fact]
    public async Task Abandon_resolves_with_reason()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var goal = await CreateGoalAsync(api);
        var resp = await api.PostAsJsonAsync($"/api/goals/{goal.Id}/abandon", new AbandonGoalRequest { Reason = "priorities changed" }, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        var abandoned = await resp.Content.ReadFromJsonAsync<GoalDto>(TestJson.Options);
        Assert.Equal(GoalStatus.Abandoned, abandoned!.Status);
        Assert.Equal("priorities changed", abandoned.ResolutionReason);
    }

    [Fact]
    public async Task Operations_on_a_missing_goal_are_404()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var missing = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.NotFound, (await api.GetAsync($"/api/goals/{missing}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.PostAsJsonAsync($"/api/goals/{missing}/progress", new RecordProgressRequest { Note = "x", LinkedEventId = null }, TestJson.Options)).StatusCode);
    }
}
