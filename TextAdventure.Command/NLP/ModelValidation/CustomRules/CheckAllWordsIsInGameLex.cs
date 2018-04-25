using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;
using Microsoft.CSharp.RuntimeBinder;
using TextAdventure.Command.Lexicons;
using TextAdventure.Command.NLP.Models;

namespace TextAdventure.Command.NLP.ModelValidation.CustomRules
{
    public class CheckAllWordsIsInGameLex : PropertyValidator
    {
        private LexRepo _lexRepo;

        private IDictionary<string, dynamic> _wordsFoundInGameLexes = new Dictionary<string, dynamic>();

        private IList<Token> _tokensNotFoundInGameLexes = new List<Token>();

        private IList<int> _wordsFoundAt = new List<int>();

        public CheckAllWordsIsInGameLex(LexRepo lexRepo)
            : base("Property {PropertyName} contains a word not known to the game!")
        {
            _lexRepo = lexRepo;
        }

        public IDictionary<string, dynamic> GetWordsFoundInGameLexes()
        {
            return _wordsFoundInGameLexes;
        }

        public IList<Token> GetTokensNotFoundInGameLexes()
        {
            return _tokensNotFoundInGameLexes;
        }


        private bool DoMatchCheck(dynamic a, string stringToCheck)
        {


            if (a.name.Equals(stringToCheck))
            {
                return true;
            }

            var isInSynonyms = false;
            var isInEquivalenceClasses = false;
            var isInAdjectives = false;

            if (a.synonyms != null && ((IList<string>)a.synonyms).Contains(stringToCheck))
            {
                isInSynonyms = true;
            }

            if (a.equivalenceClasses != null && ((IList<string>)a.equivalenceClasses).Contains(stringToCheck))
            {
                isInEquivalenceClasses = true;
            }

            try
            {
                if (a.adjectives != null && ((IList<string>)a.adjectives).Contains(stringToCheck))
                {
                    isInAdjectives = true;
                }
            }
            catch (RuntimeBinderException)
            {
                // Intetionally empty to supress errors where property "adjectives" doesn't exist for those particular lexes.
                // Needed because this func is taking a "dynamic" input.
            }


            if (isInSynonyms ||
                isInEquivalenceClasses ||
                isInAdjectives)
            {
                return true;
            }

            return false;

        }


        protected override bool IsValid(PropertyValidatorContext context)
        {
            var tokens = context.PropertyValue as IList<Token>;

            dynamic result1 = null;
            dynamic result2 = null;

            int i = 0;

            while (i < tokens.Count)
            {
                foreach (var gameLex in _lexRepo.GetAllNestedRepos())
                {
                    
                    if (i < tokens.Count - 1)
                    {
                        var twoTokenWordToCheck = tokens[i].word + tokens[i].after + tokens[i + 1].word;

                        result1 = gameLex.FindFirstBy(a =>

                            DoMatchCheck(a, twoTokenWordToCheck)

                        );

                        if (result1 != null)
                        {
                            

                            _wordsFoundInGameLexes[twoTokenWordToCheck] = result1;


                            _wordsFoundAt.Add(i);
                            _wordsFoundAt.Add(i+1);

                            break;

                        }
                    }
                    
                    var wordToCheck = tokens[i].word;

                    result2 = gameLex.FindFirstBy(a =>

                        DoMatchCheck(a, wordToCheck)

                    );

                    if (result2 != null)
                    {


                        _wordsFoundInGameLexes[wordToCheck] = result2;


                        _wordsFoundAt.Add(i);

                        break;
                    }
                    


                }

                if (result1 != null)
                {
                    i += 2;
                }
                if (result2 != null)
                {
                    i++;
                }

                if (result1 == null && result2 == null)
                {
                    i++;
                }

                result1 = null;
                result2 = null;
            }


            for (int j = 0; j < tokens.Count; j++)
            {
                if (!(_wordsFoundAt.Contains(j)))
                {
                    _tokensNotFoundInGameLexes.Add(tokens[j]);
                }
            }
            


            if (_tokensNotFoundInGameLexes.Count > 0)
            {
                return false;
            }

            return true;
        }
            /*
            foreach (var lex in _lexRepo.GetAllNestedRepos())
            {
                var result = lex.FindFirstBy(a =>
                {

                    if (a.name.Equals(word))
                    {
                        return true;
                    }

                    var isInSynonyms = false;
                    var isInEquivalenceClasses = false;
                    var isInAdjectives = false;
                    var isInAdverbs = false;

                    if (a.synonyms != null && ((IList<string>)a.synonyms).Contains(word))
                    {
                        isInSynonyms = true;
                    }

                    if (a.equivalenceClasses != null && ((IList<string>)a.equivalenceClasses).Contains(word))
                    {
                        isInEquivalenceClasses = true;
                    }

                    try
                    {
                        if (a.adjectives != null && ((IList<string>)a.adjectives).Contains(word))
                        {
                            isInAdjectives = true;
                        }
                    }
                    catch (RuntimeBinderException)
                    {
                        // Intetionally empty to supress errors where property "adjectives" doesn't exist for those particular lexes.
                        // Needed because this func is taking a "dynamic" input.
                    }

                    try
                    {
                        if (a.adverbs != null && ((IList<string>)a.adverbs).Contains(word))
                        {
                            isInAdverbs = true;
                        }
                    }
                    catch (RuntimeBinderException)
                    {

                    }


                    if (isInSynonyms ||
                        isInEquivalenceClasses ||
                        isInAdjectives ||
                        isInAdverbs)
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
            */

    }
}
