using UnityEngine;

namespace ECS.Core.Ptr
{
    [System.Serializable]
    public struct EntityPtr
    {
        [SerializeField]
        int m_World;
        public int World => m_World;

        [SerializeField]
        int m_Entity;
        public int Entity => m_Entity;

        public EntityPtr(in int world, in int entity)
        {
            m_World = world;
            m_Entity = entity;
        }
    }
}
