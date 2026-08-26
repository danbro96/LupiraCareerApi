using LupiraCareerApi.Core.Domain.Engagements;
using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Dtos;

/// <summary>Partial update (PATCH): only non-null fields are applied, each emitting its own domain event.
/// Supplying <see cref="End"/> ends the engagement.</summary>
public sealed class UpdateEngagementRequest
{
    public string? Summary { get; set; }

    public EngagementKind? Kind { get; set; }

    public Location? Location { get; set; }

    public DateOnly? End { get; set; }

    public string? EndReason { get; set; }
}
