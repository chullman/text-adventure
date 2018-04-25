using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Lexicons.Models.Infrastructure
{
    public interface IAdjActions : IActionRoot
    {
        IList<string> adjectives { get; set; }
    }
}
