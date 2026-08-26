namespace LupiraCareerApi.Core.Domain.Media.Events;

public sealed record MediaRegistered(
    Guid MediaId,
    Guid OwnerPrincipalId,
    string BlobRef,
    string MimeType,
    int? Width,
    int? Height,
    string AltText,
    string? Caption,
    DateTimeOffset OccurredAt);
