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
  const matchId = document.getElementById("matchId").value.trim();
  const resultEl = document.getElementById("matchResult");
  resultEl.textContent = "";

  if (!matchId) {
    return displayMessage("matchResult", "Please fill all match fields.");
  }

  fetch(`${matchApiUrl}/${encodeURIComponent(matchId)}`)
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

      function highlightSetScore(wGame, lGame) {
        if (wGame == null && lGame == null) return ['-', '-'];
        if (wGame > lGame) return [
          `<span class="text-black font-bold">${wGame}</span>`,
          `<span class="text-gray-500">${lGame}</span>`
        ];
        if (lGame > wGame) return [
          `<span class="text-gray-500">${wGame}</span>`,
          `<span class="text-black font-bold">${lGame}</span>`
        ];
        return [wGame ?? '-', lGame ?? '-'];
      }

      const [firstServeW, firstServeL]       = highlightStat(result.winner.firstServe, result.loser.firstServe);
      const [firstServeWinW, firstServeWinL] = highlightStat(result.winner.firstServeWin, result.loser.firstServeWin);
      const [secondServeWinW, secondServeWinL] = highlightStat(result.winner.secondServeWin, result.loser.secondServeWin);
      const [acesW, acesL]                   = highlightStat(result.winner.aces, result.loser.aces);
      const [dfW, dfL]                       = highlightStat(result.winner.doubleFaults, result.loser.doubleFaults, true);
      const [bpW, bpL]                       = highlightStat(result.winner.breakPoints, result.loser.breakPoints);
      const [sgwW, sgwL]                     = highlightStat(result.winner.serviceGamesWon, result.loser.serviceGamesWon);
      const [tpwW, tpwL]                     = highlightStat(result.winner.totalPointsWon, result.loser.totalPointsWon);
      const [spwW, spwL]                     = highlightStat(result.winner.servePointsWon, result.loser.servePointsWon);
      const [rpwW, rpwL]                     = highlightStat(result.winner.receivingPointsWon, result.loser.receivingPointsWon);

      const finishNote = (result.gameFinishStatus === "Walkover" || result.gameFinishStatus === "Retired")
        ? `<div class="text-red-600 font-semibold text-sm mb-2">${result.gameFinishStatus.toUpperCase()}</div>`
        : "";

      let setHeader = '';
      let setRowW = '';
      let setRowL = '';
      for (let i = 1; i <= 5; i++) {
        const w = result.winner[`set${i}Games`];
        const l = result.loser[`set${i}Games`];
        if (w == null && l == null) continue;

        const [hw, hl] = highlightSetScore(w, l);
        setHeader += `<th class="p-2 text-center">Set ${i}</th>`;
        setRowW += `<td class="p-2">${hw}</td>`;
        setRowL += `<td class="p-2">${hl}</td>`;
      }

      resultEl.innerHTML = `
        <div class="match-summary mb-6">
          ${finishNote}
          <h3 class="text-2xl font-bold text-gray-900">${result.tournament}</h3>
          <p class="text-sm text-gray-500">${result.date} &bull; Surface: <span class="font-medium">${result.surface}</span></p>
        </div>

        <div class="mb-6">
          <table class="w-full text-sm table-auto border border-gray-200 rounded-lg overflow-hidden shadow-sm">
            <thead class="bg-gray-100 text-gray-700">
              <tr>
                <th class="p-2 text-left font-semibold text-gray-700">${result.round}</th>
                ${setHeader}
              </tr>
            </thead>
            <tbody class="bg-white text-center">
              <tr>
                <td class="text-left p-2">${result.winner.name}</td>
                ${setRowW}
              </tr>
              <tr>
                <td class="text-left p-2 text-gray-500">${result.loser.name}</td>
                ${setRowL}
              </tr>
            </tbody>
          </table>
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
            <tr><td class="p-3">${tpwW}</td><td class="font-medium text-gray-600">Total Points Won</td><td>${tpwL}</td></tr>
            <tr><td class="p-3">${spwW}</td><td class="font-medium text-gray-600">Serve Points Won</td><td>${spwL}</td></tr>
            <tr><td class="p-3">${rpwW}</td><td class="font-medium text-gray-600">Receiving Points Won</td><td>${rpwL}</td></tr>
          </tbody>
        </table>
      `;
    })
    .catch(err => {
      displayMessage("matchResult", "Error fetching match flashcard.");
      console.error(err);
    });
}

