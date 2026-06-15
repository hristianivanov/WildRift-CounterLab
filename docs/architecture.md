# Architecture

## Overview

Wild Rift Counter Lab uses Clean Architecture to keep recommendation logic deterministic, testable, and independent from PostgreSQL and external AI providers.

```text
Frontend -> Api -> Application -> Domain
                    ^
                    |
              Infrastructure
```

## Backend Layers

### Domain

Contains core entities and constants:

- `Champion`
- `MatchupRule`
- Allowed role values

Domain has no project dependencies.

### Application

Contains use-case behavior and contracts:

- `DraftService`
- `ScoreEngine`
- `ReasonEngine`
- `PlanEngine`
- Champion and matchup-rule administration services
- DTOs and repository/AI interfaces

Application depends on Domain, but never on Infrastructure.

### Infrastructure

Implements Application contracts:

- EF Core `ApplicationDbContext`
- PostgreSQL repositories
- Idempotent database seeding
- Cached AI explanation orchestration
- Configurable GroqCloud and Gemini provider implementations

### Api

Provides HTTP composition and transport behavior:

- Controllers and routes
- Dependency injection
- CORS, OpenAPI generation, and Scalar API reference
- Standard error responses
- Global exception handling

## Frontend Architecture

```text
pages -> hooks -> api
  |
  v
components -> types
```

- `src/api`: axios client and endpoint functions
- `src/hooks`: champion loading and recommendation request state
- `src/components`: reusable and draft-specific presentation
- `src/pages`: page-level composition
- `src/types`: backend-aligned TypeScript contracts

## Recommendation Pipeline

1. `DraftService` validates the request.
2. Repositories load champion and matchup-rule data.
3. `ScoreEngine` calculates deterministic score categories.
4. `ReasonEngine` creates rule/tag-based reasons.
5. `PlanEngine` chooses a matchup plan or deterministic fallback.
6. Recommendations are ranked and limited to the top five.
7. When requested, the frontend asynchronously asks the configured provider to explain the top recommendations without blocking the main recommendation response.

## Why AI Does Not Decide Scores

Generative model output can vary and is difficult to audit. Recommendation rankings therefore come entirely from database rules and deterministic scoring logic. AI receives the completed recommendation as context and produces a human-readable explanation only. GroqCloud is the current production provider, while Gemini remains an optional alternative. AI failures cannot prevent deterministic recommendations from returning.
