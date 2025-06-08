using System.Reflection;
using APBD_12.Data;
using APBD_12.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Debugging output
Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
Console.WriteLine($"Content Root Path: {builder.Environment.ContentRootPath}");

try
{
    var config = new ConfigurationBuilder()
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();
    
    builder.Configuration.AddConfiguration(config);
}
catch (Exception ex)
{
    Console.WriteLine($"CONFIG LOAD ERROR: {ex.Message}");
    throw;
}

var connectionString = builder.Configuration.GetConnectionString("Default") 
    ?? throw new InvalidOperationException("Connection string 'Default' not found");

Console.WriteLine($"Using Connection String: {connectionString}");

builder.Services.AddDbContext<Apbd12Context>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();