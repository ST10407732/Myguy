using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MYGUYY.Models
{
    public class TransactionModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(TransactionStatus), ErrorMessage = "Invalid transaction status.")]
        public TransactionStatus Status { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow; // Default to current date/time

        // Foreign Key for User
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }
    }

    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
}
