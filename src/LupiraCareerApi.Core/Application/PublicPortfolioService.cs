using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>
/// The public, read-only portfolio surface. Resolves a published <see cref="Profile.PublicHandle"/> to its owner,
/// then serves that owner's career graph minus the private parts — goals are never included, archived
/// projects/media/artifacts and retired skills are filtered out. Owner identity comes from the handle, not the
/// caller, so the caller only needs a valid token (the surface is gated, not owner-scoped). Unknown or unpublished
/// handles read as <see cref="OpStatus.NotFound"/>, making an unpublished portfolio indistinguishable from a missing
/// one. Composition reuses the owner-scoped services unchanged — they already take the owner id as a parameter.
/// </summary>
public sealed class PublicPortfolioService(
    IDocumentSession session,
    ProfileService profiles,
    ResumeService resume,
    EngagementService engagements,
    ProjectService projects,
    SkillService skills,
    MediaService media,
    ArtifactService artifacts)
{
    public Task<OpResult<ProfileDto>> GetProfileAsync(string handle, CancellationToken ct = default) =>
        WithOwnerAsync(handle, owner => profiles.GetAsync(owner, ct), ct);

    public Task<OpResult<PublicPortfolioDto>> GetPortfolioAsync(string handle, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
        {
            var r = (await resume.GetResumeAsync(owner, ct)).Value!;
            var experience = (await resume.GetExperienceAsync(owner, ct)).Value!;
            var mediaItems = (await media.ListAsync(owner, ct)).Value!;
            var artifactItems = (await artifacts.ListAsync(owner, ct)).Value!;

            var publishedProjects = r.Projects.Where(IsPublished).ToList();
            var publishedProjectIds = publishedProjects.Select(p => p.Id).ToHashSet();

            return OpResult<PublicPortfolioDto>.Ok(new PublicPortfolioDto
            {
                Profile = r.Profile,
                Engagements = r.Engagements,
                Projects = publishedProjects,
                Skills = [.. r.Skills.Where(IsPublished)],
                Experience = [.. experience.Where(x => IsPublishedExperience(x, publishedProjectIds))],
                Media = [.. mediaItems.Where(IsPublished)],
                Artifacts = [.. artifactItems.Where(IsPublished)],
            });
        }, ct);

    public Task<OpResult<List<EngagementDto>>> ListEngagementsAsync(string handle, CancellationToken ct = default) =>
        WithOwnerAsync(handle, owner => engagements.ListAsync(owner, ct), ct);

    public Task<OpResult<EngagementDto>> GetEngagementAsync(string handle, Guid id, CancellationToken ct = default) =>
        WithOwnerAsync(handle, owner => engagements.GetAsync(owner, id, ct), ct);

    public Task<OpResult<List<ProjectDto>>> ListProjectsAsync(string handle, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
        {
            var r = await projects.ListAsync(owner, null, ct);
            return r.IsOk ? OpResult<List<ProjectDto>>.Ok([.. r.Value!.Where(IsPublished)]) : r;
        }, ct);

    public Task<OpResult<ProjectDto>> GetProjectAsync(string handle, Guid id, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
        {
            var r = await projects.GetAsync(owner, id, ct);
            return r.IsOk && !IsPublished(r.Value!) ? OpResult<ProjectDto>.NotFound() : r;
        }, ct);

    public Task<OpResult<List<SkillDto>>> ListSkillsAsync(string handle, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
        {
            var r = await skills.ListAsync(owner, ct);
            return r.IsOk ? OpResult<List<SkillDto>>.Ok([.. r.Value!.Where(IsPublished)]) : r;
        }, ct);

    public Task<OpResult<SkillDto>> GetSkillAsync(string handle, Guid id, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
        {
            var r = await skills.GetAsync(owner, id, ct);
            return r.IsOk && !IsPublished(r.Value!) ? OpResult<SkillDto>.NotFound() : r;
        }, ct);

    public Task<OpResult<SkillTimeline>> GetSkillTimelineAsync(string handle, Guid id, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
            await IsPublishedSkillAsync(owner, id, ct)
                ? await skills.GetTimelineAsync(owner, id, ct)
                : OpResult<SkillTimeline>.NotFound(), ct);

    public Task<OpResult<SkillMaturity>> GetSkillMaturityAsync(string handle, Guid id, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
            await IsPublishedSkillAsync(owner, id, ct)
                ? await skills.GetMaturityAsync(owner, id, ct)
                : OpResult<SkillMaturity>.NotFound(), ct);

    public Task<OpResult<List<ExperienceItemDto>>> GetExperienceAsync(string handle, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
        {
            var experience = (await resume.GetExperienceAsync(owner, ct)).Value!;
            var publishedProjectIds = (await projects.ListAsync(owner, null, ct)).Value!.Where(IsPublished).Select(p => p.Id).ToHashSet();
            return OpResult<List<ExperienceItemDto>>.Ok([.. experience.Where(x => IsPublishedExperience(x, publishedProjectIds))]);
        }, ct);

    public Task<OpResult<List<MediaDto>>> ListMediaAsync(string handle, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
        {
            var r = await media.ListAsync(owner, ct);
            return r.IsOk ? OpResult<List<MediaDto>>.Ok([.. r.Value!.Where(IsPublished)]) : r;
        }, ct);

    public Task<OpResult<MediaDto>> GetMediaAsync(string handle, Guid id, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
        {
            var r = await media.GetAsync(owner, id, ct);
            return r.IsOk && !IsPublished(r.Value!) ? OpResult<MediaDto>.NotFound() : r;
        }, ct);

    public Task<OpResult<List<ArtifactDto>>> ListArtifactsAsync(string handle, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
        {
            var r = await artifacts.ListAsync(owner, ct);
            return r.IsOk ? OpResult<List<ArtifactDto>>.Ok([.. r.Value!.Where(IsPublished)]) : r;
        }, ct);

    public Task<OpResult<ArtifactDto>> GetArtifactAsync(string handle, Guid id, CancellationToken ct = default) =>
        WithOwnerAsync(handle, async owner =>
        {
            var r = await artifacts.GetAsync(owner, id, ct);
            return r.IsOk && !IsPublished(r.Value!) ? OpResult<ArtifactDto>.NotFound() : r;
        }, ct);

    /// <summary>Resolves the published handle to its owner and runs <paramref name="inner"/> with the owner id;
    /// an unknown or unpublished handle short-circuits to <see cref="OpStatus.NotFound"/>.</summary>
    private async Task<OpResult<T>> WithOwnerAsync<T>(string handle, Func<Guid, Task<OpResult<T>>> inner, CancellationToken ct)
    {
        var normalized = handle?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalized)) return OpResult<T>.NotFound();
        var profile = await session.Query<Profile>().FirstOrDefaultAsync(p => p.PublicHandle == normalized && p.IsPublished, ct);
        return profile is null ? OpResult<T>.NotFound() : await inner(profile.OwnerPrincipalId);
    }

    private async Task<bool> IsPublishedSkillAsync(Guid owner, Guid id, CancellationToken ct)
    {
        var s = await skills.GetAsync(owner, id, ct);
        return s.IsOk && IsPublished(s.Value!);
    }

    private static bool IsPublished(ProjectDto p) => p.Status != ProjectStatus.Archived;
    private static bool IsPublished(SkillDto s) => !s.Retired;
    private static bool IsPublished(MediaDto m) => !m.Archived;
    private static bool IsPublished(ArtifactDto a) => !a.Archived;

    private static bool IsPublishedExperience(ExperienceItemDto x, HashSet<Guid> publishedProjectIds) =>
        x.Kind == ExperienceKind.Engagement || publishedProjectIds.Contains(x.Id);
}
