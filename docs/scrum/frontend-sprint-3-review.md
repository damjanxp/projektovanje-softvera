# Sprint Review — Frontend Sprint 3

## Sprint
- Sprint #: FE-3
- Period: Frontend Sprint 3
- Sprint Goal:
  Implementirati kupovinu tura kroz UI: korpa, primena bonus poena i potvrda kupovine za turiste.

## Completed
- [x] Implementirano dodavanje tura u korpu sa public liste i stranice detalja ture
- [x] Implementirana Cart stranica (stavke, ukupna cena, prazna korpa)
- [x] Implementirano uklanjanje tura iz korpe
- [x] Implementirana primena bonus poena (use bonus toggle)
- [x] Confirm purchase flow (sa i bez bonus poena)
- [x] UI feedback: loading state, success/error poruke
- [x] Role-based ograničenja (samo Tourist pristupa cart/purchase; Guide/Admin nemaju add-to-cart)

## Demo Notes
- Demo scenario steps:
  1) Prijava kao Tourist
  2) Public pregled tura i dodavanje ture u korpu
  3) Otvaranje korpe i provera ukupne cene
  4) Uklanjanje ture iz korpe
  5) Potvrda kupovine bez bonus poena
  6) Potvrda kupovine sa bonus poenima
  7) Provera da je korpa prazna nakon uspešne kupovine i da se UI korektno ažurira

## What went well
- Kupovina je implementirana kao jasan i demonstrabilan UI flow
- Bonus poeni su integrisani u UI bez narušavanja poslovnih pravila
- Role-based ograničenja su jasno sprovedena
- Komunikacija sa backendom je stabilna kroz postojeću infrastrukturu (interceptor + ApiResponse)

## What to improve
- Dodati globalne UI notifikacije (toast) radi boljeg UX-a
- Unaprediti izgled cart stranice i prikaz grešaka
- Dodati više unit testova za komponente (ne samo servise)

## Next Sprint plan (high level)
- Implementacija zamene vodiča i prikaz statusa tura u UI (Frontend Sprint 4)