using System.ComponentModel.DataAnnotations;

namespace MYGUYY.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public int UserId { get; set; }  // Foreign key to the User model

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int TaskId { get; set; }

        //Navigation property to link to User
        public User User { get; set; }
    }
}
