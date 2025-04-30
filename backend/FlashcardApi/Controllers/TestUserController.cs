using Microsoft.AspNetCore.Mvc;

namespace FlashcardAPI.Controllers;

[ApiController]
[Route("api/test-user")]
public class TestUserController : ControllerBase
{
    private readonly AppDbContext _db;
    public TestUserController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> AddSampleUser()
    {
        var user = new User
        {
            GoogleId = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            Name = "Test User",
            AvatarUrl = "https://example.com/avatar.png"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(user);
    }
}
