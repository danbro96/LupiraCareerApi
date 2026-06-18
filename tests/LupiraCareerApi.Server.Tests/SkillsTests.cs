using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class SkillsTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Register_with_blank_name_is_rejected()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var resp = await api.PostAsJsonAsync("/api/skills", new RegisterSkillRequest("  ", SkillCategory.Language, null, null));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Patch_renames_recategorizes_and_reparents()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var parent = await RegisterSkillAsync(api, ".NET");
        var skill = await RegisterSkillAsync(api, "C#");

        var resp = await api.PatchAsJsonAsync($"/api/skills/{skill.Id}", new UpdateSkillRequest("C Sharp", SkillCategory.Framework, parent.Id));
        resp.EnsureSuccessStatusCode();
        var updated = await resp.Content.ReadFromJsonAsync<SkillDto>();
        Assert.Equal("C Sharp", updated!.Name);
        Assert.Equal(SkillCategory.Framework, updated.Category);
        Assert.Equal(parent.Id, updated.ParentSkillId);
    }

    [Fact]
    public async Task Aliases_are_deduplicated()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var skill = await RegisterSkillAsync(api);
        await api.PostAsJsonAsync($"/api/skills/{skill.Id}/aliases", new AddAliasRequest("csharp"));
        await api.PostAsJsonAsync($"/api/skills/{skill.Id}/aliases", new AddAliasRequest("csharp"));
        var resp = await api.PostAsJsonAsync($"/api/skills/{skill.Id}/aliases", new AddAliasRequest("c-sharp"));
        var dto = await resp.Content.ReadFromJsonAsync<SkillDto>();
        Assert.Equal(2, dto!.Aliases.Count);
    }

    [Fact]
    public async Task Retire_flags_the_skill()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var skill = await RegisterSkillAsync(api);
        var dto = await (await api.PostAsync($"/api/skills/{skill.Id}/retire", null)).Content.ReadFromJsonAsync<SkillDto>();
        Assert.True(dto!.Retired);
    }

    [Fact]
    public async Task Edge_events_drive_the_timeline_and_maturity()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var skill = await RegisterSkillAsync(api, "Kafka");
        var ctx = SkillEdgeContext.External("self-study");

        await api.PostAsJsonAsync($"/api/skills/{skill.Id}/learnings", new LearnSkillRequest(new DateOnly(2019, 1, 1), Maturity.Working, ctx, null, null));
        await api.PostAsJsonAsync($"/api/skills/{skill.Id}/applications", new ApplySkillRequest(new DateOnly(2020, 1, 1), Intensity.Core, ctx, null, null));
        await api.PostAsJsonAsync($"/api/skills/{skill.Id}/deepenings", new DeepenSkillRequest(new DateOnly(2021, 1, 1), Maturity.Working, Maturity.Expert, "led a migration", ctx, null, null));

        var timeline = await api.GetFromJsonAsync<SkillTimeline>($"/api/skills/{skill.Id}/timeline");
        Assert.Equal(3, timeline!.Entries.Count);
        Assert.Contains(timeline.Entries, e => e.Kind == "Learned");
        Assert.Contains(timeline.Entries, e => e.Kind == "Applied");
        Assert.Contains(timeline.Entries, e => e.Kind == "Deepened");

        // Maturity tracks Learned + Deepened only — an application does not move it.
        var maturity = await api.GetFromJsonAsync<SkillMaturity>($"/api/skills/{skill.Id}/maturity");
        Assert.Equal(Maturity.Expert, maturity!.Current);
        Assert.Equal(2, maturity.Trajectory.Count);
    }

    [Fact]
    public async Task Timeline_and_maturity_for_a_missing_skill_are_404()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var missing = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.NotFound, (await api.GetAsync($"/api/skills/{missing}/timeline")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.GetAsync($"/api/skills/{missing}/maturity")).StatusCode);
    }
}
