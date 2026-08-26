namespace LupiraCareerApi.Core.Domain.Engagements.Events;

public sealed record EngagementEnded(Guid EngagementId, DateOnly EndDate, string? Reason);
