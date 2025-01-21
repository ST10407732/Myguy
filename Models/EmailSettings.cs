namespace MYGUYY.Models
{
    public class EmailSettings
    {
        // The email address used as the "From" address
        public string FromEmail { get; set; }

        // The SMTP server to send emails
        public string SmtpServer { get; set; }

        // The port for the SMTP server (usually 587 for TLS, 465 for SSL, 25 for no encryption)
        public int Port { get; set; }

        // The username for SMTP authentication (typically your email address)
        public string Username { get; set; }

        // The password for SMTP authentication (consider securing this)
        public string Password { get; set; }

        // Flag to indicate if SSL or TLS is required for the connection
        public bool UseSsl { get; set; } = true;

        // Optionally, you can add a display name for the sender
        public string DisplayName { get; set; } = "MYGUYY Service";
    }
}
