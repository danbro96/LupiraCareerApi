using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Domain.Experiences;

/// <summary>Inline read model: a unified, owner-scoped timeline of engagements and projects with their applied
/// skills. <see cref="OwnerPrincipalId"/> is stamped from the creation events. Engagements carry no <see cref="Title"/>;
/// <see cref="OrganizationId"/> stands in and the read service resolves the organization name.</summary>
public sealed class ExperienceRow
{
    public Guid Id { get; set; }

    public Guid OwnerPrincipalId { get; set; }

    public ExperienceKind Kind { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateOnly OccurredOn { get; set; }

    public DateOnly? EndDate { get; set; }

    public Guid? EngagementId { get; set; }

    public Guid? ProjectId { get; set; }

    public Guid? OrganizationId { get; set; }

    public List<Guid> SkillIds { get; set; } = new();

    public Location? Location { get; set; }
}
