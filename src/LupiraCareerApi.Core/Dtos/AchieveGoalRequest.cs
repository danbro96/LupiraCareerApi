namespace LupiraCareerApi.Core.Dtos;

public sealed class AchieveGoalRequest
{
    public required DateOnly AchievedOn { get; set; }
    public Guid? EvidenceArtifactId { get; set; }
}
