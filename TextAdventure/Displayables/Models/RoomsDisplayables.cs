using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Game.Displayables.Models
{
    public class RoomsDisplayables : IDisplayables
    {


        public T GetDisplayable<T>() where T : IDisplayable
        {
            throw new NotImplementedException();
        }

        public T GetDisplayableFromList<T>(string id) where T : IDisplayable
        {
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof (IList<T>))
                {
                    IList<T> list = (IList<T>)prop.GetValue(this,null);
                    foreach (var item in list)
                    {
                        if ((string)item.GetType().GetProperty("id").GetValue(item,null) == id)
                        {
                            return item;
                        }

                    }
                }
            }
            throw new Exception("Displayable not found");
        }

        public IList<RoomsDis> roomsDis { get; set; }


        public class RoomsDis : IDisplayable
        {
            public string id { get; set; }
            public string onEntryDescription { get; set; }
            public string lookAction { get; set; }

            public string lookNorth { get; set; }
            public string northExitOpen { get; set; }
            public string northExitBlocked { get; set; }
            public string northExitNotFastEnough { get; set; }
            public string northExitNotSlowEnough { get; set; }
            public string northExitNormalSpeedNotAllowed { get; set; }
            public string northExitMustJump { get; set; }
            public string northExitMustCrawl { get; set; }
            public string northExitWalkingMethodNotAllowed { get; set; }
            public string lookSouth { get; set; }
            public string southExitOpen { get; set; }
            public string southExitBlocked { get; set; }
            public string southExitNotFastEnough { get; set; }
            public string southExitNotSlowEnough { get; set; }
            public string southExitNormalSpeedNotAllowed { get; set; }
            public string southExitMustJump { get; set; }
            public string southExitMustCrawl { get; set; }
            public string southExitWalkingMethodNotAllowed { get; set; }
            public string lookEast { get; set; }
            public string eastExitOpen { get; set; }
            public string eastExitBlocked { get; set; }
            public string eastExitNotFastEnough { get; set; }
            public string eastExitNotSlowEnough { get; set; }
            public string eastExitNormalSpeedNotAllowed { get; set; }
            public string eastExitMustJump { get; set; }
            public string eastExitMustCrawl { get; set; }
            public string eastExitWalkingMethodNotAllowed { get; set; }
            public string lookWest { get; set; }
            public string westExitOpen { get; set; }
            public string westExitBlocked { get; set; }
            public string westExitNotFastEnough { get; set; }
            public string westExitNotSlowEnough { get; set; }
            public string westExitNormalSpeedNotAllowed { get; set; }
            public string westExitMustJump { get; set; }
            public string westExitMustCrawl { get; set; }
            public string westExitWalkingMethodNotAllowed { get; set; }
            public string lookUp { get; set; }
            public string upExitOpen { get; set; }
            public string upExitBlocked { get; set; }
            public string upExitNotFastEnough { get; set; }
            public string upExitNotSlowEnough { get; set; }
            public string upExitNormalSpeedNotAllowed { get; set; }
            public string upExitMustJump { get; set; }
            public string upExitMustCrawl { get; set; }
            public string upExitWalkingMethodNotAllowed { get; set; }
            public string lookDown { get; set; }
            public string downExitOpen { get; set; }
            public string downExitBlocked { get; set; }
            public string downExitNotFastEnough { get; set; }
            public string downExitNotSlowEnough { get; set; }
            public string downExitNormalSpeedNotAllowed { get; set; }
            public string downExitMustJump { get; set; }
            public string downExitMustCrawl { get; set; }
            public string downExitWalkingMethodNotAllowed { get; set; }

        }


    }




}
