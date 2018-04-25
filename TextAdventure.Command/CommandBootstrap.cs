using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using edu.stanford.nlp.parser.lexparser;
using edu.stanford.nlp.pipeline;
using FluentValidation;
using TextAdventure.Command.Determination;
using TextAdventure.Command.Determination.Intention;
using TextAdventure.Command.Determination.Mappers;
using TextAdventure.Command.Determination.Result.Handlers;
using TextAdventure.Command.Determination.Result.Models;
using TextAdventure.Command.Displayables.Models;
using TextAdventure.Command.Iterator;
using TextAdventure.Command.Lexicons;
using TextAdventure.Command.Lexicons.Models;
using TextAdventure.Command.NLP;
using TextAdventure.Command.NLP.Models;
using TextAdventure.Command.NLP.ModelValidation;
using TextAdventure.Command.NLP.ModelValidation.CustomRules;
using TextAdventure.Infrastructure.Application;
using TextAdventure.Infrastructure.Services.ContentProvider;

namespace TextAdventure.Command
{
    // I'm aware this is a form of a "God Object"

    public class CommandBootstrap
    {
        private ApplicationRuntime _runtime;

        private CoreNlpProcessor _coreNlpProcessor;

        private StanfordCoreNLP _pipeline;
        private LexicalizedParser _lexicalizedParser;

        private LexRepo _lexRepo;

        private LexToIntentionMapper _lexToIntentMapping;
        private NerStringToIntentionMapper _nerToIntentMapping;
        private PosStringToIntentionMapper _posToIntentMapping;
        private IntentToIntentBuilderMapper _intentToIntentBuilderMapping;

        private MapperRepo _mapperRepo;

        private BaseIntentions _baseIntentionResult;

        private IntentionBuilderFactory _intentionBuilderFactory;


        private CommandResultModelHandler _resultModelHandler;


        public CommandBootstrap(ApplicationRuntime runtime)
        {
            _runtime = runtime;
        }

