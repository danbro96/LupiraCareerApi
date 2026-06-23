using LupiraCareerApi.Application;
using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

/// <summary>The public portfolio surface (handle-addressed). Unlike the owner handlers it takes no
/// <c>CurrentUser</c> — the owner is resolved from the route handle and the policy only requires a valid token.
/// An unknown/unpublished handle (or a filtered-out item) is a 404.</summary>
public sealed class PublicPortfolioHandler(PublicPortfolioService portfolio)
{
    public async Task<Results<Ok<PublicPortfolioDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetPortfolioAsync(string handle, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.GetPortfolioAsync(handle, ct));

    public async Task<Results<Ok<ProfileDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetProfileAsync(string handle, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.GetProfileAsync(handle, ct));

    public async Task<Results<Ok<List<EngagementDto>>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ListEngagementsAsync(string handle, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.ListEngagementsAsync(handle, ct));

    public async Task<Results<Ok<EngagementDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetEngagementAsync(string handle, Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.GetEngagementAsync(handle, id, ct));

    public async Task<Results<Ok<List<ProjectDto>>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ListProjectsAsync(string handle, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.ListProjectsAsync(handle, ct));

    public async Task<Results<Ok<ProjectDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetProjectAsync(string handle, Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.GetProjectAsync(handle, id, ct));

    public async Task<Results<Ok<List<SkillDto>>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ListSkillsAsync(string handle, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.ListSkillsAsync(handle, ct));

    public async Task<Results<Ok<SkillDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetSkillAsync(string handle, Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.GetSkillAsync(handle, id, ct));

    public async Task<Results<Ok<SkillTimeline>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> SkillTimelineAsync(string handle, Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.GetSkillTimelineAsync(handle, id, ct));

    public async Task<Results<Ok<SkillMaturity>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> SkillMaturityAsync(string handle, Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.GetSkillMaturityAsync(handle, id, ct));

    public async Task<Results<Ok<List<ExperienceItemDto>>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetExperienceAsync(string handle, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.GetExperienceAsync(handle, ct));

    public async Task<Results<Ok<List<MediaDto>>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ListMediaAsync(string handle, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.ListMediaAsync(handle, ct));

    public async Task<Results<Ok<MediaDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetMediaAsync(string handle, Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.GetMediaAsync(handle, id, ct));

    public async Task<Results<Ok<List<ArtifactDto>>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ListArtifactsAsync(string handle, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.ListArtifactsAsync(handle, ct));

    public async Task<Results<Ok<ArtifactDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetArtifactAsync(string handle, Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await portfolio.GetArtifactAsync(handle, id, ct));
}
