using LupiraCareerApi.Core.Domain.Engagements.Events;
using LupiraCareerApi.Core.Domain.Projects.Events;
using Marten.Events.Projections;

namespace LupiraCareerApi.Core.Domain.Experiences;

public sealed partial class ExperienceProjection : MultiStreamProjection<ExperienceRow, Guid>
{
    public ExperienceProjection()
    {
        Identity<EngagementStarted>(e => e.EngagementId);
        Identity<EngagementEnded>(e => e.EngagementId);
        Identity<EngagementRelocated>(e => e.EngagementId);
        Identity<EngagementSkillAttached>(e => e.EngagementId);
        Identity<EngagementSkillDetached>(e => e.EngagementId);

        Identity<ProjectStarted>(e => e.ProjectId);
        Identity<ProjectShipped>(e => e.ProjectId);
        Identity<ProjectShelved>(e => e.ProjectId);
        Identity<ProjectSkillAttached>(e => e.ProjectId);
        Identity<ProjectSkillDetached>(e => e.ProjectId);
    }

    public ExperienceRow Create(EngagementStarted e) => new()
    {
        Id = e.EngagementId,
        OwnerPrincipalId = e.OwnerPrincipalId,
        Kind = ExperienceKind.Engagement,
        OccurredOn = e.StartDate,
        EngagementId = e.EngagementId,
        OrganizationId = e.OrganizationId,
        Location = e.Location,
    };

    public ExperienceRow Create(ProjectStarted e) => new()
    {
        Id = e.ProjectId,
        OwnerPrincipalId = e.OwnerPrincipalId,
        Kind = ExperienceKind.Project,
        Title = e.Title,
        OccurredOn = e.StartDate ?? DateOnly.MinValue,
        EngagementId = e.EngagementId,
        ProjectId = e.ProjectId,
    };

    public void Apply(EngagementEnded e, ExperienceRow row) => row.EndDate = e.EndDate;

    public void Apply(EngagementRelocated e, ExperienceRow row) => row.Location = e.NewLocation;

    public void Apply(EngagementSkillAttached e, ExperienceRow row)
    {
        if (!row.SkillIds.Contains(e.SkillId))
            row.SkillIds.Add(e.SkillId);
    }

    public void Apply(EngagementSkillDetached e, ExperienceRow row) =>
        row.SkillIds.Remove(e.SkillId);

    public void Apply(ProjectShipped e, ExperienceRow row) => row.EndDate = e.ShippedOn;

    public void Apply(ProjectSkillAttached e, ExperienceRow row)
    {
        if (!row.SkillIds.Contains(e.SkillId))
            row.SkillIds.Add(e.SkillId);
    }

    public void Apply(ProjectSkillDetached e, ExperienceRow row) =>
        row.SkillIds.Remove(e.SkillId);
}
