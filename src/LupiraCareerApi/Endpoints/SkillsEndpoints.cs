using LupiraCareerApi.Core.Dtos;
using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class SkillsEndpoints
{
    public static IEndpointRouteBuilder MapSkills(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/skills").RequireAuthorization("ApiPolicy").WithTags("Skills");

        g.MapGet(string.Empty, (SkillsHandler h, CancellationToken ct) => h.ListAsync(ct))
            .WithName("ListSkills");
        g.MapPost(string.Empty, (SkillsHandler h, RegisterSkillRequest body, CancellationToken ct) => h.RegisterAsync(body, ct))
            .WithName("RegisterSkill");
        g.MapGet("{id:guid}", (SkillsHandler h, Guid id, CancellationToken ct) => h.GetAsync(id, ct))
            .WithName("GetSkill");
        g.MapPatch("{id:guid}", (SkillsHandler h, Guid id, UpdateSkillRequest body, CancellationToken ct) => h.UpdateAsync(id, body, ct))
            .WithName("UpdateSkill");

        g.MapPost("{id:guid}/aliases", (SkillsHandler h, Guid id, AddAliasRequest body, CancellationToken ct) => h.AddAliasAsync(id, body, ct))
            .WithName("AddSkillAlias");
        g.MapPost("{id:guid}/retire", (SkillsHandler h, Guid id, CancellationToken ct) => h.RetireAsync(id, ct))
            .WithName("RetireSkill");

        // Edge observations — append-only sub-collections of the skill's timeline.
        g.MapPost("{id:guid}/learnings", (SkillsHandler h, Guid id, LearnSkillRequest body, CancellationToken ct) => h.LearnAsync(id, body, ct))
            .WithName("LearnSkill");
        g.MapPost("{id:guid}/applications", (SkillsHandler h, Guid id, ApplySkillRequest body, CancellationToken ct) => h.ApplyAsync(id, body, ct))
            .WithName("ApplySkill");
        g.MapPost("{id:guid}/deepenings", (SkillsHandler h, Guid id, DeepenSkillRequest body, CancellationToken ct) => h.DeepenAsync(id, body, ct))
            .WithName("DeepenSkill");

        g.MapGet("{id:guid}/timeline", (SkillsHandler h, Guid id, CancellationToken ct) => h.TimelineAsync(id, ct))
            .WithName("GetSkillTimeline");
        g.MapGet("{id:guid}/maturity", (SkillsHandler h, Guid id, CancellationToken ct) => h.MaturityAsync(id, ct))
            .WithName("GetSkillMaturity");

        return app;
    }
}
