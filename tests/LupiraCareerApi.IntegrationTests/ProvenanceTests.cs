using LupiraCareerApi.Domain;
using Marten;
using Xunit;

namespace LupiraCareerApi.IntegrationTests;

/// <summary>
/// Provenance stamping moved out of <c>PrincipalDirectory</c> and onto the caller's own resolution site
/// (<c>CurrentUser</c>), so a plain identity lookup no longer mutates session state. Provenance is
/// unbackfillable, so it needs a test proving it still reaches the event store.
/// </summary>
public class ProvenanceTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Events_carry_the_calling_principal_as_actor()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api, "Strivo AB");
        var engagement = await CreateEngagementAsync(api, org.Id);

        await using var q = Factory.Store.QuerySession();
        var events = await q.Events.FetchStreamAsync(engagement.Id);
        Assert.NotEmpty(events);
        Assert.Equal("anna@strivo.se", events[0].Headers?[EventActor.EmailHeaderKey]);
        Assert.Equal(EventActor.SourceApi, events[0].Headers?[EventActor.SourceHeaderKey]);
    }

    [Fact]
    public async Task Resolving_a_principal_does_not_restamp_the_session()
    {
        await using var session = Factory.Store.LightweightSession();
        var directory = new LupiraCareerApi.Application.PrincipalDirectory(session);

        var caller = await directory.ResolveOrProvisionAsync("sub-caller", "caller@x.test", "Caller");
        EventActor.Stamp(session, caller, EventActor.SourceApi);

        await directory.ResolveOrProvisionAsync(null, "someone-else@x.test", null);

        Assert.Equal(caller.Id.ToString(), session.LastModifiedBy);
        Assert.Equal(caller.Email, session.GetHeader(EventActor.EmailHeaderKey));
        Assert.Equal(EventActor.SourceApi, session.GetHeader(EventActor.SourceHeaderKey));
    }
}
