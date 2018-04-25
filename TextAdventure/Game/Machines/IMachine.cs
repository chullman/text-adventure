using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using TextAdventure.Game.Game.States;
using TextAdventure.Game.Game.Triggers;


namespace TextAdventure.Game.Game.Machines
{
    public interface IMachine
    {
        void Start(IState initialState, IState errorState);

        void Configure(IStates states, ITriggers triggers);

        StateMachine<IState, ITrigger> Get();
    }
}
