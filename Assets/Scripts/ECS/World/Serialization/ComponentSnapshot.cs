using System;
using UnityEngine;

namespace ECS
{
    [Serializable]
    public struct ComponentSnapshot
    {
        [SerializeField]
        int t;

        [SerializeField]
        string d;

        public ComponentSnapshot(IComponent targetComponenent)
        {
            t = Core.ComponentTypeUtility.HashCodeOf(targetComponenent.GetType());
            d = JsonUtility.ToJson(targetComponenent);
        }

        public IComponent Restore()
        {
            Core.ComponentTypeUtility.TryGetType(t, out var componenentType);
            var result = JsonUtility.FromJson(d, componenentType);
            return (IComponent)result;
        }
    }
}
