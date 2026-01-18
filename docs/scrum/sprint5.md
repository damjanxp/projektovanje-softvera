# Sprint 5 Plan — Rating, Recommendations & Problems (Optional)

## Sprint Goal
Završiti napredne funkcionalnosti: ocenjivanje, preporuke i prijava problema sa event logom.

## Sprint Backlog (US)
- US-F1 Rating (1–5) + comment rules + 7-day window
- US-E2 Recommendations by interests (email)
- US-G1 Problems reporting
- US-G2 Problem status events (event sourcing log)

## Deliverable (Inkrement)
- Rating radi sa svim pravilima
- Recommendations email šalje se po pravilima
- Problem lifecycle + istorija promena

## Demo Scenario
- Nakon date: rating allowed, comment mandatory for 1–2
- New published tour -> recommendation emails (ako enabled)
- Problem created -> status changes -> event log prikazan
