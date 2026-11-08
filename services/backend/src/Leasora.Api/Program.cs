using Leasora.Api.Data;
using Microsoft.EntityFrameworkCore;
using Leasora.Api.Models;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var connectionString =
    builder.Configuration.GetConnectionString("LeasoraDb")
    ?? throw new InvalidOperationException(
        "Connection string 'LeasoraDb' is not configured.");

builder.Services.AddDbContext<LeasoraDbContext>(options =>
    options.UseNpgsql(connectionString));
// Registers user management services for ApplicationUser.
builder.Services.AddIdentityCore<ApplicationUser>()
    // Registers services for creating and managing roles.
    .AddRoles<IdentityRole>()
    // Uses our database context to store users and roles.
    .AddEntityFrameworkStores<LeasoraDbContext>();
var app = builder.Build();
// Seeds roles automatically when running in Development.
if (app.Environment.IsDevelopment())
{
    // Creates a scope for services that use the database context.
    await using var scope = app.Services.CreateAsyncScope();

    // Gets the Identity service responsible for managing roles.
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    // Creates missing roles and skips roles that already exist.
    await RoleSeeder.SeedAsync(roleManager);
    // Gets the database context from the existing service scope.
    var db = scope.ServiceProvider
        .GetRequiredService<LeasoraDbContext>();

    // Adds missing provinces and districts.
    await LocationSeeder.SeedAsync(db);

    // Logs completion after location data has been saved.
    app.Logger.LogInformation("Development location seeding completed.");

    // Reports completion after all required roles are available.
    app.Logger.LogInformation("Development role seeding completed.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapGet("/health/db", async (LeasoraDbContext db) =>
    {
        var connected = await db.Database.CanConnectAsync();

        if (!connected)
        {
            return Results.StatusCode(503);
        }

        return Results.Ok(new { database = "Connected" });
    });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
