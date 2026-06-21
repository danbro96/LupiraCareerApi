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
        var resp = await api.PostAsJsonAsync("/api/skills", new RegisterSkillRequest { Name = "  ", Category = SkillCategory.Language, Aliases = null, ParentSkillId = null }, TestJson.Options);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Patch_renames_recategorizes_and_reparents()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var parent = await RegisterSkillAsync(api, ".NET");
        var skill = await RegisterSkillAsync(api, "C#");

        var resp = await api.PatchAsJsonAsync($"/api/skills/{skill.Id}", new UpdateSkillRequest { Name = "C Sharp", Category = SkillCategory.Framework, ParentSkillId = parent.Id }, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        var updated = await resp.Content.ReadFromJsonAsync<SkillDto>(TestJson.Options);
        Assert.Equal("C Sharp", updated!.Name);
        Assert.Equal(SkillCategory.Framework, updated.Category);
        Assert.Equal(parent.Id, updated.ParentSkillId);
    }

    [Fact]
    public async Task Aliases_are_deduplicated()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var skill = await RegisterSkillAsync(api);
        await api.PostAsJsonAsync($"/api/skills/{skill.Id}/aliases", new AddAliasRequest { Alias = "csharp" }, TestJson.Options);
        await api.PostAsJsonAsync($"/api/skills/{skill.Id}/aliases", new AddAliasRequest { Alias = "csharp" }, TestJson.Options);
        var resp = await api.PostAsJsonAsync($"/api/skills/{skill.Id}/aliases", new AddAliasRequest { Alias = "c-sharp" }, TestJson.Options);
        var dto = await resp.Content.ReadFromJsonAsync<SkillDto>(TestJson.Options);
        Assert.Equal(2, dto!.Aliases.Count);
    }

    [Fact]
    public async Task Retire_flags_the_skill()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var skill = await RegisterSkillAsync(api);
        var dto = await (await api.PostAsync($"/api/skills/{skill.Id}/retire", null)).Content.ReadFromJsonAsync<SkillDto>(TestJson.Options);
        Assert.True(dto!.Retired);
    }

    [Fact]
    public async Task Edge_events_drive_the_timeline_and_maturity()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var skill = await RegisterSkillAsync(api, "Kafka");
        var ctx = SkillEdgeContext.External("self-study");

        await api.PostAsJsonAsync($"/api/skills/{skill.Id}/learnings", new LearnSkillRequest { OccurredOn = new DateOnly(2019, 1, 1), InitialMaturity = Maturity.Working, Context = ctx, Evidence = null, Location = null }, TestJson.Options);
        await api.PostAsJsonAsync($"/api/skills/{skill.Id}/applications", new ApplySkillRequest { OccurredOn = new DateOnly(2020, 1, 1), Intensity = Intensity.Core, Context = ctx, Evidence = null, Location = null }, TestJson.Options);
        await api.PostAsJsonAsync($"/api/skills/{skill.Id}/deepenings", new DeepenSkillRequest { OccurredOn = new DateOnly(2021, 1, 1), FromMaturity = Maturity.Working, ToMaturity = Maturity.Expert, Note = "led a migration", Context = ctx, Evidence = null, Location = null }, TestJson.Options);

        var timeline = await api.GetFromJsonAsync<SkillTimeline>($"/api/skills/{skill.Id}/timeline", TestJson.Options);
        Assert.Equal(3, timeline!.Entries.Count);
        Assert.Contains(timeline.Entries, e => e.Kind == "Learned");
        Assert.Contains(timeline.Entries, e => e.Kind == "Applied");
        Assert.Contains(timeline.Entries, e => e.Kind == "Deepened");

        // Maturity tracks Learned + Deepened only — an application does not move it.
        var maturity = await api.GetFromJsonAsync<SkillMaturity>($"/api/skills/{skill.Id}/maturity", TestJson.Options);
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
