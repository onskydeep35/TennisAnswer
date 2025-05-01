const flashcardApiUrl = "http://localhost:5019/flashcard";
const authApiUrl = "http://localhost:5019/api/auth";

let flashcards = [];
let currentUser = null;

function showFlashcardsUI() {
  document.getElementById("authContainer").style.display = "none";
  document.getElementById("container").style.display = "block";
  loadFlashcards();
}

function displayMessage(id, msg) {
  document.getElementById(id).textContent = msg;
}

function register() {
  const email = document.getElementById("regEmail").value;
  const password = document.getElementById("regPassword").value;

  if (!email || !password) return displayMessage("authResult", "Fill both fields.");

  fetch(`${authApiUrl}/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password })
  })
    .then(res => res.ok ? login(email, password) : res.text())
    .then(res => {
      if (typeof res === "string") displayMessage("authResult", res);
    });
}

function login(emailOverride = null, passwordOverride = null) {
  const email = emailOverride || document.getElementById("loginEmail").value;
  const password = passwordOverride || document.getElementById("loginPassword").value;

  if (!email || !password) return displayMessage("authResult", "Fill both fields.");

  return fetch(`${authApiUrl}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password })
  })
    .then(res => res.ok ? res.json() : res.text())
    .then(result => {
      if (typeof result === "string") {
        displayMessage("authResult", result);
      } else {
        currentUser = result;
        displayMessage("authResult", `Welcome, ${result.name}`);
        showFlashcardsUI();
      }
    });
}

function loadFlashcards() {
  fetch(flashcardApiUrl)
    .then(res => res.json())
    .then(data => {
      flashcards = data;
      console.log("Flashcards loaded:", flashcards);
    })
    .catch(err => console.error("Error loading flashcards:", err));
}

function searchFlashcard() {
  const userInput = document.getElementById("questionInput").value.trim().toLowerCase();
  const result = flashcards.find(f => f.question.toLowerCase() === userInput);

  const answerElement = document.getElementById("answerResult");
  if (result) {
    answerElement.textContent = `Answer: ${result.answer}`;
  } else {
    answerElement.textContent = "No matching flashcard found.";
  }
}
