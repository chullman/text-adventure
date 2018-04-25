using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TextAdventure.Command.Lexicons.Models.Infrastructure;

namespace TextAdventure.Command.Lexicons.Models
{
    public class PoiLex : ILex
    {

        public IList<Action> Actions { get; set; }

        public class Action : ICommonActions, IAdjActions, IPoiPluralActions, IPoiIds
        {
            public Action()
            {
                poiPlural = false;
            }

            public Type OfLex
            {
                get
                {
                    return typeof(PoiLex);
                }
            }

            public string name { get; set; }
            public IList<string> adjectives { get; set; }
            public IList<string> synonyms { get; set; }
            public IList<string> equivalenceClasses { get; set; }
            public bool poiPlural { get; set; }
            public string locId { get; set; }
            public string itemId { get; set; }
        }
    }


}
