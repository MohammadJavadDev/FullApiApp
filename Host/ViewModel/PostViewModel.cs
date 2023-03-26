using AutoMapper;
using Entities.Posts;
using WebFramework.Api;

namespace Host.ViewModel
{
    public class PostViewModel:BaseViewModel<PostViewModel, Post,int>
    {
 

        public string? Title { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }

        public string? CategoryName { get; set; }

        public string? AuthorFullName { get; set; }
    }
}
