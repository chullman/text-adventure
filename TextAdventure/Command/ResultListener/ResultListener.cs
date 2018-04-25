using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Result.Handlers;
using TextAdventure.Command.Determination.Result.Models;

namespace TextAdventure.Game.Command.ResultListener
{
    public class ResultListener
    {
        private Type _propSourceBeingChanged;
        private readonly IList<string> _propertiesBeingChanged = new List<string>();
        private IResultModel _resultModel;

        public void ListenAndLearn(object sender, PropertyChangedEventArgs e, CommandResultModelHandler commandResultModelHandler)
        {

            Debug.WriteLine("A property has changed: " + e.PropertyName + " in " + commandResultModelHandler.GetPropSource());

            _propertiesBeingChanged.Add(e.PropertyName);

            if (commandResultModelHandler.isPropSourceChanged)
            {
                _propSourceBeingChanged = commandResultModelHandler.GetPropSource();
                _resultModel = commandResultModelHandler.GetResultModel();
            }
        }

        public IList<string> GetPropsThatWereChanged()
        {
            return _propertiesBeingChanged;
        }

        public Type GetPropSourceThatWasChanged()
        {
            return _propSourceBeingChanged;
        }

        public IResultModel GetObjectThatWasChanged()
        {
            return _resultModel;
        }
    }


}