        public void Initiate()
        {
            _coreNlpProcessor = _runtime.Container.Resolve<CoreNlpProcessor>();

            var jsonContentProvider = _runtime.Container.Resolve<IContentProvider>();

            _coreNlpProcessor.JarRootDir = @"C:\stanford-corenlp-full-2017-06-09\models";

            _pipeline = _coreNlpProcessor.LoadNlp(jsonContentProvider);

            _lexicalizedParser = _coreNlpProcessor.LoadLexicalizedParser(@"C:\stanford-corenlp-full-2017-06-09\models\edu\stanford\nlp\models\lexparser\englishPCFG.ser.gz");

            var currentAssemblyName = GetType().GetTypeInfo().Assembly.GetName().Name;

            var movementLex = _runtime.Container.Resolve<MovementLex>();
            var adverbLex = _runtime.Container.Resolve<AdverbLex>();
            var adjectivesLex = _runtime.Container.Resolve<AdjectivesLex>();
            var inventoryLex = _runtime.Container.Resolve<InventoryLex>();
            var poiLex = _runtime.Container.Resolve<PoiLex>();
            var miscObjectLex = _runtime.Container.Resolve<MiscObjectLex>();
            var takeLex = _runtime.Container.Resolve<TakeLex>();
            var directionsLex = _runtime.Container.Resolve<DirectionsLex>();
            var locationsLex = _runtime.Container.Resolve<LocationsLex>();
            var lookLex = _runtime.Container.Resolve<LookLex>();
            var adverbModLex = _runtime.Container.Resolve<AdverbModLex>();

            jsonContentProvider.Populate<MovementLex>(movementLex, currentAssemblyName);
            jsonContentProvider.Populate<AdverbLex>(adverbLex, currentAssemblyName);
            jsonContentProvider.Populate<AdjectivesLex>(adjectivesLex, currentAssemblyName);
            jsonContentProvider.Populate<InventoryLex>(inventoryLex, currentAssemblyName);
            jsonContentProvider.Populate<PoiLex>(poiLex, currentAssemblyName);
            jsonContentProvider.Populate<MiscObjectLex>(miscObjectLex, currentAssemblyName);
            jsonContentProvider.Populate<TakeLex>(takeLex, currentAssemblyName);
            jsonContentProvider.Populate<DirectionsLex>(directionsLex, currentAssemblyName);
            jsonContentProvider.Populate<LocationsLex>(locationsLex, currentAssemblyName);
            jsonContentProvider.Populate<LookLex>(lookLex, currentAssemblyName);
            jsonContentProvider.Populate<AdverbModLex>(adverbModLex, currentAssemblyName);


            _lexRepo = _runtime.Container.Resolve<LexRepo>(new NamedParameter("nestedRepos", new List<INestedRepo<dynamic>>
            {
                (INestedRepo<InventoryLex.Action>)new LexActionRepo<InventoryLex.Action>(inventoryLex, "Actions"),
                (INestedRepo<MovementLex.Action>)new LexActionRepo<MovementLex.Action>(movementLex, "Actions"),
                (INestedRepo<AdverbLex.Action>)new LexActionRepo<AdverbLex.Action>(adverbLex, "Actions"),
                (INestedRepo<AdjectivesLex.Action>)new LexActionRepo<AdjectivesLex.Action>(adjectivesLex, "Actions"),
                (INestedRepo<PoiLex.Action>)new LexActionRepo<PoiLex.Action>(poiLex, "Actions"),
                (INestedRepo<MiscObjectLex.Action>)new LexActionRepo<MiscObjectLex.Action>(miscObjectLex, "Actions"),
                (INestedRepo<TakeLex.Action>)new LexActionRepo<TakeLex.Action>(takeLex, "Actions"),
                (INestedRepo<DirectionsLex.Action>)new LexActionRepo<DirectionsLex.Action>(directionsLex, "Actions"),
                (INestedRepo<LocationsLex.Action>)new LexActionRepo<LocationsLex.Action>(locationsLex, "Actions"),
                (INestedRepo<LookLex.Action>)new LexActionRepo<LookLex.Action>(lookLex, "Actions"),
                (INestedRepo<AdverbModLex.Action>)new LexActionRepo<AdverbModLex.Action>(adverbModLex, "Actions"),
            }));

            _lexToIntentMapping = _runtime.Container.Resolve<LexToIntentionMapper>();

            _lexToIntentMapping.AddToMap(typeof(MovementLex), BaseIntentions.Move);
            _lexToIntentMapping.AddToMap(typeof(AdverbLex), BaseIntentions.Adverb);
            _lexToIntentMapping.AddToMap(typeof(AdverbModLex), BaseIntentions.AdverbMod);
            _lexToIntentMapping.AddToMap(typeof(AdjectivesLex), BaseIntentions.Adjective);
            _lexToIntentMapping.AddToMap(typeof(InventoryLex), BaseIntentions.Inventory);
            _lexToIntentMapping.AddToMap(typeof(PoiLex), BaseIntentions.ObjectOfInterest);
            _lexToIntentMapping.AddToMap(typeof(MiscObjectLex), BaseIntentions.MiscObject);
            _lexToIntentMapping.AddToMap(typeof(TakeLex), BaseIntentions.Take);
            _lexToIntentMapping.AddToMap(typeof(DirectionsLex), BaseIntentions.Direction);
            _lexToIntentMapping.AddToMap(typeof(LocationsLex), BaseIntentions.Location);
            _lexToIntentMapping.AddToMap(typeof(LookLex), BaseIntentions.Look);

            _nerToIntentMapping = _runtime.Container.Resolve<NerStringToIntentionMapper>();

            _nerToIntentMapping.AddToMap("PERSON", BaseIntentions.NerPerson);
            _nerToIntentMapping.AddToMap("LOCATION", BaseIntentions.NerLocation);
            _nerToIntentMapping.AddToMap("ORGANIZATION", BaseIntentions.NerOrganization);
            _nerToIntentMapping.AddToMap("MISC", BaseIntentions.NerMisc);
            _nerToIntentMapping.AddToMap("MONEY", BaseIntentions.NerMoney);
            _nerToIntentMapping.AddToMap("NUMBER", BaseIntentions.NerNumber);
            _nerToIntentMapping.AddToMap("ORDINAL", BaseIntentions.NerOrdinal);
            _nerToIntentMapping.AddToMap("PERCENT", BaseIntentions.NerPercent);
            _nerToIntentMapping.AddToMap("O", BaseIntentions.NerUnknown);

            _posToIntentMapping = _runtime.Container.Resolve<PosStringToIntentionMapper>();

            _posToIntentMapping.AddToMap("NN", BaseIntentions.UnknownNoun);
            _posToIntentMapping.AddToMap("NNS", BaseIntentions.UnknownNounPlural);
            _posToIntentMapping.AddToMap("VB", BaseIntentions.UnknownVerb);
            _posToIntentMapping.AddToMap("JJ", BaseIntentions.UnknownAdjective);
            _posToIntentMapping.AddToMap("RB", BaseIntentions.UnknownAdverb);

            _intentToIntentBuilderMapping = _runtime.Container.Resolve<IntentToIntentBuilderMapper>();

            _intentToIntentBuilderMapping.AddToMap(BaseIntentions.Move, typeof(MovementIntention));
            _intentToIntentBuilderMapping.AddToMap(BaseIntentions.Look, typeof(LookIntention));

            _mapperRepo = _runtime.Container.Resolve<MapperRepo>();

            _mapperRepo.Set(_intentToIntentBuilderMapping, _lexToIntentMapping, _nerToIntentMapping, _posToIntentMapping);

        }

