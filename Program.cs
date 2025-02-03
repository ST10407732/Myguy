using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MYGUYY.Data;
using MYGUYY.Hubs;
using MYGUYY.Models;
using MYGUYY.Services;

var builder = WebApplication.CreateBuilder(args);

// Load Configuration
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Register Services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<Notification>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

// Configure Database (Only Once)
var connectionString = builder.Configuration.GetConnectionString("MYGUYYYS");
builder.Services.AddDbContext<MYGUYYContext>(options =>
    options.UseSqlServer(connectionString));

// Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });
builder.Services.AddAuthorization();

// Configure Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

// Map SignalR Hubs
app.MapHub<MessageHub>("/messageHub");
app.MapHub<LocationHub>("/locationHub");
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<TaskHub>("/taskHub");

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/dashboard",
    defaults: new { controller = "Admin", action = "Dashboard" });

app.MapRazorPages();

// Run App
app.Run();
