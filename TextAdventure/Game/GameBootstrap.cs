using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TextAdventure.Game.Displayables.Models;
using TextAdventure.Game.Game.Machines;
using TextAdventure.Game.Game.States;
using TextAdventure.Game.Game.Triggers;
using TextAdventure.Infrastructure.Application;
using System.Reflection;
using TextAdventure.Command;
using TextAdventure.Command.Determination.Result.Models;
using TextAdventure.Game.Command;
using TextAdventure.Game.Command.Commands;
using TextAdventure.Game.Command.ResultListener;
using TextAdventure.Game.Command.ResultSubscriber;
using TextAdventure.Game.Configuration.Models;
using TextAdventure.Game.Displayables;
using TextAdventure.Infrastructure.Services.ContentProvider;

namespace TextAdventure.Game.Game
{
    public class GameBootstrap
    {
        private ApplicationRuntime _runtime;

        private List<IMachine> _initializedMachines = new List<IMachine>();

        private RoomsStates _roomsStates;
        private RoomsTriggers _roomsTriggers;

        private DisplayablesRepo _displayablesRepo;

        private void InitializeMachine<T>(IState initialState, IState errorState, IStates states, ITriggers triggers, IDisplayables displayables) where T : IMachine
        {
            var transitionHandler = _runtime.Container.Resolve<TransitionHandler>(new TypedParameter(typeof(IDisplayables), displayables));

            var machine = _runtime.Container.Resolve<T>(new TypedParameter(typeof(TransitionHandler), transitionHandler));

            machine.Start(initialState, errorState);
            machine.Configure(states, triggers);

            _initializedMachines.Add(machine);

        }

        public GameBootstrap(ApplicationRuntime runtime)
        {
            _runtime = runtime;
        }


        public void Initiate()
        {
            var currentAssemblyName = GetType().GetTypeInfo().Assembly.GetName().Name;

            var gameStates = _runtime.Container.Resolve<GameStates>();
            var gameTriggers = _runtime.Container.Resolve<GameTriggers>();

            var jsonContentProvider = _runtime.Container.Resolve<IContentProvider>();


            _displayablesRepo = _runtime.Container.Resolve<DisplayablesRepo>();

            _displayablesRepo.AddPopulatedDisplayables(() =>
                {
                    var displayables = _runtime.Container.Resolve<GameStatesDisplayables>();
                    jsonContentProvider.Populate<GameStatesDisplayables>(displayables, currentAssemblyName);
                    return displayables;
                });

            _displayablesRepo.AddPopulatedDisplayables(() =>
            {
                var displayables = _runtime.Container.Resolve<RoomsDisplayables>();
                jsonContentProvider.Populate<RoomsDisplayables>(displayables, currentAssemblyName);
                return displayables;
            });

            _displayablesRepo.AddPopulatedDisplayables(() =>
                {
                    var displayables = _runtime.Container.Resolve<DefaultsDisplayables>();
                    jsonContentProvider.Populate<DefaultsDisplayables>(displayables, currentAssemblyName);
                    return displayables;
                });

            _displayablesRepo.AddPopulatedDisplayables(() =>
                {
                    var displayables = _runtime.Container.Resolve<ItemsDisplayables>();
                    jsonContentProvider.Populate<ItemsDisplayables>(displayables, currentAssemblyName);
                    return displayables;
                });
                

            InitializeMachine<GameMachine>(
                gameStates.Fetch<GameStates.Inactive>(),
                gameStates.Fetch<GameStates.Error>(),
                gameStates,
                gameTriggers,
                _displayablesRepo.GetADisplayables<GameStatesDisplayables>());

            
            _initializedMachines.Single(m => m.GetType() == typeof(GameMachine))
                .Get()
                .TryFire(gameTriggers.Fetch<GameTriggers.Begin>());


            _roomsStates = _runtime.Container.Resolve<RoomsStates>();
            _roomsTriggers = _runtime.Container.Resolve<RoomsTriggers>();


            var playerConf = _runtime.Container.Resolve<PlayerConf>();

            jsonContentProvider.Populate<RoomsStates>(_roomsStates, currentAssemblyName);
            jsonContentProvider.Populate<PlayerConf>(playerConf, currentAssemblyName);


            InitializeMachine<RoomMachine>(
                _roomsStates.rooms.Single(r => r.id == playerConf.spawnRoom),
                _roomsStates.rooms.Single(r => r.id == "ERROR"),
                _roomsStates,
                _roomsTriggers,
                _displayablesRepo.GetADisplayables<RoomsDisplayables>());


            /*
            _initializedMachines.Single(m => m.GetType() == typeof(RoomMachine))
                .Get()
                .TryFire(roomsTriggers.Fetch<RoomsTriggers.South>());

            _initializedMachines.Single(m => m.GetType() == typeof(RoomMachine))
                .Get()
                .TryFire(roomsTriggers.Fetch<RoomsTriggers.East>());

            _initializedMachines.Single(m => m.GetType() == typeof(RoomMachine))
                .Get()
                .TryFire(roomsTriggers.Fetch<RoomsTriggers.South>());

            _initializedMachines.Single(m => m.GetType() == typeof(RoomMachine))
                .Get()
                .TryFire(roomsTriggers.Fetch<RoomsTriggers.North>()); 
            */
            
        }

        public void ProcessCommand(IResultModel resultModel, IList<string> resultModelPropsChanged)
        {

            var commandHandler = _runtime.Container.Resolve<ICommandHandler>(
                new TypedParameter(typeof(IResultModel), resultModel), 
                new TypedParameter(typeof(IList<string>), resultModelPropsChanged),
                new TypedParameter(typeof(DisplayablesRepo), _displayablesRepo)
                );

            if (resultModel.GetType() == typeof(MovementResultModel))
            {

                commandHandler.SetCommand<MovementCommand>();
                var command = (MovementCommand)commandHandler.GetCommand();

                // Inject in any other dependencies specific to movement commands
                command.InjectInRoomMachine(_initializedMachines.Single(m => m.GetType() == typeof(RoomMachine)).Get());

                command.InjectInRoomStates(_roomsStates);

                command.InjectInRoomTriggers(_roomsTriggers);

                command.Execute();
            }

            else if (resultModel.GetType() == typeof(LookResultModel))
            {
                commandHandler.SetCommand<LookCommand>();
                var command = (LookCommand)commandHandler.GetCommand();

                command.InjectInRoomMachine(_initializedMachines.Single(m => m.GetType() == typeof(RoomMachine)).Get());

                command.InjectInRoomStates(_roomsStates);

                command.Execute();

            }

        }
    }
}
