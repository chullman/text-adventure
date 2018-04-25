# Console Text Adventure Game
Inspired by the original Zork games. A console-based text adventure game with a full featured Natural Language Processing (NLP) implementation and use.

## Tech Stack
- .NET / C#
- Stanford CoreNLP for .NET https://sergey-tihon.github.io/Stanford.NLP.NET/StanfordCoreNLP.html

## Features

- Code base purposely over-engineered to best demonstrated my skills and knowledge in .NET and C# code design
- Adhered to SOLID design pricinples
- All movement and look intentions are recognised by the fully featured parser and actioned upon
- All other possible intentions that the player may want to do are
- Adverbs and adjectives are all fully recognised and utilised - e.g. "very quickly move ..." results in a different action to "slowly move ..."
- Full Named Entity Recognition (NER) support, so points of interests like real world locations are recognised by the NLP system

## Gameplay

### Movement
The player can freely move around the environment using intuitive English phrases as commands. E.g. "go to the north", "walk to the hallway", or simply "north"

### Look
Similarly, the player can look around their environment or at points of interests or items. E.g. "look at room", "inspect the rock"

## Author

@chullman https://github.com/chullman/