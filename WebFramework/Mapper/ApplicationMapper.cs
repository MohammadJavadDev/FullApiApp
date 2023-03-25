using AutoMapper;
using Entities.Posts;
using Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Models;

namespace WebFramework.Mapper
{
    public class ApplicationMapper :Profile
    {
        public ApplicationMapper() {
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<Post, PostViewModel>().ReverseMap()
                .ForMember(c=>c.Author,c=>c.Ignore())
                .ForMember(c=>c.Category,c=>c.Ignore());
        }
    }
}
