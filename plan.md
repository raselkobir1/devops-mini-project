# Plan & Checklist (source: learn_dev_ops.md)

Legend: `[x]` built **and verified on this machine** · `[~]` built and syntax-checked, but cannot run here (needs your GitHub / Docker Hub account)

## Environment notes
- .NET SDK 10 preview + .NET 8 runtime (project targets `net8.0`); Docker Desktop 29; Terraform installed via winget during this work.
- Ansible has no native Windows support, so it was verified through `ansible/Dockerfile.runner` (Ansible + Docker CLI container).
- Host ports 5432/8080 were already used by your other containers, so verification used `API_PORT=8085`, `POSTGRES_PORT=5434`. Defaults in the repo are still 8080/5432. Your existing containers were not touched.

## Phase 1 – .NET 8 API + PostgreSQL
- [x] Layout: Controllers, Services, Repositories, Data, Models, DTOs, Program.cs
- [x] Product model with all 8 fields
- [x] CRUD endpoints + `/health` + Swagger (Development)
- [x] Connection string from `ConnectionStrings__DefaultConnection`
- [x] EF Core migration `InitialCreate`, applied at startup
- [x] Verified against real PostgreSQL: create/update/get/delete, 400 on invalid input, 404 on missing

## Phase 2 – Unit tests
- [x] xUnit: create, retrieve, validation, update, delete, business logic (14 tests, all pass)

## Phase 3 – Dockerfile
- [x] Multi-stage SDK → ASP.NET runtime, port 8080, non-root, healthcheck, `.dockerignore`
- [x] Image builds

## Phase 4 – Docker Compose
- [x] `product-api` + `product-postgres`, network, volume, `pg_isready`, `service_healthy` dependency
- [x] Both containers reported `healthy`; data survived `down` / `up` (volume works); API connects via `Host=postgres`

## Phase 5 – Terraform
- [x] versions.tf, variables.tf, main.tf, outputs.tf
- [x] init, validate, plan, apply (6 resources, API healthy), destroy (clean)

## Phase 6 – Ansible
- [x] inventory.ini, playbook.yml, group_vars/all.yml, templates/docker-compose.yml.j2
- [x] Run 1: changed=3, API healthy. Run 2: **changed=0** (idempotent)
- [x] `IMAGE_TAG` selects the tag (rollback)

## Phase 7 – Docker Hub
- [x] Naming `<user>/product-api:<tag>` verified with `docker tag`; `scripts/docker-push.sh` (latest + sha + version)
- [~] Actual push/pull: needs your Docker Hub account and token (not available here)

## Phase 8 – GitHub repository
- [x] git repo, `.gitignore` (.env, tfstate, keys), LICENSE, README, local commit
- [~] Creating the GitHub remote and pushing: needs your account (`gh` CLI not installed). Steps in README §10

## Phase 9 – GitHub Actions CI
- [x] `ci.yml`: checkout, .NET 8, restore, build, test, docker build (YAML parsed; same commands pass locally)
- [~] First run on GitHub happens after you push

## Phase 10 – Self-hosted runner
- [x] Setup guide in README §13 (WSL2 Ubuntu + Docker + Ansible)
- [~] Registering the runner needs your repo token

## Phase 11 – GitHub Actions CD
- [x] `cd.yml`: after CI success on main, push `latest` + short-sha, deploy job on self-hosted runner via Ansible with `IMAGE_TAG`
- [~] Needs secrets `DOCKERHUB_USERNAME`, `DOCKERHUB_TOKEN` and a runner

## Phase 12 – Documentation
- [x] README: all 17 sections, Mermaid diagrams
- [x] `EXPLAINED.md` (what / how / why)

## Phase 13 – Final verification
- [x] `dotnet test` 14/14, `docker compose up` healthy, terraform full cycle, ansible twice

## Definition of Done
| Area | Item | Status |
|---|---|---|
| Application | API starts, CRUD, PostgreSQL stores data, health | [x] |
| Application | Swagger works | [~] enabled in Development only; not opened in a browser here |
| Docker | Build, run API + DB, volume, connectivity, Compose | [x] |
| Terraform | init, validate, plan, apply, destroy | [x] |
| Ansible | inventory, playbook, idempotent, Docker deployment | [x] |
| Docker Hub | image builds | [x] |
| Docker Hub | push, pull, deploy from Hub image | [~] needs account |
| CI | workflow written; fails on build/test/docker errors by design | [~] runs after push |
| CD | workflow, runner, automatic deploy | [~] needs account + runner |
| Documentation | README, architecture, setup, troubleshooting, future remote | [x] |
