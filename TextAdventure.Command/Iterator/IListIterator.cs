using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Iterator
{
    public interface IListIterator<T>
    {
        void SetAggregate(IListAggregate<T> modelAggregate);

        T FirstItem { get; }
        T NextItem { get; }
        T CurrentItem { get; }
        bool IsDone { get; }
    }
}
