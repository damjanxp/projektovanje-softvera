# Sprint 4 Plan — Replacement, Cancellation & Notifications

## Sprint Goal
Implementirati traženje zamene, automatsko otkazivanje bez zamene 24h pre i email podsetnike.

## Sprint Backlog (US)
- US-D1 Traženje zamene (guide board)
- US-D2 Auto-cancel 24h pre + bonus poeni kupcima
- US-E1 Podsetnik 48h pre ture

## Deliverable (Inkrement)
- Replacement flow radi po pravilima
- Auto-cancel i bonus points se pravilno dodeljuju
- Reminder email radi kroz background job

## Demo Scenario
- Označi turu za zamenu -> drugi vodič preuzima (ako nema turu tog datuma)
- Ako nema zamene 24h pre -> cancel + bonus points
- 48h pre ture -> reminder email
