const apiUrl = "http://localhost:5019/flashcard";
let flashcards = [];

window.onload = () => {
  fetch("http://localhost:5019/flashcard")
    .then(res => res.json())
    .then(data => {
      console.log("Loaded flashcards:", data);  // ✅ Check this appears
      flashcards = data;
    })
    .catch(error => console.error("Error fetching flashcards:", error));
};

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
