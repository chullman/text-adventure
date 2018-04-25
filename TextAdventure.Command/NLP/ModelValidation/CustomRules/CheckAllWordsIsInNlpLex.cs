using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using edu.stanford.nlp.parser.lexparser;
using FluentValidation.Validators;
using TextAdventure.Command.NLP.Models;

namespace TextAdventure.Command.NLP.ModelValidation.CustomRules
{
    public class CheckAllWordsIsInNlpLex : PropertyValidator
    {
        private LexicalizedParser _loadedCoreNlpLexModel;

        private IList<Token> _tokensFoundInNlpLex = new List<Token>();
        private IList<Token> _tokensNotFoundInNlpLex = new List<Token>();

        public CheckAllWordsIsInNlpLex(LexicalizedParser loadedCoreNlpLexModel)
            : base("Property {PropertyName} contains an unknown word!")
        {
            _loadedCoreNlpLexModel = loadedCoreNlpLexModel;
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var tokens = context.PropertyValue as IList<Token>;

            foreach (var token in tokens)
            {
                // For some reason, even if a word fails NLP's .isKnown check, it'll still have a valid NER attached to it, so we'll check for that
                if ((_loadedCoreNlpLexModel.getLexicon().isKnown(token.word)) || (token.ner != "O"))
                {
                    _tokensFoundInNlpLex.Add(token);
                }
                else
                {
                    _tokensNotFoundInNlpLex.Add(token);
                }
            }

            if (_tokensNotFoundInNlpLex.Count > 0)
            {
                return false;
            }
            return true;
        }

        public IList<Token> GetTokensFoundInNlpLex()
        {
            return _tokensFoundInNlpLex;
        }

        public IList<Token> GetTokensNotFoundInNlpLex()
        {
            return _tokensNotFoundInNlpLex;
        }
    }
}
