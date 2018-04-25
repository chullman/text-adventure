using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Autofac;
using Stateless;
using TextAdventure.Game.Game.States;
using TextAdventure.Game.Game.Triggers;

namespace TextAdventure.Game.Game.Machines
{
    
    public class GameMachine : IMachine
    {
        private StateMachine<IState, ITrigger> _gameMachine;

        private TransitionHandler _transitionHandler;
        
        public GameMachine(TransitionHandler transitionHandler)
        {
            _transitionHandler = transitionHandler;
        }

        public StateMachine<IState, ITrigger> Get()
        {
            
            return _gameMachine;
        }

        public void Start(IState initialState, IState errorState)
        {
            _gameMachine = new StateMachine<IState, ITrigger>(
                () => initialState != null ? (IState) initialState : errorState,
                newState => initialState = newState
            );

            _transitionHandler.HandleEntry(initialState);

        }


        public void Configure(IStates states, ITriggers triggers)
        {
            if (_gameMachine == null)
            {
                throw new Exception();
            }

            var gameState = (GameStates) states;
            var gameTrigger = (GameTriggers) triggers;


            _gameMachine.Configure(gameState.Fetch<GameStates.Exited>())
                .OnEntry(() => _transitionHandler.HandleEntry(gameState.Fetch<GameStates.Exited>()));

            _gameMachine.Configure(gameState.Fetch<GameStates.Inactive>())
                .OnEntry(() => _transitionHandler.HandleEntry(gameState.Fetch<GameStates.Inactive>()))
                .Permit(gameTrigger.Fetch<GameTriggers.Exit>(), gameState.Fetch<GameStates.Exited>())
                .Permit(gameTrigger.Fetch<GameTriggers.Begin>(), gameState.Fetch<GameStates.Active>());

            _gameMachine.Configure(gameState.Fetch<GameStates.Running>());

            _gameMachine.Configure(gameState.Fetch<GameStates.Active>())
                .SubstateOf(gameState.Fetch<GameStates.Running>())
                .OnEntry(() => _transitionHandler.HandleEntry(gameState.Fetch<GameStates.Active>()))
                .OnExit(() => _transitionHandler.HandleExit(gameState.Fetch<GameStates.Active>()))
                .Permit(gameTrigger.Fetch<GameTriggers.End>(), gameState.Fetch<GameStates.Inactive>())
                .Permit(gameTrigger.Fetch<GameTriggers.Pause>(), gameState.Fetch<GameStates.Paused>());

            _gameMachine.Configure(gameState.Fetch<GameStates.Paused>())
                .SubstateOf(gameState.Fetch<GameStates.Running>())
                .OnEntry(() => _transitionHandler.HandleEntry(gameState.Fetch<GameStates.Paused>()))
                .Permit(gameTrigger.Fetch<GameTriggers.Resume>(), gameState.Fetch<GameStates.Active>())
                .Permit(gameTrigger.Fetch<GameTriggers.End>(), gameState.Fetch<GameStates.Inactive>());

            _gameMachine.Configure(gameState.Fetch<GameStates.Error>())
                .OnEntry(() => _transitionHandler.HandleEntry(gameState.Fetch<GameStates.Error>()));

        }
    }
}
