using LupiraCareerApi.Core.Domain.Experiences;
using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Dtos;

/// <summary>One row of the unified experience timeline (engagements + projects).</summary>
public sealed class ExperienceItemDto
{
    public required ExperienceKind Kind { get; set; }
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required DateOnly OccurredOn { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public Location? Location { get; set; }
    public required IReadOnlyList<Guid> SkillIds { get; set; }
}
