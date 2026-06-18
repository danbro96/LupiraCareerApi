using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Mappers;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>The caller's media assets (blobs on the shared MinIO; <see cref="MediaAsset.BlobRef"/> is the object
/// key) and their links to projects/skills. Owner-scoped.</summary>
public sealed class MediaService(IDocumentSession session)
{
    public async Task<OpResult<List<MediaDto>>> ListAsync(Guid ownerId, CancellationToken ct = default)
    {
        var items = await session.Query<MediaAsset>().Where(m => m.OwnerPrincipalId == ownerId).ToListAsync(ct);
        return OpResult<List<MediaDto>>.Ok([.. items.Select(m => m.ToDto())]);
    }

    public async Task<OpResult<List<MediaDto>>> ForProjectAsync(Guid ownerId, Guid projectId, CancellationToken ct = default)
    {
        var items = await session.Query<MediaAsset>().Where(m => m.OwnerPrincipalId == ownerId && m.LinkedProjects.Any(p => p.ProjectId == projectId)).ToListAsync(ct);
        return OpResult<List<MediaDto>>.Ok([.. items.Select(m => m.ToDto())]);
    }

    public async Task<OpResult<MediaDto>> GetAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var m = await LoadOwnedAsync(ownerId, id, ct);
        return m is null ? OpResult<MediaDto>.NotFound() : OpResult<MediaDto>.Ok(m.ToDto());
    }

    public async Task<OpResult<MediaDto>> RegisterAsync(Guid ownerId, RegisterMediaRequest r, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(r.BlobRef)) return OpResult<MediaDto>.Invalid("BlobRef is required.");
        if (string.IsNullOrWhiteSpace(r.MimeType)) return OpResult<MediaDto>.Invalid("MimeType is required.");
        var id = Guid.NewGuid();
        session.Events.StartStream<MediaAsset>(id, new MediaRegistered(id, ownerId, r.BlobRef.Trim(), r.MimeType.Trim(), r.Width, r.Height, r.AltText ?? "", r.Caption, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    public Task<OpResult<MediaDto>> LinkToProjectAsync(Guid ownerId, Guid id, Guid projectId, MediaRole role, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new MediaLinkedToProject(id, projectId, role, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<MediaDto>> LinkToSkillAsync(Guid ownerId, Guid id, Guid skillId, string? note, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new MediaLinkedToSkill(id, skillId, note, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<MediaDto>> UnlinkAsync(Guid ownerId, Guid id, MediaTargetKind targetKind, Guid targetId, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new MediaUnlinked(id, targetKind, targetId, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<MediaDto>> ArchiveAsync(Guid ownerId, Guid id, string? reason, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new MediaArchived(id, reason, DateTimeOffset.UtcNow), ct);

    private async Task<OpResult<MediaDto>> AppendAsync(Guid ownerId, Guid id, object @event, CancellationToken ct)
    {
        var m = await LoadOwnedAsync(ownerId, id, ct);
        if (m is null) return OpResult<MediaDto>.NotFound();
        session.Events.Append(id, @event);
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    private async Task<MediaAsset?> LoadOwnedAsync(Guid ownerId, Guid id, CancellationToken ct)
    {
        var m = await session.LoadAsync<MediaAsset>(id, ct);
        return m is null || m.OwnerPrincipalId != ownerId ? null : m;
    }
}
