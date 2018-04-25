using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using TextAdventure.Command.Determination.Result.Models;
using TextAdventure.Game.Game.Machines;
using TextAdventure.Game.Game.States;
using TextAdventure.Game.Game.Triggers;
using TextAdventure.Infrastructure.Extensions;
using JetBrains.Annotations;
using TextAdventure.Game.Command.Commands.MovementCommandHelpers;
using TextAdventure.Game.Displayables;
using TextAdventure.Game.Displayables.Models;
using TextAdventure.Infrastructure.Helpers;

namespace TextAdventure.Game.Command.Commands
{
    class MovementCommand : ICommand
    {
        private IResultModel _resultModel;
        private IList<string> _resultModelPropsChanged;

        private DisplayablesRepo _displayablesRepo;

        private StateMachine<IState, ITrigger> _roomMachine;

        private RoomsStates _roomsStates;
        private RoomsTriggers _roomsTriggers;

        private void ProcessMovementFromCardDir(ICardDir cardDir, MovementResultModel movementResultModel)
        {
            if (!(_roomMachine.CanFire(cardDir.FetchRoomTrigger)))
            {
                // No exit that way
                if (cardDir.GetDirExitBlockedDisp != null)
                {
                    Console.WriteLine(cardDir.GetDirExitBlockedDisp);
                }
                else
                {
                    Console.WriteLine(cardDir.GetUnavailableDirDisp);
                }
                return;
            }

            if (!cardDir.GetExitAllowedMod)
            {
                // exit that way, but is currently blocked off
                if (cardDir.GetDirExitBlockedDisp != null)
                {
                    Console.WriteLine(cardDir.GetDirExitBlockedDisp);
                }
                else
                {
                    Console.WriteLine(cardDir.GetUnavailableDirDisp);
                }
                return;
            }

            // Check if the movement speed is suitable to exit

            var speedIsSuitable = false;

            if (cardDir.GetFastExitRequiredMod && !cardDir.GetSlowExitRequiredMod)
            {
                if (movementResultModel.Speed.ToLower() == "fast")
                {
                    speedIsSuitable = true;
                }
                else
                {
                    // not fast enough
                    if (cardDir.GetExitNotFastEnoughDisp != null)
                    {
                        Console.WriteLine(cardDir.GetExitNotFastEnoughDisp, movementResultModel.BaseActionVerb);
                    }
                }
            }
            else if (!cardDir.GetFastExitRequiredMod && cardDir.GetSlowExitRequiredMod)
            {
                if (movementResultModel.Speed.ToLower() == "slow")
                {
                    speedIsSuitable = true;
                }
                else
                {
                    // not slow enough
                    if (cardDir.GetExitNotSlowEnoughDisp != null)
                    {
                        Console.WriteLine(cardDir.GetExitNotSlowEnoughDisp, movementResultModel.BaseActionVerb);
                    }
                }
            }
            else if (cardDir.GetFastExitRequiredMod && cardDir.GetSlowExitRequiredMod)
            {
                if (movementResultModel.Speed.ToLower() == "slow" || movementResultModel.Speed.ToLower() == "fast")
                {
                    speedIsSuitable = true;
                }
                else
                {
                    // not slow or fast enough
                    Console.WriteLine(cardDir.GetNormalSpeedNotAllowedDisp, movementResultModel.BaseActionVerb);
                }
            }
            else if (!cardDir.GetFastExitRequiredMod && !cardDir.GetSlowExitRequiredMod)
            {
                speedIsSuitable = true;
            }

            if (!speedIsSuitable)
            {
                return;
            }

            // check if the movement method is suitable to exit

            var methodIsSuitable = false;

            if (cardDir.GetJumpExitRequiredMod && !cardDir.GetCrawlExitRequiredMod)
            {
                if (movementResultModel.Method.ToLower() == "jump")
                {
                    methodIsSuitable = true;
                }
                else
                {
                    // must jump
                    if (cardDir.GetExitMustJumpDisp != null)
                    {
                        Console.WriteLine(cardDir.GetExitMustJumpDisp, movementResultModel.BaseActionVerb);
                    }
                }
            }
            else if (!cardDir.GetJumpExitRequiredMod && cardDir.GetCrawlExitRequiredMod)
            {
                if (movementResultModel.Method.ToLower() == "crawl")
                {
                    methodIsSuitable = true;
                }
                else
                {
                    // must crawl
                    if (cardDir.GetExitMustCrawlDisp != null)
                    {
                        Console.WriteLine(cardDir.GetExitMustCrawlDisp, movementResultModel.BaseActionVerb);
                    }
                }
            }
            else if (cardDir.GetJumpExitRequiredMod && cardDir.GetCrawlExitRequiredMod)
            {
                if (movementResultModel.Method.ToLower() == "jump" || movementResultModel.Method.ToLower() == "crawl")
                {
                    methodIsSuitable = true;
                }
                else
                {
                    // must either jump or crawl
                    if (cardDir.GetWalkingMethodNotAllowedDisp != null)
                    {
                        Console.WriteLine(cardDir.GetWalkingMethodNotAllowedDisp, movementResultModel.BaseActionVerb);
                    }
                }
            }
            else if (!cardDir.GetJumpExitRequiredMod && !cardDir.GetCrawlExitRequiredMod)
            {
                methodIsSuitable = true;
            }

            if (!methodIsSuitable)
            {
                return;
            }


            if (speedIsSuitable && methodIsSuitable)
            {
                if (cardDir.GetExitOpenDisp != null)
                {
                    Console.WriteLine(cardDir.GetExitOpenDisp, movementResultModel.BaseActionVerb);
                }
                _roomMachine.TryFire(cardDir.FetchRoomTrigger);
                return;
            }
        }
        
