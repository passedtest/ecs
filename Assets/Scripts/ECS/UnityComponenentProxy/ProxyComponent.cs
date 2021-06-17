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
            foreach(var componenentHash in ProxyComponentUtility.GetPersistentComponenentTypeHashCodes())
            {
                if (!Core.ComponentTypeUtility.TryGetType(componenentHash, out var componenentType))
                    continue;

                if (!CanAddComponent(componenentType))
                    continue;

                m_Components.Insert(0, Core.SharedComponentMap.ProduseComponent(componenentType));
            }
        }

        public bool TryAddComponenent(Type componenentType)
        {
            var canAddComponentnt = CanAddComponent(componenentType);
            if(canAddComponentnt)
                m_Components.Add(Core.SharedComponentMap.ProduseComponent(componenentType));

            return canAddComponentnt;
        }

        public bool CanAddComponent(Type componenentType)
        {
            foreach (var component in m_Components)
                if (component.GetType().Equals(componenentType))
                    return false;

            return true;
        }

        public bool TryAddComponenent(IComponent componenent)
        {
            var componenentType = componenent.GetType();
            var canAddComponentnt = CanAddComponent(componenentType);
            if (canAddComponentnt)
                m_Components.Add(componenent);

            return canAddComponentnt;
        }

        public EntityLazyBuilder ToBuilder(ECSWorld world) =>
            EntityLazyBuilder
                .New(world)
                .AddComponents(m_Components);

        public IReadOnlyList<IComponent> Components => m_Components;

        public void SetTransform(TransformComponent newTransformComponent)
        {
            for(var i = 0; i < m_Components.Count; i++)
                if (m_Components[i] is TransformComponent)
                    m_Components[i] = newTransformComponent;
        }

        public void ConvertCurrentGameObject()
        {
            var components = GetComponents<IProxyComponenentConverter>();
            foreach (var component in components)
                TryAddComponenent(component.Convert());
        }
    }    
}

