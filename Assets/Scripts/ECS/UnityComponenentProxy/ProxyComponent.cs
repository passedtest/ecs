using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.UnityProxy
{
    [DisallowMultipleComponent]
    public sealed class ProxyComponent : MonoBehaviour
    {
        [SerializeReference]
        List<IComponent> m_Components = new List<IComponent>();

        void OnValidate()
        {
            if (CanAddComponent(typeof(TransformComponent)))
                m_Components.Insert(0, new TransformComponent());
        }

        public bool TryAddComponenent(Type componenentType)
        {
            var canAddComponentnt = CanAddComponent(componenentType);
            if(canAddComponentnt)
                m_Components.Add(Core.SharedComponentMap.ProduseComponennt(componenentType));

            return canAddComponentnt;
        }

        public bool CanAddComponent(Type componenentType)
        {
            foreach (var component in m_Components)
                if (component.GetType().Equals(componenentType))
                    return false;

            return true;
        }

        public EntityLazyBuilder ToBuilder(ECSWorld world) =>
            EntityLazyBuilder.New(world).AddComponents(m_Components);

        public IReadOnlyList<IComponent> Components => m_Components;
    }    
}

