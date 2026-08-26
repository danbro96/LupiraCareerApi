using LupiraCareerApi.Core.Domain.Media;

namespace LupiraCareerApi.Core.Dtos;

public sealed class ProjectLinkDto
{
    public required Guid ProjectId { get; set; }

    public required MediaRole Role { get; set; }
}
