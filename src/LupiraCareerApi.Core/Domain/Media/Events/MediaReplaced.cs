namespace LupiraCareerApi.Core.Domain.Media.Events;

public sealed record MediaReplaced(
    Guid MediaId,
    string NewBlobRef,
    string NewMimeType,
    int? NewWidth,
    int? NewHeight,
    DateTimeOffset OccurredAt);
