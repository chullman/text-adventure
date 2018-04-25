using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TextAdventure.Game.Command;
using TextAdventure.Game.Command.Commands;
using TextAdventure.Game.Command.ResultListener;
using TextAdventure.Game.Command.ResultSubscriber;

namespace TextAdventure.Game.Bootstrapping.Registration
{
    public class CommandServices : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ResultSubscriber>().InstancePerDependency();
            builder.RegisterType<ResultListener>().InstancePerDependency();
            builder.RegisterType<ResultListener>().InstancePerDependency();

            builder.RegisterType<CommandHandler>().As<ICommandHandler>().InstancePerDependency();
        }
    }
}
