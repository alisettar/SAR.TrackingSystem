using Carter;
using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Api.Middleware.ExceptionHandler;
using SAR.TrackingSystem.Application;
using SAR.TrackingSystem.Domain.Configuration;
using SAR.TrackingSystem.Infrastructure;
using SAR.TrackingSystem.Infrastructure.Persistence;
using SAR.TrackingSystem.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Add Configuration
builder.Services.Configure<SectorConfiguration>(
    builder.Configuration.GetSection(SectorConfiguration.SectionName));

// Add Entity Framework
builder.Services.AddDbContext<SarDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Application & Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Add Carter
builder.Services.AddCarter();

// Add Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>().AddProblemDetails();

var app = builder.Build();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SarDbContext>();
    await context.Database.EnsureCreatedAsync();
    await DatabaseSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map Carter routes
app.MapCarter();

app.Run();