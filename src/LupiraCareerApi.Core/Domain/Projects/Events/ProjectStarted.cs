namespace LupiraCareerApi.Core.Domain.Projects.Events;

public sealed record ProjectStarted(
    Guid ProjectId,
    Guid OwnerPrincipalId,
    ProjectKind Kind,
    string Title,
    string? Description,
    Guid? EngagementId,
    string? Url,
    DateOnly? StartDate);
