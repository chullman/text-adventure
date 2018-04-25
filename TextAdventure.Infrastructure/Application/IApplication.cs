using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace TextAdventure.Infrastructure.Application
{
    public interface IApplication<out T> where T : IDisposable
    {
        T Bootstrap(params Module[] moduleToRegister);
    }
}
