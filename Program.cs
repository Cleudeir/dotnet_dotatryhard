using dotatryhard.Data;
using dotatryhard.Interfaces;
using dotatryhard.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env
DotNetEnv.Env.Load();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpClient();

// Configure MySQL
var connectionString =
    $"Server={Environment.GetEnvironmentVariable("MYSQL_HOST")};"
    + $"Port={Environment.GetEnvironmentVariable("MYSQL_PORT")};"
    + $"Database={Environment.GetEnvironmentVariable("MYSQL_DATABASE")};"
    + $"User={Environment.GetEnvironmentVariable("MYSQL_USER")};"
    + $"Password={Environment.GetEnvironmentVariable("MYSQL_PASSWORD")};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32)))
);
builder.Services.AddScoped<IMatchHistoryService, MatchHistoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
