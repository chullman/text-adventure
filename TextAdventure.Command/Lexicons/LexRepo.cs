using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Lexicons.Models.Infrastructure;

namespace TextAdventure.Command.Lexicons
{
    public class LexRepo
    {
        private IList<INestedRepo<dynamic>> _nestedRepos;

        public LexRepo(IList<INestedRepo<dynamic>> nestedRepos)
        {
            _nestedRepos = nestedRepos;
        }

        // dynamic return type??
        public INestedRepo<T> GetNestedRepo<T>() where T : IActionRoot
        {
            foreach (var nestedRepo in _nestedRepos)
            {
                if (typeof(T) == nestedRepo.GetActionType())
                {
                    return (INestedRepo<T>)nestedRepo;
                }
            }
            return null;
        }

        public IList<INestedRepo<dynamic>> GetAllNestedRepos()
        {
            return _nestedRepos;
        }

    }
}
