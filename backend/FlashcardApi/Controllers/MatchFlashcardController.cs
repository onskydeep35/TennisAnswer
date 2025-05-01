using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataBase;
using Data.Models.Tennis;

namespace FlashcardApi.Controllers
{
    [ApiController]
    [Route("api/match")]
    public class MatchFlashcardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MatchFlashcardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{matchId}")]
        public async Task<IActionResult> GetMatchFlashcard(string matchId)
        {
            var match = await _db.Matches.FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
                return NotFound("No such match found.");

            var winner = await _db.Players.FindAsync(match.WinnerId);
            var loser = await _db.Players.FindAsync(match.LoserId);

            var formattedDate = match.TourneyDate.HasValue
                ? match.TourneyDate.Value.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture)
                : "Unknown";

            var (sets, winnerGames, loserGames, gameStatus) = ParseSetScores(match.Score);

            return Ok(new
            {
                matchId = new { match.TourneyId, match.TourneyDate, match.MatchNum },
                tournament = match.TourneyName,
                surface = match.Surface,
                date = formattedDate,
                round = NormalizeRound(match.Round),
                score = match.Score,
                gameFinishStatus = gameStatus,
                winner = new
                {
                    id = match.WinnerId,
                    name = winner != null ? $"{winner.FirstName} {winner.LastName}" : "Unknown",
                    hand = winner?.Hand,
                    ioc = winner?.Ioc,
                    height = winner?.Height,
                    aces = match.WAce,
                    doubleFaults = match.WDoubleFault,
                    firstServe = (match.WServePoints > 0 && match.WFirstIn.HasValue)
                        ? $"{Math.Round((double)match.WFirstIn.Value / match.WServePoints.Value * 100):F0}%"
                        : "N/A",
                    firstServeWin = (match.WFirstIn > 0 && match.WFirstWon.HasValue)
                        ? $"{Math.Round((double)match.WFirstWon.Value / match.WFirstIn.Value * 100):F0}%"
                        : "N/A",
                    secondServeWin = ((match.WServePoints - match.WFirstIn) > 0 && match.WSecondWon.HasValue)
                        ? $"{Math.Round((double)match.WSecondWon.Value / (match.WServePoints.Value - match.WFirstIn.Value) * 100):F0}%"
                        : "N/A",
                    breakPoints = $"{match.LBpFaced - match.LBpSaved}/{match.LBpFaced}",
                    serviceGamesWon = (match.LBpFaced - match.LBpSaved >= 0 && match.WServiceGames > 0)
                        ? $"{match.WServiceGames - (match.WBpFaced - match.WBpSaved)}"
                        : "N/A",
                    totalPointsWon = match.WFirstWon + match.WSecondWon + (match.LServePoints - match.LFirstWon - match.LSecondWon),
                    servePointsWon = match.WFirstWon + match.WSecondWon,
                    receivingPointsWon = match.LServePoints - match.LFirstWon - match.LSecondWon,
                    set1Games = winnerGames[0],
                    set2Games = winnerGames[1],
                    set3Games = winnerGames[2],
                    set4Games = winnerGames[3],
                    set5Games = winnerGames[4],
                },
                loser = new
                {
                    id = match.LoserId,
                    name = loser != null ? $"{loser.FirstName} {loser.LastName}" : "Unknown",
                    hand = loser?.Hand,
                    ioc = loser?.Ioc,
                    height = loser?.Height,
                    aces = match.LAce,
                    doubleFaults = match.LDoubleFault,
                    firstServe = (match.LServePoints > 0 && match.LFirstIn.HasValue)
                        ? $"{Math.Round((double)match.LFirstIn.Value / match.LServePoints.Value * 100):F0}%"
                        : "N/A",
                    firstServeWin = (match.LFirstIn > 0 && match.LFirstWon.HasValue)
                        ? $"{Math.Round((double)match.LFirstWon.Value / match.LFirstIn.Value * 100):F0}%"
                        : "N/A",
                    secondServeWin = ((match.LServePoints - match.LFirstIn) > 0 && match.LSecondWon.HasValue)
                        ? $"{Math.Round((double)match.LSecondWon.Value / (match.LServePoints.Value - match.LFirstIn.Value) * 100):F0}%"
                        : "N/A",
                    breakPoints = $"{match.WBpFaced - match.WBpSaved}/{match.WBpFaced}",
                    serviceGamesWon = (match.WBpFaced - match.WBpSaved >= 0 && match.LServiceGames > 0)
                        ? $"{match.LServiceGames - (match.LBpFaced - match.LBpSaved)}"
                        : "N/A",
                    totalPointsWon = match.LFirstWon + match.LSecondWon + (match.WServePoints - match.WFirstWon - match.WSecondWon),
                    servePointsWon = match.LFirstWon + match.LSecondWon,
                    receivingPointsWon = match.WServePoints - match.WFirstWon - match.WSecondWon,
                    set1Games = loserGames[0],
                    set2Games = loserGames[1],
                    set3Games = loserGames[2],
                    set4Games = loserGames[3],
                    set5Games = loserGames[4],
                }
            });
        }

        private static (string[] setStrings, int?[] winnerGames, int?[] loserGames, string gameStatus) ParseSetScores(string score)
        {
            var sets = new List<string>();
            var winnerGames = new List<int?>();
            var loserGames = new List<int?>();
            var gameStatus = "Finished";

            if (!string.IsNullOrWhiteSpace(score))
            {
                var rawSets = score.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var set in rawSets.Take(5))
                {
                    var cleanSet = set.Split('(')[0].Trim(); // remove tiebreak
                    sets.Add(cleanSet);

                    var games = cleanSet.Split('-');
                    if (games.Length == 2 &&
                        int.TryParse(games[0], out var w) &&
                        int.TryParse(games[1], out var l))
                    {
                        winnerGames.Add(w);
                        loserGames.Add(l);
                    }
                    else
                    {
                        winnerGames.Add(null);
                        loserGames.Add(null);
                    }

                    if(games.Contains("W/O")){
                        gameStatus = "Walkover";
                    }
                    if(games.Contains("RET")){
                        gameStatus = "Retired";
                    }
                }
            }

            // Pad to 5 sets
            while (sets.Count < 5) sets.Add(null);
            while (winnerGames.Count < 5) winnerGames.Add(null);
            while (loserGames.Count < 5) loserGames.Add(null);

            return (sets.ToArray(), winnerGames.ToArray(), loserGames.ToArray(), gameStatus);
        }

        public static string NormalizeRound(string round)
        {
            var roundMapping = new Dictionary<string, string>
            {
                { "F", "Final" },
                { "SF", "Semi-Final" },
                { "QF", "Quarter-Final" }
            };

            if (roundMapping.TryGetValue(round, out var mapped))
                return mapped;

            if (round.StartsWith("R"))
            {
                var parts = round.Split('R');
                if (parts.Length > 1 && int.TryParse(parts[1], out var roundNum))
                    return $"Round of {roundNum}";
            }

            return round;
        }
    }
}
