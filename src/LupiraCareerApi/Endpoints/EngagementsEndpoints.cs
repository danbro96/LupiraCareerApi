using LupiraCareerApi.Dtos;
using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class EngagementsEndpoints
{
    public static IEndpointRouteBuilder MapEngagements(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/engagements").RequireAuthorization("ApiPolicy").WithTags("Engagements");

        g.MapGet("", (EngagementsHandler h, CancellationToken ct) => h.ListAsync(ct));
        g.MapPost("", (EngagementsHandler h, CreateEngagementRequest body, CancellationToken ct) => h.CreateAsync(body, ct));
        g.MapGet("{id:guid}", (EngagementsHandler h, Guid id, CancellationToken ct) => h.GetAsync(id, ct));
        g.MapPatch("{id:guid}", (EngagementsHandler h, Guid id, UpdateEngagementRequest body, CancellationToken ct) => h.UpdateAsync(id, body, ct));

        g.MapPost("{id:guid}/titles", (EngagementsHandler h, Guid id, AssumeTitleRequest body, CancellationToken ct) => h.AssumeTitleAsync(id, body, ct));
        g.MapPatch("{id:guid}/titles/{titleId:guid}", (EngagementsHandler h, Guid id, Guid titleId, UpdateTitleRequest body, CancellationToken ct) => h.UpdateTitleAsync(id, titleId, body, ct));

        g.MapPut("{id:guid}/skills/{skillId:guid}", (EngagementsHandler h, Guid id, Guid skillId, DateOnly? on, CancellationToken ct) => h.AttachSkillAsync(id, skillId, on, ct));
        g.MapDelete("{id:guid}/skills/{skillId:guid}", (EngagementsHandler h, Guid id, Guid skillId, CancellationToken ct) => h.DetachSkillAsync(id, skillId, ct));

        return app;
    }
}
