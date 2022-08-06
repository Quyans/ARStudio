using System.Collections.Generic;

namespace TriLib
{
    public class ConcurrentList<T>
    {
        private List<T> _list = new List<T>();
        private object _sync = new object();

        public void Add(T value)
        {
            lock (_sync)
            {
                _list.Add(value);
            }
        }

        public int Count
        {
            get
            {
                lock (_sync)
                {
                    return _list.Count;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                lock (_sync)
                {
                    return _list[index];
                }
            }
            set
            {
                lock (_sync)
                {
                    _list[index] = value;
                }
            }
        }
    }
}
