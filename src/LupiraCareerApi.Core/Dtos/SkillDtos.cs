using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public record SkillDto(
    Guid Id,
    string Name,
    SkillCategory Category,
    IReadOnlyList<string> Aliases,
    Guid? ParentSkillId,
    bool Retired,
    DateOnly? FirstLearnedOn,
    Maturity CurrentMaturity);

public record RegisterSkillRequest(
    string Name,
    SkillCategory Category,
    IReadOnlyList<string>? Aliases,
    Guid? ParentSkillId);

/// <summary>Partial update (PATCH): rename and/or recategorize and/or reparent.</summary>
public record UpdateSkillRequest(string? Name, SkillCategory? Category, Guid? ParentSkillId);

public record AddAliasRequest(string Alias);

public record LearnSkillRequest(
    DateOnly OccurredOn,
    Maturity InitialMaturity,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);

public record ApplySkillRequest(
    DateOnly OccurredOn,
    Intensity Intensity,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);

public record DeepenSkillRequest(
    DateOnly OccurredOn,
    Maturity FromMaturity,
    Maturity ToMaturity,
    string? Note,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);
