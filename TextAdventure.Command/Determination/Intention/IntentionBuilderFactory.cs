using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Mappers;
using TextAdventure.Command.Iterator;
using TextAdventure.Command.Lexicons;
using TextAdventure.Infrastructure.Extensions;
using TextAdventure.Command.Lexicons.Models;
using TextAdventure.Command.Lexicons.Models.Infrastructure;

namespace TextAdventure.Command.Determination.Intention
{
    public class IntentionBuilderFactory
    {
        private CommandAggregatesRepo _commandAggregatesRepo;
        private MapperRepo _mapperRepo;
        private LexRepo _gameLexesRepo;

        private Dictionary<BaseIntentions, int> _intentionPoints;

        public IntentionBuilderFactory(CommandAggregatesRepo commandAggregatesRepo, MapperRepo mapperRepo, LexRepo gameLexesRepo)
        {
            _commandAggregatesRepo = commandAggregatesRepo;
            _mapperRepo = mapperRepo;
            _gameLexesRepo = gameLexesRepo;

            _intentionPoints = new Dictionary<BaseIntentions, int>();

            foreach (BaseIntentions enumValue in Enum.GetValues(typeof(BaseIntentions)))
            {
                _intentionPoints.Add(enumValue, 0);
            }
        }

        private dynamic GetGameLexForTwoWordEntity(int pos1, int pos2)
        {
            var twoWordEntity = _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].word +
                                _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].after +
                                _commandAggregatesRepo.AllTokensAggregate.Get()[pos2].word;


