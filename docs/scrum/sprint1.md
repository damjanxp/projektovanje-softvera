# Sprint 1 Plan — Authentication & User Management

## Sprint Goal
Omogućiti registraciju turista i prijavu svih korisnika uz bezbednost (JWT) i zaštitu sistema (blokiranje pogrešnih pokušaja).

## Sprint Backlog (US)
- US-A1 Registracija turista
- US-A2 Login + JWT
- US-A3 Blokiranje nakon 5 neuspešnih pokušaja
- US-A4 Admin pregled blokiranih + odblokiranje (osim 3. put)

## Deliverable (Inkrement)
- Turista može da se registruje i login-uje
- Admin/Guide postoje u bazi i mogu login
- Nakon 5 pogrešnih login pokušaja korisnik je blokiran
- Admin endpointi rade (list/unblock)

## Demo Scenario
1) POST register tourist
2) POST login tourist -> JWT
3) 5x pogrešan login -> blok
4) Login admin -> GET blocked users -> unblock
