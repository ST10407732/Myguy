using System;
using System.ComponentModel.DataAnnotations;

namespace MYGUYY.Models
{
    public class TaskRequest
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; } // Task Description

        public string Status { get; set; } // Task Status: Pending, Accepted, etc.

        [Required]
        public int ClientId { get; set; } // Foreign key to the client (User)

        public int? DriverId { get; set; } // Nullable foreign key to the driver (User)

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double PickupLatitude { get; set; } // Pickup location latitude

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double PickupLongitude { get; set; } // Pickup location longitude

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double DropoffLatitude { get; set; } // Dropoff location latitude

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double DropoffLongitude { get; set; } // Dropoff location longitude

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double? DriverLatitude { get; set; } // Driver's current latitude

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double? DriverLongitude { get; set; } // Driver's current longitude

        [Required]
        public DateTime CreatedAt { get; set; } // Task creation timestamp

        // Navigation properties
        public User Client { get; set; } // Navigation property to Client (User)
        public User Driver { get; set; } // Navigation property to Driver (User)

        //// Changed from 'object' to 'string'
        //public string PickupLocation { get; set; } // Pick-up location description
        //public string DropoffLocation { get; set; } // Drop-off location description
    }
}
