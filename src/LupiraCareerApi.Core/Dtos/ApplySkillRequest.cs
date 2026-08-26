using LupiraCareerApi.Core.Domain.Shared;
using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

public sealed class ApplySkillRequest
{
    public required DateOnly OccurredOn { get; set; }

    public required Intensity Intensity { get; set; }

    public required SkillEdgeContext Context { get; set; }

    public Evidence? Evidence { get; set; }

    public Location? Location { get; set; }
}
