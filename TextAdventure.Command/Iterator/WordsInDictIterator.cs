using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Iterator
{
    public class WordsInDictIterator<T, U> : IDictionaryIterator<T, U>
    {
        private int _currentValuesIndex = 0;
        private int _currentKeysIndex = 0;

        private IDictionaryAggregate<T, U> _dictAggregate;

        public void SetAggregate(IDictionaryAggregate<T, U> dictAggregate)
        {
            _dictAggregate = dictAggregate;
        }

        public U FirstDictValue
        {
            get
            {
                _currentValuesIndex = 0;

                if (_dictAggregate.Get().Values.ElementAt(0) != null)
                {
                    return _dictAggregate.Get().Values.ElementAt(0);
                }
                return default(U);
            }
        }

        public U NextDictValue
        {
            get
            {
                _currentValuesIndex += 1;

                if (IsDoneForValues == false)
                {
                    if (_dictAggregate.Get().Values.ElementAt(_currentValuesIndex) != null)
                    {
                        return _dictAggregate.Get().Values.ElementAt(_currentValuesIndex);
                    }
                    return default(U);
                }
                return default(U);
            }
        }

        public U CurrentDictValue
        {
            get
            {
                if (_dictAggregate.Get().Values.ElementAt(_currentValuesIndex) != null)
                {
                    return _dictAggregate.Get().Values.ElementAt(_currentValuesIndex);
                }
                return default(U);
            }
        }

        public T FirstDictKey
        {
            get
            {
                _currentKeysIndex = 0;
                if (_dictAggregate.Get().Keys.ElementAt(0) != null)
                {
                    return _dictAggregate.Get().Keys.ElementAt(0);
                }
                return default(T);
            }
        }

        public T NextDictKey
        {
            get
            {
                _currentKeysIndex += 1;

                if (IsDoneForKeys == false)
                {
                    if (_dictAggregate.Get().Keys.ElementAt(_currentKeysIndex) != null)
                    {
                        return _dictAggregate.Get().Keys.ElementAt(_currentKeysIndex);
                    }
                    return default(T);
                }
                return default(T);
            }
        }

        public T CurrentDictKey
        {
            get
            {
                if (_dictAggregate.Get().Keys.ElementAt(_currentKeysIndex) != null)
                {
                    return _dictAggregate.Get().Keys.ElementAt(_currentKeysIndex);
                }
                return default(T);
            }
        }

        public bool IsDoneForValues
        {
            get
            {
                if (_currentValuesIndex < _dictAggregate.Count)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsDoneForKeys
        {
            get
            {
                if (_currentKeysIndex < _dictAggregate.Count)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
