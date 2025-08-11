using SAR.TrackingSystem.Web.Services;
using SAR.TrackingSystem.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure HttpClient for API calls
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7001/";
builder.Services.AddHttpClient("SarApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add API services
builder.Services.AddScoped<IVolunteerService, VolunteerService>();
builder.Services.AddScoped<IMovementService, MovementService>();
builder.Services.AddScoped<ISectorService, SectorService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IDashboardService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("SarApi");
    var volunteerService = provider.GetRequiredService<IVolunteerService>();
    return new DashboardService(httpClient, volunteerService);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
