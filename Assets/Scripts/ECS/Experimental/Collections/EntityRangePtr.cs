﻿using ECS.Core.Ptr;
using UnityEngine;

namespace ECS.Core.Experimental.Collections
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

        public EntityPtr this[in int index] =>
            new EntityPtr(m_World, IndexToEntity(index));

        public int IndexToEntity(int index) => 
            m_Start + index;
    }
}
