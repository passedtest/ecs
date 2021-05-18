using System.Collections.Generic;

namespace ECS.Core.GameObjectProxy
{
    public class GameObjectWorldMap
    {
        int m_CurrentObjectUID;

        readonly Dictionary<int, UnityEngine.GameObject> m_TrackedObjects;
        readonly UnityEngine.GameObject m_Root;

        public GameObjectWorldMap(in int world)
        {
            m_TrackedObjects = new Dictionary<int, UnityEngine.GameObject>();
            UnityEngine.Object.DontDestroyOnLoad(m_Root = new UnityEngine.GameObject($"World {world}"));
        }

        public bool Exists(in int reference) => m_TrackedObjects.ContainsKey(reference);

        public bool TryGetObject(in int reference, out UnityEngine.GameObject go) =>
            m_TrackedObjects.TryGetValue(reference, out go);

        public UnityEngine.GameObject GetOrReproduse(in UnityEngine.GameObject prefab, in int referenceToReproduse) =>
            TryGetObject(referenceToReproduse, out var go) ? go : ProduseWithId(prefab, referenceToReproduse);

        public int Produse(in UnityEngine.GameObject prefab)
        {
            ProduseWithId(prefab, ++m_CurrentObjectUID);
            return m_CurrentObjectUID;
        }

        UnityEngine.GameObject ProduseWithId(in UnityEngine.GameObject prefab, in int reference)
        {
            if (!GameObjectUtils.IsReferenceIdValid(reference))
                throw new System.InvalidOperationException();

            var instance = UnityEngine.Object.Instantiate(prefab, m_Root.transform);
            m_TrackedObjects.Add(reference, instance);
            return instance;
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(m_Root);
            m_TrackedObjects.Clear();
        }
    }
}
