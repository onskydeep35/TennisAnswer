const playerApiUrl = "http://localhost:5019/api/player";
const matchApiUrl = "http://localhost:5019/api/match";
const authApiUrl = "http://localhost:5019/api/auth";

let currentUser = null;

function showFlashcardsUI() {
  const authEl = document.getElementById("authContainer");
  const mainEl = document.getElementById("container");

  if (authEl) authEl.classList.add("hidden");
  if (mainEl) {
    mainEl.classList.remove("hidden");
    mainEl.classList.add("animate-fade-in");
  } else {
    console.error("Element with id='container' not found in DOM.");
  }
}

function displayMessage(id, msg) {
  document.getElementById(id).textContent = msg;
}

function toggleAuthView(showRegister) {
  const loginForm = document.getElementById("loginForm");
  const registerForm = document.getElementById("registerForm");
  if (showRegister) {
    loginForm.classList.add("hidden");
    registerForm.classList.remove("hidden");
  } else {
    registerForm.classList.add("hidden");
    loginForm.classList.remove("hidden");
  }
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
  const name = document.getElementById("playerInput").value.trim();
  const playerResult = document.getElementById("playerResult");
  const matchResult = document.getElementById("matchResult");
  playerResult.innerHTML = "";
  matchResult.innerHTML = "";

  if (!name.includes(" ")) {
    return displayMessage("playerResult", "Please enter full name.");
  }

  playerResult.innerHTML = `<div class='text-center text-gray-500'>Loading...</div>`;

  fetch(`${playerApiUrl}/${encodeURIComponent(name)}`)
    .then(res => res.ok ? res.json() : Promise.reject("Player not found"))
    .then(player => {
      playerResult.innerHTML = `
        <div class="text-left border rounded-lg p-4 bg-white shadow">
          <h2 class="text-xl font-semibold mb-2">${player.name}</h2>
          <p><strong>Date of Birth:</strong> ${player.dateOfBirth}</p>
          <p><strong>Country:</strong> ${player.country} (${player.ioc})</p>
          <p><strong>Height:</strong> ${player.height} cm</p>
          <p><strong>Plays:</strong> ${player.hand}</p>
        </div>
      `;
      return fetch(`${playerApiUrl}/matches/${player.id}`);
    })
    .then(res => res.ok ? res.json() : Promise.reject("No matches found"))
    .then(matches => {
      const grouped = groupBy(matches, m => `${m.tournament}_${m.tournamentYear}`);

      const scrollWrapper = document.createElement("div");
      scrollWrapper.className = "relative mt-6";

      const container = document.createElement("div");
      container.className = "flex gap-6 overflow-x-auto pb-4 scroll-smooth";
      container.id = "tournamentScroll";

      Object.entries(grouped).forEach(([key, games]) => {
        const [tournament, year] = key.split("_");
        games.sort((a, b) => b.matchNum - a.matchNum);

        const surface = games[0].surface?.toLowerCase();
        const bgColor = surface === "clay" ? "bg-orange-100"
                       : surface === "hard" ? "bg-blue-100"
                       : surface === "grass" ? "bg-green-100"
                       : surface === "carpet" ? "bg-yellow-100"
                       : "bg-gray-100";

        const col = document.createElement("div");
        col.className = "min-w-[260px] flex-shrink-0";

        const header = document.createElement("div");
        header.className = `${bgColor} p-3 rounded-lg shadow text-center mb-3`;
        header.innerHTML = `
          <div class="text-lg font-semibold text-blue-900">${tournament}</div>
          <div class="text-sm text-blue-700">${year}</div>
        `;
        col.appendChild(header);
        
        // Match list
        const matchList = document.createElement("div");
        matchList.className = "flex flex-col gap-2";
        
        games.forEach(g => {
          const matchCard = renderMiniMatchCard(g); // returns DOM element now
          matchList.appendChild(matchCard);
        });
        
        col.appendChild(matchList);

        container.appendChild(col);
      });

      // Scroll buttons
      const leftBtn = document.createElement("button");
      const rightBtn = document.createElement("button");

      [leftBtn, rightBtn].forEach(btn => {
        btn.className = "absolute top-[42%] w-6 h-6 flex items-center justify-center bg-gray-200 hover:bg-gray-300 text-sm rounded-full z-10 shadow";
      });

      leftBtn.innerHTML = "◀";
      rightBtn.innerHTML = "▶";
      leftBtn.style.left = "-10px";
      rightBtn.style.right = "-10px";

      leftBtn.onclick = () => container.scrollBy({ left: -300, behavior: "smooth" });
      rightBtn.onclick = () => container.scrollBy({ left: 300, behavior: "smooth" });

      scrollWrapper.appendChild(leftBtn);
      scrollWrapper.appendChild(container);
      scrollWrapper.appendChild(rightBtn);
      playerResult.appendChild(scrollWrapper);
    })
    .catch(err => {
      console.error("Error:", err);
      playerResult.innerHTML = `<div class="text-red-600">${err}</div>`;
    });
}

