# Plan & Checklist (source: learn_dev_ops.md)

Legend: `[x]` done and verified here · `[~]` written but cannot be fully verified on this machine (reason noted) · `[ ]` not done

## Environment findings
| Tool | Status |
|---|---|
| .NET | SDK 10 preview + .NET 8 runtime; project targets `net8.0` |
| Docker Desktop | installed |
| Terraform / Ansible / gh CLI | checked at start, see per-phase notes |

## Phases

### Phase 1 – .NET 8 API + PostgreSQL
- [ ] Solution + `src/ProductApi` (Controllers, Services, Repositories, Data, Models, DTOs, Program.cs)
- [ ] Product model (Id, Name, Description, Price, StockQuantity, IsActive, CreatedAt, UpdatedAt)
- [ ] CRUD endpoints (GET list, GET id, POST, PUT, DELETE)
- [ ] `/health` endpoint
- [ ] Swagger (dev)
- [ ] Connection string from `ConnectionStrings__DefaultConnection` env var
- [ ] EF Core migration + auto-migrate at startup
- [ ] Verify against real PostgreSQL

### Phase 2 – Unit tests (xUnit)
- [ ] create / retrieve / validation / update / delete / business logic tests
- [ ] `dotnet test` passes

### Phase 3 – Dockerfile
- [ ] Multi-stage (SDK build → ASP.NET runtime), port 8080, `docker/Dockerfile`
- [ ] `.dockerignore`
- [ ] Image builds

### Phase 4 – Docker Compose
- [ ] `api` (product-api:8080) + `postgres` (product-postgres:5432)
- [ ] volume, network, `pg_isready` healthcheck, `depends_on: service_healthy`, API `/health` healthcheck
- [ ] env vars via `.env.example` (API_PORT, POSTGRES_*, IMAGE_TAG)
- [ ] `docker compose up -d` works, data persists

### Phase 5 – Terraform (Docker provider)
- [ ] versions.tf, variables.tf, main.tf, outputs.tf (network, volume, containers)
- [ ] init / validate / plan / apply / destroy

### Phase 6 – Ansible (local connection)
- [ ] inventory.ini, playbook.yml, group_vars/all.yml, templates/docker-compose.yml.j2
- [ ] checks Docker, creates dirs, renders compose, pulls image, starts, verifies API
- [ ] idempotent; `IMAGE_TAG` override for rollback

### Phase 7 – Docker Hub
- [ ] Image naming `<user>/product-api:<tag>` (latest + git sha)
- [ ] Push/pull documented; no credentials in repo

### Phase 8 – GitHub repository
- [ ] git init, .gitignore, LICENSE, README, first commit, secret hygiene
- [ ] Remote creation/push documented (needs user's GitHub account)

### Phase 9 – GitHub Actions CI (`ci.yml`)
- [ ] checkout → setup .NET 8 → restore → build → test → docker build check

### Phase 10 – Self-hosted runner
- [ ] Setup guide in README (needs user's GitHub token)

### Phase 11 – GitHub Actions CD (`cd.yml`)
- [ ] After CI success on main: build, login (secrets), push `latest` + sha, deploy via Ansible on self-hosted runner

### Phase 12 – Documentation
- [ ] README with all 17 required sections + Mermaid diagrams
- [ ] `EXPLAINED.md` (what / how / why in short)

### Phase 13 – Final end-to-end verification
- [ ] Definition-of-Done table below updated honestly

## Definition of Done (updated at the end)
(see bottom of file – filled in after verification)
