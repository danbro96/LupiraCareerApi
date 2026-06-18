using LupiraCareerApi.Application;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

public sealed class ResumeHandler(CurrentUser user, ResumeService resume)
{
    public async Task<Results<Ok<ResumeDto>, ProblemHttpResult, UnauthorizedHttpResult>> GetResumeAsync(CancellationToken ct) =>
        OpResultMap.OkProblem(await resume.GetResumeAsync((await user.GetAsync(ct)).Id, ct));

    public async Task<Results<Ok<List<ExperienceItemDto>>, ProblemHttpResult, UnauthorizedHttpResult>> GetExperienceAsync(CancellationToken ct) =>
        OpResultMap.OkProblem(await resume.GetExperienceAsync((await user.GetAsync(ct)).Id, ct));
}
