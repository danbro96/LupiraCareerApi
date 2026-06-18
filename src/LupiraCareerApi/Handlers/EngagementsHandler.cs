using LupiraCareerApi.Application;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

public sealed class EngagementsHandler(CurrentUser user, EngagementService engagements)
{
    public async Task<Results<Ok<List<EngagementDto>>, ProblemHttpResult, UnauthorizedHttpResult>> ListAsync(CancellationToken ct) =>
        OpResultMap.OkProblem(await engagements.ListAsync((await user.GetAsync(ct)).Id, ct));

    public async Task<Results<Ok<EngagementDto>, ProblemHttpResult, UnauthorizedHttpResult>> CreateAsync(CreateEngagementRequest body, CancellationToken ct) =>
        OpResultMap.OkProblem(await engagements.CreateAsync((await user.GetAsync(ct)).Id, body, ct));

    public async Task<Results<Ok<EngagementDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await engagements.GetAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<EngagementDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UpdateAsync(Guid id, UpdateEngagementRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await engagements.UpdateAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<Ok<EngagementDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> AssumeTitleAsync(Guid id, AssumeTitleRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await engagements.AssumeTitleAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<Ok<EngagementDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UpdateTitleAsync(Guid id, Guid titleId, UpdateTitleRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await engagements.UpdateTitleAsync((await user.GetAsync(ct)).Id, id, titleId, body, ct));

    public async Task<Results<Ok<EngagementDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> AttachSkillAsync(Guid id, Guid skillId, DateOnly? on, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await engagements.AttachSkillAsync((await user.GetAsync(ct)).Id, id, skillId, on, ct));

    public async Task<Results<Ok<EngagementDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> DetachSkillAsync(Guid id, Guid skillId, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await engagements.DetachSkillAsync((await user.GetAsync(ct)).Id, id, skillId, ct));
}
