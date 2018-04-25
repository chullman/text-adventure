using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Displayables.Models
{
    public class ErrorDisplayables : IDisplayables
    {
        public CommandValidationErrors commandValidationErrors { get; set; }

        public T GetDisplayable<T>() where T : IDisplayable
        {
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(T))
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

        public class CommandValidationErrors : IDisplayable
        {
            public string moreThanOneSentence { get; set; }
            public string emptyCommand { get; set; }
        }
    }


}
