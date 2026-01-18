# Sprint Review — Sprint 2

## Sprint
- Sprint #: 2
- Period: Sprint 2
- Sprint Goal:
  Omogućiti vodičima kreiranje i upravljanje turama, uz pregled objavljenih tura turistima.

## Completed
- [x] Kreiranje ture u draft stanju
- [x] Dodavanje ključnih tačaka (geografske koordinate, naziv, opis, slika)
- [x] Validacija koordinata i ulaznih podataka
- [x] Pravila za objavljivanje ture (minimum 2 ključne tačke)
- [x] Publish ture i promena statusa
- [x] Pregled sopstvenih tura za vodiča
- [x] Sortiranje tura po datumu održavanja
- [x] Javni pregled published tura (turista/anon)

## Demo Notes
- Demo scenario steps:
  1) Login vodiča
  2) Kreiranje nove ture (draft)
  3) Dodavanje najmanje dve ključne tačke
  4) Objavljivanje ture
  5) Pregled published tura kao anoniman korisnik

## What went well
- Poslovna pravila za ture su jasno implementirana
- DDD agregati su pravilno modelovani
- Razdvajanje draft i published stanja radi kako je očekivano

## What to improve
- Poboljšati validacije za kompleksnije slučajeve
- Dodati detaljniji prikaz ture (npr. mapa) u narednim fazama

## Next Sprint plan (high level)
- Implementacija kupovine tura, korpe i bonus poena
