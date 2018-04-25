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
    public class UpCardDir : ICardDir
    {
        private readonly DisplayablesRepo _displayablesRepo;

        private readonly RoomsStates.Room _currentRoomDetails;

        private readonly RoomsTriggers _roomsTriggers;

        public UpCardDir(DisplayablesRepo displayablesRepo, RoomsStates.Room currentRoomDetails, RoomsTriggers roomsTriggers)
        {
            _displayablesRepo = displayablesRepo;
            _currentRoomDetails = currentRoomDetails;
            _roomsTriggers = roomsTriggers;
        }

        public ITrigger FetchRoomTrigger
        {
            get
            {
                return _roomsTriggers.Fetch<RoomsTriggers.Up>();
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
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).upExitBlocked;
            }
        }

        public bool GetExitAllowedMod
        {
            get
            {
                return _currentRoomDetails.upExitModifiers.exitAllowed;
            }
        }

        public bool GetFastExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.upExitModifiers.fastExitRequired;
            }
        }

        public bool GetSlowExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.upExitModifiers.slowExitRequired;
            }
        }

        public string GetExitNotFastEnoughDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).upExitNotFastEnough;
            }
        }

        public string GetExitNotSlowEnoughDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).upExitNormalSpeedNotAllowed;
            }
        }

        public string GetNormalSpeedNotAllowedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).upExitNormalSpeedNotAllowed;
            }
        }

        public bool GetJumpExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.upExitModifiers.jumpExitRequired;
            }
        }

        public bool GetCrawlExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.upExitModifiers.crawlExitRequired;
            }
        }

        public string GetExitMustJumpDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).upExitMustJump;
            }
        }

        public string GetExitMustCrawlDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).upExitMustCrawl;
            }
        }

        public string GetWalkingMethodNotAllowedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).upExitWalkingMethodNotAllowed;
            }
        }

        public string GetExitOpenDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).upExitOpen;
            }
        }
    }
}
