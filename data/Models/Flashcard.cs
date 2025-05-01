namespace Data.Models;

public class Flashcard
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Category { get; set; } = "Music";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