        public bool BootstrapCommandDetermination(string stringInput)
        {
            var nlpResultModel = _runtime.Container.Resolve<NlpResult>();

            nlpResultModel = _coreNlpProcessor.DeserializeInput(_pipeline, nlpResultModel, stringInput);

            var jsonContentProvider = _runtime.Container.Resolve<IContentProvider>();
            var currentAssemblyName = GetType().GetTypeInfo().Assembly.GetName().Name;
            var errorDisplayables = _runtime.Container.Resolve<ErrorDisplayables>();
            jsonContentProvider.Populate<ErrorDisplayables>(errorDisplayables, currentAssemblyName);

            var basicValidatorHndlr = _runtime.Container.Resolve<BasicValidatorHandler>();

            var rootValidator = basicValidatorHndlr.FetchModel<NlpResult>(nlpResultModel);

            rootValidator.RuleFor(result => result.sentences).SetValidator(new ListMustContainOnlyOneItem<Sentence>(errorDisplayables));
            rootValidator.RuleFor(result => result.sentences).SetValidator(new ListMustNotBeEmpty<Sentence>(errorDisplayables));

            var rootValidatorResult = basicValidatorHndlr.ValidateModel(rootValidator);

            if (!rootValidatorResult.IsValid)
            {
                foreach (var failure in rootValidatorResult.Errors)
                {
                    Console.WriteLine(failure.ErrorMessage);
                }
                return false;
            }

            var nerTypes = new List<string>
            {
                "PERSON",
                "LOCATION",
                "ORGANIZATION",
                "MISC",
                "MONEY",
                "NUMBER",
                "ORDINAL",
                "PERCENT"
            };

            stringInput = "";

            foreach (var token in nlpResultModel.sentences[0].tokens)
            {
                if (nerTypes.Contains(token.ner))
                {
                    stringInput += token.word;
                    stringInput += token.after;
                }
                else
                {
                    stringInput += token.word.ToLower();
                    stringInput += token.after.ToLower();
                }

            }

            nlpResultModel = _runtime.Container.Resolve<NlpResult>();
            nlpResultModel = _coreNlpProcessor.DeserializeInput(_pipeline, nlpResultModel, stringInput);



            var sentenceModelToValidate = basicValidatorHndlr.FetchModel<Sentence>(nlpResultModel.sentences[0]);

            sentenceModelToValidate.RuleFor(result => result.tokens).SetValidator(new CheckAllWordsIsInGameLex(_lexRepo));

            basicValidatorHndlr.ValidateModel(sentenceModelToValidate);


            var checkAllWordsIsInGameLex = basicValidatorHndlr.GetAValidator<CheckAllWordsIsInGameLex>(sentenceModelToValidate, "tokens");





            var truncatedTokensToValidate = basicValidatorHndlr.FetchModel<IList<Token>>(checkAllWordsIsInGameLex.GetTokensNotFoundInGameLexes());

            truncatedTokensToValidate.RuleFor(result => result).SetValidator(new CheckAllWordsIsInNlpLex(_lexicalizedParser));

            var truncatedTokensToValidateResult = basicValidatorHndlr.ValidateModel(truncatedTokensToValidate);

            var checkAllWordsIsInNlpLex = basicValidatorHndlr.GetAValidatorOfEnumerable<CheckAllWordsIsInNlpLex, Token>(truncatedTokensToValidate, 0);


            if (!truncatedTokensToValidateResult.IsValid)
            {
                foreach (var tokenNotFound in checkAllWordsIsInNlpLex.GetTokensNotFoundInNlpLex())
                {
                    Console.WriteLine("I do not understand the word ", tokenNotFound.word);
                }
                return false;
            }


            var commandAggregatesRepo = _runtime.Container.Resolve<CommandAggregatesRepo>();

            var allTokensIterator = _runtime.Container.Resolve<IListIterator<Token>>();
            var allTokensAggregate = _runtime.Container.Resolve<IListAggregate<Token>>(new TypedParameter(typeof(IListIterator<>), allTokensIterator));

            foreach (var token in nlpResultModel.sentences[0].tokens)
            {
                allTokensAggregate.Add(token);
            }

            var allBasicDependenciesIterator = _runtime.Container.Resolve<IListIterator<BasicDependency>>();
            var allBasicDependenciesAggregate = _runtime.Container.Resolve<IListAggregate<BasicDependency>>(new TypedParameter(typeof(IListIterator<>), allBasicDependenciesIterator));

            foreach (var basicDependency in nlpResultModel.sentences[0].basicDependencies)
            {
                allBasicDependenciesAggregate.Add(basicDependency);
            }

            var allEntityMentionsIterator = _runtime.Container.Resolve<IListIterator<Entitymention>>();
            var allEntityMentionsAggregate = _runtime.Container.Resolve<IListAggregate<Entitymention>>(new TypedParameter(typeof(IListIterator<>), allEntityMentionsIterator));

            foreach (var entityMention in nlpResultModel.sentences[0].entitymentions)
            {
                allEntityMentionsAggregate.Add(entityMention);
            }

            var wordsFoundInGameLexesIterator = _runtime.Container.Resolve<IDictionaryIterator<string, dynamic>>();
            var wordsFoundInGameLexesAggregate = _runtime.Container.Resolve<IDictionaryAggregate<string, dynamic>>(new TypedParameter(typeof(IDictionaryIterator<,>), wordsFoundInGameLexesIterator));


            foreach (var wordsFoundInGameLex in checkAllWordsIsInGameLex.GetWordsFoundInGameLexes())
            {
                wordsFoundInGameLexesAggregate.AddOrReplace(wordsFoundInGameLex.Key, wordsFoundInGameLex.Value);
            }


            var tokensNotFoundInGameLexesIterator = _runtime.Container.Resolve<IListIterator<Token>>();
            var tokensNotFoundInGameLexesAggregate = _runtime.Container.Resolve<IListAggregate<Token>>(new TypedParameter(typeof(IListIterator<>), tokensNotFoundInGameLexesIterator));

            foreach (var tokenNotFoundInGameLexes in checkAllWordsIsInGameLex.GetTokensNotFoundInGameLexes())
            {
                tokensNotFoundInGameLexesAggregate.Add(tokenNotFoundInGameLexes);
            }

            var tokensFoundInNlpLexIterator = _runtime.Container.Resolve<IListIterator<Token>>();
            var tokensFoundInNlpLexAggregate = _runtime.Container.Resolve<IListAggregate<Token>>(new TypedParameter(typeof(IListIterator<>), tokensFoundInNlpLexIterator));

            foreach (var tokenFoundInNlpLex in checkAllWordsIsInNlpLex.GetTokensFoundInNlpLex())
            {
                tokensFoundInNlpLexAggregate.Add(tokenFoundInNlpLex);
            }

            var tokensNotFoundInNlpLexIterator = _runtime.Container.Resolve<IListIterator<Token>>();
            var tokensNotFoundInNlpLexAggregate = _runtime.Container.Resolve<IListAggregate<Token>>(new TypedParameter(typeof(IListIterator<>), tokensNotFoundInNlpLexIterator));

            foreach (var tokenNotFoundInNlpLex in checkAllWordsIsInNlpLex.GetTokensNotFoundInNlpLex())
            {
                tokensNotFoundInNlpLexAggregate.Add(tokenNotFoundInNlpLex);
            }

            commandAggregatesRepo.Set(
                allTokensAggregate, 
                allBasicDependenciesAggregate,
                allEntityMentionsAggregate,
                wordsFoundInGameLexesAggregate,
                tokensNotFoundInGameLexesAggregate,
                tokensFoundInNlpLexAggregate,
                tokensNotFoundInNlpLexAggregate
                );

            // BUG - for an unknown reason, cannot use Autofac to resolve IntentionBuilderFactory, as it does not adhere to InstancePerDependency registration. Using New instead as a workaround.
            _intentionBuilderFactory = new IntentionBuilderFactory(commandAggregatesRepo, _mapperRepo, _lexRepo);


            _baseIntentionResult = _intentionBuilderFactory.DetermineBaseIntention();


            // **TEMP**
            switch (_baseIntentionResult)
            {
                case BaseIntentions.Adverb:
                    Console.WriteLine("I'm not sure what you're trying to do?");
                    break;
                case BaseIntentions.AdverbMod:
                    Console.WriteLine("What?");
                    break;
                case BaseIntentions.Adjective:
                    Console.WriteLine("I'm not sure what you're trying to describe?");
                    break;
                case BaseIntentions.Inventory:
                    Console.WriteLine("Sorry, I don't have any inventory functionality programmed in me");
                    break;
                case BaseIntentions.Take:
                    Console.WriteLine("Sorry, I don't yet know how to take things");
                    break;
                case BaseIntentions.ObjectOfInterest:
                    Console.WriteLine("What about it?");
                    break;
                case BaseIntentions.MiscObject:
                    Console.WriteLine("What about it?");
                    break;
                case BaseIntentions.Direction:
                    Console.WriteLine("What about that direction?");
                    break;
                case BaseIntentions.Location:
                    Console.WriteLine("What about that location?");
                    break;
                case BaseIntentions.NerPerson:
                    Console.WriteLine("I know that person!");
                    break;
                case BaseIntentions.NerLocation:
                    Console.WriteLine("I know that place! I think it's very far away from here");
                    break;
                case BaseIntentions.NerOrganization:
                    Console.WriteLine("What about them?");
                    break;
                case BaseIntentions.NerMisc:
                    Console.WriteLine("What about it?");
                    break;
                case BaseIntentions.NerMoney:
                    Console.WriteLine("Money won't help me now");
                    break;
                case BaseIntentions.NerNumber:
                    Console.WriteLine("What about the number?");
                    break;
                case BaseIntentions.NerOrdinal:
                    Console.WriteLine("Ordinal for what?");
                    break;
                case BaseIntentions.NerPercent:
                    Console.WriteLine("Percentage of what?");
                    break;
                case BaseIntentions.NerUnknown:
                    Console.WriteLine("I don't recognise that");
                    break;
                case BaseIntentions.UnknownVerb:
                    Console.WriteLine("What are you trying to do");
                    break;
                case BaseIntentions.UnknownNoun:
                    Console.WriteLine("What about it?");
                    break;
                case BaseIntentions.UnknownNounPlural:
                    Console.WriteLine("What about them?");
                    break;
                case BaseIntentions.UnknownAdjective:
                    Console.WriteLine("What are you trying to describe?");
                    break;
                case BaseIntentions.UnknownAdverb:
                    Console.WriteLine("I'm not sure what you're trying to do");
                    break;
                case BaseIntentions.EntirelyUnknown:
                    Console.WriteLine("I don't understand");
                    break;
            }


            _resultModelHandler = _runtime.Container.Resolve<CommandResultModelHandler>();

            return true;
        }

