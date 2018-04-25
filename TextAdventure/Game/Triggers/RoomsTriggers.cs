using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Game.Game.Triggers
{
    public class RoomsTriggers : IStaticTriggers
    {
        private List<ITrigger> _triggerInstances = new List<ITrigger>();


        public T Fetch<T>() where T : ITrigger
        {
            foreach (var stateInstance in _triggerInstances)
            {
                if (stateInstance.GetType() == typeof(T))
                {
                    return (T)stateInstance;
                }
            }
            _triggerInstances.Add((T)Activator.CreateInstance(typeof(T)));
            return (T)_triggerInstances.Single(i => typeof(T) == i.GetType());
        }


        public class North : ITrigger
        {
        }

        public class South : ITrigger
        {
        }

        public class East : ITrigger
        {
        }

        public class West : ITrigger
        {
        }

        public class Up : ITrigger
        {
        }

        public class Down : ITrigger
        {
        }

    }



}
