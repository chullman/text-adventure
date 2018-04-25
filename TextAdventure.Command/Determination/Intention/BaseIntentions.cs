using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Determination.Intention
{
    public enum BaseIntentions
    {
        Move,
        Adverb,
        AdverbMod,
        Adjective,
        Look,
        Inventory,
        Take,
        ObjectOfInterest,
        MiscObject,
        Direction,
        Location,

        NerPerson,
        NerLocation,
        NerOrganization,
        NerMisc,
        NerMoney,
        NerNumber,
        NerOrdinal,
        NerPercent,
        NerUnknown,

        UnknownVerb,
        UnknownNoun,
        UnknownNounPlural,
        UnknownAdjective,
        UnknownAdverb,

        EntirelyUnknown
    }
}
