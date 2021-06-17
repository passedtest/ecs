#define TYPE_AS_HASH

using System;
using UnityEngine;

namespace ECS
{
    [Serializable]
    public struct ComponentSnapshot
    {
#if TYPE_AS_HASH
        [SerializeField]
        int t;
#else
        [SerializeField]
        string t;
#endif

        [SerializeField]
        string d;

        public ComponentSnapshot(IComponent targetComponenent)
        {
#if TYPE_AS_HASH
            t = Core.ComponentTypeUtility.HashCodeOf(targetComponenent.GetType());
#else
            t = targetComponenent.GetType().FullName;
#endif
            d = JsonUtility.ToJson(targetComponenent);
        }

        public IComponent Restore()
        {
#if TYPE_AS_HASH
            Core.ComponentTypeUtility.TryGetType(t, out var componenentType);
#else
            var componenentType = Type.GetType(t);
#endif
            var result = JsonUtility.FromJson(d, componenentType);
            return (IComponent)result;
        }
    }
}
