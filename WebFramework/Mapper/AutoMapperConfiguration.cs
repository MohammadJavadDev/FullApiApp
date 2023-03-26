using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.Mapper
{
    public static class AutoMapperConfiguration
    {
        public static void InitilizeAutoMapper(this IServiceCollection    service)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddCustomMappingProfile();
            });
            mapperConfig.CompileMappings();
            IMapper mapper = mapperConfig.CreateMapper();
             

            service.AddSingleton(mapper);
 
        }

        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config)
        {
            config.AddCustomMappingProfile(Assembly.GetEntryAssembly());
        }

        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config,params Assembly[] assemblies)
        {
            var allTypes = assemblies.SelectMany(c=>c.ExportedTypes);

            var list = allTypes.Where(c=>c.IsClass && !c.IsAbstract 
            && c.GetInterfaces().Contains(typeof(IHaveCustomMapping)))
                .Select(c=>(IHaveCustomMapping)Activator.CreateInstance(c));

            config.AddProfile(new CustomMappingProfiule(list));
        }
    }
}