            foreach (var gameLex in _gameLexesRepo.GetAllNestedRepos())
            {
                var result = gameLex.FindFirstBy(a =>
                {

                    if (a.name == twoWordEntity)
                    {
                        return true;
                    }

                    if (!((IEnumerable<string>)a.synonyms).IsNullOrEmpty())
                    {
                        return a.synonyms.Contains(twoWordEntity);
                    }
                    if (!((IEnumerable<string>)a.equivalenceClasses).IsNullOrEmpty())
                    {
                        return a.equivalenceClasses.Contains(twoWordEntity);
                    }

                    return false;
                });

                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private dynamic GetGameLexForOneWordEntity(int pos1)
        {
            var oneWordEntity = _commandAggregatesRepo.AllTokensAggregate.Get()[pos1].word;

            foreach (var gameLex in _gameLexesRepo.GetAllNestedRepos())
            {
                var result = gameLex.FindFirstBy(a =>
                {

                    if (a.name == oneWordEntity)
                    {
                        return true;
                    }

                    if (!((IEnumerable<string>)a.synonyms).IsNullOrEmpty())
                    {
                        return a.synonyms.Contains(oneWordEntity);
                    }
                    if (!((IEnumerable<string>)a.equivalenceClasses).IsNullOrEmpty())
                    {
                        return a.equivalenceClasses.Contains(oneWordEntity);
                    }

                    return false;
                });

                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private BaseIntentions GetWinner()
        {
            var winningPoints = _intentionPoints.Values.Max();

            if (winningPoints != 0)
            {
                // In the hopefully unlikely event that there is more than one winner (i.e. more than one intention with the same top score), we'll just grab the first matching intention in the dictionary
                return _intentionPoints.First(kvp => kvp.Value == winningPoints).Key;
            }
            return BaseIntentions.EntirelyUnknown;
        }

        public BaseIntentions DetermineBaseIntention()
        {
            // TO DO

            IActionRoot gameLexForTwoWordEntity = null;
            IActionRoot gameLexForOneWordEntity = null;

            if (_commandAggregatesRepo.AllTokensAggregate.Count == 1)
            {
                gameLexForOneWordEntity = GetGameLexForOneWordEntity(0);
                if (gameLexForOneWordEntity != null)
                {
                    _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                }
            }

            if (GetWinner() != BaseIntentions.EntirelyUnknown)
            {
                return GetWinner();
            }


            if (_commandAggregatesRepo.AllTokensAggregate.Count > 1)
            {
                gameLexForTwoWordEntity = GetGameLexForTwoWordEntity(0, 1);

                if (gameLexForTwoWordEntity != null)
                {
                    if (gameLexForTwoWordEntity.OfLex == typeof(AdverbLex) || gameLexForTwoWordEntity.OfLex == typeof(AdjectivesLex))
                    {
                        if (_commandAggregatesRepo.AllTokensAggregate.Count > 3)
                        {
                            gameLexForTwoWordEntity = GetGameLexForTwoWordEntity(2,3);


                            if (gameLexForTwoWordEntity != null && gameLexForTwoWordEntity.OfLex != typeof(AdverbLex) && gameLexForTwoWordEntity.OfLex != typeof(AdjectivesLex))
                            {
                                _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                            }

                            if (gameLexForTwoWordEntity == null)
                            {
                                gameLexForOneWordEntity = GetGameLexForOneWordEntity(2);

                                if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex))
                                {
                                    _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                                }
                            }

                        }
                        else if (_commandAggregatesRepo.AllTokensAggregate.Count == 3)
                        {
                            gameLexForOneWordEntity = GetGameLexForOneWordEntity(2);

                            if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex))
                            {
                                _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                            }
                        }
                        else if (_commandAggregatesRepo.AllTokensAggregate.Count == 2)
                        {
                            _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                        }
                    }

                    else if (gameLexForTwoWordEntity.OfLex == typeof(AdverbModLex))
                    {
                        if (_commandAggregatesRepo.AllTokensAggregate.Count > 3)
                        {
                            gameLexForTwoWordEntity = GetGameLexForTwoWordEntity(2, 3);

                            if (gameLexForTwoWordEntity != null && (gameLexForTwoWordEntity.OfLex == typeof(AdverbLex) || gameLexForTwoWordEntity.OfLex == typeof(AdjectivesLex)))
                            {
                                if (_commandAggregatesRepo.AllTokensAggregate.Count > 5)
                                {
                                    gameLexForTwoWordEntity = GetGameLexForTwoWordEntity(4, 5);

                                    if (gameLexForTwoWordEntity != null && gameLexForTwoWordEntity.OfLex != typeof(AdverbLex) && gameLexForTwoWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForTwoWordEntity.OfLex != typeof(AdverbModLex))
                                    {
                                        _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                                    }

                                    if (gameLexForTwoWordEntity == null)
                                    {
                                        gameLexForOneWordEntity = GetGameLexForOneWordEntity(4);

                                        if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForOneWordEntity.OfLex != typeof(AdverbModLex))
                                        {
                                            _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                                        }
                                    }
                                }
                                else if (_commandAggregatesRepo.AllTokensAggregate.Count == 5)
                                {
                                    gameLexForOneWordEntity = GetGameLexForOneWordEntity(4);

                                    if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForOneWordEntity.OfLex != typeof(AdverbModLex))
                                    {
                                        _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                                    }
                                }
                                else if (_commandAggregatesRepo.AllTokensAggregate.Count == 4)
                                {
                                    _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                                }
                            }

                            if (gameLexForTwoWordEntity == null)
                            {
                                gameLexForOneWordEntity = GetGameLexForOneWordEntity(2);

                                if (gameLexForOneWordEntity != null && (gameLexForOneWordEntity.OfLex == typeof(AdverbLex) || gameLexForOneWordEntity.OfLex == typeof(AdjectivesLex)))
                                {
                                    if (_commandAggregatesRepo.AllTokensAggregate.Count > 4)
                                    {
                                        gameLexForTwoWordEntity = GetGameLexForTwoWordEntity(3, 4);

                                        if (gameLexForTwoWordEntity != null && gameLexForTwoWordEntity.OfLex != typeof(AdverbLex) && gameLexForTwoWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForTwoWordEntity.OfLex != typeof(AdverbModLex))
                                        {
                                            _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                                        }

                                        if (gameLexForTwoWordEntity == null)
                                        {
                                            gameLexForOneWordEntity = GetGameLexForOneWordEntity(3);

                                            if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForOneWordEntity.OfLex != typeof(AdverbModLex))
                                            {
                                                _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                                            }
                                        }
                                    }
                                    else if (_commandAggregatesRepo.AllTokensAggregate.Count == 4)
                                    {
                                        gameLexForOneWordEntity = GetGameLexForOneWordEntity(3);

                                        if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForOneWordEntity.OfLex != typeof(AdverbModLex))
                                        {
                                            _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                                        }
                                    }

                                }
                            }

                        }
                        else if (_commandAggregatesRepo.AllTokensAggregate.Count == 3)
                        {
                            gameLexForOneWordEntity = GetGameLexForOneWordEntity(2);

                            if (gameLexForOneWordEntity != null && (gameLexForOneWordEntity.OfLex == typeof(AdverbLex) || gameLexForOneWordEntity.OfLex == typeof(AdjectivesLex)))
                            {
                                _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                            }
                        }
                        else if (_commandAggregatesRepo.AllTokensAggregate.Count == 2)
                        {
                            _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                        }
                    }

                    else
                    {
                        _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                    }
                    
                }
            }

            if (GetWinner() != BaseIntentions.EntirelyUnknown)
            {
                return GetWinner();
            }


            if (_commandAggregatesRepo.AllTokensAggregate.Count > 1)
            {
                gameLexForOneWordEntity = GetGameLexForOneWordEntity(0);
                if (gameLexForOneWordEntity != null)
                {
                    if (gameLexForOneWordEntity.OfLex == typeof(AdverbLex) || gameLexForOneWordEntity.OfLex == typeof(AdjectivesLex))
                    {
                        if (_commandAggregatesRepo.AllTokensAggregate.Count > 2)
                        {
                            gameLexForTwoWordEntity = GetGameLexForTwoWordEntity(1, 2);

                            if (gameLexForTwoWordEntity != null && gameLexForTwoWordEntity.OfLex != typeof(AdverbLex) && gameLexForTwoWordEntity.OfLex != typeof(AdjectivesLex))
                            {
                                _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                            }

                            if (gameLexForTwoWordEntity == null)
                            {
                                gameLexForOneWordEntity = GetGameLexForOneWordEntity(1);

                                if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex))
                                {
                                    _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                                }
                            }
                        }
                        else if (_commandAggregatesRepo.AllTokensAggregate.Count == 2)
                        {
                            gameLexForOneWordEntity = GetGameLexForOneWordEntity(1);

                            if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex))
                            {
                                _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                            }
                        }   
                    }

                    else if (gameLexForOneWordEntity.OfLex == typeof(AdverbModLex))
                    {
                        if (_commandAggregatesRepo.AllTokensAggregate.Count > 2)
                        {
                            gameLexForTwoWordEntity = GetGameLexForTwoWordEntity(1, 2);

                            if (gameLexForTwoWordEntity != null && (gameLexForTwoWordEntity.OfLex == typeof(AdverbLex) || gameLexForTwoWordEntity.OfLex == typeof(AdjectivesLex)))
                            {
                                if (_commandAggregatesRepo.AllTokensAggregate.Count > 4)
                                {
                                    gameLexForTwoWordEntity = GetGameLexForTwoWordEntity(3, 4);

                                    if (gameLexForTwoWordEntity != null && gameLexForTwoWordEntity.OfLex != typeof(AdverbLex) && gameLexForTwoWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForTwoWordEntity.OfLex != typeof(AdverbModLex))
                                    {
                                        _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                                    }

                                    if (gameLexForTwoWordEntity == null)
                                    {
                                        gameLexForOneWordEntity = GetGameLexForOneWordEntity(3);

                                        if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForOneWordEntity.OfLex != typeof(AdverbModLex))
                                        {
                                            _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                                        }
                                    }
                                }
                                else if (_commandAggregatesRepo.AllTokensAggregate.Count == 4)
                                {
                                    gameLexForOneWordEntity = GetGameLexForOneWordEntity(3);

                                    if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForOneWordEntity.OfLex != typeof(AdverbModLex))
                                    {
                                        _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                                    }
                                }
                                else if (_commandAggregatesRepo.AllTokensAggregate.Count == 3)
                                {
                                    _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                                }
                            }

                            if (gameLexForTwoWordEntity == null)
                            {
                                gameLexForOneWordEntity = GetGameLexForOneWordEntity(1);

                                if (gameLexForOneWordEntity != null && (gameLexForOneWordEntity.OfLex == typeof(AdverbLex) || gameLexForOneWordEntity.OfLex == typeof(AdjectivesLex)))
                                {
                                    if (_commandAggregatesRepo.AllTokensAggregate.Count > 3)
                                    {
                                        gameLexForTwoWordEntity = GetGameLexForTwoWordEntity(2, 3);

                                        if (gameLexForTwoWordEntity != null && gameLexForTwoWordEntity.OfLex != typeof(AdverbLex) && gameLexForTwoWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForTwoWordEntity.OfLex != typeof(AdverbModLex))
                                        {
                                            _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForTwoWordEntity.OfLex]]++;
                                        }

                                        if (gameLexForTwoWordEntity == null)
                                        {
                                            gameLexForOneWordEntity = GetGameLexForOneWordEntity(2);

                                            if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForOneWordEntity.OfLex != typeof(AdverbModLex))
                                            {
                                                _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                                            }
                                        }
                                    }
                                    else if (_commandAggregatesRepo.AllTokensAggregate.Count == 3)
                                    {
                                        gameLexForOneWordEntity = GetGameLexForOneWordEntity(2);

                                        if (gameLexForOneWordEntity != null && gameLexForOneWordEntity.OfLex != typeof(AdverbLex) && gameLexForOneWordEntity.OfLex != typeof(AdjectivesLex) && gameLexForOneWordEntity.OfLex != typeof(AdverbModLex))
                                        {
                                            _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                                        }
                                    }

                                }
                            }

                        }
                        else if (_commandAggregatesRepo.AllTokensAggregate.Count == 2)
                        {
                            gameLexForOneWordEntity = GetGameLexForOneWordEntity(1);

                            if (gameLexForOneWordEntity != null && (gameLexForOneWordEntity.OfLex == typeof(AdverbLex) || gameLexForOneWordEntity.OfLex == typeof(AdjectivesLex)))
                            {
                                _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                            }
                        }

                    }

                    else
                    {
                        _intentionPoints[_mapperRepo.lexToIntentionMap[gameLexForOneWordEntity.OfLex]]++;
                    }
                }
            }

            if (GetWinner() != BaseIntentions.EntirelyUnknown)
            {
                return GetWinner();
            }

            if (_commandAggregatesRepo.AllTokensAggregate.Get().Count == 1)
            {
                if (_commandAggregatesRepo.AllTokensAggregate.GetIterator().FirstItem.ner != _mapperRepo.nerStringToIntentionMap.First(kvp => kvp.Value == BaseIntentions.NerUnknown).Key)
                {
                    _intentionPoints[_mapperRepo.nerStringToIntentionMap[_commandAggregatesRepo.AllTokensAggregate.GetIterator().FirstItem.ner]]++;
                }
                else if (_commandAggregatesRepo.AllTokensAggregate.GetIterator().FirstItem.ner == _mapperRepo.nerStringToIntentionMap.First(kvp => kvp.Value == BaseIntentions.NerUnknown).Key)
                {
                    if (_mapperRepo.posStringToIntentionMap.ContainsKey(_commandAggregatesRepo.AllTokensAggregate.GetIterator().FirstItem.pos))
                    {
                        _intentionPoints[_mapperRepo.posStringToIntentionMap[_commandAggregatesRepo.AllTokensAggregate.GetIterator().FirstItem.pos]]++;
                    }
                    else
                    {
                        _intentionPoints[BaseIntentions.EntirelyUnknown]++;
                    }
                }
            }

            if (_commandAggregatesRepo.AllTokensAggregate.Get().Count > 1)
            {
                if (!_commandAggregatesRepo.AllEntityMentionsAggregate.Get().IsNullOrEmpty())
                {
                    _intentionPoints[_mapperRepo.nerStringToIntentionMap[_commandAggregatesRepo.AllEntityMentionsAggregate.GetIterator().FirstItem.ner]]++;
                }
                else
                {
                    var rootWord = _commandAggregatesRepo.AllBasicDependenciesAggregate.Get().First(p => p.dep == "ROOT").dependentGloss;
                    var rootToken = _commandAggregatesRepo.AllTokensAggregate.Get().First(p => p.word == rootWord);
                    if (rootToken.ner != _mapperRepo.nerStringToIntentionMap.First(kvp => kvp.Value == BaseIntentions.NerUnknown).Key)
                    {
                        _intentionPoints[_mapperRepo.nerStringToIntentionMap[rootToken.ner]]++;
                    }
                    else if (rootToken.ner == _mapperRepo.nerStringToIntentionMap.First(kvp => kvp.Value == BaseIntentions.NerUnknown).Key)
                    {
                        if (_mapperRepo.posStringToIntentionMap.ContainsKey(rootToken.pos))
                        {
                            _intentionPoints[_mapperRepo.posStringToIntentionMap[rootToken.pos]]++;
                        }
                        else
                        {
                            _intentionPoints[BaseIntentions.EntirelyUnknown]++;
                        }
                    }
                }
            }

            return GetWinner();
        }

        public IBaseIntention Build(BaseIntentions baseIntentionEnum)
        {
            return (IBaseIntention)Activator.CreateInstance(_mapperRepo.intentToIntentBuilderMap[baseIntentionEnum], _commandAggregatesRepo, _mapperRepo, _gameLexesRepo);
        }

    }
}
