using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using TextAdventure.Command.Determination.Result.Models;
using TextAdventure.Game.Displayables;
using TextAdventure.Game.Displayables.Models;
using TextAdventure.Game.Game.States;
using TextAdventure.Game.Game.Triggers;
using TextAdventure.Infrastructure.Helpers;

namespace TextAdventure.Game.Command.Commands
{
    public class LookCommand : ICommand
    {
        private IResultModel _resultModel;
        private IList<string> _resultModelPropsChanged;

        private DisplayablesRepo _displayablesRepo;

        private StateMachine<IState, ITrigger> _roomMachine;

        private RoomsStates _roomsStates;

        public void Execute()
        {
            if (AnyNullReferences())
            {
                throw new NullReferenceException("ERROR: One or more required references haven't been injected into LookCommand");
            }

            var lookResultModel = (LookResultModel)_resultModel;

            var currentRoomDetails = _roomsStates.rooms.Single(r => r.id == _roomMachine.State.FetchIdentifier());


            // e.g. look south
            if (_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.CardinalDirection)) &&
                !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.LocationDirection)) &&
                !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.LocationDirectionRoomIds)) &&
                !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.UnknownLocationDirection)))
            {
                var cardLookDisplayed = false;
                switch (lookResultModel.CardinalDirection.ToLower())
                {
                    case "south":
                        if (_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookSouth != null)
                        {
                            Console.WriteLine(_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookSouth);
                            cardLookDisplayed = true;
                        }
                        break;
                    case "north":
                        if (_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookNorth != null)
                        {
                            Console.WriteLine(_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookNorth);
                            cardLookDisplayed = true;
                        }
                        break;
                    case "east":
                        if (_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookEast != null)
                        {
                            Console.WriteLine(_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookEast);
                            cardLookDisplayed = true;
                        }
                        break;
                    case "west":
                        if (_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookWest != null)
                        {
                            Console.WriteLine(_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookWest);
                            cardLookDisplayed = true;
                        }
                        break;
                    case "up":
                        if (_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookUp != null)
                        {
                            Console.WriteLine(_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookUp);
                            cardLookDisplayed = true;
                        }
                        break;
                    case "down":
                        if (_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookDown != null)
                        {
                            Console.WriteLine(_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookDown);
                            cardLookDisplayed = true;
                        }
                        break;
                }

                if (!cardLookDisplayed)
                {
                    Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.LookDefaults>().noLookDir);
                }
            }

            // If player looks in the room they're currently in, e.g. "look at hallway"
            else if (_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.LocationDirectionRoomIds)) &&
                     !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.CardinalDirection)) &&
                     !_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.UnknownLocationDirection)))
            {
                var isLookingInCurrentRoom = false;
                foreach (var locationDirectionRoomId in lookResultModel.LocationDirectionRoomIds)
                {
                    if (locationDirectionRoomId == currentRoomDetails.id)
                    {
                        Console.WriteLine(_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookAction);
                        isLookingInCurrentRoom = true;
                    }
                }

                if (!isLookingInCurrentRoom)
                {
                    Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.LookDefaults>().roomInDifferentLoc);
                }
            }

            // A single word was entered, e.g. "look" or with an adverb "quickly look"
            else if (_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.BaseActionVerb)) &&
                (_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.Method)) ||
                _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.Adverb))) &&
                (_resultModelPropsChanged.Count == 2 || _resultModelPropsChanged.Count == 3))
            {
                Console.WriteLine(_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookAction);
            }

            else if (_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.PoiItemId)) &&
                     _resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.MiscObjectLocId)))
            {
                var itemIsInRoom = false;
                var indexCounter = 0;
                var indexOfItem = 0;
                foreach (var item in currentRoomDetails.items)
                {
                    if (item.itemId == lookResultModel.PoiItemId)
                    {
                        itemIsInRoom = true;
                        indexOfItem = indexCounter;
                    }
                    indexCounter += 1;
                }

                if (itemIsInRoom && currentRoomDetails.items[indexOfItem].miscObjId == lookResultModel.MiscObjectLocId)
                {
                    Console.WriteLine(_displayablesRepo.GetADisplayables<ItemsDisplayables>().GetDisplayableFromList<ItemsDisplayables.ItemsDis>(lookResultModel.PoiItemId).lookAction);
                }
                else
                {
                    if (lookResultModel.PoiPlural)
                    {
                        Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.LookDefaults>().itemNotInRoomPlural, lookResultModel.PoiName);
                    }
                    else
                    {
                        Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.LookDefaults>().itemNotInRoomNotPlural, lookResultModel.PoiName);
                    }
                }
            }

            else if (_resultModelPropsChanged.Contains(ReflectionHelper.GetPropertyName((LookResultModel m) => m.PoiItemId)))
            {
                var itemIsInRoom = false;
                foreach (var item in currentRoomDetails.items)
                {
                    if (item.itemId == lookResultModel.PoiItemId)
                    {
                        itemIsInRoom = true;
                    }
                }

                if (itemIsInRoom)
                {
                    Console.WriteLine(_displayablesRepo.GetADisplayables<ItemsDisplayables>().GetDisplayableFromList<ItemsDisplayables.ItemsDis>(lookResultModel.PoiItemId).lookAction);
                }
                else
                {
                    if (lookResultModel.PoiPlural)
                    {
                        Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.LookDefaults>().itemNotInRoomPlural, lookResultModel.PoiName);
                    }
                    else
                    {
                        Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.LookDefaults>().itemNotInRoomNotPlural, lookResultModel.PoiName);
                    }
                }
            }

            else if (lookResultModel.UnknownPoi == "room")
            {
                if (currentRoomDetails.isInside)
                {
                    Console.WriteLine(_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookAction);
                }
                else
                {
                    Console.WriteLine(_displayablesRepo.GetADisplayables<DefaultsDisplayables>().GetDisplayable<DefaultsDisplayables.LookDefaults>().notInRoom);
                }
            }

            else if (lookResultModel.UnknownPoi == "area")
            {
                Console.WriteLine(_displayablesRepo.GetADisplayables<RoomsDisplayables>().GetDisplayableFromList<RoomsDisplayables.RoomsDis>(currentRoomDetails.id).lookAction);
            }
        }

        public void InjectInResultModel(TextAdventure.Command.Determination.Result.Models.IResultModel resultModel, IList<string> resultModelPropsChanged)
        {
            _resultModel = resultModel;
            _resultModelPropsChanged = resultModelPropsChanged;
        }

        public void InjectInDisplayablesRepo(Displayables.DisplayablesRepo displayablesRepo)
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

        private bool AnyNullReferences()
        {
            if (_resultModel == null || _resultModelPropsChanged == null || _displayablesRepo == null || _roomMachine == null || _roomsStates == null)
            {
                return true;
            }

            return false;
        }
    }
}
