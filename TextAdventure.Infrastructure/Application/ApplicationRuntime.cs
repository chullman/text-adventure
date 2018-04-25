using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace TextAdventure.Infrastructure.Application
{
    public class ApplicationRuntime : IDisposable
    {
        public ApplicationRuntime(IContainer container)
        {
            Container = container;
        }

        public IContainer Container { get; private set; }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
