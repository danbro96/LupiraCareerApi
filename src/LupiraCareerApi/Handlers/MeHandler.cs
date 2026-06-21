using LupiraCareerApi.Auth;
using LupiraCareerApi.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

public sealed class MeHandler(CurrentUser user)
{
    public async Task<Results<Ok<MeDto>, UnauthorizedHttpResult>> GetAsync(CancellationToken ct)
    {
        var u = await user.GetAsync(ct);
        return TypedResults.Ok(new MeDto { Id = u.Id, Email = u.Email, DisplayName = u.DisplayName });
    }
}
