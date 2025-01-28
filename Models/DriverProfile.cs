using System.ComponentModel.DataAnnotations;

namespace MYGUYY.Models
{
    public class DriverProfile
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //public int UserId { get; set; } // Foreign key to User

        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; }

        [Required]
        public DateTime LicenseExpiry { get; set; }

        [Required]
        public string VehicleType { get; set; }

        [Required]
        public string VehicleRegistration { get; set; }

        public string? IDDocumentPath { get; set; } // File path to uploaded ID

        public string? LicenseDocumentPath { get; set; } // File path to uploaded license

        public bool IsVerified { get; set; } = false;


    }
}