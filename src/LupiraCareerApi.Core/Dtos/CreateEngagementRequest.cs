using LupiraCareerApi.Core.Domain.Engagements;
using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Dtos;

public sealed class CreateEngagementRequest
{
    public required EngagementKind Kind { get; set; }
    public required Guid OrganizationId { get; set; }
    public required DateOnly Start { get; set; }
    public Location? Location { get; set; }
    public string? Summary { get; set; }
}
