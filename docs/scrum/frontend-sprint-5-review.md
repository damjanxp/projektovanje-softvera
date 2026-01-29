# Sprint Review — Frontend Sprint 5

## Sprint
- Sprint #: FE-5
- Period: Frontend Sprint 5
- Sprint Goal:
  Implementirati UI za ocenjivanje tura i prijavu problema, kao i prikaz statusa problema i event log-a uz role-based akcije.

## Completed
- [x] Implementiran UI za kreiranje ocene (1–5) za kupljene ture
- [x] Implementirano pravilo obaveznog komentara za ocene 1 i 2 (frontend validacija)
- [x] Implementiran UI za prijavu problema (title + description) za kupljene ture
- [x] Implementirana stranica detalja problema sa prikazom trenutnog statusa
- [x] Implementiran prikaz event log-a (istorija promena statusa)
- [x] Implementirane akcije promene statusa problema:
  - Guide: Resolve / Send to review
  - Admin: Reject
- [x] Role-based prikaz dugmadi i zaštita ruta
- [x] Loading state i error/success poruke za sve akcije

## Demo Notes
- Demo scenario steps:
  1) Prijava kao Tourist
  2) Kreiranje rating-a (provera: ocena 1/2 zahteva komentar)
  3) Prijava problema za turu i otvaranje detalja problema
  4) Provera statusa Pending i event log prikaza
  5) Prijava kao Guide i promena statusa (Resolve ili Send to review)
  6) Provera refresh-a statusa i novog event-a
  7) Prijava kao Admin i Reject problema
  8) Provera statusa Rejected i event log-a

## What went well
- Jasno implementirane UX validacije (obavezni komentar za niske ocene)
- Problem lifecycle je kompletno demonstrabilan kroz UI
- Event log daje preglednu istoriju promena i pojačava DDD deo projekta
- Role-based prikaz akcija je konzistentan

## What to improve
- Dodati listu svih problema po korisniku (tourist/guide) za lakšu navigaciju
- Unaprediti UI (bolji layout, bolji prikaz event log-a)
- Dodati unit testove za ključne komponente

## Next Sprint plan (high level)
- Finalno poliranje aplikacije (Frontend Sprint 6): UX poboljšanja, error handling, loading states, demo flow
