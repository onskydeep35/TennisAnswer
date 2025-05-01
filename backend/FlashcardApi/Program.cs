using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using FlashcardAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Register services BEFORE builder.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ Make sure this comes before `builder.Build()`
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=flashcards.db"));

builder.Services.AddControllers();

var app = builder.Build();

// Optional: Force database creation
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the app
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
