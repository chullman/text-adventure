using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Intention;
using TextAdventure.Command.Lexicons.Models.Infrastructure;


namespace TextAdventure.Command.Determination.Mappers
{
    public class LexToIntentionMapper
    {

        private Dictionary<Type, BaseIntentions> _map = new Dictionary<Type, BaseIntentions>();

        public void AddToMap(Type key, BaseIntentions value)
        {
            if (typeof(ILex).IsAssignableFrom(key))
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
                throw new ArgumentException("ERROR: Key is not of a type which implements ILex");
            }
        }

        public Dictionary<Type, BaseIntentions> GetMap()
        {
            return _map;
        }

    }
}
