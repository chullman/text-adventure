using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TextAdventure.Command;
using TextAdventure.Command.Bootstrapping.Registration;
using TextAdventure.Command.Determination.Result.Models;
using TextAdventure.Game.Bootstrapping.Registration;
using TextAdventure.Game.Command.ResultListener;
using TextAdventure.Game.Command.ResultSubscriber;
using TextAdventure.Game.Game;
using TextAdventure.Infrastructure.Application;

namespace TextAdventure.Game
{
    class Program
    {
        static void Main(string[] args)
        {
            var runtime = DefaultApplication.Build().Bootstrap
            (
                new GameDisplayablesServices(),
                new GameServices(),
                new CommandServices(),

                new LexiconServices(),
                new NlpServices(),
                new DeterminationServices(),
                new IteratorServices(),
                new ComDisplayablesServices()
            );

            var commandBootstrap = new CommandBootstrap(runtime);

            var gameBootstrap = new GameBootstrap(runtime);

            commandBootstrap.Initiate();

            gameBootstrap.Initiate();

            string stringInput;
            do
            {
                Console.Write(@"> ");
                stringInput = Console.ReadLine();

                var successfulDetermination = commandBootstrap.BootstrapCommandDetermination(stringInput);

                if (successfulDetermination)
                {
                    var commandResultListener = runtime.Container.Resolve<ResultListener>();
                    var commandResultSubscriber = runtime.Container.Resolve<ResultSubscriber>(new TypedParameter(typeof(ResultListener), commandResultListener));

                    commandResultSubscriber.Subscribe(commandBootstrap.GetCommandResultModelHandlerForGame());

                    commandBootstrap.BootstrapCommandResult();

                    commandResultSubscriber.Unsubscribe();

                    if (commandResultListener.GetObjectThatWasChanged() != null)
                    {
                        gameBootstrap.ProcessCommand(commandResultListener.GetObjectThatWasChanged(), commandResultListener.GetPropsThatWereChanged());
                    }
                    else
                    {
                        Debug.WriteLine("ERROR: Listener didn't capture any command result model that was changed.");
                    }
                }

            } while (stringInput != null && stringInput.ToLower() != "exit");

        }

    }
}
