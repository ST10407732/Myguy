using Microsoft.AspNetCore.Mvc;
using MYGUYY.Models;
using System.Diagnostics;
using MYGUYY.Data;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MYGUYYContext _context;

    public HomeController(ILogger<HomeController> logger, MYGUYYContext context)
    {
        _logger = logger;
        _context = context;
    }

    // GET: Home page (no authentication check)
    public IActionResult Index()
    {
        return View();
    }

    // GET: Privacy page
    public IActionResult Privacy()
    {
        return View();
    }

    // GET: Error page
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // POST: Register new user (added to HomeController for direct registration)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string username, string password, string role)
    {
        // Check if the username already exists
        if (_context.Users.Any(u => u.Username == username))
        {
            ModelState.AddModelError("", "Username already exists.");
            return View("Index");
        }

        // Hash the password securely
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Create a new user
        var user = new User
        {
            Username = username,
            PasswordHash = hashedPassword,
            Role = role
        };

        // Save the user to the database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        TempData["Message"] = "Registration successful! You can now log in.";
        return RedirectToAction("Login", "Account");
    }
}
