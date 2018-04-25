using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TextAdventure.Command.Lexicons.Models.Infrastructure;

namespace TextAdventure.Command.Lexicons.Models
{

    public class MovementLex : ILex
    {

        public IList<Action> Actions { get; set; }

        public class Action : ICommonActions, ISpeedActions, IMethodActions
        {
            public Type OfLex
            {
                get
                {
                    return typeof(MovementLex);
                }
            }

            public string name { get; set; }
            public string speed { get; set; }
            public string method { get; set; }
            public IList<string> synonyms { get; set; }
            public IList<string> equivalenceClasses { get; set; }

        }
    }

}




