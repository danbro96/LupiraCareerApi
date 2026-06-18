using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Mappers;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>Creates and curates the caller's projects, optionally filed under an engagement. Owner-scoped.</summary>
public sealed class ProjectService(IDocumentSession session)
{
    public async Task<OpResult<List<ProjectDto>>> ListAsync(Guid ownerId, Guid? engagementId = null, CancellationToken ct = default)
    {
        var q = session.Query<Project>().Where(p => p.OwnerPrincipalId == ownerId);
        if (engagementId is Guid eid) q = q.Where(p => p.EngagementId == eid);
        var items = await q.ToListAsync(ct);
        return OpResult<List<ProjectDto>>.Ok([.. items.OrderByDescending(p => p.Start ?? DateOnly.MinValue).Select(p => p.ToDto())]);
    }

    public async Task<OpResult<ProjectDto>> GetAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var p = await LoadOwnedAsync(ownerId, id, ct);
        return p is null ? OpResult<ProjectDto>.NotFound() : OpResult<ProjectDto>.Ok(p.ToDto());
    }

    public async Task<OpResult<ProjectDto>> CreateAsync(Guid ownerId, CreateProjectRequest r, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(r.Title)) return OpResult<ProjectDto>.Invalid("Title is required.");
        if (r.EngagementId is Guid eid)
        {
            var eng = await session.LoadAsync<Engagement>(eid, ct);
            if (eng is null || eng.OwnerPrincipalId != ownerId)
                return OpResult<ProjectDto>.Invalid("EngagementId must reference one of your engagements.");
        }

        var id = Guid.NewGuid();
        session.Events.StartStream<Project>(id, new ProjectStarted(id, ownerId, r.Kind, r.Title.Trim(), r.Description, r.EngagementId, r.Url, r.Start));
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    public async Task<OpResult<ProjectDto>> UpdateAsync(Guid ownerId, Guid id, UpdateProjectRequest r, CancellationToken ct = default)
    {
        var p = await LoadOwnedAsync(ownerId, id, ct);
        if (p is null) return OpResult<ProjectDto>.NotFound();
        if (r.Title is not null) session.Events.Append(id, new ProjectRenamed(id, r.Title.Trim()));
        if (r.Description is not null) session.Events.Append(id, new ProjectDescribed(id, r.Description));
        if (r.Url is not null) session.Events.Append(id, new ProjectUrlSet(id, r.Url));
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    public Task<OpResult<ProjectDto>> ShipAsync(Guid ownerId, Guid id, ShipProjectRequest r, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ProjectShipped(id, r.ShippedOn, r.Outcome), ct);

    public Task<OpResult<ProjectDto>> ShelveAsync(Guid ownerId, Guid id, string? reason, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ProjectShelved(id, reason), ct);

    public Task<OpResult<ProjectDto>> ArchiveAsync(Guid ownerId, Guid id, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ProjectArchived(id), ct);

    public Task<OpResult<ProjectDto>> AttachSkillAsync(Guid ownerId, Guid id, Guid skillId, DateOnly? on, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ProjectSkillAttached(id, skillId, on), ct);

    public Task<OpResult<ProjectDto>> DetachSkillAsync(Guid ownerId, Guid id, Guid skillId, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ProjectSkillDetached(id, skillId), ct);

    public async Task<OpResult<ProjectDto>> AttachToEngagementAsync(Guid ownerId, Guid id, Guid engagementId, CancellationToken ct = default)
    {
        var eng = await session.LoadAsync<Engagement>(engagementId, ct);
        if (eng is null || eng.OwnerPrincipalId != ownerId)
            return OpResult<ProjectDto>.Invalid("EngagementId must reference one of your engagements.");
        return await AppendAsync(ownerId, id, new ProjectAttachedToEngagement(id, engagementId), ct);
    }

    public Task<OpResult<ProjectDto>> DetachFromEngagementAsync(Guid ownerId, Guid id, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new ProjectDetachedFromEngagement(id), ct);

    private async Task<OpResult<ProjectDto>> AppendAsync(Guid ownerId, Guid id, object @event, CancellationToken ct)
    {
        var p = await LoadOwnedAsync(ownerId, id, ct);
        if (p is null) return OpResult<ProjectDto>.NotFound();
        session.Events.Append(id, @event);
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    private async Task<Project?> LoadOwnedAsync(Guid ownerId, Guid id, CancellationToken ct)
    {
        var p = await session.LoadAsync<Project>(id, ct);
        return p is null || p.OwnerPrincipalId != ownerId ? null : p;
    }
}
