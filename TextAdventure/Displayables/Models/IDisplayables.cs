using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Game.Displayables.Models
{
    public interface IDisplayables
    {
        T GetDisplayable<T>() where T : IDisplayable;

        T GetDisplayableFromList<T>(string id) where T : IDisplayable;
    }
}
