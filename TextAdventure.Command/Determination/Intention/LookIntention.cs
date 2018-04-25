using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.omg.CORBA;
using TextAdventure.Command.Determination.Mappers;
using TextAdventure.Command.Determination.Result.Models;
using TextAdventure.Command.Iterator;
using TextAdventure.Command.Lexicons;
using TextAdventure.Command.Lexicons.Models;
using TextAdventure.Infrastructure.Extensions;

namespace TextAdventure.Command.Determination.Intention
{
    public class LookIntention : IBaseIntention
    {

        private CommandAggregatesRepo _commandAggregatesRepo;
        private MapperRepo _mapperRepo;
        private LexRepo _gameLexesRepo;

        private LookLex.Action _lookLexAction = null;
        private AdverbLex.Action _adverbLexAction = null;
        private DirectionsLex.Action _directionsLexAction = null;
        private LocationsLex.Action _locationsLexAction = null;
        private PoiLex.Action _poiLexAction = null;
        private MiscObjectLex.Action _miscObjectLexAction = null;

        public LookIntention(CommandAggregatesRepo commandAggregatesRepo, MapperRepo mapperRepo, LexRepo gameLexesRepo)
        {
            _commandAggregatesRepo = commandAggregatesRepo;
            _mapperRepo = mapperRepo;
            _gameLexesRepo = gameLexesRepo;
        }

        public void PopulateResultModel(Result.Models.IResultModel resultModel)
        {
            var lookResultModel = resultModel as LookResultModel;

            FindInAdverbLex();
            FindInLookLex();
            FindInDirectionsLex();
            FindInLocationsLex();
            FindInPoiLex();
            FindInMiscObjectLex();

            if (_adverbLexAction != null)
            {
                lookResultModel.Adverb = _adverbLexAction.name;
            }

            if (CheckAppearanceOfLookVerbAtBeginning())
            {
                lookResultModel.BaseActionVerb = _lookLexAction.name;

                if (_adverbLexAction != null)
                {
                    if (_lookLexAction.method == "careful" && _adverbLexAction.speed == "fast")
                    {
                        lookResultModel.Method = "normal";
                    }
                    else if (_lookLexAction.method == "normal" && _adverbLexAction.speed == "slow")
                    {
                        lookResultModel.Method = "careful";
                    }
                    else
                    {
                        lookResultModel.Method = _lookLexAction.method;
                    }
                }
                else if (_adverbLexAction == null)
                {
                    lookResultModel.Method = _lookLexAction.method;
                }

                if (_directionsLexAction != null)
                {
                    if (CheckCardinalDirectionIsNmodToLookVerb())
                    {
                        lookResultModel.CardinalDirection = _directionsLexAction.name;
                    }
                }

                if (_locationsLexAction != null)
                {
                    lookResultModel.LocationDirection = _locationsLexAction.name;

                    if (!_locationsLexAction.gameRoomIds.IsNullOrEmpty())
                    {
                        lookResultModel.LocationDirectionRoomIds = _locationsLexAction.gameRoomIds;
                    }
                }


                if (_poiLexAction != null)
                {
                    if (CheckPoiIsDepToLookAction())
                    {
                        lookResultModel.PoiName = _poiLexAction.name;
                        lookResultModel.PoiPlural = _poiLexAction.poiPlural;

                        if (_poiLexAction.locId != null)
                        {
                            lookResultModel.PoiLocId = _poiLexAction.locId;
                        }

                        if (_poiLexAction.itemId != null)
                        {
                            lookResultModel.PoiItemId = _poiLexAction.itemId;
                        }

                        if (!_poiLexAction.adjectives.IsNullOrEmpty())
                        {
                            lookResultModel.PoiAdjectives = _poiLexAction.adjectives;

                        }
                    }

                } 
                else if (_poiLexAction == null)
                {
                    var unknownPoi = GetUnknownPoiIfExists();
                    if (unknownPoi != null && lookResultModel.CardinalDirection != unknownPoi && lookResultModel.LocationDirection != unknownPoi)
                    {
                        lookResultModel.UnknownPoi = unknownPoi;

                        lookResultModel.PoiPlural = GetPluralisationOfUnknownPoi(unknownPoi);
                    }
                }

                if (_miscObjectLexAction != null)
                {
                    lookResultModel.MiscObjectName = _miscObjectLexAction.name;

                    if (_miscObjectLexAction.locId != null)
                    {
                        lookResultModel.MiscObjectLocId = _miscObjectLexAction.locId;
                    }

                    if (_miscObjectLexAction.itemId != null)
                    {
                        lookResultModel.MiscObjectItemId = _miscObjectLexAction.itemId;
                    }
                }

                if (_poiLexAction != null && _miscObjectLexAction == null)
                {
                    lookResultModel.UnknownMiscObject = CheckIfUnknownMiscObjectIsNmodToPoi();
                }

                if (GetLocationFromEntityMentions() != null)
                {
                    lookResultModel.UnknownLocationDirection = GetLocationFromEntityMentions();
                }


            }
        }

