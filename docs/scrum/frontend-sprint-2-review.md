# Sprint Review — Frontend Sprint 2

## Sprint
- Sprint #: FE-2
- Period: Frontend Sprint 2
- Sprint Goal:
  Omogućiti public pregled tura i upravljanje turama za vodiče kroz frontend aplikaciju.

## Completed
- [x] Implementiran public pregled published tura
- [x] Omogućeno sortiranje tura (asc / desc)
- [x] Implementirana stranica sa detaljima ture
- [x] Prikaz key point-ova na Leaflet mapi (markeri + povezivanje tačaka)
- [x] Implementirana Guide sekcija za upravljanje turama
- [x] Kreiranje nove ture (Draft status)
- [x] Dodavanje key point-ova uz interakciju sa mapom (klik → lat/lng)
- [x] Omogućeno objavljivanje ture nakon dodavanja minimum 2 key point-a
- [x] Role-based zaštita ruta (Guide)

## Demo Notes
- Demo scenario steps:
  1) Public pregled svih tura (/tours)
  2) Otvaranje detalja ture i prikaz mape sa key point-ovima
  3) Prijava kao Guide
  4) Kreiranje nove ture
  5) Dodavanje najmanje dva key point-a klikom na mapu
  6) Objavljivanje ture
  7) Provera da se tura pojavljuje u public listi

## What went well
- Leaflet mapa je uspešno integrisana i stabilno radi
- Jasno razdvojene public i guide funkcionalnosti
- UI prati backend poslovna pravila (min. 2 key point-a za publish)
- Dobra povezanost sa backend API-jem

## What to improve
- Unaprediti UI (vizuelni prikaz kartica i detalja)
- Dodati loading indikatore i bolje error poruke
- Refaktorisati map komponentu radi ponovne upotrebe

## Next Sprint plan (high level)
- Implementacija Cart & Purchase funkcionalnosti (Frontend Sprint 3)
