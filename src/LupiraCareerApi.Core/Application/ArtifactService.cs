using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Mappers;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>The caller's artifacts (repos, talks, certifications, …) and their links to projects/skills/engagements.
/// Reverse-link views (artifacts for a project/skill/engagement) are served by query-time <c>Contains()</c>.
/// Owner-scoped.</summary>
public sealed class ArtifactService(IDocumentSession session)
{
    public async Task<OpResult<List<ArtifactDto>>> ListAsync(Guid ownerId, CancellationToken ct = default)
    {
        var items = await session.Query<Artifact>().Where(a => a.OwnerPrincipalId == ownerId).ToListAsync(ct);
        return OpResult<List<ArtifactDto>>.Ok([.. items.OrderByDescending(a => a.ProducedOn ?? DateOnly.MinValue).Select(a => a.ToDto())]);
    }

    public async Task<OpResult<List<ArtifactDto>>> ForProjectAsync(Guid ownerId, Guid projectId, CancellationToken ct = default)
    {
        var items = await session.Query<Artifact>().Where(a => a.OwnerPrincipalId == ownerId && a.LinkedProjectIds.Contains(projectId)).ToListAsync(ct);
        return OpResult<List<ArtifactDto>>.Ok([.. items.Select(a => a.ToDto())]);
    }

    public async Task<OpResult<List<ArtifactDto>>> ForEngagementAsync(Guid ownerId, Guid engagementId, CancellationToken ct = default)
    {
        var items = await session.Query<Artifact>().Where(a => a.OwnerPrincipalId == ownerId && a.LinkedEngagementIds.Contains(engagementId)).ToListAsync(ct);
        return OpResult<List<ArtifactDto>>.Ok([.. items.Select(a => a.ToDto())]);
    }

    public async Task<OpResult<ArtifactDto>> GetAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var a = await LoadOwnedAsync(ownerId, id, ct);
        return a is null ? OpResult<ArtifactDto>.NotFound() : OpResult<ArtifactDto>.Ok(a.ToDto());
    }

    public async Task<OpResult<ArtifactDto>> RegisterAsync(Guid ownerId, RegisterArtifactRequest r, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(r.Url)) return OpResult<ArtifactDto>.Invalid("Url is required.");
        if (string.IsNullOrWhiteSpace(r.Title)) return OpResult<ArtifactDto>.Invalid("Title is required.");
        var id = Guid.NewGuid();
        session.Events.StartStream<Artifact>(id, new ArtifactRegistered(id, ownerId, r.Kind, r.Url.Trim(), r.Title.Trim(), r.Description, r.ProducedOn, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    public Task<OpResult<ArtifactDto>> UpdateAsync(Guid ownerId, Guid id, string? url, string? title, string? description, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ArtifactUpdated(id, url, title, description, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<ArtifactDto>> LinkToProjectAsync(Guid ownerId, Guid id, Guid projectId, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ArtifactLinkedToProject(id, projectId, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<ArtifactDto>> LinkToSkillAsync(Guid ownerId, Guid id, Guid skillId, ArtifactRole role, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ArtifactLinkedToSkill(id, skillId, role, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<ArtifactDto>> LinkToEngagementAsync(Guid ownerId, Guid id, Guid engagementId, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ArtifactLinkedToEngagement(id, engagementId, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<ArtifactDto>> UnlinkAsync(Guid ownerId, Guid id, ArtifactTargetKind targetKind, Guid targetId, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ArtifactUnlinked(id, targetKind, targetId, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<ArtifactDto>> ArchiveAsync(Guid ownerId, Guid id, string? reason, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ArtifactArchived(id, reason, DateTimeOffset.UtcNow), ct);

    private async Task<OpResult<ArtifactDto>> AppendAsync(Guid ownerId, Guid id, object @event, CancellationToken ct)
    {
        var a = await LoadOwnedAsync(ownerId, id, ct);
        if (a is null) return OpResult<ArtifactDto>.NotFound();
        session.Events.Append(id, @event);
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    private async Task<Artifact?> LoadOwnedAsync(Guid ownerId, Guid id, CancellationToken ct)
    {
        var a = await session.LoadAsync<Artifact>(id, ct);
        return a is null || a.OwnerPrincipalId != ownerId ? null : a;
    }
}
