using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using TextAdventure.Game.Game.States;
using TextAdventure.Game.Game.Triggers;


namespace TextAdventure.Game.Game.Machines
{
    public class RoomMachine : IMachine
    {
        private StateMachine<IState, ITrigger> _roomMachine;

        private TransitionHandler _transitionHandler;
        
        public RoomMachine(TransitionHandler transitionHandler)
        {
            _transitionHandler = transitionHandler;
        }

        public StateMachine<IState, ITrigger> Get()
        {
            return _roomMachine;
        }

        public void Start(IState initialState, IState errorState)
        {
            _roomMachine = new StateMachine<IState, ITrigger>(
                                () => initialState != null ? (IState)initialState : errorState,
                                newState => initialState = newState
                            );

            _transitionHandler.HandleEntry(initialState);
        }


        public void Configure(IStates states, ITriggers triggers)
        {
            if (_roomMachine == null)
            {
                throw new Exception();
            }

            var roomsStates = (RoomsStates)states;
            var roomsTriggers = (RoomsTriggers)triggers;


            foreach (var roomState in roomsStates.rooms)
            {
                _roomMachine.Configure(roomState)
                    .OnEntry(() => _transitionHandler.HandleEntry(roomState))
                    .OnExit(() => _transitionHandler.HandleExit(roomState));

                if (!(String.IsNullOrEmpty(roomState.north)))
                {
                    _roomMachine.Configure(roomState)
                        .Permit(roomsTriggers.Fetch<RoomsTriggers.North>(), roomsStates.rooms.Single(rs => rs.id == roomState.north));
                }

                if (!(String.IsNullOrEmpty(roomState.south)))
                {
                    _roomMachine.Configure(roomState)
                        .Permit(roomsTriggers.Fetch<RoomsTriggers.South>(), roomsStates.rooms.Single(rs => rs.id == roomState.south));
                }

                if (!(String.IsNullOrEmpty(roomState.east)))
                {
                    _roomMachine.Configure(roomState)
                        .Permit(roomsTriggers.Fetch<RoomsTriggers.East>(), roomsStates.rooms.Single(rs => rs.id == roomState.east));
                }

                if (!(String.IsNullOrEmpty(roomState.west)))
                {
                    _roomMachine.Configure(roomState)
                        .Permit(roomsTriggers.Fetch<RoomsTriggers.West>(), roomsStates.rooms.Single(rs => rs.id == roomState.west));
                }

                if (!(String.IsNullOrEmpty(roomState.up)))
                {
                    _roomMachine.Configure(roomState)
                        .Permit(roomsTriggers.Fetch<RoomsTriggers.Up>(), roomsStates.rooms.Single(rs => rs.id == roomState.up));
                }

                if (!(String.IsNullOrEmpty(roomState.down)))
                {
                    _roomMachine.Configure(roomState)
                        .Permit(roomsTriggers.Fetch<RoomsTriggers.Down>(), roomsStates.rooms.Single(rs => rs.id == roomState.down));
                }

            }

        }


    }
}
