using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MYGUYY.Data; // Adjust based on your project namespace
using MYGUYY.Hubs; // Include the namespace for your SignalR hubs
using MYGUYY.Models;
using MYGUYY.Services; // Include EmailService namespace

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<Notification>(); // Add the NotificationService to handle storing and sending notifications

// Configure services BEFORE calling Build()
builder.Services.AddControllersWithViews(); // MVC
builder.Services.AddRazorPages(); // Razor Pages
builder.Services.AddSignalR(); // Add SignalR services

// Configure the database connection
var connectionString = builder.Configuration.GetConnectionString("MYGUYYYS");
builder.Services.AddDbContext<MYGUYYContext>(options =>
    options.UseSqlServer(connectionString));

// Add authentication and authorization services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Path to the login page
        options.LogoutPath = "/Account/Logout"; // Path to the logout page
        options.AccessDeniedPath = "/Account/AccessDenied"; // Path to the access denied page
    });

builder.Services.AddAuthorization(); // Add authorization services

// Register the EmailService for email-related functionality
builder.Services.AddScoped<EmailService>();

// Register NotificationService for handling notifications
builder.Services.AddScoped<Notification>();

// Configure email settings (from appsettings.json)
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Redirect to error page in production
    app.UseHsts(); // Enable HTTP Strict Transport Security (HSTS)
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseStaticFiles(); // Serve static files (wwwroot)

app.UseRouting();

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Enable CORS
app.UseCors("AllowAll");

// Map SignalR hubs for real-time communication
app.MapHub<MessageHub>("/messageHub"); // Chat/MessageHub
app.MapHub<LocationHub>("/locationHub"); // Location tracking (if used)
app.MapHub<NotificationHub>("/notificationHub"); // Notifications

// Map default route for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages
app.MapRazorPages();

// Run the application
app.Run();
