using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

/// <summary>Partial update (PATCH): only non-null fields are applied, each emitting its own domain event.</summary>
public sealed class RescopeGoalRequest
{
    public Maturity? TargetMaturity { get; set; }

    public DateOnly? Deadline { get; set; }
}
