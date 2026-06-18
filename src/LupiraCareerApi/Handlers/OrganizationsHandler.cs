using LupiraCareerApi.Application;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LupiraCareerApi.Handlers;

public sealed class OrganizationsHandler(CurrentUser user, OrganizationService orgs)
{
    public async Task<Results<Ok<List<OrganizationDto>>, ProblemHttpResult, UnauthorizedHttpResult>> ListAsync(CancellationToken ct) =>
        OpResultMap.OkProblem(await orgs.ListAsync((await user.GetAsync(ct)).Id, ct));

    public async Task<Results<Ok<OrganizationDto>, ProblemHttpResult, UnauthorizedHttpResult>> CreateAsync(CreateOrganizationRequest body, CancellationToken ct) =>
        OpResultMap.OkProblem(await orgs.CreateAsync((await user.GetAsync(ct)).Id, body, ct));

    public async Task<Results<Ok<OrganizationDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> GetAsync(Guid id, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await orgs.GetAsync((await user.GetAsync(ct)).Id, id, ct));

    public async Task<Results<Ok<OrganizationDto>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> UpdateAsync(Guid id, UpdateOrganizationRequest body, CancellationToken ct) =>
        OpResultMap.OkNotFoundProblem(await orgs.UpdateAsync((await user.GetAsync(ct)).Id, id, body, ct));

    public async Task<Results<NoContent, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> DeleteAsync(Guid id, CancellationToken ct) =>
        OpResultMap.NoContentNotFoundProblem(await orgs.DeleteAsync((await user.GetAsync(ct)).Id, id, ct));
}
