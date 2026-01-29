# Sprint Review — Frontend Sprint 4

## Sprint
- Sprint #: FE-4
- Period: Frontend Sprint 4
- Sprint Goal:
  Implementirati UI za zamenu vodiča (replacement flow) i poboljšati prikaz statusa tura (uključujući Canceled i ReplacementRequested).

## Completed
- [x] Implementiran “Request replacement” na manage tour stranici (Guide)
- [x] Implementiran replacement board (Guide-only) sa listom tura koje traže zamenu
- [x] Implementirana akcija “Take over” ture sa prikazom success/error poruka
- [x] Implementirana role-based zaštita replacement ruta (Guide)
- [x] Dodati status badge indikatori (Draft / Published / Canceled / ReplacementRequested)
- [x] Poboljšan UX za canceled ture (disable/sakrivanje akcija, zabrana add-to-cart)
- [x] Loading state i error handling za replacement akcije i listu

## Demo Notes
- Demo scenario steps:
  1) Prijava kao Guide A i zahtev za zamenu na turi
  2) Prijava kao Guide B i pregled replacement board-a
  3) Preuzimanje ture (Take over) i provera da tura nestaje sa board-a
  4) Provera da je tura vidljiva u “My Tours” kod novog vodiča
  5) Provera status indikatora na listama i detaljima tura
  6) Provera ponašanja za canceled ture (akcije disabled/skrivene)

## What went well
- Replacement flow je jasno demonstrabilan kroz UI
- Role-based pristup je dosledno sproveden
- Status indikatori poboljšavaju preglednost i UX
- Error poruke iz backend-a su prikazane korisniku na razumljiv način

## What to improve
- Vizuelno doterati replacement board (bolji prikaz kartica i filtera)
- Dodati dodatne UI validacije i potvrde (
::contentReference[oaicite:0]{index=0}
