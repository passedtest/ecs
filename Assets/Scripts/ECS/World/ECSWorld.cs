using ECS.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public sealed class ECSWorld
    {
        static int LastWorldUID = 0;

        int m_LastEntityUID;
        public int AllocateEntityUID => ++m_LastEntityUID;

        public int UID { get; private set; }
        public bool IsValid => UID > 0;

        public IReadOnlyDictionary<int, HashSet<int>> EntityComponetPairs => SharedComponentMap.GetWorldEntitiesCollection(UID);
        public IEnumerable<int> Entities => EntityComponetPairs.Keys;
        public int EntitiesCount => EntityComponetPairs.Count;

        ECSWorld(int id)
        {
            UID = id;

            //Root = new GameObject($"World {UID}");
           // UnityEngine.Object.DontDestroyOnLoad(Root);
        }

        public ECSWorld() : this(++LastWorldUID) { }

        public ECSWorld(WorldSnapshot snapshot) : this(snapshot.ID)
        {
            UID = snapshot.ID;

            foreach (var componentCollection in snapshot.ComponentsData)
            {
                foreach (var serializedComponent in componentCollection)
                {
                    var component = (IComponent)serializedComponent.Restore();
                    component.Register(snapshot.ID, componentCollection.Entity);
                }
            }
        }

        public void UpdateForm(ComponentMapSnapshot snapshot)
        {
            SharedComponentMap.RestoreFromSnapshot(snapshot);
        }

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