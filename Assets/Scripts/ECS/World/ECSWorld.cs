using ECS.Core;
using System.Collections.Generic;

namespace ECS
{
    public sealed class ECSWorld
    {
        static int LastWorldUID = 0;

        public EntityAllocator EntityAllocator { get; private set; }

        public int UID { get; private set; }
        public bool IsValid => UID > 0;

        public IReadOnlyDictionary<int, HashSet<int>> EntityComponetPairs => SharedComponentMap.GetWorldEntitiesCollection(UID);
        public IEnumerable<int> Entities => EntityComponetPairs.Keys;
        public int EntitiesCount => EntityComponetPairs.Count;

        readonly ISystem m_MasterSystem;

        ECSWorld(int uid, int entityUID, ISystem masterSystem)
        {
            UID = uid;
            EntityAllocator = new EntityAllocator(entityUID);
            m_MasterSystem = masterSystem;
        }

        ECSWorld(int uid, ISystem masterSystem) : this(uid, 0, masterSystem) { }

        public ECSWorld(ISystem masterSystem) : this(++LastWorldUID, masterSystem) { }
        public ECSWorld(params ISystem[] systems) : this(new MasterSystem(systems)) { }
        public ECSWorld(WorldSnapshot snapshot, ISystem masterSystem) : this(snapshot.UID, snapshot.EntityUID, masterSystem)
        {
            foreach (var componentCollection in snapshot.ComponentsData)
                foreach (var serializedComponent in componentCollection)
                    serializedComponent
                        .Restore()
                        .Register(snapshot.UID, componentCollection.Entity);
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

        public void DestroyView() =>
            GameObjectByRef.Dispose(UID);

        public IUnsafeComponentCollection<TComponent> GetOpenedComponentsWithEntity<TComponent>() where TComponent : struct, IComponent =>
            ComponentMap<TComponent>.ForWorldByEntityOpened(UID);

        public IComponentCollection<TComponent> GetComponentsWithEntity<TComponent>() where TComponent : struct, IComponent =>
            ComponentMap<TComponent>.ForWorldByEntity(UID);

        public IReadOnlyComponentCollection<TComponent> GetComponentsWithEntityReadOnly<TComponent>() where TComponent : struct, IComponent =>
            ComponentMap<TComponent>.ForWorldByEntityReadOnly(UID);

        public WorldSnapshot ToSnapshot() => new WorldSnapshot(this);
        public ComponentMapSnapshot ToComponentMapSnapshot() => SharedComponentMap.BuildSnapshotForWorld(UID);

        public Experimental.Collections.CollectionPtr<TElement> AllocateCollection<TElement>(in int length) where TElement : struct =>
            AllocateCollectionViaCommandBuffer<TElement>(length, new DirectCommandBufferConcatenator());

        public Experimental.Collections.CollectionPtr<TElement> AllocateCollectionViaCommandBuffer<TElement>(in int length, in ICommandBufferConcatenator commandBuffer) where TElement : struct
        {
            var entityRangePtp = new Experimental.Collections.EntityRangePtr(UID, EntityAllocator.NextEntityUID, length);

            for (var i = 0; i < length; i++)
                commandBuffer.TryAddOrSetComponent(UID, EntityAllocator.AllocateUID, new Experimental.Collections.CollectionElementComponent<TElement>());

            return Experimental.Collections.CollectionPtr<TElement>.New(entityRangePtp);
        }
    }
}