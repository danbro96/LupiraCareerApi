using JasperFx.Events.Projections;
using LupiraCareerApi.Domain;
using Marten;
using Weasel.Core;

namespace LupiraCareerApi;

/// <summary>Configures the single Marten store for the Career API: event-sourced aggregates (inline snapshots),
/// derived read models, and plain documents (identity, profile, organizations), all in the <c>career</c> schema.
/// Enums serialize as strings. Mirrors the LupiraCalApi pattern.</summary>
public static class MartenRegistrations
{
    public static StoreOptions UseLupiraCareer(this StoreOptions opts)
    {
        opts.DatabaseSchemaName = "career";
        opts.UseSystemTextJsonForSerialization(EnumStorage.AsString);

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
        opts.Schema.For<Profile>().Index(x => x.OwnerPrincipalId);
        opts.Schema.For<Organization>().Index(x => x.OwnerPrincipalId);

        return opts;
    }
}
