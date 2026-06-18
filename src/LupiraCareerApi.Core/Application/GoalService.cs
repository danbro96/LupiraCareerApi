using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Mappers;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>The caller's skill-development goals. Always private (never surfaced on the public portfolio path).
/// Owner-scoped.</summary>
public sealed class GoalService(IDocumentSession session)
{
    public async Task<OpResult<List<GoalDto>>> ListAsync(Guid ownerId, CancellationToken ct = default)
    {
        var items = await session.Query<Goal>().Where(g => g.OwnerPrincipalId == ownerId).ToListAsync(ct);
        return OpResult<List<GoalDto>>.Ok([.. items.Select(g => g.ToDto())]);
    }

    public async Task<OpResult<GoalDto>> GetAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var g = await LoadOwnedAsync(ownerId, id, ct);
        return g is null ? OpResult<GoalDto>.NotFound() : OpResult<GoalDto>.Ok(g.ToDto());
    }

    public async Task<OpResult<GoalDto>> SetAsync(Guid ownerId, SetGoalRequest r, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(r.Motivation)) return OpResult<GoalDto>.Invalid("Motivation is required.");
        var id = Guid.NewGuid();
        session.Events.StartStream<Goal>(id, new GoalSet(id, ownerId, r.SkillId, r.TargetMaturity, r.Deadline, r.Motivation.Trim(), DateTimeOffset.UtcNow));
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    public Task<OpResult<GoalDto>> RescopeAsync(Guid ownerId, Guid id, Maturity? newTarget, DateOnly? newDeadline, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new GoalRescoped(id, newTarget, newDeadline, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<GoalDto>> RecordProgressAsync(Guid ownerId, Guid id, RecordProgressRequest r, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new GoalProgressRecorded(id, r.Note, r.LinkedEventId, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<GoalDto>> AchieveAsync(Guid ownerId, Guid id, AchieveGoalRequest r, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new GoalAchieved(id, r.AchievedOn, r.EvidenceArtifactId, DateTimeOffset.UtcNow), ct);

    public Task<OpResult<GoalDto>> AbandonAsync(Guid ownerId, Guid id, string reason, CancellationToken ct = default) =>
        AppendAsync(ownerId, id, new GoalAbandoned(id, DateOnly.FromDateTime(DateTime.UtcNow), reason, DateTimeOffset.UtcNow), ct);

    private async Task<OpResult<GoalDto>> AppendAsync(Guid ownerId, Guid id, object @event, CancellationToken ct)
    {
        var g = await LoadOwnedAsync(ownerId, id, ct);
        if (g is null) return OpResult<GoalDto>.NotFound();
        session.Events.Append(id, @event);
        await session.SaveChangesAsync(ct);
        return await GetAsync(ownerId, id, ct);
    }

    private async Task<Goal?> LoadOwnedAsync(Guid ownerId, Guid id, CancellationToken ct)
    {
        var g = await session.LoadAsync<Goal>(id, ct);
        return g is null || g.OwnerPrincipalId != ownerId ? null : g;
    }
}
