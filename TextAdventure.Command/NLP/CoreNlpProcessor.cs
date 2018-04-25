using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using edu.stanford.nlp.parser.lexparser;
using edu.stanford.nlp.pipeline;
using java.io;
using java.util;
using TextAdventure.Command.NLP.Models;
using TextAdventure.Infrastructure.Extensions;
using TextAdventure.Infrastructure.Services.ContentProvider;

namespace TextAdventure.Command.NLP
{
    public class CoreNlpProcessor : IExternalJavaModel
    {
        private string _jarRoot;

        private IContentProvider _jsonContentProvider;

        public string JarRootDir
        {
            get
            {
                return _jarRoot;
            }
            set
            {
                _jarRoot = value;
            }
        }

        public StanfordCoreNLP LoadNlp(IContentProvider jsonContentProvider)
        {
            _jsonContentProvider = jsonContentProvider;

            // Annotation pipeline configuration
            var props = new Properties();
            props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, parse, entitymentions");
            props.setProperty("ner.useSUTime", "0");

            // We should change current directory, so StanfordCoreNLP could find all the model files automatically
            var curDir = Environment.CurrentDirectory;

            if (!_jarRoot.IsNullOrEmpty())
            {
                Directory.SetCurrentDirectory(_jarRoot);
            }
            else
            {
                throw new NullReferenceException("ERROR: Core NLP Java model directory not specified. Set string JarRootDir.");
            }
            

            // UNCOMMENT ME TO SUPRESS LOGGING OUTPUT
            //RedwoodConfiguration.empty().capture(java.lang.System.err).apply();
            var pipeline = new StanfordCoreNLP(props);
            //RedwoodConfiguration.current().clear().apply();

            Directory.SetCurrentDirectory(curDir);

            return pipeline;
        }

        public edu.stanford.nlp.parser.lexparser.LexicalizedParser LoadLexicalizedParser(string jarLexicalizedParserFile)
        {
            return LexicalizedParser.loadModel(jarLexicalizedParserFile);
        }

        public NlpResult DeserializeInput(StanfordCoreNLP pipeline, NlpResult nlpResult, string stringInput)
        {

            // Annotation
            var annotation = new Annotation(stringInput);
            pipeline.annotate(annotation);

            // Result - Pretty Print
            using (var stream = new ByteArrayOutputStream())
            {
                pipeline.jsonPrint(annotation, new PrintWriter(stream));

                _jsonContentProvider.PopulateFromString(nlpResult, stream.toString());

                Debug.WriteLine(stream.toString());

                stream.close();
            }

            return nlpResult;
        }

    }
}
