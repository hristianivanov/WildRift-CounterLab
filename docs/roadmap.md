# Roadmap

## Completed

- Clean Architecture backend
- Deterministic multi-category recommendation pipeline
- Rule/tag-based reasons and plans
- Optional cached AI explanations with configurable GroqCloud or Gemini providers
- PostgreSQL and EF Core persistence
- Idempotent initial champion and matchup-rule dataset
- Champion and matchup-rule CRUD APIs
- Standardized errors, OpenAPI/Scalar documentation, CORS, and health endpoint
- Responsive React draft interface
- Application unit tests and API integration tests
- Vercel, Railway/Render, and Supabase deployment configuration
- Rate limiting on draft and AI endpoints (fixed-window per IP)
- Structured error logging with request context
- CancellationToken propagation from controller through to repositories and AI providers
- Input length validation on all request DTOs
- Tag allowlist validation on champion create and update
- ScoreEngine scoring constants documented with design rationale
- Database index on MatchupRules(Role, EnemyChampion) for the hot query path
- AI provider validated at startup rather than on first request
- AI provider failure logging (batch and single paths)
- Champions list cached in sessionStorage to avoid redundant fetches
- Architecture decision record: engine-decides, AI-explains pattern

## Next

- Expand matchup-rule coverage and tune seed data
- Add frontend tests for the primary draft workflow
- Add authentication before exposing administration workflows

## Later

- Build champion and matchup-rule administration UI
- Add patch-aware data ingestion with explicit versioning
- Add distributed caching only if future scale and profiling justify it
