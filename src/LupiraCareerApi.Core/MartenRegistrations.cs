using JasperFx.Events.Projections;
using LupiraCareerApi.Domain;
using Marten;
using Weasel.Core;

namespace LupiraCareerApi;

/// <summary>Configures the single Marten store for the Career API: event-sourced aggregates (inline snapshots),
/// derived read models, and plain documents (identity, profile, organizations), all in the <c>career</c> schema.
/// Enums serialize as strings.</summary>
public static class MartenRegistrations
{
    public static StoreOptions UseLupiraCareer(this StoreOptions opts)
    {
        opts.DatabaseSchemaName = "career";
        opts.UseSystemTextJsonForSerialization(EnumStorage.AsString);

        // Provenance stamped on every event — unbackfillable, so captured at write time (see
        // PrincipalDirectory.StampSession). Correlation = OTel TraceId, causation = OTel SpanId (this API
        // isn't offline-first, so there's no command id), headers = actor.email + source, username =
        // acting principal id (LastModifiedBy).
        opts.Events.MetadataConfig.CorrelationIdEnabled = true;
        opts.Events.MetadataConfig.CausationIdEnabled = true;
        opts.Events.MetadataConfig.HeadersEnabled = true;
        opts.Events.MetadataConfig.UserNameEnabled = true;

        // Stable, explicit event names decoupled from CLR type names, so the classes can be renamed/moved
        // freely. Each alias equals Marten's current snake_case default, so pinning them changes nothing in
        // storage — it freezes the contract. Never re-map a name once events of it exist in a live store
        // (it would orphan them); evolve payloads via a new versioned type + upcaster.
        MapEvents(opts);

        // Event-sourced aggregates (resource read models) — inline for read-your-write.
        opts.Projections.Snapshot<Engagement>(SnapshotLifecycle.Inline);
        opts.Projections.Snapshot<Project>(SnapshotLifecycle.Inline);
        opts.Projections.Snapshot<Skill>(SnapshotLifecycle.Inline);
        opts.Projections.Snapshot<Goal>(SnapshotLifecycle.Inline);
        opts.Projections.Snapshot<Artifact>(SnapshotLifecycle.Inline);
        opts.Projections.Snapshot<MediaAsset>(SnapshotLifecycle.Inline);

        // Derived read models (the reverse-link views are served via query-time Contains() in the services).
        opts.Projections.Add<SkillTimelineProjection>(ProjectionLifecycle.Inline);
        opts.Projections.Add<SkillMaturityProjection>(ProjectionLifecycle.Inline);
        opts.Projections.Add<ExperienceProjection>(ProjectionLifecycle.Inline);

        // Plain documents (identity, profile, organizations) + the indexes the services query by.
        opts.Schema.For<Principal>().Index(x => x.AuthentikSub).Index(x => x.Email);
        opts.Schema.For<Profile>().Index(x => x.OwnerPrincipalId).Index(x => x.PublicHandle, idx => idx.IsUnique = true);
        opts.Schema.For<Organization>().Index(x => x.OwnerPrincipalId);

        return opts;
    }

    /// <summary>
    /// Pins every event's durable storage alias to its current snake_case default, so this is a no-op for
    /// existing data and a contract going forward: the CLR record may be renamed/relocated, and a breaking
    /// shape change becomes a new versioned type mapped to the same alias plus an upcaster.
    /// </summary>
    private static void MapEvents(StoreOptions opts)
    {
        // Engagement stream
        opts.Events.MapEventType<EngagementStarted>("engagement_started");
        opts.Events.MapEventType<EngagementEnded>("engagement_ended");
        opts.Events.MapEventType<EngagementSummaryRevised>("engagement_summary_revised");
        opts.Events.MapEventType<EngagementRelocated>("engagement_relocated");
        opts.Events.MapEventType<EngagementKindReclassified>("engagement_kind_reclassified");
        opts.Events.MapEventType<TitleAssumed>("title_assumed");
        opts.Events.MapEventType<TitleRevised>("title_revised");
        opts.Events.MapEventType<TitleRetired>("title_retired");
        opts.Events.MapEventType<EngagementSkillAttached>("engagement_skill_attached");
        opts.Events.MapEventType<EngagementSkillDetached>("engagement_skill_detached");

        // Project stream
        opts.Events.MapEventType<ProjectStarted>("project_started");
        opts.Events.MapEventType<ProjectRenamed>("project_renamed");
        opts.Events.MapEventType<ProjectDescribed>("project_described");
        opts.Events.MapEventType<ProjectUrlSet>("project_url_set");
        opts.Events.MapEventType<ProjectAttachedToEngagement>("project_attached_to_engagement");
        opts.Events.MapEventType<ProjectDetachedFromEngagement>("project_detached_from_engagement");
        opts.Events.MapEventType<ProjectShipped>("project_shipped");
        opts.Events.MapEventType<ProjectShelved>("project_shelved");
        opts.Events.MapEventType<ProjectArchived>("project_archived");
        opts.Events.MapEventType<ProjectSkillAttached>("project_skill_attached");
        opts.Events.MapEventType<ProjectSkillDetached>("project_skill_detached");

        // Skill stream
        opts.Events.MapEventType<SkillRegistered>("skill_registered");
        opts.Events.MapEventType<SkillRenamed>("skill_renamed");
        opts.Events.MapEventType<SkillCategoryChanged>("skill_category_changed");
        opts.Events.MapEventType<SkillAliasAdded>("skill_alias_added");
        opts.Events.MapEventType<SkillReparented>("skill_reparented");
        opts.Events.MapEventType<SkillRetired>("skill_retired");
        opts.Events.MapEventType<SkillLearned>("skill_learned");
        opts.Events.MapEventType<SkillApplied>("skill_applied");
        opts.Events.MapEventType<SkillDeepened>("skill_deepened");
        opts.Events.MapEventType<SkillTaught>("skill_taught");
        opts.Events.MapEventType<SkillReferenced>("skill_referenced");
        opts.Events.MapEventType<SkillsCombined>("skills_combined");

        // Goal stream
        opts.Events.MapEventType<GoalSet>("goal_set");
        opts.Events.MapEventType<GoalRescoped>("goal_rescoped");
        opts.Events.MapEventType<GoalProgressRecorded>("goal_progress_recorded");
        opts.Events.MapEventType<GoalAchieved>("goal_achieved");
        opts.Events.MapEventType<GoalAbandoned>("goal_abandoned");

        // Artifact stream
        opts.Events.MapEventType<ArtifactRegistered>("artifact_registered");
        opts.Events.MapEventType<ArtifactUpdated>("artifact_updated");
        opts.Events.MapEventType<ArtifactLinkedToProject>("artifact_linked_to_project");
        opts.Events.MapEventType<ArtifactLinkedToSkill>("artifact_linked_to_skill");
        opts.Events.MapEventType<ArtifactLinkedToEngagement>("artifact_linked_to_engagement");
        opts.Events.MapEventType<ArtifactUnlinked>("artifact_unlinked");
        opts.Events.MapEventType<ArtifactArchived>("artifact_archived");

        // MediaAsset stream
        opts.Events.MapEventType<MediaRegistered>("media_registered");
        opts.Events.MapEventType<MediaLinkedToProject>("media_linked_to_project");
        opts.Events.MapEventType<MediaLinkedToSkill>("media_linked_to_skill");
        opts.Events.MapEventType<MediaUnlinked>("media_unlinked");
        opts.Events.MapEventType<MediaReplaced>("media_replaced");
        opts.Events.MapEventType<MediaArchived>("media_archived");
    }
}
