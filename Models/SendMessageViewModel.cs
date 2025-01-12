namespace YourNamespace.ViewModels
{
    public class SendMessageViewModel
    {
        public int TaskId { get; set; } // Task ID
        public string Content { get; set; } // Message content
        public IFormFile File { get; set; } // Uploaded file
    }
}
