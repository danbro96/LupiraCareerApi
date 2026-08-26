namespace LupiraCareerApi.Core.Domain;

public sealed record SkillRegistered(
    Guid SkillId,
    Guid OwnerPrincipalId,
    string Name,
    SkillCategory Category,
    IReadOnlyList<string>? Aliases,
    Guid? ParentSkillId);

public sealed record SkillRenamed(Guid SkillId, string NewName);

public sealed record SkillCategoryChanged(Guid SkillId, SkillCategory NewCategory);

public sealed record SkillAliasAdded(Guid SkillId, string Alias);

public sealed record SkillReparented(Guid SkillId, Guid? NewParentSkillId);

public sealed record SkillRetired(Guid SkillId);

public sealed record SkillLearned(
    Guid SkillId,
    DateOnly OccurredOn,
    Maturity InitialMaturity,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);

public sealed record SkillApplied(
    Guid SkillId,
    DateOnly OccurredOn,
    Intensity Intensity,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);

public sealed record SkillDeepened(
    Guid SkillId,
    DateOnly OccurredOn,
    Maturity FromMaturity,
    Maturity ToMaturity,
    string? Note,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);

public sealed record SkillTaught(
    Guid SkillId,
    DateOnly OccurredOn,
    string Audience,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);

public sealed record SkillReferenced(
    Guid SkillId,
    DateOnly OccurredOn,
    string Note,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);

public sealed record SkillsCombined(
    Guid SkillId,
    Guid OtherSkillId,
    DateOnly OccurredOn,
    bool IsPrimary,
    SkillEdgeContext Context,
    Evidence? Evidence);
