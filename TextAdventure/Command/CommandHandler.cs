using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Result.Models;
using TextAdventure.Game.Command.Commands;
using TextAdventure.Game.Displayables;
using TextAdventure.Game.Game.Machines;

namespace TextAdventure.Game.Command
{
    public class CommandHandler : ICommandHandler
    {
        private IResultModel _resultModel;
        private IList<string> _resultModelPropsChanged;
        private DisplayablesRepo _displayablesRepo;

        private ICommand _command;

        public CommandHandler(IResultModel resultModel, IList<string> resultModelPropsChanged, DisplayablesRepo displayablesRepo)
        {
            _resultModel = resultModel;
            _resultModelPropsChanged = resultModelPropsChanged;
            _displayablesRepo = displayablesRepo;
        }
        public void SetCommand<T>() where T : ICommand, new()
        {
            _command = new T();

            _command.InjectInResultModel(_resultModel, _resultModelPropsChanged);

            _command.InjectInDisplayablesRepo(_displayablesRepo);
        }

        public ICommand GetCommand()
        {
            if (_command != null)
            {
                return _command;
            }
            return null;
        }
    }
}
