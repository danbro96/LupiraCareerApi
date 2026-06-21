using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public sealed class SkillDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required SkillCategory Category { get; set; }
    public required IReadOnlyList<string> Aliases { get; set; }
    public Guid? ParentSkillId { get; set; }
    public required bool Retired { get; set; }
    public DateOnly? FirstLearnedOn { get; set; }
    public required Maturity CurrentMaturity { get; set; }
}

public sealed class RegisterSkillRequest
{
    public required string Name { get; set; }
    public required SkillCategory Category { get; set; }
    public IReadOnlyList<string>? Aliases { get; set; }
    public Guid? ParentSkillId { get; set; }
}

/// <summary>Partial update (PATCH): only non-null fields are applied, each emitting its own domain event.</summary>
public sealed class UpdateSkillRequest
{
    public string? Name { get; set; }
    public SkillCategory? Category { get; set; }
    public Guid? ParentSkillId { get; set; }
}

public sealed class AddAliasRequest
{
    public required string Alias { get; set; }
}

public sealed class LearnSkillRequest
{
    public required DateOnly OccurredOn { get; set; }
    public required Maturity InitialMaturity { get; set; }
    public required SkillEdgeContext Context { get; set; }
    public Evidence? Evidence { get; set; }
    public Location? Location { get; set; }
}

public sealed class ApplySkillRequest
{
    public required DateOnly OccurredOn { get; set; }
    public required Intensity Intensity { get; set; }
    public required SkillEdgeContext Context { get; set; }
    public Evidence? Evidence { get; set; }
    public Location? Location { get; set; }
}

public sealed class DeepenSkillRequest
{
    public required DateOnly OccurredOn { get; set; }
    public required Maturity FromMaturity { get; set; }
    public required Maturity ToMaturity { get; set; }
    public string? Note { get; set; }
    public required SkillEdgeContext Context { get; set; }
    public Evidence? Evidence { get; set; }
    public Location? Location { get; set; }
}
