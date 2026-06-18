using LupiraCareerApi.Application;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

public sealed class GoalsHandler(CurrentUser user, GoalService goals)
{
    public async Task<Results<Ok<List<GoalDto>>, ProblemHttpResult, UnauthorizedHttpResult>> ListAsync(CancellationToken ct) =>
        OpResultMap.OkProblem(await goals.ListAsync((await user.GetAsync(ct)).Id, ct));

    public async Task<Results<Ok<GoalDto>, ProblemHttpResult, UnauthorizedHttpResult>> CreateAsync(SetGoalRequest body, CancellationToken ct) =>
        OpResultMap.OkProblem(await goals.SetAsync((await user.GetAsync(ct)).Id, body, ct));

    public async Task<Results<Ok<GoalDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await goals.GetAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<GoalDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> RescopeAsync(Guid id, RescopeGoalRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await goals.RescopeAsync((await user.GetAsync(ct)).Id, id, body.TargetMaturity, body.Deadline, ct));

    public async Task<Results<Ok<GoalDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> RecordProgressAsync(Guid id, RecordProgressRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await goals.RecordProgressAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<Ok<GoalDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> AchieveAsync(Guid id, AchieveGoalRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await goals.AchieveAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<Ok<GoalDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> AbandonAsync(Guid id, AbandonGoalRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await goals.AbandonAsync((await user.GetAsync(ct)).Id, id, body.Reason, ct));
}
