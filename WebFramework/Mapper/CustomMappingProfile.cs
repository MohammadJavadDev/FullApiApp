using AutoMapper;

namespace WebFramework.Mapper
{
    internal class CustomMappingProfile : Profile
    {
 
        public CustomMappingProfile(IEnumerable<IHaveCustomMapping?> list)
        {
            foreach(var item in list)
            {
                item.CreateMappings(this);
            }
            
        }
    }
}