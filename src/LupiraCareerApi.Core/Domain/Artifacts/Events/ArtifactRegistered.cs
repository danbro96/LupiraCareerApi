namespace LupiraCareerApi.Core.Domain.Artifacts.Events;

public sealed record ArtifactRegistered(
    Guid ArtifactId,
    Guid OwnerPrincipalId,
    ArtifactKind Kind,
    string Url,
    string Title,
    string? Description,
    DateOnly? ProducedOn,
    DateTimeOffset OccurredAt);
