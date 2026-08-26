using LupiraCareerApi.Core.Domain.Shared;
using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

public sealed class LearnSkillRequest
{
    public required DateOnly OccurredOn { get; set; }
    public required Maturity InitialMaturity { get; set; }
    public required SkillEdgeContext Context { get; set; }
    public Evidence? Evidence { get; set; }
    public Location? Location { get; set; }
}
