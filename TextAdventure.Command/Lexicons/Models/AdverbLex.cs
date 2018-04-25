using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Lexicons.Models.Infrastructure;

namespace TextAdventure.Command.Lexicons.Models
{
    public class AdverbLex : ILex
    {

        public IList<Action> Actions { get; set; }

        public class Action : ICommonActions, ISpeedActions
        {
            public Type OfLex
            {
                get
                {
                    return typeof(AdverbLex);
                }
            }

            public string name { get; set; }
            public string speed { get; set; }

            public IList<string> synonyms { get; set; }

            public IList<string> equivalenceClasses { get; set; }

        }
    }
}
