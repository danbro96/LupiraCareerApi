using LupiraCareerApi.Application;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

public sealed class ProfileHandler(CurrentUser user, ProfileService profiles)
{
    public async Task<Results<Ok<ProfileDto>, ProblemHttpResult, UnauthorizedHttpResult>> GetAsync(CancellationToken ct) =>
        OpResultMap.OkProblem(await profiles.GetAsync((await user.GetAsync(ct)).Id, ct));

    public async Task<Results<Ok<ProfileDto>, ProblemHttpResult, UnauthorizedHttpResult>> UpsertAsync(UpdateProfileRequest body, CancellationToken ct) =>
        OpResultMap.OkProblem(await profiles.UpsertAsync((await user.GetAsync(ct)).Id, body, ct));
}
