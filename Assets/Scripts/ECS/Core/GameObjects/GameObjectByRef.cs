using ECS.Core.GameObjectProxy;
using System.Collections.Generic;

namespace ECS.Core
{
    public static class GameObjectByRef
    {
        readonly static Dictionary<int, GameObjectWorldMap> s_TrackedGameObjects;

        static GameObjectByRef()
        {
            s_TrackedGameObjects = new Dictionary<int, GameObjectWorldMap>();
        }
        public static bool Exists(in int world, in int reference) => GetOrAllocateWorldMap(world).Exists(reference);

        public static bool TryGetObject(in int world, in int reference, out UnityEngine.GameObject go) =>
            GetOrAllocateWorldMap(world).TryGetObject(reference, out go);

        public static UnityEngine.GameObject GetOrReproduse(in int world, in UnityEngine.GameObject prefab, in int referenceToReproduse) =>
            GetOrAllocateWorldMap(world).GetOrReproduse(prefab, referenceToReproduse);

        public static int Produse(in int world, in UnityEngine.GameObject prefab) =>
            GetOrAllocateWorldMap(world).Produse(prefab);

        public static void Dispose(in int world)
        {
            if (s_TrackedGameObjects.TryGetValue(world, out var map))
            {
                map.Dispose();
                s_TrackedGameObjects.Remove(world);
            }
        }

        static GameObjectWorldMap GetOrAllocateWorldMap(in int world)
        {
            if (!s_TrackedGameObjects.TryGetValue(world, out var map))
                s_TrackedGameObjects.Add(world, map = new GameObjectWorldMap(world));

            return map;
        }
    }
}

