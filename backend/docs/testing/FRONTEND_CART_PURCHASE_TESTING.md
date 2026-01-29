# ?? Testiranje Cart & Purchase funkcionalnosti na Frontend-u

## ?? Preduslovi

### 1. Pokreni Backend API
```powershell
cd C:\Faks\psw\projektovanje-softvera\backend\src\TourApp.Api
dotnet run
```
? Backend radi na: `https://localhost:7143` (ili `http://localhost:5143`)

### 2. Pokreni Frontend aplikaciju
```powershell
cd C:\Faks\psw\projektovanje-softvera\frontend
npm install  # Samo prvi put
npm start    # ili: ng serve
```
? Frontend radi na: `http://localhost:4200`

---

## ?? Test Scenariji

### **Scenario 1: Dodavanje tura u korpu**

1. **Otvori pretraživa?**: `http://localhost:4200`

2. **Prijavi se kao turista**:
   - Username: `john_doe`
   - Password: `Tourist123!`

3. **Pregledaj dostupne ture**:
   - Idi na stranicu sa turama (npr. "Browse Tours" ili sli?no)
   - Trebalo bi da vidiš published ture

4. **Dodaj turu u korpu**:
   - Klikni na dugme "Add to Cart" / "Dodaj u korpu"
   - Trebalo bi da se pojavi notifikacija: "Tour added to cart"

5. **Proveri korpu**:
   - Klikni na ikonicu korpe (u navigaciji)
   - Trebalo bi da vidiš:
     - Naziv ture
     - Cenu ture
     - Ukupnu cenu

---

### **Scenario 2: Dodavanje više tura**

1. **Dodaj drugu turu**:
   - Vrati se na listu tura
   - Dodaj još jednu turu u korpu

2. **Proveri korpu**:
   - Ukupna cena = zbir cena dve ture
   - Trebalo bi da vidiš obe ture u listi

3. **Pokušaj dodati istu turu ponovo**:
   - Trebalo bi da se pojavi greška: "This tour is already in your cart"

---

### **Scenario 3: Uklanjanje ture iz korpe**

1. **Otvori korpu**:
   - Klikni na ikonicu korpe

2. **Ukloni turu**:
   - Klikni na dugme "Remove" / "Ukloni" pored ture
   - Tura bi trebalo da nestane iz korpe
   - Ukupna cena se automatski ažurira

---

### **Scenario 4: Kupovina BEZ bonus poena**

1. **Dodaj jednu turu u korpu** (npr. "Mountain Hiking Adventure" - $150)

2. **Idi u korpu i proveri prikaz**:
   - Original Price: $150
   - Available Bonus Points: 0 (ili koliko ve? imaš)
   - Total Price: $150

3. **Klikni "Confirm Purchase" / "Potvrdi kupovinu"**:
   - Ostavi checkbox "Use Bonus Points" **neselektovan**

4. **Proveri rezultat**:
   - Trebalo bi da dobiješ poruku: "Purchase successful!"
   - Korpa bi trebalo da bude prazna
   - Trebalo bi da vidiš detalje kupovine:
     - Final Price: $150
     - Bonus Points Used: 0
     - Bonus Points Earned: +10

---

### **Scenario 5: Kupovina SA bonus poenima**

#### 5a. **Pripremi bonus poene (ako nemaš)**
Prvo kupi jednu turu bez bonus poena da dobiješ 10 bonus poena.

#### 5b. **Dodaj turu u korpu**
Dodaj turu koja košta manje od tvojih bonus poena (npr. "City Art Gallery Tour" - $75)

#### 5c. **Potvrdi kupovinu sa bonus poenima**:
1. Idi u korpu
2. **Selektuj checkbox**: "Use Bonus Points"
3. Trebalo bi da vidiš:
   - Original Price: $75
   - Available Bonus Points: 10
   - Max Bonus Discount: $10
   - **Final Price: $65** (75 - 10 = 65)

4. Klikni "Confirm Purchase"

5. Proveri rezultat:
   - Final Price: $65
   - Bonus Points Used: 10
   - Bonus Points Earned: +10
   - Total Bonus Points: 10 (novo stanje)

---

### **Scenario 6: Bonus poeni ve?i od cene**

1. **Pripremi više bonus poena**: Kupi nekoliko tura da dobiješ npr. 200 bonus poena

2. **Dodaj jeftinu turu** (npr. "Sunset Beach Walk" - $45)

