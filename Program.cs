using dotatryhard.Data;
using dotatryhard.Interfaces;
using dotatryhard.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env
DotNetEnv.Env.Load();

// Add services to the container
builder.Services.AddControllers();

// Register HttpClient for dependency injection
builder.Services.AddHttpClient();

// Configure MySQL Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        .Replace("${MYSQL_HOST}", Environment.GetEnvironmentVariable("MYSQL_HOST"))
        .Replace("${MYSQL_PORT}", Environment.GetEnvironmentVariable("MYSQL_PORT"))
        .Replace("${MYSQL_DATABASE}", Environment.GetEnvironmentVariable("MYSQL_DATABASE"))
        .Replace("${MYSQL_USER}", Environment.GetEnvironmentVariable("MYSQL_USER"))
        .Replace("${MYSQL_PASSWORD}", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"));

    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 27)));
});

// Register application services
builder.Services.AddScoped<IMatchHistoryService, MatchHistoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
