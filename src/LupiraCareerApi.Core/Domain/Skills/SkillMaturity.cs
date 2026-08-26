using LupiraCareerApi.Core.Domain.Skills.Events;

namespace LupiraCareerApi.Core.Domain.Skills;

/// <summary>Inline read model: a skill's current maturity plus the trajectory that produced it. Single-stream,
/// owner-scoped via <see cref="OwnerPrincipalId"/> from <see cref="SkillRegistered"/>.</summary>
public sealed class SkillMaturity
{
    public Guid Id { get; set; }

    public Guid OwnerPrincipalId { get; set; }

    public Maturity Current { get; set; } = Maturity.Aware;

    public List<SkillMaturityPoint> Trajectory { get; set; } = new();
}
