# Deployment Guide

Target deployment:

- Frontend: Vercel
- Backend: Railway, with Render as fallback
- Database: Supabase PostgreSQL

No secrets should be committed. Configure every production value in the hosting provider dashboard.

## Production Environment Variables

### Backend

| Variable | Required | Example / purpose |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Yes | `Production` |
| `ASPNETCORE_FORWARDEDHEADERS_ENABLED` | Recommended | `true` for Railway/Render TLS proxies |
| `ConnectionStrings__DefaultConnection` | Yes | Supabase PostgreSQL connection string |
| `Frontend__AllowedOrigins__0` | Yes | Exact Vercel origin, such as `https://wildrift-counterlab.vercel.app` |
| `Frontend__AllowedOrigins__1` | Optional | Additional custom frontend origin |
| `Database__ApplyMigrationsOnStartup` | First deploy / optional | `true` applies pending EF Core migrations before seeding |
| `ApiDocumentation__EnabledInProduction` | Optional | `true` exposes Scalar and the OpenAPI document in Production; defaults to `false` |
| `Ai__Provider` | Recommended | `Groq` for the primary provider, or `Gemini` |
| `Groq__ApiKey` | Required when using Groq | GroqCloud API key |
| `Groq__Model` | Recommended for Groq | `llama-3.1-8b-instant` |
| `Gemini__ApiKey` | Required when using Gemini | Gemini API key |
| `Gemini__Model` | Recommended for Gemini | `gemini-2.5-flash` |
| `PORT` | Platform-provided | Railway/Render port; the Docker command uses it automatically |

Supabase Npgsql connection string format:

```text
Host=YOUR_SUPABASE_HOST;Port=5432;Database=postgres;Username=YOUR_SUPABASE_USER;Password=YOUR_SUPABASE_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

Use Supabase's direct database connection for migrations and for a persistent backend when the host supports IPv6 or the Supabase project has the IPv4 add-on. If Railway/Render cannot reach the direct host, use Supavisor session mode for runtime traffic and run migrations separately from a direct-capable trusted machine. Do not use transaction mode for EF Core migrations.

### Frontend

| Variable | Required | Example |
| --- | --- | --- |
| `VITE_API_BASE_URL` | Yes | `https://YOUR-BACKEND-DOMAIN/api` |
| `VITE_AI_ENABLED` | Optional | Set `true` to expose AI analysis; set `false` to disable it for demo stability |

Vite embeds this value during the build. Redeploy the frontend after changing it.

## 1. Create Supabase PostgreSQL

1. Create a Supabase project.
2. Open the database connection settings.
3. Copy the direct PostgreSQL connection details for migrations.
4. Choose direct connection or Supavisor session mode for the deployed runtime based on network reachability.
5. Build `ConnectionStrings__DefaultConnection` using the SSL-enabled format above.
6. Keep the connection string only in Railway/Render secrets.

## 2. Deploy Backend To Railway

The repository includes `backend/railway.toml` and `backend/Dockerfile`. Both are intentionally written for `backend` as the Railway service root.

1. Create a Railway project from the repository.
2. Set **Root Directory** to `/backend`.
3. Set **Config as Code / Railway Config File** to `/backend/railway.toml` if Railway does not discover it automatically.
4. Set **Builder** to `Dockerfile`; do not select Railpack.
5. Confirm the **Dockerfile Path** is `Dockerfile` relative to `/backend`. If the dashboard asks for a repository-relative path, use `/backend/Dockerfile`.
6. Leave custom build and start commands empty. The Dockerfile restores, publishes, and starts the API.
7. Add every required backend environment variable from the table above.
8. Set `Frontend__AllowedOrigins__0` to the final Vercel origin. It can temporarily use the first Vercel deployment URL and be updated later.
9. For the first deployment, set `Database__ApplyMigrationsOnStartup=true` only when the configured connection supports migrations. Otherwise, run the manual migration command first.
10. Deploy and wait for Railway's internal `/health` health check to pass.
11. Confirm the public `GET /api/health` endpoint returns `{"status":"ok"}`.
12. Verify `/api/champions` returns seeded champion data.

The Railway build log should contain Docker build stages from `mcr.microsoft.com/dotnet/sdk:8.0`. If it shows Railpack or runs plain `dotnet restore`, the service root/builder settings are incorrect or the deployment is using a commit that does not contain `backend/railway.toml`.

