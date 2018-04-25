using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Game.Displayables.Models;

namespace TextAdventure.Game.Game.States
{
    public interface IState
    {
        void ExecuteServicesOnEntry(bool isReentry, IDisplayables displayables);

        void DisposeServicesOnExit();

        string FetchIdentifier();

    }
}
