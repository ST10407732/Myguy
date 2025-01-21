using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MYGUYY.Models;

namespace MYGUYY.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        // Constructor receives email settings and logger
        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;  // Access the EmailSettings from configuration
            _logger = logger;
        }

        // Method to send email
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
                {
                    smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                    smtpClient.EnableSsl = _emailSettings.UseSsl;
                    smtpClient.Timeout = 10000; // Timeout in 10 seconds

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.FromEmail, _emailSettings.DisplayName),  // Sender's address
                        Subject = subject, // Subject from method parameter
                        Body = body,       // Body from method parameter
                        IsBodyHtml = true,  // Allow HTML in the body
                    };

                    mailMessage.To.Add(toEmail);  // Add recipient's email dynamically

                    // Send email asynchronously
                    await smtpClient.SendMailAsync(mailMessage);
                    return true;  // Return true if successful
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur
                _logger.LogError($"Error sending email to {toEmail}: {ex.Message}", ex);
                return false;  // Return false if there's an error
            }
        }
    }
}
