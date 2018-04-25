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
    public class SouthCardDir : ICardDir
    {
        private readonly DisplayablesRepo _displayablesRepo;

        private readonly RoomsStates.Room _currentRoomDetails;

        private readonly RoomsTriggers _roomsTriggers;

        public SouthCardDir(DisplayablesRepo displayablesRepo, RoomsStates.Room currentRoomDetails, RoomsTriggers roomsTriggers)
        {
            _displayablesRepo = displayablesRepo;
            _currentRoomDetails = currentRoomDetails;
            _roomsTriggers = roomsTriggers;
        }

        public ITrigger FetchRoomTrigger
        {
            get
            {
                return _roomsTriggers.Fetch<RoomsTriggers.South>();
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
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).southExitBlocked;
            }
        }

        public bool GetExitAllowedMod
        {
            get
            {
                return _currentRoomDetails.southExitModifiers.exitAllowed;
            }
        }

        public bool GetFastExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.southExitModifiers.fastExitRequired;
            }
        }

        public bool GetSlowExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.southExitModifiers.slowExitRequired;
            }
        }

        public string GetExitNotFastEnoughDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).southExitNotFastEnough;
            }
        }

        public string GetExitNotSlowEnoughDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).southExitNotSlowEnough;
            }
        }

        public string GetNormalSpeedNotAllowedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).southExitNormalSpeedNotAllowed;
            }
        }

        public bool GetJumpExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.southExitModifiers.jumpExitRequired;
            }
        }

        public bool GetCrawlExitRequiredMod
        {
            get
            {
                return _currentRoomDetails.southExitModifiers.crawlExitRequired;
            }
        }

        public string GetExitMustJumpDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).southExitMustJump;
            }
        }

        public string GetExitMustCrawlDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).southExitMustCrawl;
            }
        }

        public string GetWalkingMethodNotAllowedDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).southExitWalkingMethodNotAllowed;
            }
        }

        public string GetExitOpenDisp
        {
            get
            {
                return _displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(_currentRoomDetails.id).southExitOpen;
            }
        }
    }
}
