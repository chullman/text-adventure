using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ikvm.extensions;
using TextAdventure.Command.Determination.Mappers;
using TextAdventure.Command.Determination.Result.Models;
using TextAdventure.Command.Iterator;
using TextAdventure.Command.Lexicons;
using TextAdventure.Command.Lexicons.Models;
using TextAdventure.Infrastructure.Extensions;

namespace TextAdventure.Command.Determination.Intention
{
    public class MovementIntention : IBaseIntention
    {
        private CommandAggregatesRepo _commandAggregatesRepo;
        private MapperRepo _mapperRepo;
        private LexRepo _gameLexesRepo;

        private MovementLex.Action _movementLexAction = null;
        private AdverbLex.Action _adverbLexAction = null;
        private DirectionsLex.Action _directionsLexAction = null;
        private LocationsLex.Action _locationsLexAction = null;
        

        public MovementIntention(CommandAggregatesRepo commandAggregatesRepo, MapperRepo mapperRepo, LexRepo gameLexesRepo)
        {
            _commandAggregatesRepo = commandAggregatesRepo;
            _mapperRepo = mapperRepo;
            _gameLexesRepo = gameLexesRepo;
        }

        public void PopulateResultModel(IResultModel resultModel)
        {
            var movementResultModel = resultModel as MovementResultModel;

            FindInAdverbLex();
            FindInMovementLex();
            FindInDirectionsLex();
            FindInLocationsLex();
            

            // if an adverb is found, we'll get the name of it
            if (_adverbLexAction != null)
            {
                movementResultModel.Adverb = _adverbLexAction.name; 
            }
            

            // the base movement action verb should either appear at the beginning of the command, or immediately after an adverb, so let's check for this
            if (CheckAppearanceOfMovementVerbAtBeginning())
            {
                // if so, get the name and method of the movement verb
                movementResultModel.BaseActionVerb = _movementLexAction.name;
                movementResultModel.Method = _movementLexAction.method;

                if (_adverbLexAction != null)
                {
                    // The speed defined by an adverb, and the speed defined by a movement verb, can "cancel each other out"

                    // E.g. the phrase "quickly crawl" defaults to a "normal" speed
                    if (_adverbLexAction.speed == "fast" && _movementLexAction.speed == "slow")
                    {
                        movementResultModel.Speed = "normal";
                    }
                    // Similarly, the phrase "slowly run" defaults to a "normal" speed
                    else if (_adverbLexAction.speed == "slow" && _movementLexAction.speed == "fast")
                    {
                        movementResultModel.Speed = "normal";
                    }
                    else
                    {
                        movementResultModel.Speed = _adverbLexAction.speed;
                    }
                }

                // if a valid movement verb was found and no adverb was found, we'll get the speed from the movement verb
                else if (_adverbLexAction == null)
                {
                    movementResultModel.Speed = _movementLexAction.speed;
                }
            }


            if (_directionsLexAction != null)
            {
                if (CheckCardinalDirectionIsNmodToMovementVerb())
                {
                    movementResultModel.CardinalDirection = _directionsLexAction.name;
                }
            }

            if (CheckIfCardinalDirectionIsOnlyWord())
            {
                movementResultModel.CardinalDirection = _directionsLexAction.name;
            }

            if (_locationsLexAction != null)
            {
                movementResultModel.LocationDirection = _locationsLexAction.name;

                if (!_locationsLexAction.gameRoomIds.IsNullOrEmpty())
                {
                    movementResultModel.LocationDirectionRoomIds = _locationsLexAction.gameRoomIds;
                }
            }

            if (GetLocationFromEntityMentions() != null)
            {
                movementResultModel.UnknownLocationDirection = GetLocationFromEntityMentions();
            }

        }

        private string GetLocationFromEntityMentions()
        {
            if (!(_commandAggregatesRepo.AllEntityMentionsAggregate.Get().IsNullOrEmpty()))
            {
                if (_commandAggregatesRepo.AllEntityMentionsAggregate.GetIterator().FirstItem.ner == "LOCATION")
                {
                    return _commandAggregatesRepo.AllEntityMentionsAggregate.GetIterator().FirstItem.text;
                }
            }
            return null;
        }

