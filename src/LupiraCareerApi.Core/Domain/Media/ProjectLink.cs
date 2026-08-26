namespace LupiraCareerApi.Core.Domain.Media;

public sealed class ProjectLink
{
    public Guid ProjectId { get; set; }
    public MediaRole Role { get; set; }
}
