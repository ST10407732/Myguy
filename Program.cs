using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MYGUYY.Data; // Adjust based on your project namespace
using MYGUYY.Hubs; // Include the namespace for your SignalR hubs

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllersWithViews(); // MVC
builder.Services.AddRazorPages(); // Razor Pages
builder.Services.AddSignalR(); // Add SignalR services

// Configure the database connection
var connectionString = builder.Configuration.GetConnectionString("MYGUYYYS");
builder.Services.AddDbContext<MYGUYYContext>(options =>
    options.UseSqlServer(connectionString));

// Configure authentication and authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Path to the login page
        options.LogoutPath = "/Account/Logout"; // Path to the logout page
        options.AccessDeniedPath = "/Account/AccessDenied"; // Path to the access denied page
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Redirect to error page in production
    app.UseHsts(); // Enable HSTS
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseStaticFiles(); // Serve static files (wwwroot)

// Routing middleware
app.UseRouting();

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hubs
app.MapHub<MessageHub>("/messageHub");
app.MapHub<LocationHub>("/locationHub"); // Map SignalR hubs for real-time communication

// Map default route for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages
app.MapRazorPages();

// Run the application
app.Run();
