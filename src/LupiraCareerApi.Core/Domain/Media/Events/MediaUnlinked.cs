namespace LupiraCareerApi.Core.Domain.Media.Events;

public sealed record MediaUnlinked(
    Guid MediaId,
    MediaTargetKind TargetKind,
    Guid TargetId,
    DateTimeOffset OccurredAt);
