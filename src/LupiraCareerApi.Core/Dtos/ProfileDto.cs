namespace LupiraCareerApi.Core.Dtos;

public sealed class ProfileDto
{
    public required Guid OwnerPrincipalId { get; set; }

    public required string FullName { get; set; }

    public string? Tagline { get; set; }

    public string? Bio { get; set; }

    public string? Location { get; set; }

    public string? GithubUrl { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? PublicHandle { get; set; }

    public bool IsPublished { get; set; }
}
