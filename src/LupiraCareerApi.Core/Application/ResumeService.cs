using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Mappers;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>Read-side composition for the owner: the unified experience timeline (engagements + projects) and the
/// assembled résumé over all of their items. Public-facing selection is LupiraWeb's job, not this API's.</summary>
public sealed class ResumeService(IDocumentSession session)
{
    public async Task<OpResult<List<ExperienceItemDto>>> GetExperienceAsync(Guid ownerId, CancellationToken ct = default)
    {
        var rows = await session.Query<ExperienceRow>().Where(r => r.OwnerPrincipalId == ownerId).ToListAsync(ct);
        var names = await OrgNamesAsync(rows.Where(r => r.OrganizationId is not null).Select(r => r.OrganizationId!.Value), ct);

        var items = rows
            .OrderByDescending(r => r.OccurredOn)
            .Select(r => new ExperienceItemDto(
                r.Kind,
                r.Id,
                r.Kind == ExperienceKind.Engagement ? names.GetValueOrDefault(r.OrganizationId ?? Guid.Empty, "") : r.Title,
                r.OccurredOn,
                r.EndDate,
                r.OrganizationId,
                r.OrganizationId is Guid oid ? names.GetValueOrDefault(oid) : null,
                r.Location,
                r.SkillIds))
            .ToList();
        return OpResult<List<ExperienceItemDto>>.Ok(items);
    }

    public async Task<OpResult<ResumeDto>> GetResumeAsync(Guid ownerId, CancellationToken ct = default)
    {
        var profile = await session.Query<Profile>().FirstOrDefaultAsync(p => p.OwnerPrincipalId == ownerId, ct);
        var engagements = await session.Query<Engagement>().Where(e => e.OwnerPrincipalId == ownerId).ToListAsync(ct);
        var projects = await session.Query<Project>().Where(p => p.OwnerPrincipalId == ownerId).ToListAsync(ct);
        var skills = await session.Query<Skill>().Where(s => s.OwnerPrincipalId == ownerId).ToListAsync(ct);

        var names = await OrgNamesAsync(engagements.Select(e => e.OrganizationId), ct);
        var profileDto = profile?.ToDto() ?? new ProfileDto(ownerId, "", null, null, null, null, null, null);

        var dto = new ResumeDto(
            profileDto,
            [.. engagements.OrderByDescending(e => e.Start).Select(e => e.ToDto(names.GetValueOrDefault(e.OrganizationId)))],
            [.. projects.OrderByDescending(p => p.Start ?? DateOnly.MinValue).Select(p => p.ToDto())],
            [.. skills.OrderBy(s => s.Name).Select(s => s.ToDto())]);
        return OpResult<ResumeDto>.Ok(dto);
    }

    private async Task<Dictionary<Guid, string>> OrgNamesAsync(IEnumerable<Guid> orgIds, CancellationToken ct)
    {
        var ids = orgIds.Distinct().ToList();
        if (ids.Count == 0) return [];
        var orgs = await session.Query<Organization>().Where(o => ids.Contains(o.Id)).ToListAsync(ct);
        return orgs.ToDictionary(o => o.Id, o => o.Name);
    }
}
