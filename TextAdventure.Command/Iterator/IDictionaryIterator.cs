using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Iterator
{
    public interface IDictionaryIterator<T, U>
    {
        void SetAggregate(IDictionaryAggregate<T, U> dictAggregate);

        U FirstDictValue { get; }
        U NextDictValue { get; }
        U CurrentDictValue { get; }

        T FirstDictKey { get; }
        T NextDictKey { get; }
        T CurrentDictKey { get; }
        bool IsDoneForValues { get; }
        bool IsDoneForKeys { get; }
    }
}
