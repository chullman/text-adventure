using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Game.Command.Commands.MovementCommandHelpers;
using TextAdventure.Game.Displayables;
using TextAdventure.Game.Displayables.Models;
using TextAdventure.Game.Game.States;
using TextAdventure.Game.Game.Triggers;

namespace TextAdventure.Game.Command.Commands.MovementCommandHelpers
{
    public class WestCardDir : ICardDir
    {
        private readonly DisplayablesRepo _displayablesRepo;

        private readonly RoomsStates.Room _currentRoomDetails;

        private readonly RoomsTriggers _roomsTriggers;

        public WestCardDir(DisplayablesRepo displayablesRepo, RoomsStates.Room currentRoomDetails, RoomsTriggers roomsTriggers)
        {
            _displayablesRepo = displayablesRepo;
            _currentRoomDetails = currentRoomDetails;
            _roomsTriggers = roomsTriggers;
        }

        public ITrigger FetchRoomTrigger
        {
            get
            {
                return _roomsTriggers.Fetch<RoomsTriggers.West>();
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
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).westExitBlocked;
            }
        }

        public bool GetExitAllowedMod
        {
            get
            {
                return _currentRoomDetails.westExitModifiers.exitAllowed;
            }
        }

        public bool GetFastExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.westExitModifiers.fastExitRequired;
            }
        }

        public bool GetSlowExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.westExitModifiers.slowExitRequired;
            }
        }

        public string GetExitNotFastEnoughDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).westExitNotFastEnough;
            }
        }

        public string GetExitNotSlowEnoughDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).westExitNormalSpeedNotAllowed;
            }
        }

        public string GetNormalSpeedNotAllowedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).westExitNormalSpeedNotAllowed;
            }
        }

        public bool GetJumpExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.westExitModifiers.jumpExitRequired;
            }
        }

        public bool GetCrawlExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.westExitModifiers.crawlExitRequired;
            }
        }

        public string GetExitMustJumpDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).westExitMustJump;
            }
        }

        public string GetExitMustCrawlDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).westExitMustCrawl;
            }
        }

        public string GetWalkingMethodNotAllowedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).westExitWalkingMethodNotAllowed;
            }
        }

        public string GetExitOpenDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).westExitOpen;
            }
        }
    }
}
