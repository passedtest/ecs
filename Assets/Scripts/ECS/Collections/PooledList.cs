using System.Collections.Generic;

namespace ECS.Collections
{
    public class PooledList<T> : List<T>, IPooledCollection<T>
    {
        public PooledList() : base() { }

        public static PooledList<T> Provide() =>
            CollectionPool<PooledList<T>, T>.Provide();

        public void Free() =>
            CollectionPool<PooledList<T>, T>.Push(this);
    }
}