After the database is initialized, `Database__ApplyMigrationsOnStartup` may remain `true` for this single-instance portfolio deployment or be changed to `false` when migrations are managed separately.

## 3. Render Fallback

The repository includes `render.yaml` and uses the same Dockerfile.

1. Create a Render Blueprint from the repository.
2. Enter the secret environment variables marked `sync: false`.
3. Set `Frontend__AllowedOrigins__0` to the Vercel origin.
4. Set `Database__ApplyMigrationsOnStartup=true` for the first deployment only when the configured connection supports migrations. Otherwise, run the manual migration command first.
5. Deploy and confirm the public `/api/health` health check passes.

## 4. Verify Production Migrations

The first backend startup can apply migrations when:

```text
Database__ApplyMigrationsOnStartup=true
```

Verify migration success:

1. Confirm backend startup logs contain no EF Core migration errors.
2. Confirm Supabase contains the `__EFMigrationsHistory`, `Champions`, and `MatchupRules` tables.
3. Confirm `GET https://YOUR-BACKEND-DOMAIN/api/champions` returns Senna and Jhin.

For manual migration control from a trusted machine:

```powershell
dotnet tool restore
$env:ConnectionStrings__DefaultConnection="YOUR_SUPABASE_CONNECTION_STRING"
dotnet ef database update --project backend/WildRiftCounterLab.Infrastructure --startup-project backend/WildRiftCounterLab.Api
Remove-Item Env:ConnectionStrings__DefaultConnection
```

## 5. Configure And Verify AI

GroqCloud is the recommended free-friendly provider:

```text
Ai__Provider=Groq
Groq__ApiKey=YOUR_GROQ_API_KEY
Groq__Model=llama-3.1-8b-instant
```

To use Gemini instead:

```text
Ai__Provider=Gemini
Gemini__ApiKey=YOUR_GEMINI_API_KEY
Gemini__Model=gemini-2.5-flash
```

Verify the provider with `POST /api/ai/explain`, then complete the browser flow with AI enabled. The frontend requests explanations asynchronously after deterministic recommendations render.

The configured provider never affects scores or ranking. Successful explanations are cached in PostgreSQL. If the provider is unavailable or rate-limited, deterministic recommendations still return with a safe fallback.

## 6. Deploy Frontend To Vercel

The frontend includes `frontend/vercel.json`.

1. Import the repository into Vercel.
2. Set the Vercel Root Directory to `frontend`.
3. Add `VITE_API_BASE_URL=https://YOUR-BACKEND-DOMAIN/api`.
4. Set `VITE_AI_ENABLED=true` to expose AI analysis, or `false` to disable it for demo stability.
5. Deploy.
6. Copy the final Vercel origin into the backend's `Frontend__AllowedOrigins__0`.
7. Redeploy/restart the backend after changing CORS configuration.

## 7. Production Verification

```text
GET https://YOUR-BACKEND-DOMAIN/api/health
GET https://YOUR-BACKEND-DOMAIN/api/champions
```

Expected health response:

```json
{ "status": "ok" }
```

Complete the browser validation:

1. Open the Vercel frontend.
2. Select Baron and Darius.
3. Add Senna, Jhin, and Olaf.
4. Analyze without AI.
5. Analyze with AI.
6. Confirm score breakdowns, reasons, plans, and AI explanations render.

## Troubleshooting

- **Frontend cannot call backend:** verify `VITE_API_BASE_URL`, the exact CORS origin, and that neither value has an unwanted trailing slash.
- **Backend does not start:** verify the Supabase connection string and SSL settings.
- **Tables missing:** enable `Database__ApplyMigrationsOnStartup` or run the manual migration command.
- **Health check fails:** inspect Railway/Render logs and verify the service is binding the platform-provided `PORT`.
- **Railway runs plain `dotnet restore`:** set Root Directory to `/backend`, Config File to `/backend/railway.toml`, Builder to `Dockerfile`, Dockerfile Path to `Dockerfile`, clear custom build/start commands, and redeploy the commit containing `backend/railway.toml`.
- **AI unavailable:** verify `Ai__Provider` and the matching Groq or Gemini key/model variables.
