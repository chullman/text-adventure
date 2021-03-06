﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Game.Game.Triggers
{
    public interface IStaticTriggers : ITriggers
    {
        T Fetch<T>() where T : ITrigger;
    }
}
