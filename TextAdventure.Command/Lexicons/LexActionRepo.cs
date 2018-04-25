using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Lexicons.Models.Infrastructure;

namespace TextAdventure.Command.Lexicons
{
    public class LexActionRepo<T> : INestedRepo<T> where T : IActionRoot
    {

        private ILex _lex;

        private string _actionColName;

        public LexActionRepo(ILex lex, string actionCollName)
        {
            _lex = lex;
            _actionColName = actionCollName;
        }

        public T FindFirstBy(Func<T, bool> predicate)
        {
            var actionsProp = (IEnumerable<T>)_lex.GetType().GetProperty(_actionColName).GetValue(_lex, null);

            var result = actionsProp.FirstOrDefault(predicate);

            if (result != null)
            {
                return result;
            }

            return default(T);
        }

        public Type GetActionType()
        {
            return typeof(T);
        }

        public Type GetParentLexType()
        {
            var actionsProp = (IEnumerable<T>)_lex.GetType().GetProperty(_actionColName).GetValue(_lex, null);

            if (actionsProp.ToList().Any())
            {
                return actionsProp.ToList()[0].OfLex;
            }

            return null;
                
        }

    }
}
