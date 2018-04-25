using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TextAdventure.Command.Lexicons.Models.Infrastructure;

namespace TextAdventure.Command.Lexicons.Models
{

    public class DirectionsLex : ILex
    {

        public IList<Action> Actions { get; set; }

        public class Action : ICommonActions
        {
            public Type OfLex
            {
                get
                {
                    return typeof(DirectionsLex);
                }
            }

            public string name { get; set; }
            public IList<string> synonyms { get; set; }
            public IList<string> equivalenceClasses { get; set; }

        }
    }

}




