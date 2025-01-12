namespace MYGUYY.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int TaskRequestId { get; set; } // Associated task request
        public string Content { get; set; } // Message content
        public DateTime SentAt { get; set; } // Timestamp of when the message was sent

        // New properties for media files
        public string FilePath { get; set; } // Path to the uploaded file
        public string FileType { get; set; } // Type of the file, e.g., "image/jpeg", "video/mp4"

        // Navigation properties for related models
        public TaskRequest TaskRequest { get; set; }
    }
}
