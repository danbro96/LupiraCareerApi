using LupiraCareerApi.Core.Dtos;
using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class GoalsEndpoints
{
    public static IEndpointRouteBuilder MapGoals(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/goals").RequireAuthorization("ApiPolicy").WithTags("Goals");

        g.MapGet(string.Empty, (GoalsHandler h, CancellationToken ct) => h.ListAsync(ct))
            .WithName("ListGoals");
        g.MapPost(string.Empty, (GoalsHandler h, SetGoalRequest body, CancellationToken ct) => h.CreateAsync(body, ct))
            .WithName("CreateGoal");
        g.MapGet("{id:guid}", (GoalsHandler h, Guid id, CancellationToken ct) => h.GetAsync(id, ct))
            .WithName("GetGoal");
        g.MapPatch("{id:guid}", (GoalsHandler h, Guid id, RescopeGoalRequest body, CancellationToken ct) => h.RescopeAsync(id, body, ct))
            .WithName("RescopeGoal");

        g.MapPost("{id:guid}/progress", (GoalsHandler h, Guid id, RecordProgressRequest body, CancellationToken ct) => h.RecordProgressAsync(id, body, ct))
            .WithName("RecordGoalProgress");
        g.MapPost("{id:guid}/achieve", (GoalsHandler h, Guid id, AchieveGoalRequest body, CancellationToken ct) => h.AchieveAsync(id, body, ct))
            .WithName("AchieveGoal");
        g.MapPost("{id:guid}/abandon", (GoalsHandler h, Guid id, AbandonGoalRequest body, CancellationToken ct) => h.AbandonAsync(id, body, ct))
            .WithName("AbandonGoal");

        return app;
    }
}
