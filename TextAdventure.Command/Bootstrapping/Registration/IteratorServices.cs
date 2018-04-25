using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TextAdventure.Command.Iterator;

namespace TextAdventure.Command.Bootstrapping.Registration
{
    public class IteratorServices : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CommandAggregatesRepo>().InstancePerDependency(); 

            builder.RegisterGeneric(typeof(TokensAggregate<>)).As(typeof(IListAggregate<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(TokensIterator<>)).As(typeof(IListIterator<>)).InstancePerDependency();

            builder.RegisterGeneric(typeof(WordsInDictAggregate<,>)).As(typeof(IDictionaryAggregate<,>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(WordsInDictIterator<,>)).As(typeof(IDictionaryIterator<,>)).InstancePerDependency();
        }
    }
}
