using System.Collections.Generic;

namespace ECS.Core.Maps
{
    public class SharedComponentsByWorld
    {
        readonly Dictionary<int, SharedComponentsByEntity> m_WorldEntities;

        public SharedComponentsByEntity this[in int world] => m_WorldEntities[world];

        public SharedComponentsByWorld()
        {
            m_WorldEntities = new Dictionary<int, SharedComponentsByEntity>();
        }

        public bool EntityExists(in int world, in int entity) =>
            m_WorldEntities.TryGetValue(world, out var enitityCollection) && enitityCollection.EntityExists(entity);

        public bool TryAllocateEntityIfNotExists(in int world, in int entity)
        {
            if (!m_WorldEntities.TryGetValue(world, out var enitityCollection))
                m_WorldEntities.Add(world, enitityCollection = new SharedComponentsByEntity());

            return enitityCollection.TryAllocateEntityIfNotExists(entity);
        }

        public bool TryRegisterComponentType(in int world, in int entity, in int typeHash)
        {
            TryAllocateEntityIfNotExists(world, entity);
            return m_WorldEntities[world][entity].Add(typeHash);
        }

        public bool TryRemoveComponentType(in int world, in int entity, in int typeHash) =>
            m_WorldEntities.TryGetValue(world, out var enitityCollection) && enitityCollection.TryRemoveComponent(entity, typeHash);

        public bool ClearForWorld(in int world) => m_WorldEntities.Remove(world);

        public bool ClearForEntity(in int world, in int entity) =>
            m_WorldEntities.TryGetValue(world, out var enitityCollection) && enitityCollection.ClearForEntity(entity);

        public bool TryGetComponenents(in int world, in int entity, out HashSet<int> componenentTypes)
        {
            componenentTypes = default;
            return m_WorldEntities.TryGetValue(world, out var enitityCollection) && enitityCollection.Entities.TryGetValue(entity, out componenentTypes);
        }
    }
}