3. **Selektuj "Use Bonus Points"**

4. **Proveri da se cena ne spušta ispod 0**:
   - Bonus Points Used: 45 (a ne svih 200)
   - Final Price: **$0** (ne može biti negativna!)
   - Remaining Bonus Points: 200 - 45 + 10 = 165

---

### **Scenario 7: Prazna korpa**

1. **Otvori korpu bez stavki**

2. **Klikni "Confirm Purchase"**

3. **Trebalo bi da dobiješ grešku**: "Your cart is empty"

---

### **Scenario 8: Pregled istorije kupovina**

1. **Nakon potvrde kupovine**, idi na stranicu "My Purchases" / "Moje kupovine"

2. **Trebalo bi da vidiš**:
   - Listu svih kupovina
   - Datum kupovine
   - Kupljene ture
   - Cenu
   - Bonus poene iskoriš?ene/zadobijene

---

## ?? Provera Backend API-ja direktno (Swagger)

Ako frontend ne radi kako treba, možeš testirati direktno backend:

1. **Otvori Swagger**: `https://localhost:7143/swagger`

2. **Login i dobij token**:
   ```
   POST /api/auth/login
   {
     "username": "john_doe",
     "password": "Tourist123!"
   }
   ```
   Kopiraj `token` iz odgovora.

3. **Autentifikuj se** (klikni "Authorize" u Swagger-u i unesi token)

4. **Testiraj Cart endpointe**:
   ```
   GET /api/cart
   POST /api/cart/add { "tourId": "77777777-7777-7777-7777-777777777777" }
   POST /api/cart/remove { "tourId": "77777777-7777-7777-7777-777777777777" }
   ```

5. **Testiraj Purchase endpoint**:
   ```
   POST /api/purchase/confirm?useBonus=false
   GET /api/purchase/my
   ```

---

## ?? Seed Data (za testiranje)

Iz `DatabaseSeeder.cs`:

### Turisti
| Username | Password | Email | Bonus Points (po?etno) |
|----------|----------|-------|------------------------|
| john_doe | Tourist123! | john.doe@example.com | 0 |
| jane_smith | Tourist123! | jane.smith@example.com | 0 |
| bob_wilson | Tourist123! | bob.wilson@example.com | 0 |

### Published Tours (dostupne za kupovinu)
| Tour ID | Naziv | Cena |
|---------|-------|------|
| `77777777-7777-7777-7777-777777777777` | Mountain Hiking Adventure | $150 |
| `88888888-8888-8888-8888-888888888888` | City Art Gallery Tour | $75 |
| `99999999-9999-9999-9999-999999999999` | Food Market Experience | $95 |

---

## ? Checklist za testiranje

- [ ] Login kao turista radi
- [ ] Mogu da vidim published ture
- [ ] Mogu da dodam turu u korpu
- [ ] Ikonica korpe prikazuje broj stavki
- [ ] Mogu da vidim stavke u korpi
- [ ] Ukupna cena se pravilno ra?una
- [ ] Mogu da uklonim turu iz korpe
- [ ] Ne mogu dodati istu turu dva puta
- [ ] Potvrda kupovine bez bonus poena radi
- [ ] Potvrda kupovine sa bonus poenima radi
- [ ] Bonus poeni se pravilno oduzimaju
- [ ] Cena ne ide ispod 0
- [ ] Dobijam 10 bonus poena nakon svake kupovine
- [ ] Korpa se ?isti nakon potvrde kupovine
- [ ] Mogu da vidim istoriju kupovina
- [ ] Greška za praznu korpu se prikazuje

---

## ?? Ako nešto ne radi

1. **Proveri da li backend radi** (`https://localhost:7143/swagger`)
2. **Proveri konzolu u pretraživa?u** (F12 ? Console) za greške
3. **Proveri Network tab** u Developer Tools-u da vidiš API pozive
4. **Proveri da li si ulogovan** kao turista (Token mora biti validan)
5. **Restartuj backend i frontend** ako treba

---

## ?? Dodatne napomene

- **In-Memory korpa**: Korpa je trenutno ?uvana u memoriji backend-a, tako da ?e se izgubiti ako restartuje backend
- **Email potvrda**: Trenutno se samo loguje u konzolu (dev implementacija)
- **Bonus poeni**: Svaka kupovina daje +10 bonus poena, bez obzira na iznos

---

**Sre?no testiranje! ??**
