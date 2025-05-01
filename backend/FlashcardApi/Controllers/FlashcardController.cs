using Microsoft.AspNetCore.Mvc;
using Data.Models;
using DataBase;
using System.Text.Json;

namespace FlashcardAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FlashcardController : ControllerBase
{
    private static List<Flashcard> _flashcards = new();
    private static int _idCounter = 1;
    private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "flashcards.json");

    static FlashcardController()
    {
        try
        {
            Console.WriteLine($"Looking for flashcards.json at: {FilePath}");

            if (System.IO.File.Exists(FilePath))
            {
                var json = System.IO.File.ReadAllText(FilePath);
                Console.WriteLine("Raw JSON read:\n" + json);

                var list = JsonSerializer.Deserialize<List<Flashcard>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (list != null && list.Any())
                {
                    _flashcards = list;
                    _idCounter = _flashcards.Max(f => f.Id) + 1;

                    Console.WriteLine($"Loaded {_flashcards.Count} flashcards from file. Logging each entry:");

                    foreach (var card in _flashcards)
                    {
                        Console.WriteLine($"--- Flashcard ID: {card.Id} ---");
                        Console.WriteLine($"Question: {card.Question}");
                        Console.WriteLine($"Answer  : {card.Answer}");
                        Console.WriteLine($"Category: {card.Category}");
                        Console.WriteLine($"Created : {card.CreatedAt}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("flashcards.json was empty or invalid.");
                }
            }
            else
            {
                Console.WriteLine("flashcards.json file does not exist.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to load flashcards.json: " + ex.Message);
        }
    }

    private void SaveToFile()
    {
        var json = JsonSerializer.Serialize(_flashcards, new JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(FilePath, json);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_flashcards.OrderByDescending(f => f.CreatedAt));
    }

    [HttpPost]
    public IActionResult Create([FromBody] Flashcard input)
    {
        if (string.IsNullOrWhiteSpace(input.Question) || string.IsNullOrWhiteSpace(input.Answer))
        {
            Console.WriteLine("Rejected invalid POST: Empty question or answer.");
            return BadRequest("Question and answer cannot be empty.");
        }

        input.Id = _idCounter++;
        input.CreatedAt = DateTime.UtcNow;
        _flashcards.Add(input);
        SaveToFile();

        Console.WriteLine($"Flashcard added: {input.Question} (ID: {input.Id})");

        return Ok(input);
    }
}