        private string CheckIfUnknownMiscObjectIsNmodToPoi()
        {
            var miscObject = _commandAggregatesRepo.AllBasicDependenciesAggregate.Get().FirstOrDefault(bd =>
            {
                if (bd.dep == "nmod")
                {
                    if (bd.governorGloss == _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(PoiLex.Action)).Select(p => p.Key).FirstOrDefault())
                    {
                        return true;
                    }
                }
                return false;
            });

            if (miscObject != null)
            {
                return miscObject.dependentGloss;
            }

            return null;
        }

        private bool GetPluralisationOfUnknownPoi(string unknownPoi)
        {
            var token = _commandAggregatesRepo.AllTokensAggregate.Get().FirstOrDefault(t => t.word == unknownPoi);

            if (token != null && token.pos == "NNS")
            {
                return true;
            }
            return false;

        }

        private string GetUnknownPoiIfExists()
        {
            var basicDep = _commandAggregatesRepo.AllBasicDependenciesAggregate.Get().FirstOrDefault(bd =>
            {
                if (bd.dep == "dobj" || bd.dep == "nmod")
                {
                    return true;
                }
                return false;
            });

            if (basicDep != null)
            {
                return basicDep.dependentGloss;
            }

            return null;
        }

        private bool CheckCardinalDirectionIsNmodToLookVerb()
        {
            var wordInDirectionsLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(DirectionsLex.Action)).Select(p => p.Key).FirstOrDefault();

