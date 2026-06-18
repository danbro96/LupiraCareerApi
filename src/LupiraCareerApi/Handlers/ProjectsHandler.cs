using LupiraCareerApi.Application;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

public sealed class ProjectsHandler(CurrentUser user, ProjectService projects, ArtifactService artifacts, MediaService media)
{
    public async Task<Results<Ok<List<ProjectDto>>, ProblemHttpResult, UnauthorizedHttpResult>> ListAsync(Guid? engagementId, CancellationToken ct) =>
        OpResultMap.OkProblem(await projects.ListAsync((await user.GetAsync(ct)).Id, engagementId, ct));

    public async Task<Results<Ok<ProjectDto>, ProblemHttpResult, UnauthorizedHttpResult>> CreateAsync(CreateProjectRequest body, CancellationToken ct) =>
        OpResultMap.OkProblem(await projects.CreateAsync((await user.GetAsync(ct)).Id, body, ct));

    public async Task<Results<Ok<ProjectDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await projects.GetAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<ProjectDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UpdateAsync(Guid id, UpdateProjectRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await projects.UpdateAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<Ok<ProjectDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ShipAsync(Guid id, ShipProjectRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await projects.ShipAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<Ok<ProjectDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ShelveAsync(Guid id, string? reason, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await projects.ShelveAsync((await user.GetAsync(ct)).Id, id, reason, ct));

    public async Task<Results<Ok<ProjectDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ArchiveAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await projects.ArchiveAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<ProjectDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> AttachEngagementAsync(Guid id, AttachEngagementRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await projects.AttachToEngagementAsync((await user.GetAsync(ct)).Id, id, body.EngagementId, ct));

    public async Task<Results<Ok<ProjectDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> DetachEngagementAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await projects.DetachFromEngagementAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<ProjectDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> AttachSkillAsync(Guid id, Guid skillId, DateOnly? on, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await projects.AttachSkillAsync((await user.GetAsync(ct)).Id, id, skillId, on, ct));

    public async Task<Results<Ok<ProjectDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> DetachSkillAsync(Guid id, Guid skillId, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await projects.DetachSkillAsync((await user.GetAsync(ct)).Id, id, skillId, ct));

    public async Task<Results<Ok<List<ArtifactDto>>, ProblemHttpResult, UnauthorizedHttpResult>> ArtifactsAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkProblem(await artifacts.ForProjectAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<List<MediaDto>>, ProblemHttpResult, UnauthorizedHttpResult>> MediaAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkProblem(await media.ForProjectAsync((await user.GetAsync(ct)).Id, id, ct));
}
