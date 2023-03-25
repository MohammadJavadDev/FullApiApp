 

namespace WebFramework.Models
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }

        public string? CategoryName { get; set; }

        public string? AuthorFullName { get; set; }
    }
}
