# TODO – Music Flashcard App

very very very very very initial todo

Organized by functional areas: Backend + Data, Frontend, General Improvements

---

## BACKEND + DATA (ASP.NET Core + Storage Architecture)

### Core API Development
- [x] Set up ASP.NET Core Web API project
- [x] Define `Flashcard` model
- [x] Implement `GET /flashcard` and `POST /flashcard` endpoints
- [x] Enable CORS for frontend
- [x] Load flashcards from `flashcards.json`
- [x] Validate question/answer input on POST
- [x] Add detailed logging for flashcard load and POST
- [ ] Add `GET /flashcard/{question}` for direct query
- [ ] Add filtering support by category
- [ ] Add random flashcard endpoint

### Serialization & Storage Transition
- [x] Store flashcards in `flashcards.json` for development
- [ ] Define Bond schema for `Flashcard` object
- [ ] Generate Bond C# classes using `bondc`
- [ ] Migrate serialization logic from JSON → Bond
- [ ] Replace List-based storage with key/value structure (question → Flashcard)
- [ ] Create key/value-compatible read/write helper methods
- [ ] Introduce abstraction layer to switch between raw JSON and Bond-based schema
- [ ] Add schema compatibility testing for Bond evolution

---

## FRONTEND (HTML + JavaScript)

- [x] Build minimal UI with search box and answer result
- [x] Connect frontend to API using `fetch`
- [x] Style with centered layout and colors
- [x] Display “No match found” if question doesn't exist
- [ ] Add support for searching partial questions (fuzzy match)
- [ ] Add dropdown filter by flashcard category
- [ ] Add “Next Random” flashcard button
- [ ] Create UI to add a new flashcard
- [ ] Add animated card reveal effect
- [ ] Make page mobile responsive

---

## GENERAL IMPROVEMENTS

- [x] Create TODO.md to track tasks
- [ ] Add `README.md` with setup, usage, and build instructions
- [ ] Add test project with basic xUnit coverage
- [ ] Add logging for all API calls
- [ ] Create GitHub repo and push code
- [ ] Create `LICENSE` (MIT preferred)
- [ ] Add GitHub Issues for tracking features & bugs

---

## FUTURE IDEAS

- [ ] Support multiple languages for flashcards
- [ ] Group flashcards into decks
- [ ] Add spaced repetition scheduling
- [ ] Use localStorage or IndexedDB in browser for offline caching
- [ ] Create Chrome/Edge browser extension for quick review mode
