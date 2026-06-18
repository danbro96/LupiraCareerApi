namespace LupiraCareerApi.Domain;

/// <summary>
/// Per-principal "about me" — the career API's replacement for LupiraWeb's global <c>MyInfo</c> singleton.
/// One per principal (keyed by <see cref="OwnerPrincipalId"/>); a plain document, not event-sourced. What of this
/// (if anything) is shown publicly is decided downstream by LupiraWeb, not here.
/// </summary>
public sealed class Profile
{
    public Guid Id { get; set; }
    public Guid OwnerPrincipalId { get; set; }

    public string FullName { get; set; } = "";
    public string? Tagline { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? GithubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? WebsiteUrl { get; set; }
}
