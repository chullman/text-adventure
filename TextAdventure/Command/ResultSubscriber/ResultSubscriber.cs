using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Result.Handlers;

namespace TextAdventure.Game.Command.ResultSubscriber
{
    public class ResultSubscriber
    {
        private readonly ResultListener.ResultListener _resultListener;
        private CommandResultModelHandler _commandResultModelHandler;

        private PropertyChangedEventHandler _listenerDelegate;

        public ResultSubscriber(ResultListener.ResultListener resultListener)
        {
            _resultListener = resultListener;
        }

        public void Subscribe(CommandResultModelHandler commandResultModelHandler)
        {
            _commandResultModelHandler = commandResultModelHandler;

            // Using anonymous delegate to add an additional parameter, commandResultModelHandler to send to ListenAndLearn method
            _listenerDelegate = delegate(object sender, PropertyChangedEventArgs args) { _resultListener.ListenAndLearn(sender, args, _commandResultModelHandler); };
            _commandResultModelHandler.PropertyChanged += _listenerDelegate;
        }

        public void Unsubscribe()
        {
            if (_listenerDelegate != null)
            {
                _commandResultModelHandler.PropertyChanged -= _listenerDelegate;
            }
        }
    }
}
