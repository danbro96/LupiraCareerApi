using LupiraCareerApi.Dtos;
using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class MediaEndpoints
{
    public static IEndpointRouteBuilder MapMedia(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/media").RequireAuthorization("ApiPolicy").WithTags("Media");

        g.MapGet("", (MediaHandler h, CancellationToken ct) => h.ListAsync(ct));
        g.MapPost("", (MediaHandler h, RegisterMediaRequest body, CancellationToken ct) => h.RegisterAsync(body, ct));
        g.MapGet("{id:guid}", (MediaHandler h, Guid id, CancellationToken ct) => h.GetAsync(id, ct));
        g.MapDelete("{id:guid}", (MediaHandler h, Guid id, CancellationToken ct) => h.ArchiveAsync(id, ct));

        g.MapPut("{id:guid}/projects/{projectId:guid}", (MediaHandler h, Guid id, Guid projectId, MediaProjectRoleRequest body, CancellationToken ct) => h.LinkProjectAsync(id, projectId, body, ct));
        g.MapDelete("{id:guid}/projects/{projectId:guid}", (MediaHandler h, Guid id, Guid projectId, CancellationToken ct) => h.UnlinkProjectAsync(id, projectId, ct));

        g.MapPut("{id:guid}/skills/{skillId:guid}", (MediaHandler h, Guid id, Guid skillId, string? note, CancellationToken ct) => h.LinkSkillAsync(id, skillId, note, ct));
        g.MapDelete("{id:guid}/skills/{skillId:guid}", (MediaHandler h, Guid id, Guid skillId, CancellationToken ct) => h.UnlinkSkillAsync(id, skillId, ct));

        return app;
    }
}
