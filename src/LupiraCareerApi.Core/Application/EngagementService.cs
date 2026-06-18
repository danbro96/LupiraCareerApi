using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Mappers;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>Creates and curates the caller's engagements (employment/study/…). Every stream is owned by one
/// principal; reads and writes are scoped to the owner, and a non-owned id is reported as not found. A partial
/// update fans out into the individual domain events (summary revised, relocated, reclassified, ended).</summary>
public sealed class EngagementService(IDocumentSession session)
{
    public async Task<OpResult<List<EngagementDto>>> ListAsync(Guid ownerId, CancellationToken ct = default)
    {
        var items = await session.Query<Engagement>().Where(e => e.OwnerPrincipalId == ownerId).ToListAsync(ct);
        var names = await OrgNamesAsync(items.Select(e => e.OrganizationId), ct);
        return OpResult<List<EngagementDto>>.Ok(
            [.. items.OrderByDescending(e => e.Start).Select(e => e.ToDto(names.GetValueOrDefault(e.OrganizationId)))]);
    }

    public async Task<OpResult<EngagementDto>> GetAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var e = await LoadOwnedAsync(ownerId, id, ct);
        if (e is null) return OpResult<EngagementDto>.NotFound();
        var org = await session.LoadAsync<Organization>(e.OrganizationId, ct);
        return OpResult<EngagementDto>.Ok(e.ToDto(org?.Name));
    }

    public async Task<OpResult<EngagementDto>> CreateAsync(Guid ownerId, CreateEngagementRequest r, CancellationToken ct = default)
    {
        var org = await session.LoadAsync<Organization>(r.OrganizationId, ct);
        if (org is null || org.OwnerPrincipalId != ownerId)
            return OpResult<EngagementDto>.Invalid("OrganizationId must reference one of your organizations.");

        var id = Guid.NewGuid();
        session.Events.StartStream<Engagement>(id, new EngagementStarted(id, ownerId, r.Kind, r.OrganizationId, r.Start, r.Location, r.Summary));
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    /// <summary>PATCH: applies only the provided fields, each as its own event.</summary>
    public async Task<OpResult<EngagementDto>> UpdateAsync(Guid ownerId, Guid id, UpdateEngagementRequest r, CancellationToken ct = default)
    {
        var e = await LoadOwnedAsync(ownerId, id, ct);
        if (e is null) return OpResult<EngagementDto>.NotFound();
        if (r.Summary is not null) session.Events.Append(id, new EngagementSummaryRevised(id, r.Summary));
        if (r.Kind is EngagementKind kind) session.Events.Append(id, new EngagementKindReclassified(id, kind));
        if (r.Location is Location loc) session.Events.Append(id, new EngagementRelocated(id, loc));
        if (r.End is DateOnly end) session.Events.Append(id, new EngagementEnded(id, end, r.EndReason));
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    public Task<OpResult<EngagementDto>> AssumeTitleAsync(Guid ownerId, Guid id, AssumeTitleRequest r, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new TitleAssumed(id, Guid.NewGuid(), r.Text, r.EffectiveFrom), ct);

    public async Task<OpResult<EngagementDto>> UpdateTitleAsync(Guid ownerId, Guid id, Guid titleId, UpdateTitleRequest r, CancellationToken ct = default)
    {
        var e = await LoadOwnedAsync(ownerId, id, ct);
        if (e is null) return OpResult<EngagementDto>.NotFound();
        if (e.Titles.All(t => t.TitleId != titleId)) return OpResult<EngagementDto>.NotFound();
        if (r.Text is not null) session.Events.Append(id, new TitleRevised(id, titleId, r.Text));
        if (r.RetiredOn is DateOnly to) session.Events.Append(id, new TitleRetired(id, titleId, to));
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    public Task<OpResult<EngagementDto>> AttachSkillAsync(Guid ownerId, Guid id, Guid skillId, DateOnly? on, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new EngagementSkillAttached(id, skillId, on), ct);

    public Task<OpResult<EngagementDto>> DetachSkillAsync(Guid ownerId, Guid id, Guid skillId, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new EngagementSkillDetached(id, skillId), ct);

    private async Task<OpResult<EngagementDto>> AppendAsync(Guid ownerId, Guid id, object @event, CancellationToken ct)
    {
        var e = await LoadOwnedAsync(ownerId, id, ct);
        if (e is null) return OpResult<EngagementDto>.NotFound();
        session.Events.Append(id, @event);
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    private async Task<Engagement?> LoadOwnedAsync(Guid ownerId, Guid id, CancellationToken ct)
    {
        var e = await session.LoadAsync<Engagement>(id, ct);
        return e is null || e.OwnerPrincipalId != ownerId ? null : e;
    }

    private async Task<Dictionary<Guid, string>> OrgNamesAsync(IEnumerable<Guid> orgIds, CancellationToken ct)
    {
        var ids = orgIds.Distinct().ToList();
        if (ids.Count == 0) return [];
        var orgs = await session.Query<Organization>().Where(o => ids.Contains(o.Id)).ToListAsync(ct);
        return orgs.ToDictionary(o => o.Id, o => o.Name);
    }
}
