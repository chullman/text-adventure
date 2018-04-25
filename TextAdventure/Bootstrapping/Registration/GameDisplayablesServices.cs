using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TextAdventure.Game.Displayables;
using TextAdventure.Infrastructure.Services.ContentProvider;
using TextAdventure.Infrastructure.Services.DataDeserializer;
using TextAdventure.Infrastructure.Services.DataReader;

namespace TextAdventure.Game.Bootstrapping.Registration
{
    public class GameDisplayablesServices : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonDeserializer>().As<IDeserializer>().SingleInstance();
            builder.RegisterType<EmbeddedStreamProvider>().As<IStreamProvider>().SingleInstance();
            builder.RegisterType<JsonContentProvider>().As<IContentProvider>().SingleInstance();

            
        }
    }
}
