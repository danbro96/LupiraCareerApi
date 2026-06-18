using LupiraCareerApi.Dtos;
using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class ProjectsEndpoints
{
    public static IEndpointRouteBuilder MapProjects(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/projects").RequireAuthorization("ApiPolicy").WithTags("Projects");

        g.MapGet("", (ProjectsHandler h, Guid? engagementId, CancellationToken ct) => h.ListAsync(engagementId, ct));
        g.MapPost("", (ProjectsHandler h, CreateProjectRequest body, CancellationToken ct) => h.CreateAsync(body, ct));
        g.MapGet("{id:guid}", (ProjectsHandler h, Guid id, CancellationToken ct) => h.GetAsync(id, ct));
        g.MapPatch("{id:guid}", (ProjectsHandler h, Guid id, UpdateProjectRequest body, CancellationToken ct) => h.UpdateAsync(id, body, ct));

        g.MapPost("{id:guid}/ship", (ProjectsHandler h, Guid id, ShipProjectRequest body, CancellationToken ct) => h.ShipAsync(id, body, ct));
        g.MapPost("{id:guid}/shelve", (ProjectsHandler h, Guid id, string? reason, CancellationToken ct) => h.ShelveAsync(id, reason, ct));
        g.MapPost("{id:guid}/archive", (ProjectsHandler h, Guid id, CancellationToken ct) => h.ArchiveAsync(id, ct));

        g.MapPut("{id:guid}/engagement", (ProjectsHandler h, Guid id, AttachEngagementRequest body, CancellationToken ct) => h.AttachEngagementAsync(id, body, ct));
        g.MapDelete("{id:guid}/engagement", (ProjectsHandler h, Guid id, CancellationToken ct) => h.DetachEngagementAsync(id, ct));

        g.MapPut("{id:guid}/skills/{skillId:guid}", (ProjectsHandler h, Guid id, Guid skillId, DateOnly? on, CancellationToken ct) => h.AttachSkillAsync(id, skillId, on, ct));
        g.MapDelete("{id:guid}/skills/{skillId:guid}", (ProjectsHandler h, Guid id, Guid skillId, CancellationToken ct) => h.DetachSkillAsync(id, skillId, ct));

        g.MapGet("{id:guid}/artifacts", (ProjectsHandler h, Guid id, CancellationToken ct) => h.ArtifactsAsync(id, ct));
        g.MapGet("{id:guid}/media", (ProjectsHandler h, Guid id, CancellationToken ct) => h.MediaAsync(id, ct));

        return app;
    }
}
