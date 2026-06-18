using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Mappers;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>Registers the caller's skills and records the dated edge events (learned/applied/deepened/…) that drive
/// the maturity trajectory and timeline read models. Owner-scoped.</summary>
public sealed class SkillService(IDocumentSession session)
{
    public async Task<OpResult<List<SkillDto>>> ListAsync(Guid ownerId, CancellationToken ct = default)
    {
        var items = await session.Query<Skill>().Where(s => s.OwnerPrincipalId == ownerId).ToListAsync(ct);
        return OpResult<List<SkillDto>>.Ok([.. items.OrderBy(s => s.Name).Select(s => s.ToDto())]);
    }

    public async Task<OpResult<SkillDto>> GetAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var s = await LoadOwnedAsync(ownerId, id, ct);
        return s is null ? OpResult<SkillDto>.NotFound() : OpResult<SkillDto>.Ok(s.ToDto());
    }

    public async Task<OpResult<SkillDto>> RegisterAsync(Guid ownerId, RegisterSkillRequest r, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(r.Name)) return OpResult<SkillDto>.Invalid("Name is required.");
        var id = Guid.NewGuid();
        session.Events.StartStream<Skill>(id, new SkillRegistered(id, ownerId, r.Name.Trim(), r.Category, r.Aliases, r.ParentSkillId));
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    /// <summary>PATCH: rename and/or recategorize and/or reparent.</summary>
    public async Task<OpResult<SkillDto>> UpdateAsync(Guid ownerId, Guid id, UpdateSkillRequest r, CancellationToken ct = default)
    {
        var s = await LoadOwnedAsync(ownerId, id, ct);
        if (s is null) return OpResult<SkillDto>.NotFound();
        if (r.Name is not null) session.Events.Append(id, new SkillRenamed(id, r.Name.Trim()));
        if (r.Category is SkillCategory cat) session.Events.Append(id, new SkillCategoryChanged(id, cat));
        if (r.ParentSkillId is Guid parent) session.Events.Append(id, new SkillReparented(id, parent));
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    public Task<OpResult<SkillDto>> AddAliasAsync(Guid ownerId, Guid id, string alias, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new SkillAliasAdded(id, alias), ct);

    public Task<OpResult<SkillDto>> RetireAsync(Guid ownerId, Guid id, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new SkillRetired(id), ct);

    public Task<OpResult<SkillDto>> LearnAsync(Guid ownerId, Guid id, LearnSkillRequest r, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new SkillLearned(id, r.OccurredOn, r.InitialMaturity, r.Context, r.Evidence, r.Location), ct);

    public Task<OpResult<SkillDto>> ApplyAsync(Guid ownerId, Guid id, ApplySkillRequest r, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new SkillApplied(id, r.OccurredOn, r.Intensity, r.Context, r.Evidence, r.Location), ct);

    public Task<OpResult<SkillDto>> DeepenAsync(Guid ownerId, Guid id, DeepenSkillRequest r, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new SkillDeepened(id, r.OccurredOn, r.FromMaturity, r.ToMaturity, r.Note, r.Context, r.Evidence, r.Location), ct);

    public async Task<OpResult<SkillTimeline>> GetTimelineAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var t = await session.LoadAsync<SkillTimeline>(id, ct);
        return t is null || t.OwnerPrincipalId != ownerId ? OpResult<SkillTimeline>.NotFound() : OpResult<SkillTimeline>.Ok(t);
    }

    public async Task<OpResult<SkillMaturity>> GetMaturityAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var m = await session.LoadAsync<SkillMaturity>(id, ct);
        return m is null || m.OwnerPrincipalId != ownerId ? OpResult<SkillMaturity>.NotFound() : OpResult<SkillMaturity>.Ok(m);
    }

    private async Task<OpResult<SkillDto>> AppendAsync(Guid ownerId, Guid id, object @event, CancellationToken ct)
    {
        var s = await LoadOwnedAsync(ownerId, id, ct);
        if (s is null) return OpResult<SkillDto>.NotFound();
        session.Events.Append(id, @event);
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    private async Task<Skill?> LoadOwnedAsync(Guid ownerId, Guid id, CancellationToken ct)
    {
        var s = await session.LoadAsync<Skill>(id, ct);
        return s is null || s.OwnerPrincipalId != ownerId ? null : s;
    }
}
