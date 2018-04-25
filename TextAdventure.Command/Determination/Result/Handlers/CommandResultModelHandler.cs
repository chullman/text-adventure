using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Result.Models;

namespace TextAdventure.Command.Determination.Result.Handlers
{
    public class CommandResultModelHandler : INotifyPropertyChanged
    {
        private Type _propSource;
        private Type _previousPropSource;

        public bool isPropSourceChanged;

        private IResultModel _resultModel;


        public void SetPropertyField<T>(ref T field, T newValue, Type propSource, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            field = newValue;
            _propSource = propSource;
            if (_previousPropSource != _propSource)
            {
                isPropSourceChanged = true;
            }
            else
            {
                isPropSourceChanged = false;
            }
            _previousPropSource = _propSource;
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public Type GetPropSource()
        {
            return _propSource;
        }

        public void SetResultModel(IResultModel resultModel)
        {
            _resultModel = resultModel;
        }

        public IResultModel GetResultModel()
        {
            if (_resultModel != null)
            {
                return _resultModel;
            }
            throw new NullReferenceException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
