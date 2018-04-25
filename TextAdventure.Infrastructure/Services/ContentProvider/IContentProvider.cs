using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Infrastructure.Services.ContentProvider
{
    public interface IContentProvider
    {
        T Fetch<T>(string assemblyWithResource) where T : class;
        dynamic FetchDynamic(string fullFileName, string assemblyWithResource);

        void Populate<T>(object modelObject, string assemblyWithResource) where T : class;

        void PopulateFromString(object modelObject, string jsonString);

        //T Find<T>(string id);
        //IEnumerable<T> FindMany(Func<T, bool> predicate);

        //void DisposeIfExists<T>(T instanceToDispose) where T : class;
    }
}
