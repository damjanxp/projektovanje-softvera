# Sprint Review — Sprint 1

## Sprint
- Sprint #: 1
- Period: Sprint 1
- Sprint Goal:
  Omogućiti bezbednu autentifikaciju korisnika i zaštitu sistema od zlonamernih pokušaja prijave.

## Completed
- [x] Registracija turista
- [x] Login korisnika uz JWT autentifikaciju
- [x] Role-based autorizacija (Tourist, Guide, Admin)
- [x] Praćenje neuspešnih login pokušaja
- [x] Privremeno blokiranje korisnika nakon 5 neuspešnih pokušaja
- [x] Admin pregled blokiranih korisnika
- [x] Admin odblokiranje korisnika (osim trećeg blokiranja)

## Demo Notes
- Demo scenario steps:
  1) Registracija novog turističkog naloga
  2) Uspešan login i dobijanje JWT tokena
  3) Višestruki pogrešni login pokušaji
  4) Automatsko blokiranje korisnika
  5) Login administratora
  6) Pregled i odblokiranje korisnika putem admin endpointa

## What went well
- JWT autentifikacija i autorizacija rade stabilno
- Bezbednosna pravila su pravilno implementirana
- Admin funkcionalnosti su jasno razdvojene i pregledne

## What to improve
- Dodati automatizovane testove za auth flow
- Poboljšati poruke o greškama za krajnje korisnike

## Next Sprint plan (high level)
- Implementacija upravljanja turama i ključnim tačkama
