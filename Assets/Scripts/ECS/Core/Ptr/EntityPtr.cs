using UnityEngine;

namespace ECS.Core.Ptr
{
    [System.Serializable]
    public struct EntityPtr : IPtr
    {
        public bool IsValid => SharedComponentMap.EntityExists(m_World, m_Entity);

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

        public ComponentPtr<TComponenent> ToComponentPtr<TComponenent>() where TComponenent : struct, IComponent =>
            ComponentPtr<TComponenent>.New(this);
    }
}
