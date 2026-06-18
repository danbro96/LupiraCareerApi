using LupiraCareerApi.Application;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

public sealed class SkillsHandler(CurrentUser user, SkillService skills)
{
    public async Task<Results<Ok<List<SkillDto>>, ProblemHttpResult, UnauthorizedHttpResult>> ListAsync(CancellationToken ct) =>
        OpResultMap.OkProblem(await skills.ListAsync((await user.GetAsync(ct)).Id, ct));

    public async Task<Results<Ok<SkillDto>, ProblemHttpResult, UnauthorizedHttpResult>> RegisterAsync(RegisterSkillRequest body, CancellationToken ct) =>
        OpResultMap.OkProblem(await skills.RegisterAsync((await user.GetAsync(ct)).Id, body, ct));

    public async Task<Results<Ok<SkillDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await skills.GetAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<SkillDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UpdateAsync(Guid id, UpdateSkillRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await skills.UpdateAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<Ok<SkillDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> AddAliasAsync(Guid id, AddAliasRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await skills.AddAliasAsync((await user.GetAsync(ct)).Id, id, body.Alias, ct));

    public async Task<Results<Ok<SkillDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> RetireAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await skills.RetireAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<SkillDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> LearnAsync(Guid id, LearnSkillRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await skills.LearnAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<Ok<SkillDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> ApplyAsync(Guid id, ApplySkillRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await skills.ApplyAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<Ok<SkillDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> DeepenAsync(Guid id, DeepenSkillRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await skills.DeepenAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<Ok<SkillTimeline>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> TimelineAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await skills.GetTimelineAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<SkillMaturity>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> MaturityAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await skills.GetMaturityAsync((await user.GetAsync(ct)).Id, id, ct));
}
