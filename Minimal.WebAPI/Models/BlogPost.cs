namespace Minimal.WebAPI.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = "Default Title";
        public string Content { get; set; } = "Sample Content";
        public bool IsPublished { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? PublishedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
