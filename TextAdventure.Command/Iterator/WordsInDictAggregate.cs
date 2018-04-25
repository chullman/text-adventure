using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Command.Iterator
{
    public class WordsInDictAggregate<T, U> : IDictionaryAggregate<T,U>
    {
        private IDictionary<T, U> _dictionary = new Dictionary<T, U>();

        private IDictionaryIterator<T, U> _iterator;

        public WordsInDictAggregate(IDictionaryIterator<T, U> iterator)
        {
            _iterator = iterator;
        }


        public IDictionary<T, U> Get()
        {
            return _dictionary;
        }

        public U this[T itemKey]
        {
            get
            {
                return _dictionary[itemKey];
            }
        }

        public void AddOrReplace(T dictKey, U dictValue)
        {
            _dictionary[dictKey] = dictValue;
        }

        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        public IDictionaryIterator<T, U> GetIterator()
        {
            _iterator.SetAggregate(this);
            return _iterator;
        }
    }
}
