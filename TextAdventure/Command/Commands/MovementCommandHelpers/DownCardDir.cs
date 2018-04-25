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
    public class DownCardDir : ICardDir
    {
        private readonly DisplayablesRepo _displayablesRepo;

        private readonly RoomsStates.Room _currentRoomDetails;

        private readonly RoomsTriggers _roomsTriggers;

        public DownCardDir(DisplayablesRepo displayablesRepo, RoomsStates.Room currentRoomDetails, RoomsTriggers roomsTriggers)
        {
            _displayablesRepo = displayablesRepo;
            _currentRoomDetails = currentRoomDetails;
            _roomsTriggers = roomsTriggers;
        }

        public ITrigger FetchRoomTrigger
        {
            get
            {
                return _roomsTriggers.Fetch<RoomsTriggers.Down>();
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
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).downExitBlocked;
            }
        }

        public bool GetExitAllowedMod
        {
            get
            {
                return _currentRoomDetails.downExitModifiers.exitAllowed;
            }
        }

        public bool GetFastExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.downExitModifiers.fastExitRequired;
            }
        }

        public bool GetSlowExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.downExitModifiers.slowExitRequired;
            }
        }

        public string GetExitNotFastEnoughDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).downExitNotFastEnough;
            }
        }

        public string GetExitNotSlowEnoughDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).downExitNormalSpeedNotAllowed;
            }
        }

        public string GetNormalSpeedNotAllowedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).downExitNormalSpeedNotAllowed;
            }
        }

        public bool GetJumpExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.downExitModifiers.jumpExitRequired;
            }
        }

        public bool GetCrawlExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.downExitModifiers.crawlExitRequired;
            }
        }

        public string GetExitMustJumpDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).downExitMustJump;
            }
        }

        public string GetExitMustCrawlDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).downExitMustCrawl;
            }
        }

        public string GetWalkingMethodNotAllowedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).downExitWalkingMethodNotAllowed;
            }
        }

        public string GetExitOpenDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).downExitOpen;
            }
        }
    }
}
