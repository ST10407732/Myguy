namespace MYGUYY.Models
{
    public class UserTask
    {
        public int Id { get; set; } // Unique identifier for the task
        public string Description { get; set; } // Description of the task
        public DateTime DueDate { get; set; } // Task's due date
        public bool IsCompleted { get; set; } // Status of task completion
        public string Status { get; set; } // Task's current status
        public decimal Price { get; set; } // Price associated with the task
        /*public int UserId { get; set; } //*//* ID of the user assigned to the task*/
       /* public int DriverId { get;*/ /*set; } */// ID of the driver assigned to the task
    }
}
