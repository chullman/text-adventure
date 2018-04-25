using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TextAdventure.Command.Displayables.Models;
using TextAdventure.Infrastructure.Services.ContentProvider;
using TextAdventure.Infrastructure.Services.DataDeserializer;
using TextAdventure.Infrastructure.Services.DataReader;

namespace TextAdventure.Command.Bootstrapping.Registration
{
    public class ComDisplayablesServices : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonDeserializer>().As<IDeserializer>().SingleInstance();
            builder.RegisterType<EmbeddedStreamProvider>().As<IStreamProvider>().SingleInstance();
            builder.RegisterType<JsonContentProvider>().As<IContentProvider>().SingleInstance();

            builder.RegisterType<ErrorDisplayables>().SingleInstance();
        }

    }
}
