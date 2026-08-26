using LupiraCareerApi.Core.Domain.Shared;
using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

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
