using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TextAdventure.Game.Configuration.Models;
using TextAdventure.Game.Displayables;
using TextAdventure.Game.Displayables.Models;
using TextAdventure.Game.Game.Machines;
using TextAdventure.Game.Game.States;
using TextAdventure.Game.Game.Triggers;

namespace TextAdventure.Game.Bootstrapping.Registration
{
    public class GameServices : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GameMachine>().SingleInstance();
            builder.RegisterType<RoomMachine>().SingleInstance();

            builder.RegisterType<TransitionHandler>().InstancePerDependency();

            builder.RegisterType<GameStates>().SingleInstance();
            builder.RegisterType<GameTriggers>().SingleInstance();

            builder.RegisterType<RoomsStates>().SingleInstance();
            builder.RegisterType<RoomsTriggers>().SingleInstance();

            builder.RegisterType<PlayerConf>().SingleInstance();

            builder.RegisterType<DisplayablesRepo>().SingleInstance();

            builder.RegisterType<DefaultsDisplayables>();
            builder.RegisterType<GameStatesDisplayables>().SingleInstance();
            builder.RegisterType<RoomsDisplayables>();
            builder.RegisterType<ItemsDisplayables>();
        }
    }
}
