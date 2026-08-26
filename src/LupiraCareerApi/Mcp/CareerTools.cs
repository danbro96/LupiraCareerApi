using System.ComponentModel;
using LupiraCareerApi.Auth;
using LupiraCareerApi.Core.Application;
using LupiraCareerApi.Core.Application.Results;
using LupiraCareerApi.Core.Dtos;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace LupiraCareerApi.Mcp;

/// <summary>
/// The agent's MCP tool surface, mounted at /mcp. Each tool resolves the caller via <see cref="CurrentUser"/>
/// and delegates to the same Core services as REST, so everything is scoped to the caller's own career graph.
/// Non-Ok outcomes surface as a structured <see cref="McpException"/> tool error.
/// </summary>
[McpServerToolType]
public sealed class CareerTools
{
    [McpServerTool(Name = "list_engagements")]
    [Description("List the caller's engagements (employment/study/…).")]
    public static async Task<IReadOnlyList<EngagementDto>> ListEngagements(EngagementService engagements, CurrentUser user) =>
        Require(await engagements.ListAsync((await user.GetAsync()).Id));

    [McpServerTool(Name = "create_engagement")]
    [Description("Create an engagement under one of the caller's organizations.")]
    public static async Task<EngagementDto> CreateEngagement(EngagementService engagements, CurrentUser user, CreateEngagementRequest request) =>
        Require(await engagements.CreateAsync((await user.GetAsync()).Id, request));

    [McpServerTool(Name = "list_projects")]
    [Description("List the caller's projects, optionally filtered to one engagement.")]
    public static async Task<IReadOnlyList<ProjectDto>> ListProjects(
        ProjectService projects, CurrentUser user,
        [Description("Restrict to projects under this engagement id.")] Guid? engagementId = null) =>
        Require(await projects.ListAsync((await user.GetAsync()).Id, engagementId));

    [McpServerTool(Name = "create_project")]
    [Description("Create a project (optionally filed under an engagement).")]
    public static async Task<ProjectDto> CreateProject(ProjectService projects, CurrentUser user, CreateProjectRequest request) =>
        Require(await projects.CreateAsync((await user.GetAsync()).Id, request));

    [McpServerTool(Name = "list_skills")]
    [Description("List the caller's skills with their current maturity.")]
    public static async Task<IReadOnlyList<SkillDto>> ListSkills(SkillService skills, CurrentUser user) =>
        Require(await skills.ListAsync((await user.GetAsync()).Id));

    [McpServerTool(Name = "create_skill")]
    [Description("Register a new skill.")]
    public static async Task<SkillDto> CreateSkill(SkillService skills, CurrentUser user, RegisterSkillRequest request) =>
        Require(await skills.RegisterAsync((await user.GetAsync()).Id, request));

    [McpServerTool(Name = "record_skill_application")]
    [Description("Record that the caller applied a skill on a date, in some context (logs a SkillApplied edge).")]
    public static async Task<SkillDto> RecordSkillApplication(
        SkillService skills, CurrentUser user,
        [Description("The skill id.")] Guid skillId,
        ApplySkillRequest request) =>
        Require(await skills.ApplyAsync((await user.GetAsync()).Id, skillId, request));

    [McpServerTool(Name = "list_organizations")]
    [Description("List the caller's organizations (employers/institutions).")]
    public static async Task<IReadOnlyList<OrganizationDto>> ListOrganizations(OrganizationService orgs, CurrentUser user) =>
        Require(await orgs.ListAsync((await user.GetAsync()).Id));

    [McpServerTool(Name = "create_organization")]
    [Description("Create an organization (employer/institution).")]
    public static async Task<OrganizationDto> CreateOrganization(OrganizationService orgs, CurrentUser user, CreateOrganizationRequest request) =>
        Require(await orgs.CreateAsync((await user.GetAsync()).Id, request));

    [McpServerTool(Name = "get_resume")]
    [Description("Get the caller's full composed résumé (profile + engagements + projects + skills).")]
    public static async Task<ResumeDto> GetResume(ResumeService resume, CurrentUser user) =>
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
