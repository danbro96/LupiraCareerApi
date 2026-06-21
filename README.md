# LupiraCareerApi

An event-sourced **career / portfolio** API — the system of record for your professional history:
**engagements** (employment, study, volunteering), **projects**, and **skills** with a dated maturity
timeline, plus their **goals**, **artifacts** (evidence), and **media**. It exposes both a REST API and
an [MCP](https://modelcontextprotocol.io) tool surface, so the same data is usable by web apps and by
AI agents.

Built with .NET 10 and [Marten](https://martendb.io) event sourcing on PostgreSQL.

- **Event-sourced** — engagements, projects, skills, goals, artifacts, and media are aggregates with a
  full event history; profiles, organizations, and identity are plain documents.
- **Multi-principal** — every record is owned by a principal; one deployment safely serves many users,
  and you only ever see your own data.
- **Fully authenticated** — every endpoint requires an authenticated caller (OIDC JWT).
  There are no anonymous business endpoints; what (if anything) is shown publicly is a decision for the
  consuming application, not this API.
- **Two transports, one core** — REST and MCP sit on the same application services, with identical
  validation and authorization.

See [docs/architecture.md](docs/architecture.md) for the domain model, a class diagram, and the
event-sourcing design.

## API surface

### REST (at root)

Resource-oriented and conventional:

- `GET`/`POST` on a collection; `GET` on an item; `PATCH` for partial field updates.
- `PUT`/`DELETE` for sub-resource membership (e.g. attach/detach a skill or an artifact/media link).
- `POST` sub-actions only for genuine event-sourced transitions — e.g. project `ship`/`shelve`/`archive`,
  skill `learnings`/`applications`/`deepenings`, goal `achieve`/`abandon`.
- `GET /resume` and `GET /experience` compose the read models into a résumé and a unified
  engagement+project timeline.
- Errors are returned as RFC 7807 `application/problem+json`.

The full contract is served by the running app:

- **OpenAPI document** — `GET /openapi/v1.json`
- **API reference UI** ([Scalar](https://scalar.com)) — `GET /scalar/v1`

### MCP (`/mcp`)

An MCP server exposing the career graph as agent tools, each scoped to the authenticated caller:
`list_engagements`, `create_engagement`, `list_projects`, `create_project`, `list_skills`,
`register_skill`, `record_skill_application`, `list_organizations`, `create_organization`, and
`get_resume`. Because the agent acts as a real principal, it is recommended to keep this surface on a
trusted network rather than the public internet.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker — for a local PostgreSQL and for the integration tests ([Testcontainers](https://testcontainers.com))

### Build & test

```bash
dotnet build LupiraCareerApi.slnx
dotnet test  LupiraCareerApi.slnx     # integration tests start a throwaway Postgres via Docker
```

### Run locally

Start a Postgres, point the app at it, apply the schema once, then run:

```bash
docker run --rm -d --name career-pg -e POSTGRES_PASSWORD=dev -e POSTGRES_DB=career -p 5432:5432 postgres:17

export ConnectionStrings__Postgres="Host=localhost;Port=5432;Database=career;Username=postgres;Password=dev"
export ASPNETCORE_ENVIRONMENT=Development          # enables the X-Dev-User dev auth below
export ASPNETCORE_URLS=http://localhost:8080

dotnet run --project src/LupiraCareerApi -- --apply-schema   # one-shot: create the `career` schema, then exit
dotnet run --project src/LupiraCareerApi                      # serve
```

In **Development** you can authenticate without an OIDC provider by passing an email in the
`X-Dev-User` header — the principal is JIT-provisioned on first use:

```bash
curl -H "X-Dev-User: me@example.com" http://localhost:8080/me
```

Then browse the API at `http://localhost:8080/scalar/v1`.

## Configuration

All configuration is via environment variables (or any standard .NET configuration source). Telemetry
is fully **off** unless an OTLP endpoint is set.

| Variable | Required | Purpose |
|---|---|---|
| `ConnectionStrings__Postgres` | yes | PostgreSQL connection string (Marten event store + documents). |
| `Auth__Authority` | yes (prod) | OIDC issuer/authority URL used to validate JWTs. |
| `Auth__Audience` | yes (prod) | Expected JWT audience. |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | no | OTLP endpoint for traces/metrics/logs. Unset ⇒ no telemetry exported. |
| `OTEL_EXPORTER_OTLP_PROTOCOL` | no | e.g. `http/protobuf`. |
| `OTEL_EXPORTER_OTLP_HEADERS` | no | Exporter headers (e.g. auth). |
| `ASPNETCORE_ENVIRONMENT` | no | `Development` enables the `X-Dev-User` header auth. |

### Authentication

In production the API validates **OIDC JWTs** against `Auth__Authority` / `Auth__Audience` — it is an
OAuth2/OIDC *resource server* and issues no tokens itself. It works with any compliant OIDC provider
(Keycloak, Authentik, Auth0, Entra ID, …). A `Principal` is provisioned on first request from the
token's `sub` (then `email`) claim.

## Database & schema

Marten owns the **`career`** schema (event store, snapshots, and projections). The schema is applied
**deliberately, not on boot** — run the app once with `--apply-schema` as a deploy step before serving
traffic. Most schema evolution is additive (new event types need no migration; a new projection just
adds a table).

## Deployment

The repo ships a multi-stage [`Dockerfile`](Dockerfile) (build on the .NET SDK image, run on the
ASP.NET runtime image, listening on `8080`) and an example Compose definition at
[`deploy/compose.yaml`](deploy/compose.yaml). Configure it with the environment variables above; the
hostnames/ports in that file are examples to override for your own environment.

Health probes:

- `GET /livez` — liveness (no dependency checks)
- `GET /readyz` — readiness (PostgreSQL reachable)

## CI

GitHub Actions ([`.github/workflows`](.github/workflows)):

- **ci.yml** — builds and runs the full unit + Testcontainers integration suite on every PR/branch.
- **release.yml** — on merge to `main` (or a `v*` tag) re-runs CI, then builds and pushes the container
  image to a registry.

## Project layout

- [`src/LupiraCareerApi.Core`](src/LupiraCareerApi.Core) — the bounded context: Domain
  (aggregates / events / projections / value objects), Application (services + `OpResult`), DTOs,
  mappers, Marten registration. **No ASP.NET dependency.**
- [`src/LupiraCareerApi`](src/LupiraCareerApi) — the host: auth, HTTP (RFC 7807), thin endpoints →
  handlers, the MCP server, health checks, `Program.cs`.
- [`tests`](tests) — `Core.Tests` (DB-free aggregate `Apply` logic) and `Server.Tests`
  (Testcontainers + `WebApplicationFactory`).

## License

[MIT](LICENSE).
