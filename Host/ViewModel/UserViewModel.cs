using AutoMapper;
using Entities.Posts;
using Entities.Users;
using System.ComponentModel.DataAnnotations;
using WebFramework.Mapper;

namespace Host.ViewModel
{
    public class UserViewModel
    {
        [Required]
        [StringLength(100)]
        public string? UserName { get; set; }
        [Required]
        [StringLength(500)]
        public string? Password { get; set; }

        public string? FullName { get; set; }

        public Gender? Gender { get; set; }




    }

    public class UserCustomMapping : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<User, UserViewModel>().ReverseMap()
                .ForMember(c => c.Posts, c =>c.Ignore());
 
        }
    }
}
