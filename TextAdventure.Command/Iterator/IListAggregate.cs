using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Iterator
{
    public interface IListAggregate<T>
    {
        List<T> Get();
        T this[int itemIndex] { get; }
        void Add(T modelProperty);
        int Count { get; }

        IListIterator<T> GetIterator();
    }
}
