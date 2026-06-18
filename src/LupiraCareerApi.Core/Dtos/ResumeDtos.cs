using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

/// <summary>One row of the unified experience timeline (engagements + projects).</summary>
public record ExperienceItemDto(
    ExperienceKind Kind,
    Guid Id,
    string Title,
    DateOnly OccurredOn,
    DateOnly? EndDate,
    Guid? OrganizationId,
    string? OrganizationName,
    Location? Location,
    IReadOnlyList<Guid> SkillIds);

/// <summary>A composed résumé: the public profile header plus the published engagements, projects, and skills.</summary>
public record ResumeDto(
    ProfileDto Profile,
    IReadOnlyList<EngagementDto> Engagements,
    IReadOnlyList<ProjectDto> Projects,
    IReadOnlyList<SkillDto> Skills);
