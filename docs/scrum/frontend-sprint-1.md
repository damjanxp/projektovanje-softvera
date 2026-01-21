# Frontend Sprint 1 — Authentication & Roles (Plan)

## Sprint Goal
Omogućiti prijavu korisnika, čuvanje sesije i zaštitu ruta na osnovu uloge korisnika.

## Scope
- Login UI
- JWT session handling
- AuthGuard i RoleGuard

## Planned Backlog Items
- Login komponenta (reactive form)
- Poziv backend login endpointa
- JWT decoding (bez eksternih biblioteka)
- AuthGuard (zaštita autentifikovanih ruta)
- RoleGuard (Tourist / Guide / Admin)
- Forbidden (403) i NotFound (404) stranice
- Navbar session behavior (login/logout)

## Out of Scope
- Registracija korisnika
- Kompleksni UI feedback (toast, modali)

## Definition of Done
- Login funkcioniše za sve role
- Neautentifikovani korisnici su blokirani
- Role-based rute rade ispravno
