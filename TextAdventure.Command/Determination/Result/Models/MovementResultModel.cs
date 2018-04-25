using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Result.Handlers;

namespace TextAdventure.Command.Determination.Result.Models
{
    public class MovementResultModel : IResultModel
    {
        private string _baseActionVerb;
        private string _method;
        private string _speed;
        private string _cardinalDirection;
        private string _locationDirection;
        private IList<string> _locationDirectionRoomIds = new List<string>();
        private string _unknownLocationDirection;
        private string _adverb;

        private CommandResultModelHandler _resultModelHandler;

        private Type _propSource;

        public MovementResultModel(CommandResultModelHandler resultModelHandler)
        {
            _resultModelHandler = resultModelHandler;

            _propSource = typeof(MovementResultModel);
        }

        public void AddObjectToHandler()
        {
            _resultModelHandler.SetResultModel(this);
        }

        public string BaseActionVerb
        {
            get { return _baseActionVerb; }
            set { _resultModelHandler.SetPropertyField(ref _baseActionVerb, value, _propSource); }
        }

        public string Method
        {
            get { return _method; }
            set { _resultModelHandler.SetPropertyField(ref _method, value, _propSource); }
        }

        public string Speed
        {
            get { return _speed; }
            set { _resultModelHandler.SetPropertyField(ref _speed, value, _propSource); }
        }

        public string CardinalDirection
        {
            get { return _cardinalDirection; }
            set { _resultModelHandler.SetPropertyField(ref _cardinalDirection, value, _propSource); }
        }

        public string LocationDirection
        {
            get { return _locationDirection; }
            set { _resultModelHandler.SetPropertyField(ref _locationDirection, value, _propSource); }
        }

        public IList<string> LocationDirectionRoomIds
        {
            get { return _locationDirectionRoomIds; }
            set { _resultModelHandler.SetPropertyField(ref _locationDirectionRoomIds, value, _propSource); }
        }

        public string UnknownLocationDirection
        {
            get { return _unknownLocationDirection; }
            set { _resultModelHandler.SetPropertyField(ref _unknownLocationDirection, value, _propSource); }
        }

        public string Adverb
        {
            get { return _adverb; }
            set { _resultModelHandler.SetPropertyField(ref _adverb, value, _propSource); }
        }

    }
}
