using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using pointCounterBackend.Data;
using pointCounterBackend.Services;
using pointCounterBackend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IScoreboardService, ScoreboardService>();
builder.Services.AddScoped<IPointMatchService, PointMatchService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:4200",
                  "https://localhost:4200",
                  "http://127.0.0.1:4200",
                  "https://127.0.0.1:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthorization();

// Serverar Angular-filer från wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// Viktigt för Angular routes, t.ex. /spel/guid
app.MapFallbackToFile("index.html");

app.Run();
