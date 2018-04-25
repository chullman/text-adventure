using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Lexicons
{
    public interface INestedRepo<out T>
    {
        T FindFirstBy(Func<T, bool> predicate);

        Type GetActionType();

        Type GetParentLexType();
    }
}
