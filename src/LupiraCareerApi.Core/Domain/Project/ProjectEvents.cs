namespace LupiraCareerApi.Domain;

public sealed record ProjectStarted(
    Guid ProjectId,
    Guid OwnerPrincipalId,
    ProjectKind Kind,
    string Title,
    string? Description,
    Guid? EngagementId,
    string? Url,
    DateOnly? StartDate);

public sealed record ProjectRenamed(Guid ProjectId, string NewTitle);

public sealed record ProjectDescribed(Guid ProjectId, string? Description);

public sealed record ProjectUrlSet(Guid ProjectId, string? Url);

public sealed record ProjectAttachedToEngagement(Guid ProjectId, Guid EngagementId);

public sealed record ProjectDetachedFromEngagement(Guid ProjectId);

public sealed record ProjectShipped(Guid ProjectId, DateOnly ShippedOn, string? Outcome);

public sealed record ProjectShelved(Guid ProjectId, string? Reason);

public sealed record ProjectArchived(Guid ProjectId);

public sealed record ProjectSkillAttached(Guid ProjectId, Guid SkillId, DateOnly? AttachedOn);

public sealed record ProjectSkillDetached(Guid ProjectId, Guid SkillId);
