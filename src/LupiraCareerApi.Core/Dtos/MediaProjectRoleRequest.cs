using LupiraCareerApi.Core.Domain.Media;

namespace LupiraCareerApi.Core.Dtos;

/// <summary>Body for linking a media asset to a project (the project id is in the route).</summary>
public sealed class MediaProjectRoleRequest
{
    public required MediaRole Role { get; set; }
}
