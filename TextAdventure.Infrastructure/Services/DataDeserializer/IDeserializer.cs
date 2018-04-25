using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Infrastructure.Services.DataDeserializer
{
    public interface IDeserializer
    {
        //T Deserialize<T>(string data) where T : IRootModel;
        T Deserialize<T>(Stream stream) where T : class;

        void DeserializeToExisting(Stream stream, object modelObject);

        dynamic DeserailizeDynamic(Stream stream);
    }
}
