namespace LupiraCareerApi.Core.Domain.Engagements.Events;

public sealed record EngagementKindReclassified(Guid EngagementId, EngagementKind NewKind);
