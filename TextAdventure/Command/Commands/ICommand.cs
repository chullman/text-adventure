using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Result.Models;
using TextAdventure.Game.Displayables;

namespace TextAdventure.Game.Command.Commands
{
    public interface ICommand
    {
        void Execute();

        void InjectInResultModel(IResultModel resultModel, IList<string> resultModelPropsChanged);

        void InjectInDisplayablesRepo(DisplayablesRepo displayablesRepo);
    }
}
