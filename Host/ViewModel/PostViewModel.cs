using AutoMapper;
using Entities.Posts;
using WebFramework.Api;

namespace Host.ViewModel
{
    public class PostViewModel 
    {

        public string? Title { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }

        public string? CategoryName { get; set; }

        public string? AuthorFullName { get; set; }
    }
    public class PostViewModelMapper : BaseViewModel<PostViewModel, Post, int>
    {
        public PostViewModelMapper(IMapper mapper) : base(mapper)
        {
        }
    }
}
