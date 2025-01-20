using MYGUYY.Models;
using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; }

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; }

    [Required]
    public string Role { get; set; }
   


}
