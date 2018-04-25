using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Game.Displayables.Models
{
    public class DefaultsDisplayables : IDisplayables
    {
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

        public MovementDefaults movementDefaults { get; set; }
        public LookDefaults lookDefaults { get; set; }

        public class MovementDefaults : IDisplayable
        {
            public string unavailableDirection { get; set; }

            public string sameLocation { get; set; }

            public string locationWithNoRoomIdDefault { get; set; }

            public string differentDirections { get; set; }

            public string misunderstoodDirection { get; set; }

            public string unknownLocationDirection { get; set; }

            public string Display()
            {
                throw new NotImplementedException();
            }
        }

        public class LookDefaults : IDisplayable
        {
            public string noLookDir { get; set; }

            public string roomInDifferentLoc { get; set; }

            public string notInRoom { get; set; }

            public string itemNotInRoomNotPlural { get; set; }

            public string itemNotInRoomPlural { get; set; }
        }


    }
}
