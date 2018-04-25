using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TextAdventure.Command.Iterator
{
    public class TokensIterator<T> : IListIterator<T>
    {
        private int _currentIndex = 0;

        private IListAggregate<T> _modelAggregate;

        public void SetAggregate(IListAggregate<T> modelAggregate)
        {
            _modelAggregate = modelAggregate;
        }

        public T FirstItem
        {
            get
            {
                _currentIndex = 0;
                if (_modelAggregate[_currentIndex] != null)
                {
                    return _modelAggregate[_currentIndex];
                }
                return default(T);
            }
        }

        public T NextItem
        {
            get
            {
                _currentIndex += 1;

                if (IsDone == false)
                {
                    if (_modelAggregate[_currentIndex] != null)
                    {
                        return _modelAggregate[_currentIndex];
                    }
                    return default(T);
                }
                return default(T);
            }
        }

        public T CurrentItem
        {
            get
            {
                if (_modelAggregate[_currentIndex] != null)
                {
                    return _modelAggregate[_currentIndex];
                }
                return default(T);
            }
        }

        public bool IsDone
        {
            get
            {
                if (_currentIndex < _modelAggregate.Count)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
