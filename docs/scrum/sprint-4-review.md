# Sprint Review — Sprint 4

## Sprint
- Sprint #: 4
- Period: Sprint 4
- Sprint Goal:
  Omogućiti traženje zamene vodiča za turu, automatsko otkazivanje tura bez zamene i slanje email podsetnika turistima.

## Completed
- [x] Omogućeno označavanje ture za traženje zamene od strane vodiča
- [x] Implementiran pregled tura koje traže zamenu (replacement board) za vodiče
- [x] Implementirano preuzimanje ture od strane drugog vodiča uz proveru datuma
- [x] Promena vodiča na turi nakon uspešnog preuzimanja
- [x] Automatsko otkazivanje ture ukoliko se zamena ne pronađe 24h pre početka
- [x] Promena statusa ture u Canceled
- [x] Dodela bonus poena turistima koji su kupili otkazanu turu
- [x] Implementiran email podsetnik turistima 48h pre početka ture (dev/log)

## Demo Notes
- Demo scenario steps:
  1) Prijava vodiča i označavanje ture za zamenu
  2) Prijava drugog vodiča i pregled replacement board-a
  3) Preuzimanje ture (ako nema konflikta sa datumom)
  4) Provera promene vodiča na turi
  5) Simulacija isteka roka od 24h bez zamene
  6) Automatsko otkazivanje ture
  7) Provera dodele bonus poena turistima
  8) Slanje email podsetnika 48h pre ture (log output)

## What went well
- Poslovna pravila za zamenu i otkazivanje su jasno i korektno implementirana
- Automatizovani procesi (cancel i reminder) rade bez intervencije korisnika
- Bonus poeni su pravilno dodeljeni u skladu sa specifikacijom
- Sistem ponašanja je konzistentan i lako demonstrabilan

## What to improve
- Background jobovi su trenutno pokretani ručno ili na startup
- Email servis je razvojna implementacija i može se unaprediti
- Dodati automatizovane testove za edge-case scenarije (datumi, konflikati)

## Next Sprint plan (high level)
- Implementacija ocenjivanja kupljenih tura
- Prijava problema na turi i upravljanje statusima
- Evidencija promena statusa problema (event sourcing)
