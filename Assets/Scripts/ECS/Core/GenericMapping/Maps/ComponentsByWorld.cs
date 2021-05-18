using System.Collections.Generic;

namespace ECS.Core.Maps
{
    public class ComponentsByWorld<TComponent> where TComponent : struct, IComponent
    {
        readonly Dictionary<int, ComponentsByEntity<TComponent>> m_Map;

        public ComponentsByWorld()
        {
            m_Map = new Dictionary<int, ComponentsByEntity<TComponent>>();
        }

        public ComponentsByEntity<TComponent> ForWorld(in int world)
        {
            if (!m_Map.TryGetValue(world, out var components))
                m_Map.Add(world, components = new ComponentsByEntity<TComponent>(world));

            return components;
        }

        public bool TryGetComponent(in int world, in int entity, out TComponent component)
        {
            component = default;
            return m_Map.TryGetValue(world, out var componentsByEntity) && componentsByEntity.TryGetComponent(entity, out component);
        }

        public bool TryAddOrSetComponent(in int world, in int entity, TComponent component)
        {
            var entitiesList = GetWorldEntityCollection(world);
            return entitiesList.TryAddOrSetComponent(entity, component);
        }

        public bool RemoveComponent(in int world, in int entity) =>
            m_Map.TryGetValue(world, out var componentsByEntity) && componentsByEntity.RemoveComponent(entity);

        public void ClearForWorld(in int world)
        {
            if (!m_Map.TryGetValue(world, out var componentsByEntity))
                return;

            componentsByEntity.Clear();
            m_Map.Remove(world);
        }

        public void ClearForEntity(in int world, in int entity)
        {
            if (m_Map.TryGetValue(world, out var componentsByEntity))
                componentsByEntity.ClearForEntity(entity);
        }

        public bool TryGetComponentRaw(in int world, in int entity, out IComponent component)
        {
            component = default;
            if (!m_Map.TryGetValue(world, out var componentsByEntity))
                return false;

            return componentsByEntity.TryGetComponentRaw(entity, out component);
        }

        ComponentsByEntity<TComponent> GetWorldEntityCollection(in int world)
        {
            if (!m_Map.TryGetValue(world, out var entitiesList))
                m_Map.Add(world, entitiesList = new ComponentsByEntity<TComponent>(world));

            return entitiesList;
        }
    }
}