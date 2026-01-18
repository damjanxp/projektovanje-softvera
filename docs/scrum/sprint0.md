# Sprint 0 Plan — Setup & Foundations

## Sprint Goal
Postaviti inicijalnu infrastrukturu i arhitekturu: repo struktura, DDD slojevi, baza, osnovni API skeleton.

## Sprint Backlog
- Setup Git repo + .gitignore
- Kreiranje .NET solution + projekti (Api/Domain/Application/Infrastructure/Shared)
- DDD layering: project reference pravila
- PostgreSQL + EF Core setup (DbContext, connection string)
- Global exception middleware + standard ApiResponse model
- Health endpoint (/health)
- Minimalna Scrum dokumentacija (backlog, DoD)

## Deliverable (Inkrement)
- API radi lokalno (Swagger)
- Baza konekcija radi i moguće je pokrenuti migracije
- Standardizovan response i error handling

## Demo Scenario
- `GET /health` vraća success
- Swagger se otvara i API endpointi rade
