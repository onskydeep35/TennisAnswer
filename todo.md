# ✅ TODO – Tennis Flashcard Data & Search Tool

Organized by **Functional Areas**: Backend + Data Ingestion, Flashcard Engine, Search System, Frontend (Optional), and General Improvements.

---

## 🎾 BACKEND + DATA INGESTION (ASP.NET Core API + SQLite)

### Core API & Auth
- [x] Set up ASP.NET Core Web API project  
- [x] Set up basic user authorization system  
- [x] Define `Flashcard` models and routing structure  
- [x] Create API controller(s) for accessing flashcards  
- [x] Enable CORS for frontend  
- [x] Log API requests and errors  

### Player & Match Data Ingestion
- [x] Create SQLite database and `tennis_players` table  
- [x] Implement CSV ingestion for players (`PlayerProcessor`)  
- [x] Implement player-to-country and hand mapping (via `Constants`)  
- [x] Create ingestion logic with table recreation support (`DROP IF EXISTS`)  
- [x] Develop API to retrieve player flashcards by full name  
- [x] Display player flashcards in frontend via search  
- [X] Design schema for `tennis_matches` table  
- [X] Build CSV ingestion logic for `tennis_matches`  
- [X] Create ingestion runner for match data (with schema alignment)  
- [ ] Develop `MatchFlashcardController` to expose matches via API  
- [ ] Add tournament + year data model  
- [ ] Parse matches into match history records between players  
- [ ] Add player-metadata enrichment (e.g., Grand Slam titles, rankings if possible)  

---

## 📇 FLASHCARD ENGINE

### Flashcard Types
- [x] Define `PlayerFlashcard` model (API controller & UI integrated)  
- [ ] Define `MatchFlashcard` model (head-to-head or specific match)  
- [ ] Add flashcard rendering methods (to plain text or HTML)  
- [ ] Add "pronunciation" field for flashcard (generated via CMUdict or eSpeak)  
- [ ] Support flashcard categories: `Player`, `Match`, `Tournament`, `FunFact`  

### Flashcard Storage & Serving
- [x] Serve flashcards from API (`GET /api/player/{name}`)  
- [ ] Support `GET /flashcard/{question}`  
- [ ] Add filtering by flashcard category  
- [ ] Add `GET /flashcard/random` endpoint  
- [ ] Migrate to key/value structure: `question → flashcard object`  
- [ ] (Optional) Create Bond schema for flashcards  

---

## 🔎 INTERACTIVE SEARCH SYSTEM

### Search Engine & Matching Logic
- [ ] Implement autocomplete engine (`"Rog"` → `"Roger Federer"`)  
- [ ] Add fuzzy matching (e.g., `"Fed vs Nadal"` → `MatchFlashcard`)  
- [ ] Tokenize queries to detect player-vs-player patterns  
- [ ] Support query routing:
  - Single player → `PlayerFlashcard`  
  - "A vs B" → `MatchFlashcard`  
- [ ] Add query parser class to handle special patterns (year, surface, tournament)  

### Search Input UX (Console or Web)
- [ ] Show suggestions as user types  
- [ ] Display matched flashcard on selection  
- [ ] Show fallback or "no match found" message  

---

## 🖥 FRONTEND (For Web Version)

- [x] Minimal UI: input box, result panel  
- [x] Integrate backend player flashcard API  
- [X] Minimal UI for player flashcard
- [ ] Improve UI for player flashcard
- [ ] Integrate backend match flashcard API  
- [ ] Minimal UI for match flashcard
- [ ] Improve UI for match flashcard
- [ ] Add filter by card type (Player / Match)  
- [ ] Add keyboard navigation through results  

## Extension
- [ ] Implement browser extension for this application

## ✅ TESTING

- [ ] ✅ Unit test for `PlayerProcessor.ReadFromCsv()` with valid and invalid rows  
- [ ] ✅ Unit test for player hand and IOC mapping (L → Left-Handed, etc.)  
- [ ] ✅ Integration test for `PlayerIngester.IngestPlayers()` using in-memory SQLite  
- [ ] ✅ Test that `EnsureTableExists()` drops and recreates the table properly  
- [ ] ✅ API test: `GET /api/player/{fullName}` returns valid flashcard JSON  
- [ ] ✅ API test: returns 404 for unknown player name  
- [ ] ✅ Test: CSV ingestion skips malformed rows gracefully  
- [ ] ✅ Smoke test: full ingestion from CSV to API GET success  
- [ ] ✅ Unit test for DOB format parsing (yyyyMMdd edge cases)  
- [ ] ✅ Test frontend logic: displays correct player data on match  
- [ ] ✅ Test frontend logic: displays "not found" when no match  

## ⚙ GENERAL IMPROVEMENTS

- [x] Create `TODO.md` for planning  
- [ ] Add `README.md` with overview, usage, and setup instructions  
- [ ] Add test project with xUnit for data ingestion and parsing  
- [ ] Add integration tests for flashcard API  
- [ ] Create GitHub repo for version tracking  
- [ ] Add `LICENSE` (MIT)  

---

## 🚀 FUTURE IDEAS

- [ ] Add `TournamentFlashcard` (e.g., Wimbledon 2019 overview)  
- [ ] Add pronunciation/audio support using eSpeak or pre-generated MP3s  
- [ ] Add full player match history lookup  
- [ ] Create quiz or training mode with spaced repetition  
- [ ] Enable browser extension for in-page flashcard lookup  
- [ ] Develop lightweight API + mobile-first UI  