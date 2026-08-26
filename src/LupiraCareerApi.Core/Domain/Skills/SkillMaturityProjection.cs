using LupiraCareerApi.Core.Domain.Skills.Events;
using Marten.Events.Aggregation;

namespace LupiraCareerApi.Core.Domain.Skills;

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
