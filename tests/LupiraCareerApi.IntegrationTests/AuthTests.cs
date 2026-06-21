using System.Net;
using Xunit;

namespace LupiraCareerApi.IntegrationTests;

public class AuthTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Unauthenticated_request_is_rejected()
    {
        var anon = Factory.CreateClient(); // no X-Dev-User header
        var resp = await anon.GetAsync("/me");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Every_api_surface_requires_auth()
    {
        var anon = Factory.CreateClient();
        foreach (var path in new[] { "/organizations", "/engagements", "/projects", "/skills", "/goals", "/artifacts", "/media", "/resume", "/experience", "/profile" })
        {
            var resp = await anon.GetAsync(path);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }
    }
}
