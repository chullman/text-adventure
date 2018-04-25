using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Game.Game.States
{
    public interface IStaticStates : IStates
    {
        T Fetch<T>() where T : IState;
    }
}
