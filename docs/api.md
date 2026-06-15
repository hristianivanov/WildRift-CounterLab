# API Reference

Base URL for local development: `http://localhost:5069/api`

The Scalar API reference is available at `http://localhost:5069/scalar` in Development.
The generated OpenAPI document remains available at `http://localhost:5069/swagger/v1/swagger.json`.

## Health

### `GET /api/health`

Returns a simple process health response.

```json
{ "status": "ok" }
```

## Champions

### `GET /api/champions`

Returns all champions with roles and tags.

### `GET /api/champions/{id}`

Returns one champion or `404`.

### `POST /api/champions`

Creates a champion.

### `PUT /api/champions/{id}`

Updates a champion.

### `DELETE /api/champions/{id}`

Deletes an unreferenced champion. Deletion is rejected when matchup rules reference the champion.

## Draft Recommendations

### `POST /api/draft/recommendations`

```json
{
  "role": "Baron",
  "laneEnemy": "Darius",
  "enemyTeam": ["Senna", "Jhin", "Olaf"],
  "includeAiExplanation": false
}
```

Returns ranked recommendations with score breakdowns, reasons, plans, and optional AI explanations.

## AI Explanation

### `POST /api/ai/explain`

Generates an explanation for a recommendation supplied by the caller. The frontend uses this endpoint to enrich top recommendation cards asynchronously. This endpoint does not calculate or alter recommendation scores.

## Matchup Rules

- `GET /api/matchup-rules`
- `GET /api/matchup-rules/{id}`
- `POST /api/matchup-rules`
- `PUT /api/matchup-rules/{id}`
- `DELETE /api/matchup-rules/{id}`

Rules require a valid role and existing champion names. Duplicate role/champion/enemy combinations are rejected.

## Standard Errors

Validation, missing-resource, and unexpected-error responses use:

```json
{
  "error": "Human-readable summary.",
  "details": "Optional validation details.",
  "traceId": "Request trace identifier."
}
```
