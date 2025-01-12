namespace MYGUYY.Models.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public int TaskRequestId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public string FilePath { get; set; } // Path to the file (image or video)
        public string FileType { get; set; } // Type of file (e.g., "image/png", "video/mp4")
    }
}
