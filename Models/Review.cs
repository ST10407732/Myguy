namespace MYGUYY.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; } // Example: 1-5 stars
        public string Comments { get; set; }
    }
}