        public void Execute()
        {
            if (AnyNullReferences())
            {
                throw new NullReferenceException("ERROR: One or more required references haven't been injected into MovementCommand");
            }

            var movementResultModel = (MovementResultModel) _resultModel;

            var currentRoomDetails = _roomsStates.rooms.Single(r => r.id == _roomMachine.State.FetchIdentifier());

            ICardDir cardDir = null;

            var stopLoopFlag = false;
            var somethingWrittenToScreen = false;

            // If player specifies both a cardinal direction and a location (e.g. command: "go south to the hallway") and both the cardinal direction and location matches the same direction, process it as valid
            if (_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.CardinalDirection)) &&
                _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirection)) &&
                _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirectionRoomIds)) &&
                !(_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.UnknownLocationDirection))))
            {
                foreach (var locationDirectionRoomId in movementResultModel.LocationDirectionRoomIds)
                {
                    if (!stopLoopFlag)
                    {
                        if (movementResultModel.CardinalDirection.ToLower() == "south")
                        {
                            if (locationDirectionRoomId == currentRoomDetails.south)
                            {
                                cardDir = new SouthCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                                stopLoopFlag = true;
                            }
                        }
                        else if (movementResultModel.CardinalDirection.ToLower() == "north")
                        {
                            if (locationDirectionRoomId == currentRoomDetails.north)
                            {
                                cardDir = new NorthCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                                stopLoopFlag = true;
                            }
                        }
                        else if (movementResultModel.CardinalDirection.ToLower() == "east")
                        {
                            if (locationDirectionRoomId == currentRoomDetails.east)
                            {
                                cardDir = new EastCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                                stopLoopFlag = true;
                            }
                        }
                        else if (movementResultModel.CardinalDirection.ToLower() == "west")
                        {
                            if (locationDirectionRoomId == currentRoomDetails.west)
                            {
                                cardDir = new WestCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                                stopLoopFlag = true;
                            }
                        }
                        else if (movementResultModel.CardinalDirection.ToLower() == "up")
                        {
                            if (locationDirectionRoomId == currentRoomDetails.up)
                            {
                                cardDir = new UpCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                                stopLoopFlag = true;
                            }
                        }
                        else if (movementResultModel.CardinalDirection.ToLower() == "down")
                        {
                            if (locationDirectionRoomId == currentRoomDetails.down)
                            {
                                cardDir = new DownCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                                stopLoopFlag = true;
                            }
                        }
                    }
                }
                if (!stopLoopFlag)
                {
                    Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.MovementDefaults>().differentDirections);
                    somethingWrittenToScreen = true;
                }
            }

            // If player specifies just a a location (e.g. command: "go to the hallway"), but this location has no game room id associated with it (i.e. this "location" is within the same room as the player)
            // WARNING: This should never actually happen, as any "locations" not associated with a room should be instead put into the POI lex.
            else if (!_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.CardinalDirection)) &&
                _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirection)) &&
                !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirectionRoomIds)) &&
                !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.UnknownLocationDirection)))
            {
                Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.MovementDefaults>().locationWithNoRoomIdDefault, movementResultModel.LocationDirection);
                somethingWrittenToScreen = true;
            }

            // If player specifies just a a location with an associated room id (e.g. command: "go to the hallway"), then process this as valid.
            else if (!_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.CardinalDirection)) &&
                _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirection)) &&
                _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirectionRoomIds)) &&
                !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.UnknownLocationDirection)))
            {
                foreach (var locationDirectionRoomId in movementResultModel.LocationDirectionRoomIds)
                {
                    if (!stopLoopFlag)
                    {
                        if (currentRoomDetails.south == locationDirectionRoomId)
                        {
                            cardDir = new SouthCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                            stopLoopFlag = true;
                        }
                        else if (currentRoomDetails.north == locationDirectionRoomId)
                        {
                            cardDir = new NorthCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                            stopLoopFlag = true;
                        }
                        else if (currentRoomDetails.east == locationDirectionRoomId)
                        {
                            cardDir = new EastCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                            stopLoopFlag = true;
                        }
                        else if (currentRoomDetails.west == locationDirectionRoomId)
                        {
                            cardDir = new WestCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                            stopLoopFlag = true;
                        }
                        else if (currentRoomDetails.up == locationDirectionRoomId)
                        {
                            cardDir = new UpCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                            stopLoopFlag = true;
                        }
                        else if (currentRoomDetails.down == locationDirectionRoomId)
                        {
                            cardDir = new DownCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                            stopLoopFlag = true;
                        }
                        // The player is trying to move to the room that the player is already currently within
                        else if (currentRoomDetails.id == locationDirectionRoomId)
                        {
                            Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.MovementDefaults>().sameLocation);
                            somethingWrittenToScreen = true;
                            stopLoopFlag = true;
                        }
                    }
                    
                }
            }


            else if (_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.CardinalDirection)) &&
                !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirection)) &&
                !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirectionRoomIds)) &&
                !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.UnknownLocationDirection)))
            {
                switch (movementResultModel.CardinalDirection.ToLower())
                {
                    case "south":
                        cardDir = new SouthCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                        break;
                    case "north":
                        cardDir = new NorthCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                        break;
                    case "east":
                        cardDir = new EastCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                        break;
                    case "west":
                        cardDir = new WestCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                        break;
                    case "up":
                        cardDir = new UpCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                        break;
                    case "down":
                        cardDir = new DownCardDir(_displayablesRepo, currentRoomDetails, _roomsTriggers);
                        break;
                }
            }

            else if (!_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.CardinalDirection)) &&
                     !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirection)) &&
                     !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirectionRoomIds)) &&
                     _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.UnknownLocationDirection)))
            {
                Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.MovementDefaults>().unknownLocationDirection, movementResultModel.UnknownLocationDirection);
                somethingWrittenToScreen = true;
            }

            else if (_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.CardinalDirection)) &&
                     _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirection)) &&
                     _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirectionRoomIds)) &&
                     _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.UnknownLocationDirection)))
            {
                Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.MovementDefaults>().differentDirections);
                somethingWrittenToScreen = true;
            }

            else if (_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.CardinalDirection)) &&
                     !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirection)) &&
                     !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.LocationDirectionRoomIds)) &&
                     _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((MovementResultModel m) => m.UnknownLocationDirection)))
            {
                Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.MovementDefaults>().differentDirections);
                somethingWrittenToScreen = true;
            }


            if (cardDir != null)
            {
                ProcessMovementFromCardDir(cardDir, movementResultModel);
            }
            else if (!somethingWrittenToScreen)
            {
                Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.MovementDefaults>().misunderstoodDirection);
            }


        }

        public void InjectInResultModel(IResultModel resultModel, IList<string> resultModelPropsChanged)
        {
            _resultModel = resultModel;
            _resultModelPropsChanged = resultModelPropsChanged;
        }

        public void InjectInDisplayablesRepo(DisplayablesRepo displayablesRepo)
        {
            _displayablesRepo = displayablesRepo;
        }

        public void InjectInRoomMachine(StateMachine<IState, ITrigger> roomMachine)
        {
            _roomMachine = roomMachine;
        }

        public void InjectInRoomStates(RoomsStates roomsStates)
        {
            _roomsStates = roomsStates;
        }

        public void InjectInRoomTriggers(RoomsTriggers roomsTriggers)
        {
            _roomsTriggers = roomsTriggers;
        }

        private bool AnyNullReferences()
        {
            if (_resultModel == null || _resultModelPropsChanged == null || _displayablesRepo == null || _roomMachine == null || _roomsStates == null || _roomsTriggers == null)
            {
                return true;
            }

            return false;
        }

    }
}
