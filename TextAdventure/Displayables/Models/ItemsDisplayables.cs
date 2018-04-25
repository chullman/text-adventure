using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Game.Displayables.Models
{

    public class ItemsDisplayables : IDisplayables
    {

        public T GetDisplayable<T>() where T : IDisplayable
        {
            throw new NotImplementedException();
        }

        public T GetDisplayableFromList<T>(string id) where T : IDisplayable
        {
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(IList<T>))
                {
                    IList<T> list = (IList<T>)prop.GetValue(this, null);
                    foreach (var item in list)
                    {
                        if ((string)item.GetType().GetProperty("id").GetValue(item, null) == id)
                        {
                            return item;
                        }

                    }
                }
            }
            throw new Exception("Displayable not found");
        }

        public IList<ItemsDis> itemsDis { get; set; }

        public class ItemsDis : IDisplayable
        {
            public string id { get; set; }
            public string lookAction { get; set; }
            public string getAction { get; set; }
            public string pushAction { get; set; }
            public string pullAction { get; set; }
        }
    }
}
