# Product Backlog — TourApp (PSW 2025/2026)


## Legenda
- Priority: P0 (kritično), P1 (važno), P2 (poželjno)
- Status: Backlog / In Progress / Done
- Acceptance Criteria (AC): uslovi za “Done”

---

## EPIC A — Authentication & Users

### US-A1: Registracija turista
**Priority:** P0  
**Opis:** Kao turista želim da se registrujem (username, password, ime, prezime, email, interesovanja, wantsRecommendations) kako bih koristio sistem.  
**AC:**
- Registracija kreira turistu u bazi
- Username i email su jedinstveni
- Password se čuva kao hash
- Interesovanja su iz skupa: Nature/Art/Sport/Shopping/Food
- API vraća validan ApiResponse

### US-A2: Prijava korisnika (JWT)
**Priority:** P0  
**Opis:** Kao korisnik želim da se prijavim i dobijem JWT token.  
**AC:**
- Login vraća token + role + username
- Token omogućava pristup zaštićenim endpointima
- Role-based autorizacija radi (Tourist/Guide/Admin)

### US-A3: Blokiranje nakon 5 pogrešnih pokušaja
**Priority:** P0  
**Opis:** Sistem blokira korisnike koji 5 puta pogreše login.  
**AC:**
- Nakon 5 neuspešnih pokušaja korisnik je blokiran
- Blokiran korisnik ne može login
- Brojač se resetuje nakon uspešnog logina

### US-A4: Admin pregled i odblokiranje
**Priority:** P0  
**Opis:** Admin vidi blokirane korisnike i može odblokirati osim ako je blokiran 3. put.  
**AC:**
- Admin endpoint vraća listu blokiranih sa blockCount
- Admin može odblokirati ako blockCount < 3
- Ako blockCount >= 3, odblokiranje odbijeno (409)

---

## EPIC B — Tours & Key Points

### US-B1: Kreiranje ture (draft)
**Priority:** P0  
**Opis:** Kao vodič želim da kreiram turu sa podacima (naziv, opis, težina, kategorija, cena, datum) u draft stanju.  
**AC:**
- Tura se kreira kao Draft
- Tura u draft nije vidljiva turistima

### US-B2: Dodavanje ključnih tačaka
**Priority:** P0  
**Opis:** Kao vodič želim da dodam ključne tačke (lat, lng, naziv, opis, slika) za turu.  
**AC:**
- Key point se vezuje za turu
- Validacija lat/lng opsega
- Vodič može menjati samo svoje ture

### US-B3: Publish ture
**Priority:** P0  
**Opis:** Kao vodič želim da objavim turu (Published) kada je spremna.  
**AC:**
- Publish dozvoljen samo ako postoje osnovni podaci i min 2 key point-a
- Published ture su vidljive turistima

### US-B4: Pregled svojih tura + sortiranje
**Priority:** P1  
**Opis:** Kao vodič želim da vidim svoje ture i sortiram po datumu (asc/desc).  
**AC:**
- Endpoint vraća sve ture vodiča
- Sortiranje radi

### US-B5: Pregled published tura (turista/anon)
**Priority:** P1  
**Opis:** Kao turista želim da vidim sve published ture.  
**AC:**
- Samo Published ture se vraćaju
- Sortiranje radi

---

## EPIC C — Shopping: Cart & Purchase

### US-C1: Korpa (dodavanje/uklanjanje)
**Priority:** P0  
**Opis:** Kao turista želim da dodajem ture u korpu i uklanjam ih, uz prikaz ukupne cene.  
**AC:**
- Korpa pamti odabrane ture
- Ukupna cena je zbir cena tura
- Uklanjanje iz korpe radi

### US-C2: Bonus poeni
**Priority:** P0  
**Opis:** Kao turista želim da iskoristim bonus poene za umanjenje cene do minimum 0.  
**AC:**
- Cena ne može ići u minus
- Neiskorišćeni bonus ostaje za buduće kupovine

### US-C3: Potvrda kupovine + email
**Priority:** P0  
**Opis:** Kao turista želim da potvrdim kupovinu i dobijem email sa informacijama o kupljenim turama.  
**AC:**
- Kreira se purchase zapis u bazi
- Email sadrži osnovne info o svakoj kupljenoj turi

---

## EPIC D — Guide replacement & cancellation

### US-D1: Traženje zamene
**Priority:** P1  
**Opis:** Kao vodič želim da označim turu za zamenu.  
**AC:**
- Tura se pojavljuje na “replacement board” (samo vodiči)
- Vodič može ponuditi zamenu samo ako nema turu tog datuma

### US-D2: Otkazivanje 24h pre ako nema zamene + bonus poeni
**Priority:** P1  
**Opis:** Ako se 24h pre ture ne nađe zamena, tura se otkazuje i turistima se dodeljuju bonus poeni u visini cene ture.  
**AC:**
- Automatska provera 24h pre datuma
- Status ture postaje Canceled
- Bonus poeni dodeljeni svim kupcima

---

## EPIC E — Notifications

### US-E1: Podsetnik 48h pre ture
**Priority:** P1  
**Opis:** Sistem šalje email podsetnik 48h pre kupljene ture.  
**AC:**
- Background job šalje podsetnik svim kupcima ture

### US-E2: Preporuke po interesovanjima
**Priority:** P2  
**Opis:** Sistem šalje email turistima kada se objavi tura iz kategorije koja odgovara interesovanjima (ako je opcija uključena).  
**AC:**
- Poštuje se wantsRecommendations flag
- Turista može promeniti interesovanja i opciju

---

## EPIC F — Rating

### US-F1: Ocenjivanje kupljene ture
**Priority:** P1  
**Opis:** Turista može oceniti turu (1–5) nakon datuma održavanja; komentar je obavezan za ocene 1–2; ocenjivanje moguće do 7 dana.  
**AC:**
- Nije moguće oceniti pre datuma održavanja
- Komentar obavezan za 1–2
- Nakon 7 dana ocenjivanje zabranjeno

---

## EPIC G — Problems & Event sourcing

### US-G1: Prijava problema na turi
**Priority:** P2  
**Opis:** Turista prijavljuje problem (naziv, opis) za kupljenu turu.  
**AC:**
- Problem kreiran sa statusom “Pending”
- Vodič dobija obaveštenje

### US-G2: Promene statusa i eventi
**Priority:** P2  
**Opis:** Svaka promena statusa problema proizvodi event i čuva se istorija promena.  
**AC:**
- Statusi: Pending, Resolved, InReview, Rejected
- Event log postoji za svaku promenu
