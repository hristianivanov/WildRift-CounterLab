# ADR: Engine Decides, AI Explains

## Context

Wild Rift Counter Lab recommends champions for a given role and enemy composition. The core question during design was: **who is responsible for the recommendation — the deterministic engine or the AI?**

Two approaches were considered:

**Option A — Pure AI**: Send the role and enemy team to a language model and ask it to recommend champions with reasoning. Simple to implement, flexible, and capable of nuanced advice.

**Option B — Deterministic engine + AI as narrator**: Score every eligible champion with a rule-based pipeline. Rank by score. Then optionally ask an AI to explain the top results in plain language — without letting it change any score, reason, or ranking.

Option B was chosen.

## Decision

The recommendation engine owns all decisions. The AI owns none.

Concretely:
- Champion scores, score breakdowns, reasons, and plans are computed by `ScoreEngine`, `ReasonEngine`, and `PlanEngine` before any AI call is made.
- The AI receives the already-ranked recommendations and is instructed only to explain them in natural language.
- The AI prompt explicitly states: *"Explain the engine's top recommendations without changing or questioning their scores or ranking."*
- If the AI is unavailable, rate-limited, or returns garbage, the recommendation is unaffected. The engine output stands on its own.

## Why Not Pure AI

**Reproducibility.** The same role and enemy team should produce the same recommendation every time. A language model is non-deterministic. Scores that shift between requests make the tool feel unreliable and make it impossible to reason about why a recommendation changed.

**Transparency.** The score breakdown (lane, team, role fit, safety, scaling, utility) is shown to the user. Each category maps to a specific formula. A user can look at "+12 team score" and understand that their enemy team is tank-heavy. An LLM recommendation cannot expose this kind of breakdown in a structured, trustworthy way.

**Cost and latency control.** Running a scoring pass over all eligible champions is a local, free, sub-millisecond operation. AI inference costs money and adds latency. Keeping the critical path deterministic means the app is fully functional with zero AI spend and degrades gracefully when AI is slow or rate-limited.

**Auditability.** Matchup rules and champion tags are data stored in PostgreSQL. They can be reviewed, corrected, and version-controlled. An LLM's internal knowledge cannot be audited, updated for a new patch, or explained to a skeptical user.

## Why Keep AI At All

Natural language explanations improve usability. "Malphite counters the AD-heavy enemy team and has a strong engage into their immobile backline" is more actionable than a numeric breakdown for a player who is not yet fluent in the scoring model. The AI adds a communication layer without taking ownership of the decision.

## Tradeoffs Accepted

- The AI explanation can only work within the frame the engine provides. If the engine scores a champion incorrectly, the AI will enthusiastically explain why that champion is a great pick. The quality ceiling of the AI narration is bounded by the quality of the engine data.
- The engine requires seed data (matchup rules, champion tags) to be maintained manually. A pure AI approach would be self-updating as model knowledge improves.
- Two separate response phases (deterministic result first, AI enrichment second) add frontend complexity to handle partial loading states.

## What This Means for Future Changes

Any future AI integration should follow the same principle: use AI where it adds communication value, not where it makes decisions. If an LLM is ever used to suggest matchup rule updates or score calibrations, those suggestions should be treated as input to a human review process, not applied automatically.
