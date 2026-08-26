using LupiraCareerApi.Core.Dtos;
using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class MediaEndpoints
{
    public static IEndpointRouteBuilder MapMedia(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/media").RequireAuthorization("ApiPolicy").WithTags("Media");

        g.MapGet(string.Empty, (MediaHandler h, CancellationToken ct) => h.ListAsync(ct))
            .WithName("ListMedia");
        g.MapPost(string.Empty, (MediaHandler h, RegisterMediaRequest body, CancellationToken ct) => h.RegisterAsync(body, ct))
            .WithName("RegisterMedia");
        g.MapGet("{id:guid}", (MediaHandler h, Guid id, CancellationToken ct) => h.GetAsync(id, ct))
            .WithName("GetMedia");
        g.MapDelete("{id:guid}", (MediaHandler h, Guid id, CancellationToken ct) => h.ArchiveAsync(id, ct))
            .WithName("ArchiveMedia");

        g.MapPut("{id:guid}/projects/{projectId:guid}", (MediaHandler h, Guid id, Guid projectId, MediaProjectRoleRequest body, CancellationToken ct) => h.LinkProjectAsync(id, projectId, body, ct))
            .WithName("LinkMediaToProject");
        g.MapDelete("{id:guid}/projects/{projectId:guid}", (MediaHandler h, Guid id, Guid projectId, CancellationToken ct) => h.UnlinkProjectAsync(id, projectId, ct))
            .WithName("UnlinkMediaFromProject");

        g.MapPut("{id:guid}/skills/{skillId:guid}", (MediaHandler h, Guid id, Guid skillId, string? note, CancellationToken ct) => h.LinkSkillAsync(id, skillId, note, ct))
            .WithName("LinkMediaToSkill");
        g.MapDelete("{id:guid}/skills/{skillId:guid}", (MediaHandler h, Guid id, Guid skillId, CancellationToken ct) => h.UnlinkSkillAsync(id, skillId, ct))
            .WithName("UnlinkMediaFromSkill");

        return app;
    }
}
