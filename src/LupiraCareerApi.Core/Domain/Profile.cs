namespace LupiraCareerApi.Domain;

/// <summary>
/// Per-principal "about me" — the career API's replacement for LupiraWeb's global <c>MyInfo</c> singleton.
/// One per principal (keyed by <see cref="OwnerPrincipalId"/>); a plain document, not event-sourced. Opting a
/// portfolio into the public read surface happens here: set <see cref="IsPublished"/> and a <see cref="PublicHandle"/>;
/// the <c>/public/{handle}</c> reads resolve the handle back to this owner. Per-item curation is still coarse
/// (goals never published; archived/retired items filtered) — see <c>PublicPortfolioService</c>.
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

    /// <summary>Stable, unique, lowercase slug this portfolio is served under (<c>/public/{handle}</c>). Null = not addressable publicly.</summary>
    public string? PublicHandle { get; set; }

    /// <summary>Opt-in to the public read surface. A handle is required to publish; unpublished handles read as 404.</summary>
    public bool IsPublished { get; set; }
}
