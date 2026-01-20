# Sprint Review — Sprint 3

## Sprint
- Sprint #: 3
- Period: Sprint 3
- Sprint Goal:
  Omogućiti turistima kompletan proces kupovine tura kroz korpu, primenu bonus poena i potvrdu kupovine sa trajnom perzistencijom.

## Completed
- [x] Implementirana korpa za turiste (dodavanje i uklanjanje tura)
- [x] Prikaz stavki u korpi i izračunavanje ukupne cene
- [x] Implementirana primena bonus poena pri kupovini
- [x] Pravilo da cena ne može ići ispod 0
- [x] Čuvanje neiskorišćenih bonus poena za buduće kupovine
- [x] Kreiranje zapisa o kupovini (Purchase) i stavki kupovine
- [x] Ažuriranje bonus poena turista nakon potvrde kupovine
- [x] Slanje potvrde kupovine putem email-a (dev/log implementacija)

## Demo Notes
- Demo scenario steps:
  1) Prijava kao turista i dobijanje JWT tokena
  2) Pregled published tura
  3) Dodavanje jedne ili više tura u korpu
  4) Provera ukupne cene u korpi
  5) Uklanjanje ture iz korpe
  6) Potvrda kupovine bez korišćenja bonus poena
  7) Potvrda kupovine sa korišćenjem bonus poena
  8) Provera ažuriranih bonus poena i zapisa o kupovini

## What went well
- Kupovina je implementirana kao jasan i logičan flow
- Poslovna pravila za bonus poene su pravilno sprovedena
- DDD razdvajanje odgovornosti (Domain/Application/Infrastructure) je očuvano
- API endpointi su pregledni i lako testabilni kroz Swagger

## What to improve
- Korpa je trenutno in-memory i može se dodatno unaprediti perzistencijom
- Email servis je razvojna implementacija i može se zameniti realnim providerom
- Dodati automatizovane testove za purchase flow

## Next Sprint plan (high level)
- Implementacija traženja zamene vodiča i automatskog otkazivanja tura
- Dodela bonus poena turistima u slučaju otkazivanja
- Uvođenje podsetnika za kupljene ture (email notifikacije)
