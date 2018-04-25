using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Lexicons.Models.Infrastructure
{
    public interface ICommonActions : IActionRoot
    {
        
        string name { get; set; }

        IList<string> synonyms { get; set; }

        IList<string> equivalenceClasses { get; set; }
          
    }
}
