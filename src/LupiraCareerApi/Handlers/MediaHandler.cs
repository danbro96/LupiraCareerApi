using LupiraCareerApi.Application;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

public sealed class MediaHandler(CurrentUser user, MediaService media)
{
    public async Task<Results<Ok<List<MediaDto>>, ProblemHttpResult, UnauthorizedHttpResult>> ListAsync(CancellationToken ct) =>
        OpResultMap.OkProblem(await media.ListAsync((await user.GetAsync(ct)).Id, ct));

    public async Task<Results<Ok<MediaDto>, ProblemHttpResult, UnauthorizedHttpResult>> RegisterAsync(RegisterMediaRequest body, CancellationToken ct) =>
        OpResultMap.OkProblem(await media.RegisterAsync((await user.GetAsync(ct)).Id, body, ct));

    public async Task<Results<Ok<MediaDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await media.GetAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<MediaDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> LinkProjectAsync(Guid id, Guid projectId, MediaProjectRoleRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await media.LinkToProjectAsync((await user.GetAsync(ct)).Id, id, projectId, body.Role, ct));

    public async Task<Results<Ok<MediaDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UnlinkProjectAsync(Guid id, Guid projectId, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await media.UnlinkAsync((await user.GetAsync(ct)).Id, id, MediaTargetKind.Project, projectId, ct));

    public async Task<Results<Ok<MediaDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> LinkSkillAsync(Guid id, Guid skillId, string? note, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await media.LinkToSkillAsync((await user.GetAsync(ct)).Id, id, skillId, note, ct));

    public async Task<Results<Ok<MediaDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UnlinkSkillAsync(Guid id, Guid skillId, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await media.UnlinkAsync((await user.GetAsync(ct)).Id, id, MediaTargetKind.Skill, skillId, ct));

    public async Task<Results<Ok<MediaDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ArchiveAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await media.ArchiveAsync((await user.GetAsync(ct)).Id, id, null, ct));
}
