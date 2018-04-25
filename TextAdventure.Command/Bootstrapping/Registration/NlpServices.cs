using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using TextAdventure.Command.NLP;
using TextAdventure.Command.NLP.Models;
using TextAdventure.Command.NLP.ModelValidation;
using TextAdventure.Command.NLP.ModelValidation.Validators;

namespace TextAdventure.Command.Bootstrapping.Registration
{
    public class NlpServices : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CoreNlpProcessor>().SingleInstance();

            builder.RegisterType<NlpResult>().InstancePerDependency();

            builder.RegisterType<BasicValidatorHandler>().InstancePerDependency();
            builder.RegisterType<NlpResultSentenceValidator>().As<AbstractValidator<Sentence>>().InstancePerDependency();
            builder.RegisterType<NlpResultRootValidator>().As<AbstractValidator<NlpResult>>().InstancePerDependency();
            builder.RegisterType<NlpResultTokenValidator>().As<AbstractValidator<Token>>().InstancePerDependency();
            builder.RegisterType<NlpResultTokenColValidator>().As<AbstractValidator<IList<Token>>>().InstancePerDependency();

        }

    }
}
