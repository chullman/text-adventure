using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using TextAdventure.Game.Displayables.Models;
using TextAdventure.Game.Game.States;


namespace TextAdventure.Game.Game.Machines
{
    public class TransitionHandler
    {
        private List<string> isReentryRepo = new List<string>();
        private bool isReentry;

        private IDisplayables _displayables;

        public TransitionHandler()
        {
        }

        public TransitionHandler(IDisplayables displayables)
        {
            _displayables = displayables;
        }

        public void HandleEntry(IState transitionedState)
        {

            if (isReentryRepo.Contains(transitionedState.FetchIdentifier()))
            {
                isReentry = true;
            }
            else
            {
                isReentry = false;
                try
                {
                    isReentryRepo.Add(transitionedState.FetchIdentifier());
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }

            transitionedState.ExecuteServicesOnEntry(isReentry, _displayables);

        }

        public void HandleExit(IState transitionedState)
        {
            transitionedState.DisposeServicesOnExit();
        }

    }
}
