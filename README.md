# Console Text Adventure Game
Inspired by the original Zork games. A console-based text adventure game with a full featured Natural Language Processing (NLP) implementation and use.

## Tech Stack
- .NET / C#
- Stanford CoreNLP for .NET https://sergey-tihon.github.io/Stanford.NLP.NET/StanfordCoreNLP.html

## Features

- Code base purposely over-engineered to best demonstrated my skills and knowledge in .NET and C# code design
- Adhered to SOLID design pricinples
- All movement and look intentions are recognised by the fully featured parser and actioned upon
- All other possible intentions that the player may want to do are recognised by the NLP system, but the game does not act upon them (to come in a later update)
- Adverbs and adjectives are all fully recognised and utilised - e.g. "very quickly move ..." results in a different action to "slowly move ..."
- Full Named Entity Recognition (NER) support, so points of interests like real world locations are recognised by the NLP system

## Gameplay

### Movement
The player can freely move around the environment using intuitive English phrases as commands. E.g. "go to the north", "walk to the hallway", or simply "north"

### Look
Similarly, the player can look around their environment or at points of interests or items. E.g. "look at room", "inspect the rock"

## Setup

1) Clone, or download and extract this repository
2) Download both Stanford CoreNLP https://stanfordnlp.github.io/CoreNLP/download.html, as well as the English "model jar" file, and extract them both somewhere locally (use a file archiver like 7-Zip to extract the JAR file)
3) Open TextAdventure.Command/CommandBootstrap.cs
    and change line 70 to point to the /models subdirectory
    and change line 74 to point to /englishPCFG.ser.gz (found in /edu/stanford/nlp/models/lexparser subdirectory)
4) Rebuild all and run

## Author

@chullman https://github.com/chullman/
