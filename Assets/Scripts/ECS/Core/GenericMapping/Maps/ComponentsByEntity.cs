using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public interface IReadOnlyComponentCollection<TComponent> : IReadOnlyCollection<KeyValuePair<int, TComponent>> where TComponent : struct, IComponent
    {
        bool TryGetComponent(in int entity, out TComponent component);
    }

    public interface IComponentCollection<TComponent> : IReadOnlyComponentCollection<TComponent> where TComponent : struct, IComponent
    {
        bool TrySetComponent(in int entity, in TComponent component);
    }

    public interface IUnsafeComponentCollection<TComponent> : IComponentCollection<TComponent> where TComponent : struct, IComponent
    {
        bool TryAddOrSetUnsafe(in int entity, in TComponent component);
        bool TryRemoveUnsafe(in int entity);
    }
}

namespace ECS.Core.Maps
{
    public class ComponentsByEntity<TComponent> : IUnsafeComponentCollection<TComponent> where TComponent : struct, IComponent
    {
        readonly int m_World;
        readonly Dictionary<int, TComponent> m_Map;

        public int Count => m_Map.Count;
        public IReadOnlyCollection<TComponent> Components => m_Map.Values;

        public ComponentsByEntity(int world)
        {
            m_World = world;
            m_Map = new Dictionary<int, TComponent>();
        }

        /// Cycle dependency in this interface :(
        #region IUnsafeComponentCollection
        bool IUnsafeComponentCollection<TComponent>.TryAddOrSetUnsafe(in int entity, in TComponent component) =>
             TrySetComponent(entity, component) || ComponentMap<TComponent>.TryAddOrSet(m_World, entity, component);

        bool IUnsafeComponentCollection<TComponent>.TryRemoveUnsafe(in int entity) =>
            ComponentMap<TComponent>.Remove(m_World, entity);
        #endregion

        public bool TryGetComponent(in int entity, out TComponent component) =>
            m_Map.TryGetValue(entity, out component);

        public bool TryAddOrSetComponent(in int entity, in TComponent component)
        {
            var containsComponent = m_Map.ContainsKey(entity);
            if (containsComponent)
                m_Map[entity] = component;
            else
                m_Map.Add(entity, component);

            return containsComponent;
        }

        public bool TrySetComponent(in int entity, in TComponent component)
        {
            var containsComponent = m_Map.ContainsKey(entity);
            if (containsComponent)
                m_Map[entity] = component;

            return containsComponent;
        }

        public bool RemoveComponent(in int entity) =>
            m_Map.Remove(entity);

        public void Clear() => m_Map.Clear();

        public void ClearForEntity(in int entity) => m_Map.Remove(entity);

        public bool TryGetComponentRaw(in int entity, out IComponent rawComponent)
        {
            var exists = m_Map.TryGetValue(entity, out var component);
            rawComponent = component;
            return exists;
        }

        public IEnumerable<KeyValuePair<int, IComponent>> GetInterfaceEnumerator() 
        {
            foreach (var kvp in m_Map)
                yield return new KeyValuePair<int, IComponent>(kvp.Key, kvp.Value);
        }

        public IEnumerator<KeyValuePair<int, TComponent>> GetEnumerator() => m_Map.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_Map.GetEnumerator();
    }
}

