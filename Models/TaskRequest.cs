using System.ComponentModel.DataAnnotations;

namespace MYGUYY.Models
{
    public class TaskRequest
    {
        public int Id { get; set; }

        public string Status { get; set; } // Pending, Accepted, Declined, etc.
        public int ClientId { get; set; } // Foreign key to User (Client)
        public int? DriverId { get; set; } // Nullable foreign key to User (Driver)
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User Client { get; set; }
        public User Driver { get; set; }

        // Pick-up and Drop-off locations with latitude and longitude

        [Required]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double PickupLatitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double PickupLongitude { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double DropoffLatitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double DropoffLongitude { get; set; }
    }

}