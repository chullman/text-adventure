using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Intention;

namespace TextAdventure.Command.Determination.Mappers
{
    public class MapperRepo
    {
        public Dictionary<BaseIntentions, Type> intentToIntentBuilderMap { get; private set; }

        public Dictionary<Type, BaseIntentions> lexToIntentionMap { get; private set; }

        public Dictionary<string, BaseIntentions> nerStringToIntentionMap { get; private set; }

        public Dictionary<string, BaseIntentions> posStringToIntentionMap { get; private set; }

        public void Set(
            IntentToIntentBuilderMapper intentToIntentBuilderMapper,
            LexToIntentionMapper lexToIntentionMapper,
            NerStringToIntentionMapper nerStringToIntentionMapper,
            PosStringToIntentionMapper posStringToIntentionMapper
            )
        {
            intentToIntentBuilderMap = intentToIntentBuilderMapper.GetMap();
            lexToIntentionMap = lexToIntentionMapper.GetMap();
            nerStringToIntentionMap = nerStringToIntentionMapper.GetMap();
            posStringToIntentionMap = posStringToIntentionMapper.GetMap();
        }
    }
}
