using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Game.Command.Commands;

namespace TextAdventure.Game.Command
{
    public interface ICommandHandler
    {
        void SetCommand<T>() where T : ICommand, new();
        ICommand GetCommand();
    }
}
