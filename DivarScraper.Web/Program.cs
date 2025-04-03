using DivarScraper.Core.Settings;
using DivarScraper.Core.Data;
using DivarScraper.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register our services
var settings = new AppSettings
{
    DeepSeekKey= builder.Configuration["DeepSeekSettings:Key"], // از environment variables یا appsettings.json خوانده می‌شود
    DeepSeekModel= builder.Configuration["DeepSeekSettings:Model"] ?? "gpt-4",
    //OpenAIKey = builder.Configuration["OpenAI:ApiKey"], // از environment variables یا appsettings.json خوانده می‌شود
    //OpenAIModel = builder.Configuration["OpenAI:Model"] ?? "gpt-4"
};

builder.Services.AddSingleton(settings);
builder.Services.AddSingleton<ICarAdRepository, PostgresCarAdRepository>();
builder.Services.AddSingleton<ICarPricePredictor, CarPricePredictor>();
builder.Services.AddSingleton<SmartPriceAdvisor>();
builder.Services.AddSingleton<SemanticCarSearch>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
