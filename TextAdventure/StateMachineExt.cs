using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using TextAdventure.Game.Game.States;
using TextAdventure.Game.Game.Triggers;

namespace TextAdventure.Game
{
    public static class StateMachineExt
    {
        public static void TryFire<S, T>(this StateMachine<S, T> stateMachine, T gameTrigger)
            where S : IState
            where T : ITrigger
        {

            try
            {
                stateMachine.Fire(gameTrigger);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("ERROR!");
                throw;
            }


        }
    }
}
