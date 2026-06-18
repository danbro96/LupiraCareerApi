using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Mappers;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>The caller's "about me" profile (one per principal). Read returns an empty shell if none exists yet,
/// so the editor always has something to bind. What of this is shown publicly is decided downstream by LupiraWeb.</summary>
public sealed class ProfileService(IDocumentSession session)
{
    public async Task<OpResult<ProfileDto>> GetAsync(Guid ownerId, CancellationToken ct = default)
    {
        var p = await session.Query<Profile>().FirstOrDefaultAsync(x => x.OwnerPrincipalId == ownerId, ct);
        return OpResult<ProfileDto>.Ok(p?.ToDto() ?? Empty(ownerId));
    }

    public async Task<OpResult<ProfileDto>> UpsertAsync(Guid ownerId, UpdateProfileRequest r, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(r.FullName)) return OpResult<ProfileDto>.Invalid("FullName is required.");

        var p = await session.Query<Profile>().FirstOrDefaultAsync(x => x.OwnerPrincipalId == ownerId, ct)
            ?? new Profile { Id = Guid.NewGuid(), OwnerPrincipalId = ownerId };

        p.FullName = r.FullName.Trim();
        p.Tagline = r.Tagline;
        p.Bio = r.Bio;
        p.Location = r.Location;
        p.GithubUrl = r.GithubUrl;
        p.LinkedInUrl = r.LinkedInUrl;
        p.WebsiteUrl = r.WebsiteUrl;
        session.Store(p);
        await session.SaveChangesAsync(ct);
        return OpResult<ProfileDto>.Ok(p.ToDto());
    }

    private static ProfileDto Empty(Guid ownerId) => new(ownerId, "", null, null, null, null, null, null);
}
