using AutoMapper;
using Entities.Posts;
using WebFramework.Mapper;

namespace Host.ViewModel
{
    public class PostCustomMapping : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<Post, PostViewModel>().ReverseMap()
         .ForMember(c => c.Author, c => c.Ignore())
         .ForMember(c => c.Category, c => c.Ignore());
        }
    }
}
 