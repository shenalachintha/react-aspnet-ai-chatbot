using AIChatbot.Services;

var builder = WebApplication.CreateBuilder(args);

// CORS (open for local dev; restrict in production)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddHttpClient();     // HttpClientFactory for OpenAI calls
builder.Services.AddSingleton<AIService>();
builder.Services.AddControllers();

var app = builder.Build();

// Show detailed errors in Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.MapControllers();

app.Run();
