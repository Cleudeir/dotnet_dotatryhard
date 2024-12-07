using dotatryhard.Data;
using dotatryhard.Interfaces;
using dotatryhard.Services;
using Microsoft.EntityFrameworkCore;

// Load environment variables from .env
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure logging to ignore database logs
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.None);

// Add services to the container
builder.Services.AddControllers(); // Enable MVC and REST API controllers
builder.Services.AddEndpointsApiExplorer(); // Add API endpoint explorer for Swagger
builder.Services.AddSwaggerGen(); // Add Swagger for API documentation

// Register HttpClient for dependency injection
builder.Services.AddHttpClient();

// Configure MySQL Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Build connection string using environment variables
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    var connectionString = connectionStringTemplate
        .Replace("${MYSQL_HOST}", Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost")
        .Replace("${MYSQL_PORT}", Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306")
        .Replace("${MYSQL_DATABASE}", Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "database")
        .Replace("${MYSQL_USER}", Environment.GetEnvironmentVariable("MYSQL_USER") ?? "user")
        .Replace("${MYSQL_PASSWORD}", Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "password");

    // Configure MySQL provider
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 27)));
});

// Register the hosted service
//builder.Services.AddHostedService<DataPopulationService>();

// Register services for dependency injection
builder.Services.AddScoped<IMatchHistoryService, MatchHistoryService>();
builder.Services.AddScoped<PlayersMatchesService>();
builder.Services.AddScoped<ISteamUserService, PlayerProfileService>();


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{

    app.UseDeveloperExceptionPage(); // Enable detailed errors in development
    app.UseSwagger(); // Enable Swagger in development
    app.UseSwaggerUI(); // Enable Swagger UI for API exploration
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
