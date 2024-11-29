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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection")
            .Replace("${MYSQL_HOST}", Environment.GetEnvironmentVariable("MYSQL_HOST"))
            .Replace("${MYSQL_PORT}", Environment.GetEnvironmentVariable("MYSQL_PORT"))
            .Replace("${MYSQL_DATABASE}", Environment.GetEnvironmentVariable("MYSQL_DATABASE"))
            .Replace("${MYSQL_USER}", Environment.GetEnvironmentVariable("MYSQL_USER"))
            .Replace("${MYSQL_PASSWORD}", Environment.GetEnvironmentVariable("MYSQL_PASSWORD")),
        new MySqlServerVersion(new Version(8, 0, 27)) // Adjust version as needed
    ));

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
