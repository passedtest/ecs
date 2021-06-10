using UnityEngine;

namespace ECS.Experimental.Collections
{
    [System.Serializable]
    public struct EntityRangePtr
    {
        [SerializeField]
        int m_World;
        public int World => m_World;

        [SerializeField]
        int m_Start;
        public int Start => m_Start;

        [SerializeField]
        int m_Length;
        public int Length => m_Length;

        public EntityRangePtr(in int world, in int start, in int length)
        {
            m_World = world;
            m_Start = start;
            m_Length = length;
        }
    }
}
