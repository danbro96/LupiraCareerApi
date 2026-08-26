using LupiraCareerApi.Core.Domain.Engagements;
using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Dtos;

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
