using LupiraCareerApi.Application;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Dtos;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LupiraCareerApi.Mcp;

/// <summary>
/// The agent's MCP tool surface, mounted at /mcp. Each tool resolves the caller via <see cref="CurrentUser"/>
/// and delegates to the same Core services as REST, so everything is scoped to the caller's own career graph.
/// Non-Ok outcomes surface as a structured <see cref="McpException"/> tool error.
/// </summary>
[McpServerToolType]
public sealed class CareerTools
{
    [McpServerTool, Description("List the caller's engagements (employment/study/…).")]
    public static async Task<IReadOnlyList<EngagementDto>> list_engagements(EngagementService engagements, CurrentUser user) =>
        Require(await engagements.ListAsync((await user.GetAsync()).Id));

    [McpServerTool, Description("Create an engagement under one of the caller's organizations.")]
    public static async Task<EngagementDto> create_engagement(EngagementService engagements, CurrentUser user, CreateEngagementRequest request) =>
        Require(await engagements.CreateAsync((await user.GetAsync()).Id, request));

    [McpServerTool, Description("List the caller's projects, optionally filtered to one engagement.")]
    public static async Task<IReadOnlyList<ProjectDto>> list_projects(
        ProjectService projects, CurrentUser user,
        [Description("Restrict to projects under this engagement id.")] Guid? engagementId = null) =>
        Require(await projects.ListAsync((await user.GetAsync()).Id, engagementId));

    [McpServerTool, Description("Create a project (optionally filed under an engagement).")]
    public static async Task<ProjectDto> create_project(ProjectService projects, CurrentUser user, CreateProjectRequest request) =>
        Require(await projects.CreateAsync((await user.GetAsync()).Id, request));

    [McpServerTool, Description("List the caller's skills with their current maturity.")]
    public static async Task<IReadOnlyList<SkillDto>> list_skills(SkillService skills, CurrentUser user) =>
        Require(await skills.ListAsync((await user.GetAsync()).Id));

    [McpServerTool, Description("Register a new skill.")]
    public static async Task<SkillDto> register_skill(SkillService skills, CurrentUser user, RegisterSkillRequest request) =>
        Require(await skills.RegisterAsync((await user.GetAsync()).Id, request));

    [McpServerTool, Description("Record that the caller applied a skill on a date, in some context (logs a SkillApplied edge).")]
    public static async Task<SkillDto> record_skill_application(
        SkillService skills, CurrentUser user,
        [Description("The skill id.")] Guid skillId,
        ApplySkillRequest request) =>
        Require(await skills.ApplyAsync((await user.GetAsync()).Id, skillId, request));

    [McpServerTool, Description("List the caller's organizations (employers/institutions).")]
    public static async Task<IReadOnlyList<OrganizationDto>> list_organizations(OrganizationService orgs, CurrentUser user) =>
        Require(await orgs.ListAsync((await user.GetAsync()).Id));

    [McpServerTool, Description("Create an organization (employer/institution).")]
    public static async Task<OrganizationDto> create_organization(OrganizationService orgs, CurrentUser user, CreateOrganizationRequest request) =>
        Require(await orgs.CreateAsync((await user.GetAsync()).Id, request));

    [McpServerTool, Description("Get the caller's full composed résumé (profile + engagements + projects + skills).")]
    public static async Task<ResumeDto> get_resume(ResumeService resume, CurrentUser user) =>
        Require(await resume.GetResumeAsync((await user.GetAsync()).Id));

    /// <summary>Unwraps a service outcome to its value, surfacing non-Ok statuses as an MCP tool error.</summary>
    private static T Require<T>(OpResult<T> r) => r.Status switch
    {
        OpStatus.Ok => r.Value!,
        OpStatus.NotFound => throw new McpException("Not found."),
        OpStatus.Forbidden => throw new McpException(r.Error ?? "Forbidden."),
        OpStatus.Invalid => throw new McpException(r.Error ?? "Invalid request."),
        OpStatus.Conflict => throw new McpException(r.Error ?? "Conflict."),
        _ => throw new McpException("Unexpected result."),
    };
}