        public CommandResultModelHandler GetCommandResultModelHandlerForGame()
        {
            if (_resultModelHandler != null)
            {
                return _resultModelHandler;
            }
            throw new NullReferenceException("ERROR: CommandResultModelHandler has not been instantiated.");
        }

        public void BootstrapCommandResult()
        {
            if (_baseIntentionResult == BaseIntentions.Move)
            {
                var movementResultModel = _runtime.Container.Resolve<MovementResultModel>(new TypedParameter(typeof(CommandResultModelHandler), _resultModelHandler));

                movementResultModel.AddObjectToHandler();

                var builtIntention = _intentionBuilderFactory.Build(BaseIntentions.Move);

                builtIntention.PopulateResultModel(movementResultModel);


                
                Debug.WriteLine(movementResultModel.BaseActionVerb + " - IS BASE ACTION VERB");
                Debug.WriteLine(movementResultModel.Adverb + " - IS ADVERB");
                Debug.WriteLine(movementResultModel.Method + " - METHOD");
                Debug.WriteLine(movementResultModel.Speed + " - IS SPEED");
                Debug.WriteLine(movementResultModel.CardinalDirection + " - IS CARDINAL DIRECTION");
                Debug.WriteLine(movementResultModel.LocationDirection + " - IS LOCATION DIRECTION");
                ((List<string>) movementResultModel.LocationDirectionRoomIds).ForEach(a => Debug.WriteLine(a + " - IS A ROOM ID FOR LOCATION DIRECTION"));
                Debug.WriteLine(movementResultModel.UnknownLocationDirection + " - IS UNKNOWN LOCATION DIRECTION");

            }

            if (_baseIntentionResult == BaseIntentions.Look)
            {
                var lookResultModel = _runtime.Container.Resolve<LookResultModel>(new TypedParameter(typeof(CommandResultModelHandler), _resultModelHandler));

                lookResultModel.AddObjectToHandler();

                var builtIntention = _intentionBuilderFactory.Build(BaseIntentions.Look);

                builtIntention.PopulateResultModel(lookResultModel);

                 
                Debug.WriteLine(lookResultModel.BaseActionVerb + " - IS BASE ACTION VERB");
                Debug.WriteLine(lookResultModel.Method + " - IS METHOD");
                Debug.WriteLine(lookResultModel.Adverb + " - IS ADVERB");
                Debug.WriteLine(lookResultModel.PoiName + " - IS POINAME");
                Debug.WriteLine(lookResultModel.PoiLocId + " - IS POI LOC ID");
                Debug.WriteLine(lookResultModel.PoiItemId + " - IS POI ITEM ID");
                ((List<string>)lookResultModel.PoiAdjectives).ForEach(a => Debug.WriteLine(a + " - IS A POI ADJECTIVE"));
                Debug.WriteLine(lookResultModel.UnknownPoi + " - IS UNKNOWN POI");
                Debug.WriteLine(lookResultModel.MiscObjectName + " - IS MISC OBJECT");
                Debug.WriteLine(lookResultModel.MiscObjectLocId + " - IS MISC OBJECT LOC ID");
                Debug.WriteLine(lookResultModel.MiscObjectItemId + " - IS MISC OBJECT ITEM ID");
                Debug.WriteLine(lookResultModel.UnknownMiscObject + " - IS UNKNOWN MISC OBJECT");
                Debug.WriteLine(lookResultModel.PoiPlural + " - IS POI PLURAL");
                Debug.WriteLine(lookResultModel.CardinalDirection + " - IS CARDINAL DIRECTION");
                Debug.WriteLine(lookResultModel.LocationDirection + " - IS LOCATION DIRECTION");
                ((List<string>)lookResultModel.LocationDirectionRoomIds).ForEach(a => Debug.WriteLine(a + " - IS A ROOM ID FOR LOCATION DIRECTION"));
                Debug.WriteLine(lookResultModel.UnknownLocationDirection + " - IS UNKNOWN LOCATION DIRECTION");
            }
        }


    }
}
