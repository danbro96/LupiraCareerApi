using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LupiraCareerApi.Server.Tests;

public class OrganizationsTests(CareerApiTestFactory f) : IntegrationTest(f)
{
    [Fact]
    public async Task Create_with_blank_name_is_rejected()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var resp = await api.PostAsJsonAsync("/organizations", new CreateOrganizationRequest { Name = "  ", Kind = OrganizationKind.Company, Url = null, CalContactGroupRef = null }, TestJson.Options);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Create_list_and_get()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api, "Strivo AB");

        var list = await api.GetFromJsonAsync<List<OrganizationDto>>("/organizations", TestJson.Options);
        Assert.Contains(list!, o => o.Id == org.Id && o.Name == "Strivo AB");

        var got = await api.GetFromJsonAsync<OrganizationDto>($"/organizations/{org.Id}", TestJson.Options);
        Assert.Equal("Strivo AB", got!.Name);
        Assert.Equal(OrganizationKind.Company, got.Kind);
    }

    [Fact]
    public async Task List_is_name_ordered()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        await CreateOrganizationAsync(api, "Zeta");
        await CreateOrganizationAsync(api, "Alpha");
        var list = await api.GetFromJsonAsync<List<OrganizationDto>>("/organizations", TestJson.Options);
        Assert.Equal("Alpha", list![0].Name);
        Assert.Equal("Zeta", list[1].Name);
    }

    [Fact]
    public async Task Patch_reflects_name_kind_and_url()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api, "Strivo");
        var resp = await api.PatchAsJsonAsync($"/organizations/{org.Id}",
            new UpdateOrganizationRequest { Name = "Strivo AB", Kind = OrganizationKind.Nonprofit, Url = "https://strivo.se", CalContactGroupRef = null }, TestJson.Options);
        resp.EnsureSuccessStatusCode();
        var updated = await resp.Content.ReadFromJsonAsync<OrganizationDto>(TestJson.Options);
        Assert.Equal("Strivo AB", updated!.Name);
        Assert.Equal(OrganizationKind.Nonprofit, updated.Kind);
        Assert.Equal("https://strivo.se", updated.Url);
    }

    [Fact]
    public async Task Get_and_patch_missing_organization_is_404()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var missing = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.NotFound, (await api.GetAsync($"/organizations/{missing}")).StatusCode);
        var patch = await api.PatchAsJsonAsync($"/organizations/{missing}", new UpdateOrganizationRequest { Name = "X", Kind = null, Url = null, CalContactGroupRef = null }, TestJson.Options);
        Assert.Equal(HttpStatusCode.NotFound, patch.StatusCode);
    }

    [Fact]
    public async Task Delete_unused_organization_then_get_is_404()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api);
        var del = await api.DeleteAsync($"/organizations/{org.Id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await api.GetAsync($"/organizations/{org.Id}")).StatusCode);
    }

    [Fact]
    public async Task Delete_organization_in_use_conflicts()
    {
        var api = Factory.ApiClient("anna@strivo.se");
        var org = await CreateOrganizationAsync(api);
        await CreateEngagementAsync(api, org.Id);
        var resp = await api.DeleteAsync($"/organizations/{org.Id}");
        Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
    }
}
