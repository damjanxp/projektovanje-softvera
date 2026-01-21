# Sprint Review — Frontend Sprint 1

## Sprint
- Sprint #: FE-1
- Period: Frontend Sprint 1
- Sprint Goal:
  Implementirati prijavu korisnika (login), čuvanje sesije i zaštitu ruta kroz AuthGuard i RoleGuard.

## Completed
- [x] Implementirana Login stranica (reactive form + validacije)
- [x] Integracija sa backend login endpointom i obrada ApiResponse poruka
- [x] Implementirano čuvanje tokena i osnovnih korisničkih informacija (username/role)
- [x] JWT payload decoding (bez eksternih biblioteka) radi izvlačenja role/username
- [x] Implementiran AuthGuard (zaštita ruta uz redirect na /login)
- [x] Implementiran RoleGuard (allowedRoles u route data) + redirect na /forbidden
- [x] Kreirane Forbidden (403) i NotFound (404) stranice
- [x] Dodate demo role-protected rute (tourist-area, guide-area, admin-area)
- [x] Navbar prikazuje session info i omogućava logout

## Demo Notes
- Demo scenario steps:
  1) Login kao admin/guide/tourist
  2) Navigacija na role-protected rute
  3) Provera da RoleGuard blokira nedozvoljene role (redirect /forbidden)
  4) Logout i provera da se token briše i vraća na /login

## What went well
- Auth flow je stabilan i jasan za demonstraciju
- Role-based zaštita ruta je implementirana jednostavno i pregledno
- UX osnova (navbar, redirecti) je spremna za feature stranice

## What to improve
- Dodati lepši prikaz grešaka i globalni notification sistem
- Dodati refresh ponašanje sesije (npr. auto-logout na isteku tokena)

## Next Sprint plan (high level)
- Prikaz published tura + detalji ture + guide upravljanje turama (Frontend Sprint 2)
