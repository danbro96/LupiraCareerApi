using Marten;
using System.Diagnostics;

namespace LupiraCareerApi.Domain;

/// <summary>
/// Stamps event provenance onto the write session before its commit, so every event and document written in
/// this unit of work carries it: the acting principal (Marten <c>LastModifiedBy</c>), their email
/// (<c>actor.email</c>), the writing surface (<c>source</c>), and the ambient OTel trace/span as
/// correlation/causation. All of it is unbackfillable, which is why it is stamped on every request.
///
/// Deliberately separate from <c>PrincipalDirectory</c>: resolving an identity must not mutate session state,
/// so only the caller's own resolution site stamps.
/// </summary>
public static class EventActor
{
    public const string EmailHeaderKey = "actor.email";
    public const string SourceHeaderKey = "source";

    /// <summary>The writing surface. An email-only login (no OIDC sub) did not arrive over REST.</summary>
    public const string SourceApi = "api";
    public const string SourceDav = "dav";

    public static void Stamp(IDocumentSession session, Principal principal, string source)
    {
        session.LastModifiedBy = principal.Id.ToString();
        session.SetHeader(EmailHeaderKey, principal.Email);
        session.SetHeader(SourceHeaderKey, source);
        if (Activity.Current is { } a)
        {
            session.CorrelationId = a.TraceId.ToString();
            session.CausationId = a.SpanId.ToString();
        }
    }
}
