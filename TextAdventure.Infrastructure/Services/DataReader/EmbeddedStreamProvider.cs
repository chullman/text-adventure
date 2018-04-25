using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TextAdventure.Infrastructure.Services.DataReader
{
    public class EmbeddedStreamProvider : IStreamProvider
    {
        // IMPORTANT: For this to work, the JSON files must be set to be built as "Embedded Resource"
        // IMPORTANT: For this to work, the name of the root level model (e.g. class CondContent) must match the file name (i.e. CondContent.json)

        public Stream GetResourceStream<T>(string fileExtension, string assemblyWithResource) where T : class
        {

            //var assembly = GetType().GetTypeInfo().Assembly;
            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == assemblyWithResource);

            string embeddedResourcePath;
            try
            {
                embeddedResourcePath = GetResourceLocation<T>(assembly, fileExtension);
            }
            catch (Exception)
            {
                Debug.WriteLine("ERROR: Unable to retrieve the path of the resource");
                throw;
            }


            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }

        public Stream GetResourceStream(string fullFileName, string assemblyWithResource)
        {
            //var assembly = GetType().GetTypeInfo().Assembly;
            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == assemblyWithResource);

            string embeddedResourcePath;
            try
            {
                embeddedResourcePath = GetResourceLocation(assembly, fullFileName);
            }
            catch (Exception)
            {
                Debug.WriteLine("ERROR: Unable to retrieve the path of the resource");
                throw;
            }


            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }

        private string GetResourceLocation(Assembly assembly, string fullFileName)
        {

            // Get the particular Embedded Resource path string based on the interpolated file name (e.g. "CondContent.json")
            // This is assuming that the file name of the JSON is unique in this project
            var embeddedResourcePath = assembly.GetManifestResourceNames().First(name => name.Contains(fullFileName));

            return embeddedResourcePath;
        }

        private string GetResourceLocation<T>(Assembly assembly, string fileExtension) where T : class
        {

            var type = typeof(T);

            var resourceFileName = type.Name + fileExtension;

            // Get the particular Embedded Resource path string based on the interpolated file name (e.g. "CondContent.json")
            // This is assuming that the file name of the JSON is unique in this project
            var embeddedResourcePath = assembly.GetManifestResourceNames().First(name => name.Contains(resourceFileName));


            return embeddedResourcePath;
        }

    }
}