        private bool CheckIfCardinalDirectionIsOnlyWord()
        {
            var nonDirectionWordFound = false;
            foreach (var key in _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Keys)
            {
                if (_commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get()[key].GetType() != typeof(DirectionsLex.Action))
                {
                    nonDirectionWordFound = true;
                }
            }

            if (_commandAggregatesRepo.TokensNotFoundInGameLexesAggregate.Get().IsNullOrEmpty() &&
                nonDirectionWordFound == false)
            {
                return true;
            }

            return false;
        }

        private bool CheckCardinalDirectionIsNmodToMovementVerb()
        {
            var wordInDirectionsLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(DirectionsLex.Action)).Select(p => p.Key).FirstOrDefault();

            if (wordInDirectionsLexFound != null)
            {
                var result = _commandAggregatesRepo.AllBasicDependenciesAggregate.Get().FirstOrDefault(bd =>
                {
                    if ((bd.dep == "nmod" || bd.dep == "dobj" || bd.dep == "advmod") && bd.dependentGloss == wordInDirectionsLexFound)
                    {
                        return true;
                    }
                    if ((bd.dep == "dep") && bd.governorGloss == wordInDirectionsLexFound)
                    {
                        return true;
                    }
                    return false;
                });

                if (result != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckAppearanceOfMovementVerbAtBeginning()
        {
            if (_commandAggregatesRepo.AllTokensAggregate.Count == 1)
            {
                if (CheckIfWordIsMovementVerb(0))
                {
                    return true;
                }
            }
            else if (_commandAggregatesRepo.AllTokensAggregate.Count == 2)
            {
                if (CheckIfFirstTwoWordsIsMovementVerb(0, 1))
                {
                    return true;
                }
                if (CheckIfWordIsMovementVerb(0))
                {
                    return true;
                }
                if (CheckIfWordIsAdverb(0))
                {
                    if (CheckIfWordIsMovementVerb(1))
                    {
                        return true;
                    }
                }
            }
            else if (_commandAggregatesRepo.AllTokensAggregate.Count == 3)
            {
                if (CheckIfFirstTwoWordsIsMovementVerb(0, 1))
                {
                    return true;
                }
                if (CheckIfWordIsMovementVerb(0))
                {
                    return true;
                }
                if (CheckIfTwoWordsIsAdverb(0, 1))
                {
                    if (CheckIfWordIsMovementVerb(2))
                    {
                        return true;
                    }
                }
                if (CheckIfWordIsAdverb(0))
                {
                    if (CheckIfFirstTwoWordsIsMovementVerb(1, 2))
                    {
                        return true;
                    }
                    if (CheckIfWordIsMovementVerb(1))
                    {
                        return true;
                    }
                }

                if (CheckIfWordIsAdverbMod(0))
                {
                    if (CheckIfWordIsAdverb(1))
                    {
                        if (CheckIfWordIsMovementVerb(2))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (_commandAggregatesRepo.AllTokensAggregate.Count == 4)
            {
                if (CheckIfFirstTwoWordsIsMovementVerb(0, 1))
                {
                    return true;
                }
                if (CheckIfWordIsMovementVerb(0))
                {
                    return true;
                }
                if (CheckIfTwoWordsIsAdverb(0, 1))
                {
                    if (CheckIfFirstTwoWordsIsMovementVerb(2, 3))
                    {
                        return true;
                    }
                    if (CheckIfWordIsMovementVerb(2))
                    {
                        return true;
                    }
                }
                if (CheckIfWordIsAdverb(0))
                {
                    if (CheckIfFirstTwoWordsIsMovementVerb(1, 2))
                    {
                        return true;
                    }
                    if (CheckIfWordIsMovementVerb(1))
                    {
                        return true;
                    }
                }
            }
            else if (_commandAggregatesRepo.AllTokensAggregate.Count == 5)
            {
                if (CheckIfFirstTwoWordsIsMovementVerb(0, 1))
                {
                    return true;
                }
                if (CheckIfWordIsMovementVerb(0))
                {
                    return true;
                }
                if (CheckIfTwoWordsIsAdverb(0, 1))
                {
                    if (CheckIfFirstTwoWordsIsMovementVerb(2, 3))
                    {
                        return true;
                    }
                    if (CheckIfWordIsMovementVerb(2))
                    {
                        return true;
                    }
                }
                if (CheckIfWordIsAdverb(0))
                {
                    if (CheckIfFirstTwoWordsIsMovementVerb(1, 2))
                    {
                        return true;
                    }
                    if (CheckIfWordIsMovementVerb(1))
                    {
                        return true;
                    }
                }
                if (CheckIfTwoWordsIsAdverbMod(0, 1))
                {
                    if (CheckIfTwoWordsIsAdverb(2, 3))
                    {
                        if (CheckIfWordIsMovementVerb(4))
                        {
                            return true;
                        }
                    }
                    if (CheckIfWordIsAdverb(2))
                    {
                        if (CheckIfFirstTwoWordsIsMovementVerb(3, 4))
                        {
                            return true;
                        }
                        if (CheckIfWordIsMovementVerb(3))
                        {
                            return true;
                        }
                    }
                }
                if (CheckIfWordIsAdverbMod(0))
                {
                    if (CheckIfTwoWordsIsAdverb(1, 2))
                    {
                        if (CheckIfFirstTwoWordsIsMovementVerb(3, 4))
                        {
                            return true;
                        }
                        if (CheckIfWordIsMovementVerb(3))
                        {
                            return true;
                        }
                    }
                    if (CheckIfWordIsAdverb(1))
                    {
                        if (CheckIfFirstTwoWordsIsMovementVerb(2, 3))
                        {
                            return true;
                        }
                        if (CheckIfWordIsMovementVerb(2))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (_commandAggregatesRepo.AllTokensAggregate.Count > 5)
            {
                if (CheckIfFirstTwoWordsIsMovementVerb(0, 1))
                {
                    return true;
                }
                if (CheckIfWordIsMovementVerb(0))
                {
                    return true;
                }
                if (CheckIfTwoWordsIsAdverb(0, 1))
                {
                    if (CheckIfFirstTwoWordsIsMovementVerb(2, 3))
                    {
                        return true;
                    }
                    if (CheckIfWordIsMovementVerb(2))
                    {
                        return true;
                    }
                }
                if (CheckIfWordIsAdverb(0))
                {
                    if (CheckIfFirstTwoWordsIsMovementVerb(1, 2))
                    {
                        return true;
                    }
                    if (CheckIfWordIsMovementVerb(1))
                    {
                        return true;
                    }
                }
                if (CheckIfTwoWordsIsAdverbMod(0, 1))
                {
                    if (CheckIfTwoWordsIsAdverb(2, 3))
                    {
                        if (CheckIfFirstTwoWordsIsMovementVerb(4, 5))
                        {
                            return true;
                        }
                        if (CheckIfWordIsMovementVerb(4))
                        {
                            return true;
                        }
                    }
                    if (CheckIfWordIsAdverb(2))
                    {
                        if (CheckIfFirstTwoWordsIsMovementVerb(3, 4))
                        {
                            return true;
                        }
                        if (CheckIfWordIsMovementVerb(3))
                        {
                            return true;
                        }
                    }
                }
                if (CheckIfWordIsAdverbMod(0))
                {
                    if (CheckIfTwoWordsIsAdverb(1, 2))
                    {
                        if (CheckIfFirstTwoWordsIsMovementVerb(3, 4))
                        {
                            return true;
                        }
                        if (CheckIfWordIsMovementVerb(3))
                        {
                            return true;
                        }
                    }
                    if (CheckIfWordIsAdverb(1))
                    {
                        if (CheckIfFirstTwoWordsIsMovementVerb(2, 3))
                        {
                            return true;
                        }
                        if (CheckIfWordIsMovementVerb(2))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckIfWordIsMovementVerb(int pos)
        {
            var word = _commandAggregatesRepo.AllTokensAggregate.Get()[pos].word;

            var movementLexAction = _gameLexesRepo.GetNestedRepo<MovementLex.Action>().FindFirstBy(a =>
            {

                if (a.name == word)
                {
                    return true;
                }
                if (!a.synonyms.IsNullOrEmpty())
                {
                    return a.synonyms.Contains(word);
                }
                if (!a.equivalenceClasses.IsNullOrEmpty())
                {
                    return a.equivalenceClasses.Contains(word);
                }

                return false;
            });

            if (movementLexAction != null)
            {
                return true;
            }

            return false;
        }

        private bool CheckIfFirstTwoWordsIsMovementVerb(int pos1, int pos2)
        {
            var twoWords = _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].word +
                                _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].after +
                                _commandAggregatesRepo.AllTokensAggregate.Get()[pos2].word;

            var movementLexAction = _gameLexesRepo.GetNestedRepo<MovementLex.Action>().FindFirstBy(a =>
            {

                if (a.name == twoWords)
                {
                    return true;
                }
                if (!a.synonyms.IsNullOrEmpty())
                {
                    return a.synonyms.Contains(twoWords);
                }
                if (!a.equivalenceClasses.IsNullOrEmpty())
                {
                    return a.equivalenceClasses.Contains(twoWords);
                }

                return false;
            });

            if (movementLexAction != null)
            {
                return true;
            }



            return false;
        }

        private bool CheckIfWordIsAdverb(int pos)
        {
            var word = _commandAggregatesRepo.AllTokensAggregate.Get()[pos].word;

            var adverbLexAction = _gameLexesRepo.GetNestedRepo<AdverbLex.Action>().FindFirstBy(a =>
            {

                if (a.name == word)
                {
                    return true;
                }
                if (!a.synonyms.IsNullOrEmpty())
                {
                    return a.synonyms.Contains(word);
                }
                if (!a.equivalenceClasses.IsNullOrEmpty())
                {
                    return a.equivalenceClasses.Contains(word);
                }

                return false;
            });

            if (adverbLexAction != null)
            {
                return true;
            }

            if (_commandAggregatesRepo.AllTokensAggregate.GetIterator().FirstItem.pos == "RB")
            {
                return true;
            }
            return false;
        }

        private bool CheckIfTwoWordsIsAdverb(int pos1, int pos2)
        {
            var twoWords = _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].word +
                                _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].after +
                                _commandAggregatesRepo.AllTokensAggregate.Get()[pos2].word;

            var adverbLexAction = _gameLexesRepo.GetNestedRepo<AdverbLex.Action>().FindFirstBy(a =>
            {

                if (a.name == twoWords)
                {
                    return true;
                }
                if (!a.synonyms.IsNullOrEmpty())
                {
                    return a.synonyms.Contains(twoWords);
                }
                if (!a.equivalenceClasses.IsNullOrEmpty())
                {
                    return a.equivalenceClasses.Contains(twoWords);
                }

                return false;
            });

            if (adverbLexAction != null)
            {
                return true;
            }

           

            return false;
        }


        private bool CheckIfWordIsAdverbMod(int pos)
        {
            var word = _commandAggregatesRepo.AllTokensAggregate.Get()[pos].word;

            var adverbModLexAction = _gameLexesRepo.GetNestedRepo<AdverbModLex.Action>().FindFirstBy(a =>
            {

                if (a.name == word)
                {
                    return true;
                }
                if (!a.synonyms.IsNullOrEmpty())
                {
                    return a.synonyms.Contains(word);
                }
                if (!a.equivalenceClasses.IsNullOrEmpty())
                {
                    return a.equivalenceClasses.Contains(word);
                }

                return false;
            });

            if (adverbModLexAction != null)
            {
                return true;
            }

            return false;
        }

        private bool CheckIfTwoWordsIsAdverbMod(int pos1, int pos2)
        {
            var twoWords = _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].word +
                                _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].after +
                                _commandAggregatesRepo.AllTokensAggregate.Get()[pos2].word;

            var adverbModLexAction = _gameLexesRepo.GetNestedRepo<AdverbModLex.Action>().FindFirstBy(a =>
            {

                if (a.name == twoWords)
                {
                    return true;
                }
                if (!a.synonyms.IsNullOrEmpty())
                {
                    return a.synonyms.Contains(twoWords);
                }
                if (!a.equivalenceClasses.IsNullOrEmpty())
                {
                    return a.equivalenceClasses.Contains(twoWords);
                }

                return false;
            });

            if (adverbModLexAction != null)
            {
                return true;
            }



            return false;
        }


        private void FindInLocationsLex()
        {
            var wordInLocationsLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(LocationsLex.Action)).Select(p => p.Key).FirstOrDefault();

            if (wordInLocationsLexFound != null)
            {
                _locationsLexAction = _gameLexesRepo.GetNestedRepo<LocationsLex.Action>().FindFirstBy(a =>
                {

                    if (a.name == wordInLocationsLexFound)
                    {
                        return true;
                    }
                    if (!a.synonyms.IsNullOrEmpty())
                    {
                        return a.synonyms.Contains(wordInLocationsLexFound);
                    }
                    if (!a.equivalenceClasses.IsNullOrEmpty())
                    {
                        return a.equivalenceClasses.Contains(wordInLocationsLexFound);
                    }

                    return false;
                });

            }
        }

        private void FindInDirectionsLex()
        {
            var wordInDirectionsLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(DirectionsLex.Action)).Select(p => p.Key).FirstOrDefault();

            if (wordInDirectionsLexFound != null)
            {
                _directionsLexAction = _gameLexesRepo.GetNestedRepo<DirectionsLex.Action>().FindFirstBy(a =>
                {

                    if (a.name == wordInDirectionsLexFound)
                    {
                        return true;
                    }
                    if (!a.synonyms.IsNullOrEmpty())
                    {
                        return a.synonyms.Contains(wordInDirectionsLexFound);
                    }
                    if (!a.equivalenceClasses.IsNullOrEmpty())
                    {
                        return a.equivalenceClasses.Contains(wordInDirectionsLexFound);
                    }

                    return false;
                });

            }
        }

        private void FindInMovementLex()
        {
            var wordInMovementLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(MovementLex.Action)).Select(p => p.Key).FirstOrDefault();

            if (wordInMovementLexFound != null)
            {
                _movementLexAction = _gameLexesRepo.GetNestedRepo<MovementLex.Action>().FindFirstBy(a =>
                {

                    if (a.name == wordInMovementLexFound)
                    {
                        return true;
                    }
                    if (!a.synonyms.IsNullOrEmpty())
                    {
                        return a.synonyms.Contains(wordInMovementLexFound);
                    }
                    if (!a.equivalenceClasses.IsNullOrEmpty())
                    {
                        return a.equivalenceClasses.Contains(wordInMovementLexFound);
                    }

                    return false;
                });

            }
        }

        private void FindInAdverbLex()
        {
            var wordInAdverbLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(AdverbLex.Action)).Select(p => p.Key).FirstOrDefault();

            if (wordInAdverbLexFound != null)
            {
                _adverbLexAction = _gameLexesRepo.GetNestedRepo<AdverbLex.Action>().FindFirstBy(a =>
                {

                    if (a.name == wordInAdverbLexFound)
                    {
                        return true;
                    }
                    if (!a.synonyms.IsNullOrEmpty())
                    {
                        return a.synonyms.Contains(wordInAdverbLexFound);
                    }
                    if (!a.equivalenceClasses.IsNullOrEmpty())
                    {
                        return a.equivalenceClasses.Contains(wordInAdverbLexFound);
                    }
                    
                    return false;
                });

            }

        }


    }
}
