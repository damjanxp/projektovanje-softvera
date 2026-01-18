# Definition of Done (DoD) — TourApp (PSW)

Backlog item je “Done” kada:

## Kvalitet i specifikacija
- Implementacija prati specifikaciju (funkcionalno i poslovna pravila)
- Clean code: čitljivi nazivi, bez dupliranja, jasna struktura slojeva (DDD)

## Tehnički uslovi
- Projekat se uspešno build-uje (`dotnet build`)
- Migracije rade (EF Core) i baza može da se update-uje
- Swagger endpointi rade i vraćaju `ApiResponse<T>`
- Global exception handling radi (greške mapirane na status kodove)

## Testiranje
- Manual test scenario definisan (Swagger koraci)
- postojanje relevantnih testova pre implementacije (TDD princip)

## Dokumentacija i evidencija
- Kod je commit-ovan sa smislenom porukom (sprint inkrement)
- Backlog i sprint plan su ažurirani (status, deliverable)
