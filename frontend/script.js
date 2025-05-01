const playerApiUrl = "http://localhost:5019/api/player";
const matchApiUrl  = "http://localhost:5019/api/match";
const authApiUrl   = "http://localhost:5019/api/auth";

let currentUser = null;

function showFlashcardsUI() {
  document.getElementById("authContainer").style.display = "none";
  document.getElementById("container").style.display = "block";
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

// ------------------ Player Flashcard ------------------
function searchPlayerFlashcard() {
  const userInput = document.getElementById("playerInput").value.trim();
  const resultEl = document.getElementById("playerResult");
  resultEl.textContent = "";

  if (!userInput.includes(" ")) {
    return displayMessage("playerResult", "Please enter both first and last name.");
  }

  fetch(`${playerApiUrl}/${encodeURIComponent(userInput)}`)
    .then(res => res.ok ? res.json() : res.text())
    .then(result => {
      if (typeof result === "string") {
        resultEl.textContent = result;
        return;
      }

      resultEl.innerHTML = `
        <strong>${result.name}</strong><br>
        ✋ Hand: ${result.hand}<br>
        🎂 DOB: ${result.dateOfBirth}<br>
        📏 Height: ${result.height} cm<br>
        🌎 Country: ${result.country} (${result.ioc})<br>
        🔗 WikiData: ${result.wikiDataId}
      `;
    })
    .catch(err => {
      displayMessage("playerResult", "Error fetching player flashcard.");
      console.error(err);
    });
}

// ------------------ Match Flashcard ------------------
function searchMatchFlashcard() {
  const tourneyId = document.getElementById("matchTourneyId").value.trim();
  const winnerId = document.getElementById("matchWinnerId").value.trim();
  const loserId  = document.getElementById("matchLoserId").value.trim();
  const resultEl = document.getElementById("matchResult");
  resultEl.textContent = "";

  if (!tourneyId || !winnerId || !loserId) {
    return displayMessage("matchResult", "Please fill all match fields.");
  }

  const url = `${matchApiUrl}/${encodeURIComponent(tourneyId)}/${encodeURIComponent(winnerId)}/${encodeURIComponent(loserId)}`;

  fetch(url)
    .then(res => res.ok ? res.json() : res.text())
    .then(result => {
      if (typeof result === "string") {
        resultEl.textContent = result;
        return;
      }

      resultEl.innerHTML = `
        <strong>${result.tournament} (${result.date})</strong><br>
        🏟 Surface: ${result.surface}<br>
        🔄 Round: ${result.round}<br>
        📊 Score: ${result.score}<br>
        <hr>
        <strong>Winner:</strong> ${result.winner.name} (${result.winner.hand})<br>
        Aces: ${result.winner.aces}, DF: ${result.winner.doubleFaults}<br>
        <strong>Loser:</strong> ${result.loser.name} (${result.loser.hand})<br>
        Aces: ${result.loser.aces}, DF: ${result.loser.doubleFaults}
      `;
    })
    .catch(err => {
      displayMessage("matchResult", "Error fetching match flashcard.");
      console.error(err);
    });
}