namespace LupiraCareerApi.Core.Domain.Media.Events;

public sealed record MediaArchived(
    Guid MediaId,
    string? Reason,
    DateTimeOffset OccurredAt);
