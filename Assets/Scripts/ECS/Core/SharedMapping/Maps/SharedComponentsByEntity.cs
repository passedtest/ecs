using System.Collections.Generic;

namespace ECS.Core.Maps
{
    public class SharedComponentsByEntity
    {
        public HashSet<int> this[int entity] => m_TypesByEntity[entity];
        public IReadOnlyDictionary<int, HashSet<int>> Entities => m_TypesByEntity;

        readonly Dictionary<int, HashSet<int>> m_TypesByEntity;

        public SharedComponentsByEntity()
        {
            m_TypesByEntity = new Dictionary<int, HashSet<int>>();
        }

        public bool TryAllocateEntityIfNotExists(in int entity)
        {
            var hasEntity = m_TypesByEntity.ContainsKey(entity);
            if (!hasEntity)
                m_TypesByEntity.Add(entity, new HashSet<int>());

            return !hasEntity;
        }

        public bool TryRemoveComponent(in int entity, in int typeHash) =>
            m_TypesByEntity.TryGetValue(entity, out var types) && types.Remove(typeHash);

        public bool ClearForEntity(in int entity) =>
            m_TypesByEntity.Remove(entity);
    }
}
