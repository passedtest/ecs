using ECS.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    [Serializable]
    public struct WorldSnapshot
    {
        [SerializeField]
        int uid;
        public int ID => uid;

        [SerializeField]
        List<ComponentCollection> cd;
        public List<ComponentCollection> ComponentsData => cd;

        public WorldSnapshot(ECSWorld world)
        {
            uid = world.UID;

            cd = new List<ComponentCollection>();

            var componentSerializableAttributeCheck = new Dictionary<Type, bool>();

            foreach (var kvp in world.EntityComponetPairs)
            {
                var componentsData = new ComponentCollection(kvp.Key);
                foreach (var typeHash in kvp.Value)
                {
                    if (!ComponentTypeUtility.TryGetType(typeHash, out var type))
                        continue;

                    if (!componentSerializableAttributeCheck.TryGetValue(type, out var canBeSerialized))
                    {
                        canBeSerialized = Attribute.GetCustomAttribute(type, typeof(SerializableAttribute)) != null;
                        componentSerializableAttributeCheck.Add(type, canBeSerialized);
                    }

                    if (!canBeSerialized)
                        continue;

                    if (SharedComponentMap.TryGetComponent(world.UID, kvp.Key, type, out var component))
                        componentsData.Add(new ComponentSnapshot(component));
                }

                cd.Add(componentsData);
            }
        }

        [Serializable]
        public class ComponentCollection : IReadOnlyList<ComponentSnapshot>
        {
            public ComponentCollection(int entityRef)
            {
                e = entityRef;
                d = new List<ComponentSnapshot>();
            }

            [SerializeField]
            int e;
            public int Entity => e;

            [SerializeField]
            List<ComponentSnapshot> d;
            public ComponentSnapshot this[int index] => d[index];
            public int Count => d.Count;

            public void Add(ComponentSnapshot component) => d.Add(component);

            public IEnumerator<ComponentSnapshot> GetEnumerator() => d.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
