using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Infrastructure.Services.DataDeserializer;
using TextAdventure.Infrastructure.Services.DataReader;

namespace TextAdventure.Infrastructure.Services.ContentProvider
{
    public class JsonContentProvider : IContentProvider
    {
        private IStreamProvider _dataStreamHelper;
        private IDeserializer _jsonDeserializer;

        private List<dynamic> _contentInstancesCache = new List<dynamic>();

        public JsonContentProvider(IDeserializer jsonDeserializer, IStreamProvider dataStreamHelper)
        {
            _jsonDeserializer = jsonDeserializer;
            _dataStreamHelper = dataStreamHelper;
        }

        public T Fetch<T>(string assemblyWithResource) where T : class
        {
            foreach (var contentInstance in _contentInstancesCache)
            {
                if (contentInstance.GetType() == typeof(T))
                {
                    return (T)contentInstance;
                }
            }
            using (var dataStream = _dataStreamHelper.GetResourceStream<T>(".json", assemblyWithResource))
            {
                _contentInstancesCache.Add(_jsonDeserializer.Deserialize<T>(dataStream));
                return (T)_contentInstancesCache.Single(i => typeof(T) == i.GetType());
            }
        }

        public void Populate<T>(object modelObject, string assemblyWithResource) where T : class
        {
            using (var dataStream = _dataStreamHelper.GetResourceStream<T>(".json", assemblyWithResource))
            {
                _jsonDeserializer.DeserializeToExisting(dataStream, modelObject);
            }
        }

        public void PopulateFromString(object modelObject, string jsonString)
        {
            _jsonDeserializer.DeserializeToExisting(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)), modelObject);
        }

        public dynamic FetchDynamic(string fullFileName, string assemblyWithResource)
        {
            using (var dataStream = _dataStreamHelper.GetResourceStream(fullFileName, assemblyWithResource))
            {
                return _jsonDeserializer.DeserailizeDynamic(dataStream);
            }
        }


        /*
        public void DisposeIfExists<T>(T typeOfInstanceToDispose) where T : class
        {
            if (_contentInstancesCache.Count > 0)
            {
                _contentInstancesCache.Remove(
                    _contentInstancesCache.SingleOrDefault(i => i.GetType() == typeOfInstanceToDispose.GetType())
                );
            }
        }
        */

    }
}
