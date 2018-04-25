using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Lexicons.Models.Infrastructure
{
    public interface IPoiIds : IActionRoot
    {
        string locId { get; set; }
        string itemId { get; set; }
    }
}
