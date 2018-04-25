using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Iterator
{
    public interface IDictionaryAggregate<T, U>
    {
        IDictionary<T, U> Get();
        U this[T itemKey] { get; }
        void AddOrReplace(T dictKey, U dictValue);
        int Count { get; }

        IDictionaryIterator<T, U> GetIterator();
    }
}
