using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace TextAdventure.Infrastructure.Application
{
    public class DefaultApplication : IApplication<ApplicationRuntime>
    {
        public ApplicationRuntime Bootstrap(params Module[] moduleToRegister)
        {
            var builder = new ContainerBuilder();

            foreach (var aModule in moduleToRegister)
            {
                builder.RegisterModule(aModule);
            }


            var appContainer = builder.Build();

            return new ApplicationRuntime(appContainer);
        }

        public static DefaultApplication Build()
        {
            return new DefaultApplication();
        }

    }
}
