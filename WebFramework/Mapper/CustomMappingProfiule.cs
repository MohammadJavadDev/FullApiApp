using AutoMapper;

namespace WebFramework.Mapper
{
    internal class CustomMappingProfiule : Profile
    {
 
        public CustomMappingProfiule(IEnumerable<IHaveCustomMapping?> list)
        {
            foreach(var item in list)
            {
                item.CreateMappings(this);
            }
            
        }
    }
}