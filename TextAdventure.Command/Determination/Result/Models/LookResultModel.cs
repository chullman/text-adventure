using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Result.Handlers;

namespace TextAdventure.Command.Determination.Result.Models
{
    public class LookResultModel : IResultModel
    {
        private string _baseActionVerb;
        private string _method;
        private string _adverb;
        private string _poiName;
        private string _poiLocId;
        private string _poiItemId;
        private string _miscObjectName;
        private string _miscObjectLocId;
        private string _miscObjectItemId;
        private string _unknownMiscObject;
        private IList<string> _poiAdjectives = new List<string>();
        private string _cardinalDirection;
        private string _locationDirection;
        private IList<string> _locationDirectionRoomIds = new List<string>();
        private string _unknownLocationDirection;
        private string _unknownPoi;
        private bool _poiPlural;

        private CommandResultModelHandler _resultModelHandler;

        private Type _propSource;

        public LookResultModel(CommandResultModelHandler resultModelHandler)
        {
            _resultModelHandler = resultModelHandler;

            _propSource = typeof(LookResultModel);
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

        public string Adverb
        {
            get { return _adverb; }
            set { _resultModelHandler.SetPropertyField(ref _adverb, value, _propSource); }
        }

        public string PoiName
        {
            get { return _poiName; }
            set { _resultModelHandler.SetPropertyField(ref _poiName, value, _propSource); }
        }

        public string PoiLocId
        {
            get { return _poiLocId; }
            set { _resultModelHandler.SetPropertyField(ref _poiLocId, value, _propSource); }
        }

        public string PoiItemId
        {
            get { return _poiItemId; }
            set { _resultModelHandler.SetPropertyField(ref _poiItemId, value, _propSource); }
        }

        public string MiscObjectName
        {
            get { return _miscObjectName; }
            set { _resultModelHandler.SetPropertyField(ref _miscObjectName, value, _propSource); }
        }

        public string MiscObjectLocId
        {
            get { return _miscObjectLocId; }
            set { _resultModelHandler.SetPropertyField(ref _miscObjectLocId, value, _propSource); }
        }

        public string MiscObjectItemId
        {
            get { return _miscObjectItemId; }
            set { _resultModelHandler.SetPropertyField(ref _miscObjectItemId, value, _propSource); }
        }

        public string UnknownMiscObject
        {
            get { return _unknownMiscObject; }
            set { _resultModelHandler.SetPropertyField(ref _unknownMiscObject, value, _propSource); }
        }

        public IList<string> PoiAdjectives
        {
            get { return _poiAdjectives; }
            set { _resultModelHandler.SetPropertyField(ref _poiAdjectives, value, _propSource); }
        }

        public string UnknownPoi
        {
            get { return _unknownPoi; }
            set { _resultModelHandler.SetPropertyField(ref _unknownPoi, value, _propSource); }
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

        public bool PoiPlural
        {
            get { return _poiPlural; }
            set { _resultModelHandler.SetPropertyField(ref _poiPlural, value, _propSource); }
        }
    }
}
