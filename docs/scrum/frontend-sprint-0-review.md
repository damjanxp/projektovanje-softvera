# Sprint Review — Frontend Sprint 0

## Sprint
- Sprint #: FE-0
- Period: Frontend Sprint 0
- Sprint Goal:
  Postaviti osnovnu Angular aplikaciju i infrastrukturu (routing, layout, API konfiguracija) kao temelj za dalji razvoj.

## Completed
- [x] Kreiran Angular projekat i osnovna struktura foldera (core/shared/features)
- [x] Konfigurisan routing i globalni layout (navbar + router-outlet)
- [x] Definisani environment fajlovi i base API URL konfiguracija
- [x] Implementiran ApiResponse model za rad sa backend odgovorima
- [x] Implementiran osnovni AuthService (token storage, session helpers)
- [x] Implementiran JWT HTTP interceptor (Authorization header)

## Demo Notes
- Demo scenario steps:
  1) Pokretanje aplikacije (`ng serve`)
  2) Navigacija kroz osnovne rute (Home, Login)
  3) Provera da aplikacija komunicira sa backend API (osnovni request / konfiguracija)
  4) Provera da interceptor dodaje Authorization header kada token postoji

## What went well
- Postavljena jasna arhitektura i struktura frontend projekta
- Osnovni mehanizmi za komunikaciju sa backendom su spremni
- Postavljen stabilan temelj za auth i feature module razvoj

## What to improve
- Dodati uniformni UI error handling i loading state (u narednim sprintovima)
- Definisati zajedničke UI komponente (shared) kako se aplikacija bude širila

## Next Sprint plan (high level)
- Implementacija login UI, AuthGuard i RoleGuard (role-based navigacija)
