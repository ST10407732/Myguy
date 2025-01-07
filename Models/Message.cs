namespace MYGUYY.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int TaskRequestId { get; set; } // Associated task request
        public string Content { get; set; } // Message content
        public DateTime SentAt { get; set; } // Timestamp of when the message was sent

        // Navigation properties for related models
        public TaskRequest TaskRequest { get; set; }
       
    }
}
