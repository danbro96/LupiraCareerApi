using System.Net;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class AuthTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Unauthenticated_request_is_rejected()
    {
        var anon = Factory.CreateClient(); // no X-Dev-User header
        var resp = await anon.GetAsync("/api/me");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Every_api_surface_requires_auth()
    {
        var anon = Factory.CreateClient();
        foreach (var path in new[] { "/api/organizations", "/api/engagements", "/api/projects", "/api/skills", "/api/goals", "/api/artifacts", "/api/media", "/api/resume", "/api/experience", "/api/profile" })
        {
            var resp = await anon.GetAsync(path);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }
    }
}
