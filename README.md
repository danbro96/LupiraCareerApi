# LupiraCareerApi

Event-sourced **career / portfolio** service — the system of record for projects, employments (engagements), and skills, extracted from LupiraWeb's backend into a standalone .NET API. Exposes a fully-authenticated REST surface plus an MCP tool surface for the agent. LupiraWeb consumes this API and decides what to present publicly; the API itself has **no anonymous endpoints**.

## At a glance
| | |
|---|---|
| Domain → port | `https://career.lupira.com` → `:41080` (REST; `/api/mcp` is LAN/WireGuard-only, **not** tunneled) |
| Data | `medelynas-db`, Marten event store + documents, schema `career` |
| Auth | Authentik OIDC (JWT). Every endpoint requires an authenticated principal. |
| Identity | A `Principal` resolved JIT from the Authentik `sub` (+ email) — the **same anchor** the calendar API uses. |
| Repo | 2 projects: `LupiraCareerApi.Core` (bounded context) + `LupiraCareerApi` (host) |

## Domain
Event-sourced aggregates (inline snapshots): **Engagement**, **Project**, **Skill**, **Goal**, **Artifact**, **MediaAsset**. Plain documents: **Principal**, **Profile** (per-principal "about me"), **Organization** (employer of record). Derived inline read models: `SkillTimeline`, `SkillMaturity`, `Experience` (a unified engagement+project timeline). Reverse-link views (artifacts/media for a project/skill/engagement) are served by query-time `Contains()` rather than dedicated projections.

Every aggregate carries an `OwnerPrincipalId`; reads and writes are scoped to the owner and a non-owned id reads as `404` (its existence is not leaked) — this is a **multi-principal** store.

## Surfaces
- **REST** (`/api/**`) — resource-oriented: `GET/POST` collections, `GET` items, `PATCH` for partial field updates, `PUT`/`DELETE` for sub-resource membership (skills, artifact/media links), and `POST` sub-actions only for genuine event-sourced transitions (`ship`/`shelve`/`archive`, skill `learnings`/`applications`/`deepenings`, goal `achieve`/`abandon`). `GET /api/resume` and `GET /api/experience` compose the read models.
- **MCP** (`/api/mcp`, LAN-only) — `CareerTools`: list/create engagements, projects, skills, organizations; record a skill application; get the résumé. Delegates to the same Core services as REST.

## Border with the calendar API
The calendar API is authoritative on *"what I was invited to / attended / did"*; this API is authoritative on *"who I am professionally over time and what I can do."* They share only the Authentik `sub` (no shared tables). The calendar API links **into** career entities via its generic `Relation(toKind="project"|"engagement"|"skill")`; this API references calendar entities only softly, through skill `Evidence`/`SkillEdgeContext.ExternalUrl`. Organizations exist in both (here = employer of record; there = a contact group of people) and may cross-link, but neither is master of the other.

## Build & test
```
dotnet build LupiraCareerApi.slnx
dotnet test  LupiraCareerApi.slnx        # Server.Tests need Docker (Testcontainers Postgres)
```
Apply the schema as a one-shot deploy step: `dotnet LupiraCareerApi.dll --apply-schema`.

## Layout
- `src/LupiraCareerApi.Core` — Domain (aggregates/events/projections/value objects), Application (services + `OpResult`), Dtos, Mappers, `MartenRegistrations`. No ASP.NET dependency.
- `src/LupiraCareerApi` — host: Auth, Http (RFC 7807), Endpoints (thin) → Handlers, Mcp, Health, `Program.cs`.
- `tests/` — `Core.Tests` (DB-free `Apply` logic) + `Server.Tests` (Testcontainers + `WebApplicationFactory`).
