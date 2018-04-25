using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.NLP.Models;

namespace TextAdventure.Command.Iterator
{
    public class CommandAggregatesRepo
    {
        public IListAggregate<Token> AllTokensAggregate { get; private set; }

        public IListAggregate<BasicDependency> AllBasicDependenciesAggregate { get; private set; }

        public IListAggregate<Entitymention> AllEntityMentionsAggregate { get; private set; }

        public IDictionaryAggregate<string, dynamic> WordsFoundInGameLexesAggregate { get; private set; }

        public IListAggregate<Token> TokensNotFoundInGameLexesAggregate { get; private set; }

        public IListAggregate<Token> TokensFoundInNlpLexAggregate { get; private set; }

        public IListAggregate<Token> TokensNotFoundInNlpLexAggregate { get; private set; }

        public void Set(
            IListAggregate<Token> allTokensAggregate,
            IListAggregate<BasicDependency> allBasicDependenciesAggregate,
            IListAggregate<Entitymention> allEntityMentionsAggregate,
            IDictionaryAggregate<string, dynamic> wordsFoundInGameLexesAggregate,
            IListAggregate<Token> tokensNotFoundInGameLexesAggregate,
            IListAggregate<Token> tokensFoundInNlpLexAggregate,
            IListAggregate<Token> tokensNotFoundInNlpLexAggregate
            )
        {
            AllTokensAggregate = allTokensAggregate;
            AllBasicDependenciesAggregate = allBasicDependenciesAggregate;
            AllEntityMentionsAggregate = allEntityMentionsAggregate;
            WordsFoundInGameLexesAggregate = wordsFoundInGameLexesAggregate;
            TokensNotFoundInGameLexesAggregate = tokensNotFoundInGameLexesAggregate;
            TokensFoundInNlpLexAggregate = tokensFoundInNlpLexAggregate;
            TokensNotFoundInNlpLexAggregate = tokensNotFoundInNlpLexAggregate;
        }
    }

}
