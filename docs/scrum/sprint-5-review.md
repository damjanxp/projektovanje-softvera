# Sprint Review — Sprint 5

## Sprint
- Sprint #: 5
- Period: Sprint 5
- Sprint Goal:
  Implementirati napredne funkcionalnosti sistema: ocenjivanje tura, prijavu problema i praćenje promena statusa problema kroz event log, uz verifikaciju poslovnih pravila kroz unit testove (TDD).

## Completed
- [x] Omogućeno ocenjivanje kupljenih tura (ocena 1–5)
- [x] Implementirana obavezna tekstualna poruka za ocene 1 i 2
- [x] Ograničeno ocenjivanje na period nakon održavanja ture
- [x] Ograničeno ocenjivanje na maksimalno 7 dana nakon datuma ture
- [x] Implementirana prijava problema za kupljene ture
- [x] Definisani statusi problema: Pending, Resolved, InReview, Rejected
- [x] Omogućene promene statusa problema od strane vodiča i administratora
- [x] Implementiran event log za svaku promenu statusa problema (event sourcing)
- [x] Dodati unit testovi za ključna poslovna pravila (Domain i Application sloj)

## Demo Notes
- Demo scenario steps:
  1) Prijava kao turista
  2) Kreiranje ocene za kupljenu turu (validan i nevalidan slučaj)
  3) Pokušaj ocenjivanja pre datuma ture (zabranjeno)
  4) Prijava problema za kupljenu turu
  5) Prijava kao vodič i promena statusa problema (Resolve / Send to Review)
  6) Prijava kao administrator i odbacivanje problema
  7) Pregled event loga za problem (lista promena statusa)

## What went well
- Poslovna pravila za ocenjivanje su jasno definisana i pravilno sprovedena
- Implementacija problema i njihovog životnog ciklusa je pregledna i dosledna
- Event sourcing je uspešno primenjen za praćenje promena statusa
- Unit testovi pokrivaju ključne scenarije i potvrđuju korektnost implementacije

## What to improve
- Event sourcing je implementiran u minimalnom obimu i može se proširiti
- Dodati dodatne testove za edge-case scenarije
- Uvesti automatizovano izvršavanje testova u CI pipeline

## Next Sprint plan (high level)
- Front end aplikacije

