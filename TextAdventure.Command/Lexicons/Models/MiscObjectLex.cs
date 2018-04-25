using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Lexicons.Models.Infrastructure;

namespace TextAdventure.Command.Lexicons.Models
{
    public class MiscObjectLex : ILex
    {

        public IList<Action> Actions { get; set; }

        public class Action : ICommonActions, IAdjActions, IMiscObjectIds
        {

            public Type OfLex
            {
                get
                {
                    return typeof(MiscObjectLex);
                }
            }

            public string name { get; set; }
            public IList<string> adjectives { get; set; }
            public IList<string> synonyms { get; set; }
            public IList<string> equivalenceClasses { get; set; }
            public string locId { get; set; }
            public string itemId { get; set; }
        }
    }
}
