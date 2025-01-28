using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MYGUYY.Models
{
    public class TaskRequest
    {
        internal DateTime StartedAt;

        [Key]
        public int Id { get; set; } // Primary Key

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; } // Task Description

        public string? Status { get; set; } // Task Status: Pending, Accepted, Confirmed, Completed, etc.

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
        //public double? CurrentLatitude { get; set; }
        //public double? CurrentLongitude { get; set; }
        //[Required]
        public DateTime CreatedAt { get; set; } // Task creation timestamp

        public DateTime? AcceptedAt { get; set; } // Timestamp when the driver accepts the task
        public DateTime? ConfirmedAt { get; set; } // Timestamp when the task is confirmed by both parties
        public double AmountCollected { get; set; }
        public string? DriverConfirmationCode { get; set; } // Driver enters confirmation code
        public bool IsAgreementConfirmed { get; set; } // Indicates whether the agreement is confirmed

        // Navigation properties
        public User? Client { get; set; } // Navigation property to Client (User)
        public User? Driver { get; set; } // Navigation property to Driver (User)
        public ICollection<Stop> Stops { get; set; } = new List<Stop>(); // Collection of stops

        public double Cost { get; set; } // Total cost (driver + company share)
        public string VehicleType { get; set; } // Type of vehicle for the task
        public double RecommendedCost { get; set; } // System-generated recommended cost
        public string? ConfirmationCode { get; set; } // Confirmation code for mutual task agreement
    }
}
