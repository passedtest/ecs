using System.Collections;
using System.Collections.Generic;

namespace ECS.Core
{
    public class ComponentMapSnapshot : IReadOnlyCollection<KeyValuePair<int, IComponent>>
    {
        public readonly int World;
        readonly List<KeyValuePair<int, IComponent>> m_InternalCollection;

        public ComponentMapSnapshot(int world, IEnumerable<KeyValuePair<int, IComponent>> collection)
        {
            World = world;
            m_InternalCollection = new List<KeyValuePair<int, IComponent>>(collection);
        }

        public int Count => m_InternalCollection.Count;

        public IEnumerator<KeyValuePair<int, IComponent>> GetEnumerator() =>
            m_InternalCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();
    }
}
