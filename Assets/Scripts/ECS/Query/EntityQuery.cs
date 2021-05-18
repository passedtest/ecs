using System;
using System.Collections.Generic;

namespace ECS
{
    public sealed class EntityQuery : IDisposable
    {
        class WorldEntityCollection
        {
            readonly Dictionary<int, HashSet<int>> m_EntitiesByWorld;

            public WorldEntityCollection()
            {
                m_EntitiesByWorld = new Dictionary<int, HashSet<int>>();
            }

            public bool Contains(in int world, in int entity) => ForWorld(world).Contains(entity);

            public void Add(in int world, in int entity) => ForWorld(world).Add(entity);

            public void Remove(in int world, in int entity)
            {
                if (m_EntitiesByWorld.TryGetValue(world, out var entities))
                    entities.Remove(entity);
            }

            public void RemoveForWorld(in int world)
            {
                if (m_EntitiesByWorld.TryGetValue(world, out var entities))
                    entities.Clear();
            }

            public HashSet<int> ForWorld(in int world)
            {
                if (!m_EntitiesByWorld.TryGetValue(world, out var entities))
                    m_EntitiesByWorld.Add(world, entities = new HashSet<int>());

                return entities;
            }

            public void Clear()
            {
                foreach (var entities in m_EntitiesByWorld.Values)
                    entities.Clear();

                m_EntitiesByWorld.Clear();
            }

            public void ClearUnused()
            {
                var keys = new List<int>(m_EntitiesByWorld.Keys);
                foreach(var key in keys)
                {
                    m_EntitiesByWorld[key].Clear();
                    m_EntitiesByWorld.Remove(key);
                }
            }
        }
      
        readonly HashSet<int> m_IncludedAnyComponenents;
        readonly HashSet<int> m_IncludedAllComponenents;
        readonly HashSet<int> m_ExcludedComponenents;

        readonly WorldEntityCollection m_EntitiesByWorld;
        readonly WorldEntityCollection m_IncludedEntities;
        readonly WorldEntityCollection m_ExcludedEntities;

        public EntityQuery(in HashSet<int> includedAnyComponents, in HashSet<int> includedAllComponents, in HashSet<int> excludedComponents)
        {
            m_IncludedAnyComponenents = includedAnyComponents;
            m_IncludedAllComponenents = includedAllComponents;
            m_ExcludedComponenents = excludedComponents;

            foreach (var component in m_IncludedAllComponenents)
                m_IncludedAnyComponenents.Remove(component);

            m_EntitiesByWorld = new WorldEntityCollection();
            m_IncludedEntities = new WorldEntityCollection();
            m_ExcludedEntities = new WorldEntityCollection();

            Core.SharedComponentMap.ComponenentAdded += OnComponenentAdded;
            Core.SharedComponentMap.ComponenentRemoved += OnComponenentRemoved;
            Core.SharedComponentMap.ComponentsRemovedForWorld += OnComponentsRemovedForWorld;
            Core.SharedComponentMap.ComponentsRemovedForEntity += OnComponentsRemovedForEntity;
        }

        public void Dispose()
        {
            m_EntitiesByWorld.Clear();

            m_IncludedAnyComponenents.Clear();
            m_IncludedAllComponenents.Clear();
            m_ExcludedComponenents.Clear();

            Core.SharedComponentMap.ComponenentAdded -= OnComponenentAdded;
            Core.SharedComponentMap.ComponenentRemoved -= OnComponenentRemoved;
            Core.SharedComponentMap.ComponentsRemovedForWorld -= OnComponentsRemovedForWorld;
            Core.SharedComponentMap.ComponentsRemovedForEntity -= OnComponentsRemovedForEntity;
        }

        public ICollection<int> ForWorld(in int world) => m_EntitiesByWorld.ForWorld(world);

        public void ForEach(in int world, Action<int> entityProcessor)
        {
            foreach (var entity in ForWorld(world))
                entityProcessor(entity);
        }

        void OnComponenentAdded(in int world, in int entity, in int componentTypeHash)
        {
            if (m_IncludedAllComponenents.Contains(componentTypeHash) && Core.SharedComponentMap.CheckSubsetCollision(world, entity, m_IncludedAllComponenents))
                m_IncludedEntities.Add(world, entity);

            if (m_IncludedAnyComponenents.Contains(componentTypeHash))
                m_IncludedEntities.Add(world, entity);

            if (m_ExcludedComponenents.Contains(componentTypeHash))
                m_ExcludedEntities.Add(world, entity);

            UpdateQuery(world, entity);
        }

        void OnComponenentRemoved(in int world, in int entity, in int componentTypeHash)
        {
            if (m_IncludedAllComponenents.Contains(componentTypeHash))
                m_IncludedEntities.Remove(world, entity);

            if (m_IncludedAnyComponenents.Contains(componentTypeHash))
            {
                if (!Core.SharedComponentMap.CheckCollision(world, entity, m_IncludedAnyComponenents))
                    m_IncludedEntities.Remove(world, entity);
            }

            if (m_ExcludedComponenents.Contains(componentTypeHash))
            {
                if (!Core.SharedComponentMap.CheckCollision(world, entity, m_ExcludedComponenents))
                    m_ExcludedEntities.Remove(world, entity);
            }

            UpdateQuery(world, entity);
        }

        void UpdateQuery(int world, int entity)
        {
            if (m_IncludedEntities.Contains(world, entity) && !m_ExcludedEntities.Contains(world, entity))
                m_EntitiesByWorld.Add(world, entity);
            else
                m_EntitiesByWorld.Remove(world, entity);
        }

        void OnComponentsRemovedForWorld(in int world)
        {
            m_IncludedEntities.RemoveForWorld(world);
            m_ExcludedEntities.RemoveForWorld(world);
            m_EntitiesByWorld.RemoveForWorld(world);
        }

        void OnComponentsRemovedForEntity(in int world, in int entity)
        {
            m_IncludedEntities.Remove(world, entity);
            m_ExcludedEntities.Remove(world, entity);
            m_EntitiesByWorld.Remove(world, entity);
        }
    }
}
