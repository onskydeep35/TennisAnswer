using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DataBase;
using Data.Models;
using Data.Models.Tennis;

var builder = WebApplication.CreateBuilder(args);

// Allow CORS for local frontend access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register EF Core context and SQLite DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=../../data/DataBase/flashcards.db"));

// Add all controllers (including PlayerFlashcardController)
builder.Services.AddControllers();

var app = builder.Build();

// Ensure DB is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Enable CORS and map controller routes
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
