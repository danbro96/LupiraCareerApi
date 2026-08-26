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
    [Description("List every engagement on the caller's career graph — the spans of Employment, Study, Hobby, " +
        "Volunteer or OpenSource work, each hanging off one organization. Returns the whole set with no filter, " +
        "so use it to orient before drilling in; each entry carries its start/end dates (a null End means " +
        "ongoing), its title history, and the ids of the skills exercised there. It does not return the projects " +
        "filed under an engagement — call list_projects with that engagement id for those.")]
    public static async Task<IReadOnlyList<EngagementDto>> ListEngagements(EngagementService engagements, CurrentUser user) =>
        Require(await engagements.ListAsync((await user.GetAsync()).Id));

    [McpServerTool(Name = "create_engagement")]
    [Description("Record a new engagement — one span of Employment, Study, Hobby, Volunteer or OpenSource work — " +
        "under an organization the caller already has. The organization must exist first, so call " +
        "list_organizations to find its id or create_organization to add it. Leave the end date unset for " +
        "something still ongoing. Use this for the engagement itself, not for the work done inside it: individual " +
        "pieces of work are projects (create_project).")]
    public static async Task<EngagementDto> CreateEngagement(EngagementService engagements, CurrentUser user, CreateEngagementRequest request) =>
        Require(await engagements.CreateAsync((await user.GetAsync()).Id, request));

    [McpServerTool(Name = "list_projects")]
    [Description("List the caller's projects — discrete pieces of work, each Professional, Personal, OpenSource " +
        "or Academic, and each Active, Shipped, Shelved or Archived. Pass an engagement id to see only the work " +
        "done during that job or course; omit it for every project including standalone ones that belong to no " +
        "engagement. Each entry carries its status, dates, outcome, optional URL and the ids of the skills it " +
        "exercised.")]
    public static async Task<IReadOnlyList<ProjectDto>> ListProjects(
        ProjectService projects, CurrentUser user,
        [Description("Restrict to projects under this engagement id.")] Guid? engagementId = null) =>
        Require(await projects.ListAsync((await user.GetAsync()).Id, engagementId));

    [McpServerTool(Name = "create_project")]
    [Description("Record a discrete piece of work as a project — Professional, Personal, OpenSource or Academic. " +
        "File it under an engagement when it was done as part of a job or course; leave the engagement unset for " +
        "standalone work such as a side project. Status tracks its life (Active while in flight, then Shipped, " +
        "Shelved or Archived), and the outcome field is the place for what it actually achieved.")]
    public static async Task<ProjectDto> CreateProject(ProjectService projects, CurrentUser user, CreateProjectRequest request) =>
        Require(await projects.CreateAsync((await user.GetAsync()).Id, request));

    [McpServerTool(Name = "list_skills")]
    [Description("List every skill on the caller's graph with the maturity it has reached — Aware, Working, " +
        "Fluent, Expert or Teaching. Skills are categorised (Language, Framework, Tool, Platform, Method, Domain, " +
        "Other), may nest under a parent skill, and carry aliases so the same thing found under another name " +
        "resolves to one entry. Retired skills are included and flagged, so filter on that when you only want the " +
        "live set. Maturity here is the current standing only — it does not return the history of how it got " +
        "there.")]
    public static async Task<IReadOnlyList<SkillDto>> ListSkills(SkillService skills, CurrentUser user) =>
        Require(await skills.ListAsync((await user.GetAsync()).Id));

    [McpServerTool(Name = "create_skill")]
    [Description("Add a skill to the caller's graph, categorised as a Language, Framework, Tool, Platform, " +
        "Method, Domain or Other. Give aliases for the other names the same skill goes by so later mentions " +
        "resolve to this one entry, and set a parent skill to nest a specific under a general one (a framework " +
        "under its language, say). Check list_skills first — a skill the caller already has, possibly under an " +
        "alias, should not be created twice. This only registers the skill; use record_skill_application to log " +
        "actually using it, which is what moves its maturity.")]
    public static async Task<SkillDto> CreateSkill(SkillService skills, CurrentUser user, RegisterSkillRequest request) =>
        Require(await skills.RegisterAsync((await user.GetAsync()).Id, request));

    [McpServerTool(Name = "record_skill_application")]
    [Description("Log one occasion of the caller actually using a skill, on a date and at an intensity, in some " +
        "context (an engagement or project). This appends a SkillApplied edge to the skill's history rather than " +
        "overwriting anything, so call it once per occasion — repeated use is what builds the evidence trail and " +
        "moves the skill's maturity. Optional evidence and location ride along on the edge. Registering a skill " +
        "(create_skill) does not imply having applied it.")]
    public static async Task<SkillDto> RecordSkillApplication(
        SkillService skills, CurrentUser user,
        [Description("The skill id.")] Guid skillId,
        ApplySkillRequest request) =>
        Require(await skills.ApplyAsync((await user.GetAsync()).Id, skillId, request));

    [McpServerTool(Name = "list_organizations")]
    [Description("List the organizations on the caller's graph — the Companies, Schools, Nonprofits and Others " +
        "that engagements hang off. Returns the whole set unfiltered; call it to find the id you need before " +
        "creating an engagement. Each entry carries its kind and optional URL. It does not return the engagements " +
        "at each organization — use list_engagements for those.")]
    public static async Task<IReadOnlyList<OrganizationDto>> ListOrganizations(OrganizationService orgs, CurrentUser user) =>
        Require(await orgs.ListAsync((await user.GetAsync()).Id));

    [McpServerTool(Name = "create_organization")]
    [Description("Add an organization — a Company, School, Nonprofit or Other — so engagements can hang off it. " +
        "This is the first step when recording a job or course at a place the caller has not logged before; " +
        "check list_organizations first to avoid a duplicate. Creating one records only the organization, not any " +
        "engagement there: follow with create_engagement for the actual span of work or study.")]
    public static async Task<OrganizationDto> CreateOrganization(OrganizationService orgs, CurrentUser user, CreateOrganizationRequest request) =>
        Require(await orgs.CreateAsync((await user.GetAsync()).Id, request));

    [McpServerTool(Name = "get_resume")]
    [Description("Get the caller's résumé composed in one call: the profile header plus their engagements, " +
        "projects and skills, already assembled with organization names resolved. Use this instead of calling " +
        "list_engagements, list_projects and list_skills separately whenever the goal is a whole CV or a summary " +
        "of the career. Scope is everything the caller owns, so it is a superset view rather than a filtered one; " +
        "reach for the individual list tools when you want one slice or need to filter projects by engagement.")]
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
