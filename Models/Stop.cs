using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MYGUYY.Models
{
    public class Stop
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double Latitude { get; set; } // Stop latitude

        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double Longitude { get; set; } // Stop longitude

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The stop order must be greater than 0.")]
        public int Order { get; set; } // Sequence of the stop (should be a positive integer)

        [Required]
        public int TaskRequestId { get; set; } // Foreign key to TaskRequest

        // Navigation property
        [ForeignKey("TaskRequestId")]
        public TaskRequest TaskRequest { get; set; } // Related TaskRequest
    }
}
