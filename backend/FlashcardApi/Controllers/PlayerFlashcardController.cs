using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataBase;
using Data.Models.Tennis;

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

        Console.WriteLine($"FOUND: {player.FirstName} {player.LastName}, hand={player.Hand}, height={player.Height}");
        Console.WriteLine($"{player.FirstName} {player.LastName}");
        Console.WriteLine(player.Dob?.ToString("yyyy-MM-dd") ?? "Unknown");

        return Ok(new
        {
            Name = $"{player.FirstName} {player.LastName}",
            Hand = player.Hand,
            DateOfBirth = player.Dob?.ToString("yyyy-MM-dd") ?? "Unknown",
            Country = player.Country,
            Ioc = player.Ioc,
            Height = player.Height?.ToString() ?? "Unknown",
            WikiDataId = player.WikiDataId
        });
    }
}
