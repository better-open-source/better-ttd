using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Entities.Base;

namespace BetterTTD.Coan_OLD.Pools
{
    public abstract class GenericPool<TKey, TValue> : IEnumerable<TValue>
        where TValue : Poolable<TKey>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _pool = new();

        public IEnumerator<TValue> GetEnumerator()
        {
            return _pool.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pool.GetEnumerator();
        }

        public bool TryAdd(TValue value)
        {
            return TryAdd(value.Id, value);
        }
        
        public bool TryAdd(TKey key, TValue value)
        {
            return _pool.TryAdd(key, value);
        }

        public void Clean()
        {
            _pool.Clear();
        }

        public bool Exists(TKey key)
        {
            return _pool.Keys.Contains(key);
        }

        public TValue Get(TKey key)
        {
            return _pool[key];
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            return _pool.TryRemove(key, out value);
        }

        public void Set(GenericPool<TKey, TValue> pool)
        {
            Clean();
            foreach (var value in pool)
            {
                TryAdd(value);
            }
        }

        public int Size()
        {
            return _pool.Count;
        }
    }
}