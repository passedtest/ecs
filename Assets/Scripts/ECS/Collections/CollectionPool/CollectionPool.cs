using System.Collections.Generic;

namespace ECS.Collections
{
    public static class CollectionPool<TCollection, TElement> where TCollection : class, ICollection<TElement>, new()
    {
        public static TCollection Provide()
        {
            var collection = Pool<TCollection>.Provide();
            collection.Clear();
            return collection;
        }

        public static void Push(TCollection item)
        {
            item.Clear();
            Pool<TCollection>.Push(item);
        }
    }
}
