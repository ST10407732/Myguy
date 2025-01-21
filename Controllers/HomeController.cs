using Microsoft.AspNetCore.Mvc;
using MYGUYY.Models;
using MYGUYY.Services;
using MYGUYY.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using BCrypt.Net;
using System.Diagnostics;

namespace MYGUYY.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MYGUYYContext _context;
        private readonly EmailService _emailService;

        // Constructor to inject dependencies
        public HomeController(ILogger<HomeController> logger, MYGUYYContext context, EmailService emailService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
        }

        // GET: Home page
        public IActionResult Index()
        {
            return View();
        }

        // POST: Register new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string email, string password, string role)
        {
            // Check if the username already exists
            if (_context.Users.Any(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View("Index");
            }

            // Hash the password before storing it
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Create new user object
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                Role = role
            };

            // Add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Prepare email body
            var subject = "Welcome to MYGUYY!";
            var body = $"Hello {username},\n\nThank you for registering with MYGUYY. Your role is {role}.\n\nBest regards,\nMYGUYY Team";

            try
            {
                // Send the confirmation email to the user using the injected email settings
                await _emailService.SendEmailAsync(email, subject, body);

                // Set a success message
                TempData["Message"] = "Registration successful! A confirmation email has been sent.";

                // Redirect to the login page or wherever appropriate
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                // Handle the error if email sending fails
                TempData["Error"] = "There was an error sending the confirmation email.";
                return RedirectToAction("Index");
            }
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
    }
}
