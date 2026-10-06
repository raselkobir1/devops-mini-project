# What was built, how, and why (short version)

| Piece | What it is | How it is implemented here |
|---|---|---|
| **.NET 8 Product API** | A REST service to create/read/update/delete products | Controller → Service → Repository → EF Core. DTOs validate input (400 on bad data). `/health` via ASP.NET health checks. Swagger in Development. |
| **PostgreSQL + EF Core** | The database and the ORM that talks to it | `AppDbContext`, one `InitialCreate` migration applied automatically at startup (with retries). Connection string only from `ConnectionStrings__DefaultConnection`. |
| **xUnit tests** | Automatic checks of the business logic | 14 tests on `ProductService` using EF's in-memory DB: create, get, validation, update, delete, not-found cases. |
| **Dockerfile** | Recipe that packages the API into an image | Multi-stage: SDK image compiles, small ASP.NET runtime image runs it. Non-root user, port 8080, built-in healthcheck. |
| **Docker Compose** | Runs several containers together | `product-api` + `product-postgres`, shared network, named volume (data survives restarts), `pg_isready` healthcheck, API waits for a healthy DB. |
| **Terraform** | Infrastructure as Code: describe infra in files | Docker provider creates the network, volume and both containers (`tf-` prefix). `init/validate/plan/apply/destroy`. No app logic inside. |
| **Ansible** | Automates deployment to a machine | Playbook (local connection): checks Docker, makes a folder, renders the compose file from a Jinja template, pulls the image, starts containers, waits for health. Re-running changes nothing (idempotent). `IMAGE_TAG` picks the version, so rollback = redeploy an older tag. |
| **Docker Hub** | Registry where images are stored | Images named `<user>/product-api:<tag>`; tags `latest` plus the git short-SHA. Credentials only via `docker login` / GitHub Secrets. |
| **GitHub Actions CI** | Checks every push/PR | Restore → build → test → docker build. Any failure turns the pipeline red. |
| **GitHub Actions CD** | Ships `main` automatically | After CI passes: build and push the image to Docker Hub, then a **self-hosted runner** on your machine runs Ansible to update the local containers. (Hosted runners cannot reach your laptop.) |
| **README / plan.md** | Documentation and checklist | README covers all 17 topics with Mermaid diagrams; `plan.md` is the step-by-step checklist with honest verification status. |

**Flow:** `git push` → GitHub → CI (build, test, docker build) → CD (push image to Docker Hub → Ansible → Docker Compose) → API + PostgreSQL running locally.

**Why this design:** each tool does one job (Terraform = create infra, Ansible = configure/deploy, Docker = package/run, Actions = automate), and nothing is hard-coded, so moving to an EC2/DigitalOcean server later only changes the Ansible inventory and adds Terraform for the VM.
