using LupiraCareerApi.Application;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

public sealed class ArtifactsHandler(CurrentUser user, ArtifactService artifacts)
{
    public async Task<Results<Ok<List<ArtifactDto>>, ProblemHttpResult, UnauthorizedHttpResult>> ListAsync(CancellationToken ct) =>
        OpResultMap.OkProblem(await artifacts.ListAsync((await user.GetAsync(ct)).Id, ct));

    public async Task<Results<Ok<ArtifactDto>, ProblemHttpResult, UnauthorizedHttpResult>> RegisterAsync(RegisterArtifactRequest body, CancellationToken ct) =>
        OpResultMap.OkProblem(await artifacts.RegisterAsync((await user.GetAsync(ct)).Id, body, ct));

    public async Task<Results<Ok<ArtifactDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await artifacts.GetAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<ArtifactDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UpdateAsync(Guid id, UpdateArtifactRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await artifacts.UpdateAsync((await user.GetAsync(ct)).Id, id, body.Url, body.Title, body.Description, ct));

    public async Task<Results<Ok<ArtifactDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> LinkProjectAsync(Guid id, Guid projectId, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await artifacts.LinkToProjectAsync((await user.GetAsync(ct)).Id, id, projectId, ct));

    public async Task<Results<Ok<ArtifactDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UnlinkProjectAsync(Guid id, Guid projectId, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await artifacts.UnlinkAsync((await user.GetAsync(ct)).Id, id, ArtifactTargetKind.Project, projectId, ct));

    public async Task<Results<Ok<ArtifactDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> LinkSkillAsync(Guid id, Guid skillId, ArtifactSkillRoleRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await artifacts.LinkToSkillAsync((await user.GetAsync(ct)).Id, id, skillId, body.Role, ct));

    public async Task<Results<Ok<ArtifactDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UnlinkSkillAsync(Guid id, Guid skillId, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await artifacts.UnlinkAsync((await user.GetAsync(ct)).Id, id, ArtifactTargetKind.Skill, skillId, ct));

    public async Task<Results<Ok<ArtifactDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> LinkEngagementAsync(Guid id, Guid engagementId, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await artifacts.LinkToEngagementAsync((await user.GetAsync(ct)).Id, id, engagementId, ct));

    public async Task<Results<Ok<ArtifactDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UnlinkEngagementAsync(Guid id, Guid engagementId, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await artifacts.UnlinkAsync((await user.GetAsync(ct)).Id, id, ArtifactTargetKind.Engagement, engagementId, ct));

    public async Task<Results<Ok<ArtifactDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ArchiveAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await artifacts.ArchiveAsync((await user.GetAsync(ct)).Id, id, null, ct));
}
