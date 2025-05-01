using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataBase;
using Data.Models.Tennis;
using System.Globalization;

namespace FlashcardApi.Controllers;

[ApiController]
[Route("api/player")]
public class PlayerFlashcardController : ControllerBase
{
    private readonly AppDbContext _db;

    public PlayerFlashcardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("{fullName}")]
    public async Task<IActionResult> GetFlashcard(string fullName)
    {
        var parts = fullName.Trim().Split(' ');
        if (parts.Length != 2)
            return BadRequest("Please enter 'FirstName LastName'");

        var player = await _db.Players.FirstOrDefaultAsync(p =>
            p.FirstName.ToLower() == parts[0].ToLower() &&
            p.LastName.ToLower() == parts[1].ToLower());

        if (player == null)
            return NotFound("No such player found.");

            Console.WriteLine($"found Player with name {player.FirstName} {player.LastName}");
            Console.WriteLine($"found Player with id {player.PlayerId}");

        return Ok(new
        {
            Id = player.PlayerId,
            Name = $"{player.FirstName} {player.LastName}",
            Hand = player.Hand,
            DateOfBirth = player.Dob?.ToString("yyyy-MM-dd") ?? "Unknown",
            Country = player.Country,
            Ioc = player.Ioc,
            Height = player.Height?.ToString() ?? "Unknown",
            WikiDataId = player.WikiDataId
        });
    }

    [HttpGet("matches/{playerId}")]
    public async Task<IActionResult> GetPlayerMatches(int playerId)
    {
        Console.WriteLine($"Searching matches for player ID = {playerId}");
        var player = await _db.Players.FindAsync(playerId);
        if (player == null)
            return NotFound("Player not found.");

        var rawMatches = await _db.Matches
            .Where(m => m.WinnerId == playerId || m.LoserId == playerId)
            .OrderByDescending(m => m.TourneyDate)
            .ToListAsync();

        //Console.WriteLine($"matches count {rawMatches}");

        var result = rawMatches.Select(match =>
        {
            var (sets, winnerGames, loserGames, gameStatus) = ParseSetScores(match.Score);
            var formattedDate = match.TourneyDate.HasValue
                ? match.TourneyDate.Value.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture)
                : "Unknown";
            Console.WriteLine(match.WinnerName);
            Console.WriteLine(match.LoserName);
            return new
            {
                matchId = match.MatchId,
                tournament = match.TourneyName,
                surface = match.Surface,
                date = match.TourneyDate.HasValue ? match.TourneyDate.Value : DateTime.MinValue,
                tournamentYear = match.TourneyId.Split('-')[0],
                matchNum = match.MatchNum,
                //formattedDate = formattedDate,
                round = NormalizeRound(match.Round),
                gameFinishStatus = gameStatus,
                winnerName = match.WinnerName,
                loserName = match.LoserName,
                wset1Games = winnerGames[0],
                wset2Games = winnerGames[1],
                wset3Games = winnerGames[2],
                wset4Games = winnerGames[3],
                wset5Games = winnerGames[4],
                lset1Games = loserGames[0],
                lset2Games = loserGames[1],
                lset3Games = loserGames[2],
                lset4Games = loserGames[3],
                lset5Games = loserGames[4]
            };
        }).ToList();

        return Ok(result);
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

                if (games.Contains("W/O"))
                {
                    gameStatus = "Walkover";
                }
                if (games.Contains("RET"))
                {
                    gameStatus = "Retired";
                }
            }
        }

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
            { "QF", "Quarter-Final" },
            { "RR", "Round Robin" },
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
