namespace LupiraCareerApi.Core.Domain.Media.Events;

public sealed record MediaLinkedToProject(
    Guid MediaId,
    Guid ProjectId,
    MediaRole Role,
    DateTimeOffset OccurredAt);
