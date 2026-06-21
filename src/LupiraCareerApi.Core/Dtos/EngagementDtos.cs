using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public sealed class TitleEpochDto
{
    public required Guid TitleId { get; set; }
    public required string Text { get; set; }
    public required DateOnly From { get; set; }
    public DateOnly? To { get; set; }
}

public sealed class EngagementDto
{
    public required Guid Id { get; set; }
    public required EngagementKind Kind { get; set; }
    public required Guid OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public required DateOnly Start { get; set; }
    public DateOnly? End { get; set; }
    public Location? Location { get; set; }
    public string? Summary { get; set; }
    public string? CurrentTitle { get; set; }
    public required IReadOnlyList<TitleEpochDto> Titles { get; set; }
    public required IReadOnlyList<Guid> SkillIds { get; set; }
}

public sealed class CreateEngagementRequest
{
    public required EngagementKind Kind { get; set; }
    public required Guid OrganizationId { get; set; }
    public required DateOnly Start { get; set; }
    public Location? Location { get; set; }
    public string? Summary { get; set; }
}

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

public sealed class AssumeTitleRequest
{
    public required string Text { get; set; }
    public required DateOnly EffectiveFrom { get; set; }
}

public sealed class UpdateTitleRequest
{
    public string? Text { get; set; }
    public DateOnly? RetiredOn { get; set; }
}
