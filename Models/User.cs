using MYGUYY.Models;
using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(255, ErrorMessage = "Password cannot exceed 255 characters.")]
    public string PasswordHash { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    [RegularExpression("Client|Driver|Admin", ErrorMessage = "Role must be either 'Client', 'Driver', or 'Admin'.")]
    public string Role { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    public bool IsVerified { get; set; } = false;  // Default value for email verification

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Default value for account creation date
}
