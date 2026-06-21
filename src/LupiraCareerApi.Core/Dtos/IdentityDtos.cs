namespace LupiraCareerApi.Dtos;

public sealed class MeDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public string? DisplayName { get; set; }
}

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
}

public sealed class UpdateProfileRequest
{
    public required string FullName { get; set; }
    public string? Tagline { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? GithubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? WebsiteUrl { get; set; }
}
