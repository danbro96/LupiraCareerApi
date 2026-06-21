using LupiraCareerApi.Dtos;
using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class ArtifactsEndpoints
{
    public static IEndpointRouteBuilder MapArtifacts(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/artifacts").RequireAuthorization("ApiPolicy").WithTags("Artifacts");

        g.MapGet("", (ArtifactsHandler h, CancellationToken ct) => h.ListAsync(ct));
        g.MapPost("", (ArtifactsHandler h, RegisterArtifactRequest body, CancellationToken ct) => h.RegisterAsync(body, ct));
        g.MapGet("{id:guid}", (ArtifactsHandler h, Guid id, CancellationToken ct) => h.GetAsync(id, ct));
        g.MapPatch("{id:guid}", (ArtifactsHandler h, Guid id, UpdateArtifactRequest body, CancellationToken ct) => h.UpdateAsync(id, body, ct));
        g.MapDelete("{id:guid}", (ArtifactsHandler h, Guid id, CancellationToken ct) => h.ArchiveAsync(id, ct));

        g.MapPut("{id:guid}/projects/{projectId:guid}", (ArtifactsHandler h, Guid id, Guid projectId, CancellationToken ct) => h.LinkProjectAsync(id, projectId, ct));
        g.MapDelete("{id:guid}/projects/{projectId:guid}", (ArtifactsHandler h, Guid id, Guid projectId, CancellationToken ct) => h.UnlinkProjectAsync(id, projectId, ct));

        g.MapPut("{id:guid}/skills/{skillId:guid}", (ArtifactsHandler h, Guid id, Guid skillId, ArtifactSkillRoleRequest body, CancellationToken ct) => h.LinkSkillAsync(id, skillId, body, ct));
        g.MapDelete("{id:guid}/skills/{skillId:guid}", (ArtifactsHandler h, Guid id, Guid skillId, CancellationToken ct) => h.UnlinkSkillAsync(id, skillId, ct));

        g.MapPut("{id:guid}/engagements/{engagementId:guid}", (ArtifactsHandler h, Guid id, Guid engagementId, CancellationToken ct) => h.LinkEngagementAsync(id, engagementId, ct));
        g.MapDelete("{id:guid}/engagements/{engagementId:guid}", (ArtifactsHandler h, Guid id, Guid engagementId, CancellationToken ct) => h.UnlinkEngagementAsync(id, engagementId, ct));

        return app;
    }
}
