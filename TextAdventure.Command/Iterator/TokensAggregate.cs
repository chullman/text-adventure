using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TextAdventure.Command.Iterator
{
    public class TokensAggregate<T> : IListAggregate<T>
    {
        private List<T> _values = new List<T>();

        private IListIterator<T> _iterator;

        public TokensAggregate(IListIterator<T> iterator)
        {
            _iterator = iterator;
        }

        public IListIterator<T> GetIterator()
        {
            _iterator.SetAggregate(this);
            return _iterator;
        }

        public List<T> Get()
        {
            return _values;
        }

        public T this[int itemIndex]
        {
            get
            {
                return _values[itemIndex];
            }
        }

        /*
        public ModelAggregate(List<T> modelPropCollection)
        {
            _values = modelPropCollection;
        }
        */ 

        public void Add(T modelProperty)
        {
            _values.Add(modelProperty);
        }

        public int Count
        {
            get
            {
                return _values.Count;
            }
        }
    }
}
