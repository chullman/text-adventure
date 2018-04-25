using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TextAdventure.Command.Determination;
using TextAdventure.Command.Determination.Intention;
using TextAdventure.Command.Determination.Mappers;
using TextAdventure.Command.Determination.Result.Handlers;
using TextAdventure.Command.Determination.Result.Models;

namespace TextAdventure.Command.Bootstrapping.Registration
{
    public class DeterminationServices : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            
            builder.RegisterType<MapperRepo>().SingleInstance();

            builder.RegisterType<LexToIntentionMapper>().SingleInstance();
            builder.RegisterType<NerStringToIntentionMapper>().SingleInstance();
            builder.RegisterType<PosStringToIntentionMapper>().SingleInstance();
            builder.RegisterType<IntentToIntentBuilderMapper>().SingleInstance();

            builder.RegisterType<IntentionBuilderFactory>().SingleInstance();

            builder.RegisterType<CommandResultModelHandler>().InstancePerDependency();

            builder.RegisterType<MovementResultModel>().InstancePerDependency();
            builder.RegisterType<LookResultModel>().InstancePerDependency();

        }

    }
}
