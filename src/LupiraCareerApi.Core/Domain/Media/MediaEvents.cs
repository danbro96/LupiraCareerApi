namespace LupiraCareerApi.Core.Domain;

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

public sealed record MediaLinkedToProject(
    Guid MediaId,
    Guid ProjectId,
    MediaRole Role,
    DateTimeOffset OccurredAt);

public sealed record MediaLinkedToSkill(
    Guid MediaId,
    Guid SkillId,
    string? Note,
    DateTimeOffset OccurredAt);

public sealed record MediaUnlinked(
    Guid MediaId,
    MediaTargetKind TargetKind,
    Guid TargetId,
    DateTimeOffset OccurredAt);

public sealed record MediaReplaced(
    Guid MediaId,
    string NewBlobRef,
    string NewMimeType,
    int? NewWidth,
    int? NewHeight,
    DateTimeOffset OccurredAt);

public sealed record MediaArchived(
    Guid MediaId,
    string? Reason,
    DateTimeOffset OccurredAt);
