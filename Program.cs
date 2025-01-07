using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MYGUYY.Data; // Adjust based on your project namespace
using MYGUYY.Hubs; // Include the namespace for your SignalR hub

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the database connection (use your actual connection string)
var connectionString = builder.Configuration.GetConnectionString("MYGUYYYS");
builder.Services.AddDbContext<MYGUYYContext>(options =>
    options.UseSqlServer(connectionString));

// Configure authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Path to the login page
        options.LogoutPath = "/Account/Logout"; // Path to the logout page
        options.AccessDeniedPath = "/Account/AccessDenied"; // Path to an access denied page
    });

builder.Services.AddAuthorization();

// Add SignalR to the services
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map the SignalR hub for real-time messaging
app.MapHub<MessageHub>("/messageHub");

app.Run();
