using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TextAdventure.Infrastructure.Services.DataDeserializer
{
    public class JsonDeserializer : IDeserializer
    {
        public T Deserialize<T>(Stream stream) where T : class
        {
            T root;
            try
            {
                var serializer = new JsonSerializer();

                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    root = serializer.Deserialize<T>(jsonTextReader);
                }
                //root = JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception)
            {
                Debug.WriteLine("ERROR: Unable to deserialise JSON");
                throw;
            }

            return root;
        }

        public void DeserializeToExisting(Stream stream, object modelObject)
        {
            try
            {
                var serializer = new JsonSerializer();

                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    serializer.Populate(jsonTextReader, modelObject);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("ERROR: Unable to deserialise JSON");
                throw;
            }

        }

        public dynamic DeserailizeDynamic(Stream stream)
        {
            dynamic root;
            try
            {
                var serializer = new JsonSerializer();
                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    root = serializer.Deserialize(jsonTextReader);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("ERROR: Unable to deserialise JSON");
                throw;
            }

            return root;
        }
    }
}