            if (wordInDirectionsLexFound != null)
            {
                var result = _commandAggregatesRepo.AllBasicDependenciesAggregate.Get().FirstOrDefault(bd =>
                {
                    if ((bd.dep == "nmod" || bd.dep == "advmod") && bd.dependentGloss == wordInDirectionsLexFound)
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

        private bool CheckPoiIsDepToLookAction()
        {
            var wordInPoiLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(PoiLex.Action)).Select(p => p.Key).FirstOrDefault();
            var wordInLookLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(LookLex.Action)).Select(p => p.Key).FirstOrDefault();

            if (wordInPoiLexFound != null)
            {
                var result = _commandAggregatesRepo.AllBasicDependenciesAggregate.Get().FirstOrDefault(bd =>
                {
                    if ((bd.dep == "nmod" || bd.dep == "dobj") && (bd.dependentGloss == wordInPoiLexFound) && (bd.governorGloss == wordInLookLexFound))
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

        private bool CheckAppearanceOfLookVerbAtBeginning()
        {
            if (_commandAggregatesRepo.AllTokensAggregate.Count == 1)
            {
                if (CheckIfWordIsLookVerb(0))
                {
                    return true;
                }
            }
            else if (_commandAggregatesRepo.AllTokensAggregate.Count == 2)
            {
                if (CheckIfTwoWordsIsLookVerb(0, 1))
                {
                    return true;
                }
                if (CheckIfWordIsLookVerb(0))
                {
                    return true;
                }
                if (CheckIfWordIsAdverb(0))
                {
                    if (CheckIfWordIsLookVerb(1))
                    {
                        return true;
                    }
                }
            }
            else if (_commandAggregatesRepo.AllTokensAggregate.Count == 3)
            {
                if (CheckIfTwoWordsIsLookVerb(0, 1))
                {
                    return true;
                }
                if (CheckIfWordIsLookVerb(0))
                {
                    return true;
                }
                if (CheckIfTwoWordsIsAdverb(0, 1))
                {
                    if (CheckIfWordIsLookVerb(2))
                    {
                        return true;
                    }
                }
                if (CheckIfWordIsAdverb(0))
                {
                    if (CheckIfTwoWordsIsLookVerb(1, 2))
                    {
                        return true;
                    }
                    if (CheckIfWordIsLookVerb(1))
                    {
                        return true;
                    }
                }

                if (CheckIfWordIsAdverbMod(0))
                {
                    if (CheckIfWordIsAdverb(1))
                    {
                        if (CheckIfWordIsLookVerb(2))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (_commandAggregatesRepo.AllTokensAggregate.Count == 4)
            {
                if (CheckIfTwoWordsIsLookVerb(0, 1))
                {
                    return true;
                }
                if (CheckIfWordIsLookVerb(0))
                {
                    return true;
                }
                if (CheckIfTwoWordsIsAdverb(0, 1))
                {
                    if (CheckIfTwoWordsIsLookVerb(2, 3))
                    {
                        return true;
                    }
                    if (CheckIfWordIsLookVerb(2))
                    {
                        return true;
                    }
                }
                if (CheckIfWordIsAdverb(0))
                {
                    if (CheckIfTwoWordsIsLookVerb(1, 2))
                    {
                        return true;
                    }
                    if (CheckIfWordIsLookVerb(1))
                    {
                        return true;
                    }
                }
            }
            else if (_commandAggregatesRepo.AllTokensAggregate.Count == 5)
            {
                if (CheckIfTwoWordsIsLookVerb(0, 1))
                {
                    return true;
                }
                if (CheckIfWordIsLookVerb(0))
                {
                    return true;
                }
                if (CheckIfTwoWordsIsAdverb(0, 1))
                {
                    if (CheckIfTwoWordsIsLookVerb(2, 3))
                    {
                        return true;
                    }
                    if (CheckIfWordIsLookVerb(2))
                    {
                        return true;
                    }
                }
                if (CheckIfWordIsAdverb(0))
                {
                    if (CheckIfTwoWordsIsLookVerb(1, 2))
                    {
                        return true;
                    }
                    if (CheckIfWordIsLookVerb(1))
                    {
                        return true;
                    }
                }
                if (CheckIfTwoWordsIsAdverbMod(0, 1))
                {
                    if (CheckIfTwoWordsIsAdverb(2, 3))
                    {
                        if (CheckIfWordIsLookVerb(4))
                        {
                            return true;
                        }
                    }
                    if (CheckIfWordIsAdverb(2))
                    {
                        if (CheckIfTwoWordsIsLookVerb(3, 4))
                        {
                            return true;
                        }
                        if (CheckIfWordIsLookVerb(3))
                        {
                            return true;
                        }
                    }
                }
                if (CheckIfWordIsAdverbMod(0))
                {
                    if (CheckIfTwoWordsIsAdverb(1, 2))
                    {
                        if (CheckIfTwoWordsIsLookVerb(3, 4))
                        {
                            return true;
                        }
                        if (CheckIfWordIsLookVerb(3))
                        {
                            return true;
                        }
                    }
                    if (CheckIfWordIsAdverb(1))
                    {
                        if (CheckIfTwoWordsIsLookVerb(2, 3))
                        {
                            return true;
                        }
                        if (CheckIfWordIsLookVerb(2))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (_commandAggregatesRepo.AllTokensAggregate.Count > 5)
            {
                if (CheckIfTwoWordsIsLookVerb(0, 1))
                {
                    return true;
                }
                if (CheckIfWordIsLookVerb(0))
                {
                    return true;
                }
                if (CheckIfTwoWordsIsAdverb(0, 1))
                {
                    if (CheckIfTwoWordsIsLookVerb(2, 3))
                    {
                        return true;
                    }
                    if (CheckIfWordIsLookVerb(2))
                    {
                        return true;
                    }
                }
                if (CheckIfWordIsAdverb(0))
                {
                    if (CheckIfTwoWordsIsLookVerb(1, 2))
                    {
                        return true;
                    }
                    if (CheckIfWordIsLookVerb(1))
                    {
                        return true;
                    }
                }
                if (CheckIfTwoWordsIsAdverbMod(0, 1))
                {
                    if (CheckIfTwoWordsIsAdverb(2, 3))
                    {
                        if (CheckIfTwoWordsIsLookVerb(4, 5))
                        {
                            return true;
                        }
                        if (CheckIfWordIsLookVerb(4))
                        {
                            return true;
                        }
                    }
                    if (CheckIfWordIsAdverb(2))
                    {
                        if (CheckIfTwoWordsIsLookVerb(3, 4))
                        {
                            return true;
                        }
                        if (CheckIfWordIsLookVerb(3))
                        {
                            return true;
                        }
                    }
                }
                if (CheckIfWordIsAdverbMod(0))
                {
                    if (CheckIfTwoWordsIsAdverb(1, 2))
                    {
                        if (CheckIfTwoWordsIsLookVerb(3, 4))
                        {
                            return true;
                        }
                        if (CheckIfWordIsLookVerb(3))
                        {
                            return true;
                        }
                    }
                    if (CheckIfWordIsAdverb(1))
                    {
                        if (CheckIfTwoWordsIsLookVerb(2, 3))
                        {
                            return true;
                        }
                        if (CheckIfWordIsLookVerb(2))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }



        private bool CheckIfWordIsLookVerb(int pos)
        {
            var word = _commandAggregatesRepo.AllTokensAggregate.Get()[pos].word;

            var lookLexAction = _gameLexesRepo.GetNestedRepo<LookLex.Action>().FindFirstBy(a =>
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

            if (lookLexAction != null)
            {
                return true;
            }

            return false;
        }

        private bool CheckIfTwoWordsIsLookVerb(int pos1, int pos2)
        {
            var twoWords = _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].word +
                           _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].after +
                           _commandAggregatesRepo.AllTokensAggregate.Get()[pos2].word;

            var lookLexAction = _gameLexesRepo.GetNestedRepo<LookLex.Action>().FindFirstBy(a =>
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

            if (lookLexAction != null)
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

        private void FindInLookLex()
        {
            var wordInLookLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(LookLex.Action)).Select(p => p.Key).FirstOrDefault();

            if (wordInLookLexFound != null)
            {
                _lookLexAction = _gameLexesRepo.GetNestedRepo<LookLex.Action>().FindFirstBy(a =>
                {

                    if (a.name == wordInLookLexFound)
                    {
                        return true;
                    }
                    if (!a.synonyms.IsNullOrEmpty())
                    {
                        return a.synonyms.Contains(wordInLookLexFound);
                    }
                    if (!a.equivalenceClasses.IsNullOrEmpty())
                    {
                        return a.equivalenceClasses.Contains(wordInLookLexFound);
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

        private void FindInPoiLex()
        {
            var wordInPoiLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(PoiLex.Action)).Select(p => p.Key).FirstOrDefault();

            if (wordInPoiLexFound != null)
            {
                _poiLexAction = _gameLexesRepo.GetNestedRepo<PoiLex.Action>().FindFirstBy(a =>
                {

                    if (a.name == wordInPoiLexFound)
                    {
                        return true;
                    }
                    if (!a.synonyms.IsNullOrEmpty())
                    {
                        return a.synonyms.Contains(wordInPoiLexFound);
                    }
                    if (!a.equivalenceClasses.IsNullOrEmpty())
                    {
                        return a.equivalenceClasses.Contains(wordInPoiLexFound);
                    }

                    return false;
                });

            }
        }

        private void FindInMiscObjectLex()
        {
            var wordInMiscObjectLexFound = _commandAggregatesRepo.WordsFoundInGameLexesAggregate.Get().Where(p => p.Value.GetType() == typeof(MiscObjectLex.Action)).Select(p => p.Key).FirstOrDefault();

            if (wordInMiscObjectLexFound != null)
            {
                _miscObjectLexAction = _gameLexesRepo.GetNestedRepo<MiscObjectLex.Action>().FindFirstBy(a =>
                {

                    if (a.name == wordInMiscObjectLexFound)
                    {
                        return true;
                    }
                    if (!a.synonyms.IsNullOrEmpty())
                    {
                        return a.synonyms.Contains(wordInMiscObjectLexFound);
                    }
                    if (!a.equivalenceClasses.IsNullOrEmpty())
                    {
                        return a.equivalenceClasses.Contains(wordInMiscObjectLexFound);
                    }

                    return false;
                });

            }
        }
    }
}
