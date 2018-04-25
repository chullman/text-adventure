using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Intention;

namespace TextAdventure.Command.Determination.Mappers
{
    public class IntentToIntentBuilderMapper
    {
        private Dictionary<BaseIntentions, Type> _map = new Dictionary<BaseIntentions, Type>();

        public void AddToMap(BaseIntentions key, Type value)
        {
            if (typeof(IBaseIntention).IsAssignableFrom(value))
            {
                try
                {
                    _map.Add(key, value);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                throw new ArgumentException("ERROR: Key is not of a type which implements IIntention");
            }
        }

        public Dictionary<BaseIntentions, Type> GetMap()
        {
            return _map;
        }
    }
}
