using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataBase;
using Data.Models.Tennis;

namespace FlashcardApi.Controllers
{
    /// <summary>
    /// Provides endpoints to fetch detailed flashcards for individual tennis matches.
    /// </summary>
    [ApiController]
    [Route("api/match")]
    public class MatchFlashcardController : ControllerBase
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Constructor injecting the application's database context.
        /// </summary>
        public MatchFlashcardController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a detailed flashcard for a specific match identified by tournament ID, date, and match number.
        /// </summary>
        /// <param name="tourneyId">Unique identifier for the tournament (e.g., "Wimbledon").</param>
        /// <param name="tourneyDate">Date of the tournament in YYYYMMDD format.</param>
        /// <param name="matchNum">Sequential match number within the tournament.</param>
        /// <returns>A flashcard object containing match metadata, players, score, and stats.</returns>
        [HttpGet("{tourneyId}/{winnerId}/{loserId}")]
        public async Task<IActionResult> GetMatchFlashcard(string tourneyId, int winnerId, int loserId)
        {
            // Fetch the match record from the database
            var match = await _db.Matches.FirstOrDefaultAsync(m =>
                m.TourneyId == tourneyId &&
                m.WinnerId == winnerId &&
                m.LoserId == loserId);

            if (match == null)
                return NotFound("No such match found.");

            // Fetch player details for winner and loser
            var winner = await _db.Players.FindAsync(match.WinnerId);
            var loser  = await _db.Players.FindAsync(match.LoserId);
            
            Console.WriteLine($"Fetched with aces {match.WAce} and {match.LAce}");

            return Ok(new
            {
                MatchId = new { match.TourneyId, match.TourneyDate, match.MatchNum },
                Tournament = match.TourneyName,
                Surface = match.Surface,
                Date = match.TourneyDate.ToString(),
                Round = match.Round,
                Score = match.Score,
                Winner = new
                {
                    Id = match.WinnerId,
                    Name = winner != null ? $"{winner.FirstName} {winner.LastName}" : "Unknown",
                    Hand = winner?.Hand,
                    Ioc = winner?.Ioc,
                    Height = winner?.Height,
                    Aces = match.WAce,
                    DoubleFaults = match.WDoubleFault
                },
                Loser = new
                {
                    Id = match.LoserId,
                    Name = loser  != null ? $"{loser.FirstName} {loser.LastName}" : "Unknown",
                    Hand = loser?.Hand,
                    Ioc = loser?.Ioc,
                    Height = loser?.Height,
                    Aces = match.LAce,
                    DoubleFaults = match.LDoubleFault
                }
            });
        }
    }
}
