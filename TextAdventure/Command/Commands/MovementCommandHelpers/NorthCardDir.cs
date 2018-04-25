using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Game.Displayables;
using TextAdventure.Game.Displayables.Models;
using TextAdventure.Game.Game.States;
using TextAdventure.Game.Game.Triggers;

namespace TextAdventure.Game.Command.Commands.MovementCommandHelpers
{
    public class NorthCardDir : ICardDir
    {
        private readonly DisplayablesRepo _displayablesRepo;

        private readonly RoomsStates.Room _currentRoomDetails;

        private readonly RoomsTriggers _roomsTriggers;

        public NorthCardDir(DisplayablesRepo displayablesRepo, RoomsStates.Room currentRoomDetails, RoomsTriggers roomsTriggers)
        {
            _displayablesRepo = displayablesRepo;
            _currentRoomDetails = currentRoomDetails;
            _roomsTriggers = roomsTriggers;
        }

        public ITrigger FetchRoomTrigger
        {
            get
            {
                return _roomsTriggers.Fetch<RoomsTriggers.North>();
            } 
        }


        public string GetUnavailableDirDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.MovementDefaults>().unavailableDirection;
            }
        }

        public string GetDirExitBlockedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).northExitBlocked;
            }
        }

        public bool GetExitAllowedMod
        {
            get
            {
                return _currentRoomDetails.northExitModifiers.exitAllowed;
            }
        }

        public bool GetFastExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.northExitModifiers.fastExitRequired;
            }
        }

        public bool GetSlowExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.northExitModifiers.slowExitRequired;
            }
        }

        public string GetExitNotFastEnoughDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).northExitNotFastEnough;
            }
        }

        public string GetExitNotSlowEnoughDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).northExitNormalSpeedNotAllowed;
            }
        }

        public string GetNormalSpeedNotAllowedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).northExitNormalSpeedNotAllowed;
            }
        }

        public bool GetJumpExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.northExitModifiers.jumpExitRequired;
            }
        }

        public bool GetCrawlExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.northExitModifiers.crawlExitRequired;
            }
        }

        public string GetExitMustJumpDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).northExitMustJump;
            }
        }

        public string GetExitMustCrawlDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).northExitMustCrawl;
            }
        }

        public string GetWalkingMethodNotAllowedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).northExitWalkingMethodNotAllowed;
            }
        }

        public string GetExitOpenDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).northExitOpen;
            }
        }
    }
}
