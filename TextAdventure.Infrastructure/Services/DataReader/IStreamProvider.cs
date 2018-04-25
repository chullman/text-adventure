using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Infrastructure.Services.DataReader
{
    public interface IStreamProvider
    {
        Stream GetResourceStream<T>(string fileExtension, string assemblyWithResource) where T : class;
        Stream GetResourceStream(string fullFileName, string assemblyWithResource);
    }
}
