using Microsoft.AspNetCore.Mvc;
using MYGUYY.Services;
using System.Threading.Tasks;

namespace MYGUYY.Controllers
{
    public class EmailController : Controller
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<IActionResult> SendTestEmail()
        {
            var success = await _emailService.SendEmailAsync("recipient@example.com", "Test Email", "Hello, this is a test email from MYGUYY.");

            if (success)
            {
                ViewBag.Message = "Email sent successfully!";
            }
            else
            {
                ViewBag.Message = "Failed to send email.";
            }

            return View();
        }
    }
}
