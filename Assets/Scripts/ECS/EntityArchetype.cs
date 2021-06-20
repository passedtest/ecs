using ECS.Core;
using System.Collections.Generic;

namespace ECS
{
    public class EntityArchetype
    {
        readonly HashSet<int> m_Components; 

        public EntityArchetype()
        {
            m_Components = new HashSet<int>();
        }

        public void WithComponenet<TComponent>() where TComponent : struct, IComponent =>
            m_Components.Add(ComponentTypeUtility.HashCodeOf<TComponent>());

        public EntityLazyBuilder ToBuilder(in int world)
        {
            var builder = EntityLazyBuilder.New(world);
            foreach (var componenentTypeHash in m_Components)
                builder += SharedComponentMap.ProduseComponennt(componenentTypeHash);

            return builder;
        }

        public void ProduseEntity(in int world) => 
            ToBuilder(world).BuildNow();

        public bool IsEntityMatched(in int world, in int entity) =>
            SharedComponentMap.CheckSubsetCollision(world, entity, m_Components);
    }
}
