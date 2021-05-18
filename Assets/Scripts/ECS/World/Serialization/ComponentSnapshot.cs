using System;
using UnityEngine;

namespace ECS
{
    [Serializable]
    public struct ComponentSnapshot
    {
        [SerializeField]
        string t;

        [SerializeField]
        string d;

        public ComponentSnapshot(object targetObject)
        {
            t = targetObject.GetType().FullName;
            d = JsonUtility.ToJson(targetObject);
        }

        public object Restore()
        {
            var type = Type.GetType(t);
            var result = JsonUtility.FromJson(d, type);
            return result;
        }
    }
}
