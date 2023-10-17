using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Data.Contracts;
using Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Services.SdkServices;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Entities;
using Data;

namespace WebFramework.Bootstrap
{
    public   class AutofacBootsratp : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var commonAssembly = typeof(ApiResultStatusCode).Assembly;
            var entityiesAssembly = typeof(IEntity).Assembly;
            var dataAssembly = typeof(ApplicationDbContext).Assembly;
            var servicesAssembly = typeof(JwtService).Assembly;

 

            builder.RegisterAssemblyTypes(commonAssembly, entityiesAssembly,
                dataAssembly, servicesAssembly)
                .AssignableTo<IScopedDependency>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(commonAssembly, entityiesAssembly,
                   dataAssembly, servicesAssembly)
                   .AssignableTo<ITransientDependency>()
                   .AsImplementedInterfaces()
                   .InstancePerDependency();

            builder.RegisterAssemblyTypes(commonAssembly, entityiesAssembly,
                   dataAssembly, servicesAssembly)
                   .AssignableTo<ISingletonDependency>()
                   .AsImplementedInterfaces()
                   .SingleInstance();

        }
        //public static ContainerBuilder BuildAutofacServiceProvider(this IServiceCollection services)
        //{

        //    var containerBuilder = new ContainerBuilder();
        //    containerBuilder.Populate(services);

        //    containerBuilder.AddServices();





        //    return containerBuilder;
        //}

        //public static void AddServices(this ContainerBuilder containerBuilder)
        //{




        //}
    }
}
