namespace LupiraCareerApi.Core.Domain;

public sealed record EngagementStarted(
    Guid EngagementId,
    Guid OwnerPrincipalId,
    EngagementKind Kind,
    Guid OrganizationId,
    DateOnly StartDate,
    Location? Location,
    string? Summary);

public sealed record EngagementEnded(Guid EngagementId, DateOnly EndDate, string? Reason);

public sealed record EngagementSummaryRevised(Guid EngagementId, string? Summary);

public sealed record EngagementRelocated(Guid EngagementId, Location NewLocation);

public sealed record EngagementKindReclassified(Guid EngagementId, EngagementKind NewKind);

public sealed record TitleAssumed(Guid EngagementId, Guid TitleId, string Text, DateOnly EffectiveFrom);

public sealed record TitleRevised(Guid EngagementId, Guid TitleId, string NewText);

public sealed record TitleRetired(Guid EngagementId, Guid TitleId, DateOnly EffectiveTo);

public sealed record EngagementSkillAttached(Guid EngagementId, Guid SkillId, DateOnly? AttachedOn);

public sealed record EngagementSkillDetached(Guid EngagementId, Guid SkillId);
