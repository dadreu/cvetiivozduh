using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using FlowerShop.Backend.Data;
using FlowerShop.Backend.Endpoints;
using FlowerShop.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Data Protection Config (Persist keys to filesystem to keep cookies alive)
var keysDirectory = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "keys"));
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(keysDirectory)
    .SetApplicationName("FlowerShop");

builder.Services.AddSingleton<INotificationService, TelegramNotificationService>();

// 2. Cookie Authentication Config
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requires HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.IsEssential = true;
        
        // Return 401 instead of redirecting to login page (since it's an API)
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// 3. Database Context Config
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Database=flowershop;User=root;Password=secret;";

builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("FlowerShopLocal");
});

// 4. Anti-forgery Config
builder.Services.AddAntiforgery(options => 
{
    options.HeaderName = "X-CSRF-TOKEN";
});

// 5. Configure Limits for File Uploads
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Max 10 MB per request body
    serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
});

var app = builder.Build();

// Seed Database
await DataSeeder.InitializeAsync(app.Services);

// --- HTTP Request Pipeline ---

// 1. Exceptions & Forwarded Headers
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection(); // Usually Nginx handles HTTPS, but if direct, use it

// 2. Routing
app.UseRouting();

// 3. Authentication & Authorization (Strict order)
app.UseAuthentication();
app.UseAuthorization();

// 4. Anti-forgery
app.UseAntiforgery();

// Example Minimal API Endpoints
app.MapGet("/", () => "FlowerShop API is running");

// Register Modular Endpoints
app.MapAuthEndpoints();
app.MapPublicEndpoints();
app.MapOrderEndpoints();
app.MapAdminEndpoints();

app.Run();

public partial class Program { }
