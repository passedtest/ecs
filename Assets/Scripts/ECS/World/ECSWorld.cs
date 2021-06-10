using ECS.Core;
using System.Collections.Generic;

namespace ECS
{
    public sealed class ECSWorld
    {
        static int LastWorldUID = 0;

        int m_LastEntityUID;
        public int AllocateEntityUID => ++m_LastEntityUID;
        public int NexteEntityUID => m_LastEntityUID + 1;

        public int UID { get; private set; }
        public bool IsValid => UID > 0;

        public IReadOnlyDictionary<int, HashSet<int>> EntityComponetPairs => SharedComponentMap.GetWorldEntitiesCollection(UID);
        public IEnumerable<int> Entities => EntityComponetPairs.Keys;
        public int EntitiesCount => EntityComponetPairs.Count;

        readonly ISystem m_MasterSystem;

        ECSWorld(int id, ISystem masterSystem)
        {
            UID = id;
            m_MasterSystem = masterSystem;
        }

        public ECSWorld(ISystem masterSystem) : this(++LastWorldUID, masterSystem) { }
        public ECSWorld(params ISystem[] systems) : this(new MasterSystem(systems)) { }

        public ECSWorld(WorldSnapshot snapshot, ISystem masterSystem) : this(snapshot.ID, masterSystem)
        {
            UID = snapshot.ID;

            foreach (var componentCollection in snapshot.ComponentsData)
                foreach (var serializedComponent in componentCollection)
                    serializedComponent
                        .Restore()
                        .Register(snapshot.ID, componentCollection.Entity);
        }

        public void ExecuteUpdate(float deltaTime) =>
            m_MasterSystem.ExecuteUpdate(this, deltaTime);

        public void ExecuteFixedUpdate(float deltaTime) =>
            m_MasterSystem.ExecuteFixedUpdate(this, deltaTime);

        public void ExecuteGizmoUpdate(float deltaTime) =>
            m_MasterSystem.ExecuteGizmoUpdate(this, deltaTime);

        public void UpdateFrom(ComponentMapSnapshot snapshot) =>
            SharedComponentMap.RestoreFromSnapshot(snapshot);

        public void Destroy()
        {
            SharedComponentMap.ClearForWorld(UID);

            //DestroyRoot;
            DestroyView();

            //Invalidate id;
            UID = 0;
        }

        public void DestroyView()
        {
            GameObjectByRef.Dispose(UID);
        }

        public void RegisterComponent<TComponent>(int entity, TComponent component) where TComponent : struct, IComponent =>
             ComponentMap<TComponent>.TryAdd(UID, entity, component);

        public void RemoveEntity(int entity) =>
            SharedComponentMap.ClearForEntity(UID, entity);
       
        public IUnsafeComponentCollection<TComponent> GetOpenedComponentsWithEntity<TComponent>() where TComponent : struct, IComponent =>
            ComponentMap<TComponent>.ForWorldByEntityOpened(UID);

        public IComponentCollection<TComponent> GetComponentsWithEntity<TComponent>() where TComponent : struct, IComponent =>
            ComponentMap<TComponent>.ForWorldByEntity(UID);

        public IReadOnlyComponentCollection<TComponent> GetComponentsWithEntityReadOnly<TComponent>() where TComponent : struct, IComponent =>
            ComponentMap<TComponent>.ForWorldByEntityReadOnly(UID);

        public WorldSnapshot ToSnapshot() => new WorldSnapshot(this);
        public ComponentMapSnapshot ToComponentMapSnapshot() => SharedComponentMap.BuildSnapshotForWorld(UID);
    }
}