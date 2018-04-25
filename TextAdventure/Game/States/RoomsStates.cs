using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Game.Displayables.Models;

namespace TextAdventure.Game.Game.States
{


    public class RoomsStates : IStates
    {
        public IList<Room> rooms { get; set; }

        public class Room : IState
        {
            public string id { get; set; }
            public bool isInside { get; set; }
            public bool? isTooCold { get; set; }
            public string south { get; set; }
            public string north { get; set; }
            public string east { get; set; }
            public string west { get; set; }
            public string down { get; set; }
            public string up { get; set; }
            public IList<Item> items { get; set; }
            public SouthExitModifiers southExitModifiers { get; set; }
            public NorthExitModifiers northExitModifiers { get; set; }
            public EastExitModifiers eastExitModifiers { get; set; }
            public WestExitModifiers westExitModifiers { get; set; }
            public DownExitModifiers downExitModifiers { get; set; }
            public UpExitModifiers upExitModifiers { get; set; }

            public class Item
            {
                public string itemId { get; set; }
                public string miscObjId { get; set; }
            }

            public class SouthExitModifiers
            {
                public bool exitAllowed { get; set; }
                public bool fastExitRequired { get; set; }
                public bool slowExitRequired { get; set; }
                public bool jumpExitRequired { get; set; }
                public bool crawlExitRequired { get; set; }
            }

            public class NorthExitModifiers
            {
                public bool exitAllowed { get; set; }
                public bool fastExitRequired { get; set; }
                public bool slowExitRequired { get; set; }
                public bool jumpExitRequired { get; set; }
                public bool crawlExitRequired { get; set; }
            }

            public class EastExitModifiers
            {
                public bool exitAllowed { get; set; }
                public bool fastExitRequired { get; set; }
                public bool slowExitRequired { get; set; }
                public bool jumpExitRequired { get; set; }
                public bool crawlExitRequired { get; set; }
            }

            public class WestExitModifiers
            {
                public bool exitAllowed { get; set; }
                public bool fastExitRequired { get; set; }
                public bool slowExitRequired { get; set; }
                public bool jumpExitRequired { get; set; }
                public bool crawlExitRequired { get; set; }
            }

            public class DownExitModifiers
            {
                public bool exitAllowed { get; set; }
                public bool fastExitRequired { get; set; }
                public bool slowExitRequired { get; set; }
                public bool jumpExitRequired { get; set; }
                public bool crawlExitRequired { get; set; }
            }

            public class UpExitModifiers
            {
                public bool exitAllowed { get; set; }
                public bool fastExitRequired { get; set; }
                public bool slowExitRequired { get; set; }
                public bool jumpExitRequired { get; set; }
                public bool crawlExitRequired { get; set; }
            }

            public void ExecuteServicesOnEntry(bool isReentry, IDisplayables displayables)
            {
                Console.WriteLine(displayables.GetDisplayableFromList<RoomsDisplayables.RoomsDis>(id).onEntryDescription);
            }

            public void DisposeServicesOnExit()
            {
            }

            public string FetchIdentifier()
            {
                return id;
            }
        }
    }






}
