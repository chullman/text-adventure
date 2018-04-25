using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TextAdventure.Command.NLP.Models
{
    public class NlpResult
    {
        public IList<Sentence> sentences { get; set; }
    }

    public class Sentence
    {
        public int index { get; set; }
        public string parse { get; set; }
        public IList<BasicDependency> basicDependencies { get; set; }
        public IList<Token> tokens { get; set; }
        public IList<Entitymention> entitymentions { get; set; }
    }

    public class BasicDependency
    {
        public string dep { get; set; }
        public int governor { get; set; }
        public string governorGloss { get; set; }
        public int dependent { get; set; }
        public string dependentGloss { get; set; }
    }

    public class Entitymention
    {
        public int docTokenBegin { get; set; }
        public int docTokenEnd { get; set; }
        public int tokenBegin { get; set; }
        public int tokenEnd { get; set; }
        public string text { get; set; }
        public int characterOffsetBegin { get; set; }
        public int characterOffsetEnd { get; set; }
        public string ner { get; set; }
    }


    public class Token
    {
        public int index { get; set; }
        public string word { get; set; }
        public string originalText { get; set; }
        public string lemma { get; set; }
        public int characterOffsetBegin { get; set; }
        public int characterOffsetEnd { get; set; }
        public string pos { get; set; }
        public string ner { get; set; }
        public string before { get; set; }
        public string after { get; set; }
    }
}
