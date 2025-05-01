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
- [ ] Design schema for `tennis_matches` table  
- [ ] Build CSV/JSON ingestion for matches  
- [ ] Add tournament + year data model  
- [ ] Parse matches into match history records between players  
- [ ] Add player-metadata enrichment (e.g., Grand Slam titles, rankings if possible)  

---

## 📇 FLASHCARD ENGINE

### Flashcard Types
- [x] Define `PlayerFlashcard` model  
- [ ] Define `MatchFlashcard` model (head-to-head or specific match)  
- [ ] Add flashcard rendering methods (to plain text or HTML)  
- [ ] Add "pronunciation" field for flashcard (generated via CMUdict or eSpeak)  
- [ ] Support flashcard categories: `Player`, `Match`, `Tournament`, `FunFact`  

### Flashcard Storage & Serving
- [x] Serve flashcards from API  
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

## 🖥 FRONTEND (Optional - For Web Version)

- [ ] Minimal UI: input box, result panel  
- [ ] Integrate backend search API  
- [ ] Add filter by card type (Player / Match)  
- [ ] Add keyboard navigation through results  
- [ ] Add “Next Random Flashcard” button  
- [ ] Make page mobile responsive  
- [ ] Add animated reveal effect for flashcards  

---

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
