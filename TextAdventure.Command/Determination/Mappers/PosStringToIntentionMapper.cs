using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Intention;

namespace TextAdventure.Command.Determination.Mappers
{
    public class PosStringToIntentionMapper
    {

        private Dictionary<string, BaseIntentions> _map = new Dictionary<string, BaseIntentions>();

        public void AddToMap(string key, BaseIntentions value)
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

        public Dictionary<string, BaseIntentions> GetMap()
        {
            return _map;
        }
    }
}
