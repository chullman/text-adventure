using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TextAdventure.Command.Lexicons;
using TextAdventure.Command.Lexicons.Models;
using TextAdventure.Infrastructure.Services.ContentProvider;
using TextAdventure.Infrastructure.Services.DataDeserializer;
using TextAdventure.Infrastructure.Services.DataReader;

namespace TextAdventure.Command.Bootstrapping.Registration
{
    public class LexiconServices : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonDeserializer>().As<IDeserializer>().SingleInstance();
            builder.RegisterType<EmbeddedStreamProvider>().As<IStreamProvider>().SingleInstance();
            builder.RegisterType<JsonContentProvider>().As<IContentProvider>().SingleInstance();

            builder.RegisterType<LexRepo>().SingleInstance();

            builder.RegisterType<MovementLex>().SingleInstance();
            builder.RegisterType<AdverbLex>().SingleInstance();
            builder.RegisterType<AdjectivesLex>().SingleInstance();
            builder.RegisterType<InventoryLex>().SingleInstance();
            builder.RegisterType<PoiLex>().SingleInstance();
            builder.RegisterType<TakeLex>().SingleInstance();
            builder.RegisterType<DirectionsLex>().SingleInstance();
            builder.RegisterType<LocationsLex>().SingleInstance();
            builder.RegisterType<LookLex>().SingleInstance();
            builder.RegisterType<AdverbModLex>().SingleInstance();
            builder.RegisterType<MiscObjectLex>().SingleInstance();


        }

    }
}