function renderMiniMatchCard(game) {
  const roundSurface = `
    <div class="text-xs text-gray-600 font-medium mb-1">${game.round} • ${game.surface}</div>
  `;

  let winnerScores = "";
  let loserScores = "";

  for (let i = 1; i <= 5; i++) {
    const w = game[`wset${i}Games`];
    const l = game[`lset${i}Games`];
    if (w == null && l == null) continue;

    winnerScores += `<span class="${w > l ? 'font-bold text-black' : 'text-gray-500'} ml-2">${w}</span>`;
    loserScores += `<span class="${l > w ? 'font-bold text-black' : 'text-gray-500'} ml-2">${l}</span>`;
  }

  const modalId = `modal-${game.matchId}`;

  const matchCard = document.createElement("div");
  matchCard.className = "cursor-pointer border rounded-lg p-3 shadow bg-white hover:bg-blue-50 transition";
  matchCard.innerHTML = `
    <div class="text-xs text-red-500">${game.gameFinishStatus !== "Finished" ? game.gameFinishStatus : ""}</div>
    ${roundSurface}
    <div class="flex justify-between text-sm font-semibold text-gray-800">
      <span>${game.winnerName}</span>
      <span>${winnerScores}</span>
    </div>
    <div class="flex justify-between text-sm text-gray-700">
      <span>${game.loserName}</span>
      <span>${loserScores}</span>
    </div>
  `;

  matchCard.onclick = () => {
    console.log("Match card clicked:", game.matchId);
    console.log("Match card clicked:", game.winnerName);
    searchMatchFlashcardById(game.matchId);
    document.getElementById("matchResult").scrollIntoView({ behavior: "smooth" });
  };

  return matchCard;
}

function groupBy(array, keyFn) {
  return array.reduce((acc, item) => {
    const key = keyFn(item);
    if (!acc[key]) acc[key] = [];
    acc[key].push(item);
    return acc;
  }, {});
}

function searchMatchFlashcardById(id) {
  searchMatchFlashcard(id);
}

// searchMatchFlashcard() remains unchanged


function searchMatchFlashcard(matchId) {
  //const matchId = document.getElementById("matchId").value.trim();
  const resultEl = document.getElementById("matchResult");
  resultEl.textContent = "";

  if (!matchId) {
    return displayMessage("matchResult", "Please fill all match fields.");
  }

  fetch(`${matchApiUrl}/${encodeURIComponent(matchId)}`)
    .then(res => res.ok ? res.json() : res.text())
    .then(result => {
      if (typeof result === "string") {
        console.warn("⚠️ Fetch returned error string:", result);
        resultEl.textContent = result;
        return;
      }
      console.log("✅ Match object:", result);
      openMatchModal();

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
      
      console.log("📍 resultEl is:", resultEl);
      if (!resultEl) {
        console.error("❌ matchResult element not found");
        return;
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

function toggleRegister() {
  const auth = document.getElementById('authContainer');
  const reg = document.getElementById('registerContainer');
  auth.classList.toggle('hidden');
  reg.classList.toggle('hidden');
}

function openMatchModal() {
  let modal = document.getElementById("matchModal");
  if (!modal) {
    modal = document.createElement("div");
    modal.id = "matchModal";
    modal.className = "fixed inset-0 z-50 bg-black bg-opacity-60 flex items-center justify-center";

    modal.innerHTML = `
      <div class="relative bg-white w-full max-w-3xl p-6 rounded-lg shadow-lg overflow-y-auto max-h-[90vh]">
        <button onclick="closeMatchModal()" class="absolute top-2 right-2 text-gray-700 hover:text-red-600">✖</button>
        <div id="matchResult"></div>
      </div>
    `;

    document.body.appendChild(modal);
  } else {
    modal.classList.remove("hidden");
  }

  document.getElementById("container").classList.add("opacity-50");
}


function closeMatchModal() {
  const modal = document.getElementById("matchModal");
  modal.classList.add("hidden");
  document.getElementById("container").classList.remove("opacity-50");
}