using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Game.Displayables.Models;

namespace TextAdventure.Game.Displayables
{
    public class DisplayablesRepo
    {
        IList<IDisplayables> _allDisplayables = new List<IDisplayables>();

        public void AddPopulatedDisplayables<TResult>(Func<TResult> displayables) where TResult : IDisplayables
        {
            _allDisplayables.Add(displayables.Invoke());
        }

        public T GetADisplayables<T>()
        {
            return (T) _allDisplayables.Single(d => d.GetType() == typeof(T));
        }
    }
}
