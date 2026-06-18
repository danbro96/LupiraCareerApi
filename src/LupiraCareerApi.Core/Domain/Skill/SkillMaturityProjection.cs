using Marten.Events.Aggregation;

namespace LupiraCareerApi.Domain;

public sealed class SkillMaturityPoint
{
    public DateOnly OccurredOn { get; set; }
    public Maturity Maturity { get; set; }
    public string? Reason { get; set; }
}

/// <summary>Inline read model: a skill's current maturity plus the trajectory that produced it. Single-stream,
/// owner-scoped via <see cref="OwnerPrincipalId"/> from <see cref="SkillRegistered"/>.</summary>
public sealed class SkillMaturity
{
    public Guid Id { get; set; }
    public Guid OwnerPrincipalId { get; set; }
    public Maturity Current { get; set; } = Maturity.Aware;
    public List<SkillMaturityPoint> Trajectory { get; set; } = new();
}

public sealed partial class SkillMaturityProjection : SingleStreamProjection<SkillMaturity, Guid>
{
    public SkillMaturity Create(SkillRegistered e) => new()
    {
        Id = e.SkillId,
        OwnerPrincipalId = e.OwnerPrincipalId,
        Current = Maturity.Aware,
        Trajectory = new(),
    };

    public void Apply(SkillLearned e, SkillMaturity m)
    {
        m.Current = e.InitialMaturity;
        m.Trajectory.Add(new()
        {
            OccurredOn = e.OccurredOn,
            Maturity = e.InitialMaturity,
            Reason = "Learned",
        });
    }

    public void Apply(SkillDeepened e, SkillMaturity m)
    {
        m.Current = e.ToMaturity;
        m.Trajectory.Add(new()
        {
            OccurredOn = e.OccurredOn,
            Maturity = e.ToMaturity,
            Reason = e.Note,
        });
    }
}
