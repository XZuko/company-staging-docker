# Company staging environment

Single-server integration environment for Angular, ASP.NET Core/EF Core, and SQL Server. Nginx exposes one entry point while SQL Server remains isolated on an internal network.

## Prerequisites

- Linux server with Docker Engine and Docker Compose v2 (recommended), or Docker Desktop for local testing
- At least 4 GB RAM; 8 GB recommended for SQL Server and builds
- 20 GB free disk space
- Git for source control

## Start locally

1. Copy `.env.example` to `.env` and replace `SQL_PASSWORD` with a strong staging-only password.
2. Run `docker compose up -d --build --wait`.
3. Run `docker compose --profile test run --rm smoke-test`.
4. Open `http://localhost:8080`.

Use `powershell -File scripts/diagnose.ps1` for container status and recent logs. Deploy a version with `powershell -File scripts/deploy.ps1 -Version 1.0.0`.

## Failure and rollback behavior

The deployment script validates Compose, builds images, waits for all health checks, and runs an end-to-end API smoke test. On failure it prints service state and recent logs, exits with code 1, and restores the last recorded application image version when one exists.

Database data is stored in the `sql-data` volume and is not automatically rolled back. Production-grade schema changes must use reviewed EF migrations, backward-compatible releases, and a tested database backup/restore procedure. `EnsureCreated` is used only by this disposable sample.

## React Native

React Native is a client of this API rather than a long-running server container. During development, configure the mobile app API base URL to this server (for example `http://SERVER_IP:8080/api`). Android/iOS builds should run in a separate CI job with the platform SDKs.

## Multi-server path

Keep these application images and move orchestration to Kubernetes when multi-server resilience is required. Replace local volumes with managed SQL Server or durable shared storage, publish images to a registry, add TLS/secret management, and translate health checks into readiness/liveness probes.
