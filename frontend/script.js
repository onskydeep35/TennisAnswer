const playerApiUrl = "http://localhost:5019/api/player";
const matchApiUrl  = "http://localhost:5019/api/match";
const authApiUrl   = "http://localhost:5019/api/auth";

let currentUser = null;

function showFlashcardsUI() {
  document.getElementById("authContainer").classList.add("hidden");
  document.getElementById("container").classList.remove("hidden");
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

function searchPlayerFlashcard() {
  const input = document.getElementById("playerInput").value.trim();
  const resultEl = document.getElementById("playerResult");
  resultEl.textContent = "";

  if (!input.includes(" ")) {
    return displayMessage("playerResult", "Please enter both first and last name.");
  }

  fetch(`${playerApiUrl}/${encodeURIComponent(input)}`)
    .then(res => res.ok ? res.json() : res.text())
    .then(result => {
      if (typeof result === "string") {
        resultEl.textContent = result;
        return;
      }

      resultEl.innerHTML = `
        <div class="bg-blue-50 border-l-4 border-blue-500 text-left p-4 rounded-md shadow">
          <h3 class="text-lg font-semibold mb-1">${result.name}</h3>
          <p>✋ Hand: <strong>${result.hand}</strong></p>
          <p>🎂 DOB: <strong>${result.dateOfBirth}</strong></p>
          <p>📏 Height: <strong>${result.height} cm</strong></p>
          <p>🌍 Country: <strong>${result.country} (${result.ioc})</strong></p>
          <p>🔗 WikiData ID: <strong>${result.wikiDataId}</strong></p>
        </div>
      `;
    })
    .catch(err => {
      displayMessage("playerResult", "Error fetching player flashcard.");
      console.error(err);
    });
}

function searchMatchFlashcard() {
  const tourneyId = document.getElementById("matchTourneyId").value.trim();
  const winnerId = document.getElementById("matchWinnerId").value.trim();
  const loserId = document.getElementById("matchLoserId").value.trim();
  const resultEl = document.getElementById("matchResult");
  resultEl.textContent = "";

  if (!tourneyId || !winnerId || !loserId) {
    return displayMessage("matchResult", "Please fill all match fields.");
  }

  fetch(`${matchApiUrl}/${encodeURIComponent(tourneyId)}/${encodeURIComponent(winnerId)}/${encodeURIComponent(loserId)}`)
    .then(res => res.ok ? res.json() : res.text())
    .then(result => {
      if (typeof result === "string") {
        resultEl.textContent = result;
        return;
      }

      function highlightStat(wVal, lVal, lowerIsBetter = false) {
        const w = parseFloat(wVal);
        const l = parseFloat(lVal);
        const [highlightW, highlightL] = [
          `<span class="bg-blue-100 px-2 py-1 rounded-full font-bold text-blue-800">${wVal}</span>`,
          `<span class="bg-blue-100 px-2 py-1 rounded-full font-bold text-blue-800">${lVal}</span>`
        ];
        if (isNaN(w) || isNaN(l)) return [`<span>${wVal}</span>`, `<span>${lVal}</span>`];

        const winnerFirst = (!lowerIsBetter && w > l) || (lowerIsBetter && w < l);
        const loserFirst  = (!lowerIsBetter && l > w) || (lowerIsBetter && l < w);
        return [
          winnerFirst ? highlightW : `<span>${wVal}</span>`,
          loserFirst ? highlightL : `<span>${lVal}</span>`
        ];
      }

      const [firstServeW, firstServeL]       = highlightStat(result.winner.firstServe, result.loser.firstServe);
      const [firstServeWinW, firstServeWinL] = highlightStat(result.winner.firstServeWin, result.loser.firstServeWin);
      const [secondServeWinW, secondServeWinL] = highlightStat(result.winner.secondServeWin, result.loser.secondServeWin);
      const [acesW, acesL]                   = highlightStat(result.winner.aces, result.loser.aces);
      const [dfW, dfL]                       = highlightStat(result.winner.doubleFaults, result.loser.doubleFaults, true); // lower is better
      const [bpW, bpL]                       = highlightStat(result.winner.breakPoints, result.loser.breakPoints);
      const [sgwW, sgwL]                     = highlightStat(result.winner.serviceGamesWon, result.loser.serviceGamesWon);

      resultEl.innerHTML = `
        <div class="match-summary mb-6">
          <h3 class="text-2xl font-bold text-gray-900">${result.tournament}</h3>
          <p class="text-sm text-gray-500">${result.date} &bull; ${result.round} &bull; Surface: <span class="font-medium">${result.surface}</span></p>
          <div class="mt-4 inline-block bg-blue-100 text-blue-900 font-semibold px-4 py-2 rounded-lg shadow-sm text-base">
            Final Score: ${result.score}
          </div>
        </div>

        <table class="w-full text-sm table-auto border border-gray-300 shadow-sm rounded-lg overflow-hidden">
          <thead class="bg-gray-50 text-gray-700">
            <tr>
              <th class="p-3 text-center">${result.winner.name}</th>
              <th class="p-3 text-center">Stat</th>
              <th class="p-3 text-center">${result.loser.name}</th>
            </tr>
          </thead>
          <tbody class="bg-white text-center">
            <tr><td class="p-3">${firstServeW}</td><td class="font-medium text-gray-600">First Serve %</td><td>${firstServeL}</td></tr>
            <tr><td class="p-3">${firstServeWinW}</td><td class="font-medium text-gray-600">First Serve Win %</td><td>${firstServeWinL}</td></tr>
            <tr><td class="p-3">${secondServeWinW}</td><td class="font-medium text-gray-600">Second Serve Win %</td><td>${secondServeWinL}</td></tr>
            <tr><td class="p-3">${acesW}</td><td class="font-medium text-gray-600">Aces</td><td>${acesL}</td></tr>
            <tr><td class="p-3">${dfW}</td><td class="font-medium text-gray-600">Double Faults</td><td>${dfL}</td></tr>
            <tr><td class="p-3">${bpW}</td><td class="font-medium text-gray-600">Break Points Won</td><td>${bpL}</td></tr>
            <tr><td class="p-3">${sgwW}</td><td class="font-medium text-gray-600">Service Games Won</td><td>${sgwL}</td></tr>
          </tbody>
        </table>
      `;
    })
    .catch(err => {
      displayMessage("matchResult", "Error fetching match flashcard.");
      console.error(err);
    });
}