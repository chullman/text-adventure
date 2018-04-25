using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Game.Displayables.Models
{
    public class GameStatesDisplayables : IDisplayables
    {
        public T GetDisplayable<T>() where T : IDisplayable
        {
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof (T))
                {
                    return (T)prop.GetValue(this, null);
                }
            }
            throw new Exception("Displayable not found");
        }

        public T GetDisplayableFromList<T>(string id) where T : IDisplayable
        {
            throw new NotImplementedException();
        }

        
        public ActiveDis activeDis { get; set; }
        public InactiveDis inactiveDis { get; set; }
        public PausedDis pausedDis { get; set; }
        public RunningDis runningDis { get; set; }
        public ExitDis exitDis { get; set; }
        public ErrorDis errorDis { get; set; }

          
        public class ActiveDis : IDisplayable
        {
            public string message { get; set; }
        }

        public class InactiveDis : IDisplayable
        {
            public string message { get; set; }
        }

        public class PausedDis : IDisplayable
        {
            public string message { get; set; }
        }

        public class RunningDis : IDisplayable
        {
            public string message { get; set; }
        }

        public class ExitDis : IDisplayable
        {
            public string message { get; set; }
        }

        public class ErrorDis : IDisplayable
        {
            public string message { get; set; }

        }



    }
    
}
