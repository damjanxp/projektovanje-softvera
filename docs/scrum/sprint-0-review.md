# Sprint Review — Sprint 0

## Sprint
- Sprint #: 0
- Period: Inicijalna faza projekta
- Sprint Goal:
  Postavljanje osnovne infrastrukture i arhitekture sistema.

## Completed
- [x] Setup Git repozitorijuma i .gitignore konfiguracija
- [x] Kreiranje .NET solution-a i projekata (Api, Domain, Application, Infrastructure, Shared)
- [x] Postavljanje DDD slojeva i project reference pravila
- [x] Konfiguracija PostgreSQL baze i EF Core
- [x] Global exception handling middleware
- [x] Standardizovan ApiResponse model
- [x] Health endpoint (/health)

## Demo Notes
- Demo scenario steps:
  1) Pokretanje aplikacije lokalno
  2) Otvaranje Swagger UI
  3) Poziv GET /health endpointa
  4) Provera uspešne konekcije sa bazom

## What went well
- Arhitektura je jasno podeljena po DDD principima
- Projekat se uspešno build-uje i pokreće
- Baza je uspešno povezana i spremna za migracije

## What to improve
- Dodati više automatizovanih testova u narednim sprintovima
- Unaprediti dokumentaciju kako se funkcionalnosti budu dodavale

## Next Sprint plan (high level)
- Implementacija autentifikacije i upravljanja korisnicima
