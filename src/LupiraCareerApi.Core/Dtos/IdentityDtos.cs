namespace LupiraCareerApi.Dtos;

/// <summary>The caller's resolved local identity.</summary>
public record MeDto(Guid Id, string Email, string? DisplayName);

public record ProfileDto(
    Guid OwnerPrincipalId,
    string FullName,
    string? Tagline,
    string? Bio,
    string? Location,
    string? GithubUrl,
    string? LinkedInUrl,
    string? WebsiteUrl);

public record UpdateProfileRequest(
    string FullName,
    string? Tagline,
    string? Bio,
    string? Location,
    string? GithubUrl,
    string? LinkedInUrl,
    string? WebsiteUrl);
