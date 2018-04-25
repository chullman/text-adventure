using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Game.Configuration.Models
{
    public class PlayerConf
    {
        public string spawnRoom { get; set; }
        public IList<string> itemsCarrying { get; set; }
    }
}
