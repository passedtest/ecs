using System;
using System.Collections.Generic;

namespace ECS.Core
{
    /// <summary>
    /// Shared interface for each component map;
    /// </summary>
    abstract class SharedComponentMap
    {
        public delegate void ComponenentsRemovedForWorldDelegate(in int world);
        public delegate void ComponentsRemovedForEntityDelegate(in int world, in int entity);
        public delegate void ComponentStateChangedDelegate(in int world, in int entity, in int componentTypeHash);

        public static event ComponenentsRemovedForWorldDelegate ComponentsRemovedForWorld = delegate { };
        public static event ComponentsRemovedForEntityDelegate ComponentsRemovedForEntity = delegate { };
        public static event ComponentStateChangedDelegate ComponenentAdded = delegate { };
        public static event ComponentStateChangedDelegate ComponenentRemoved = delegate { };

        static readonly Dictionary<int, SharedComponentMap> s_TrackedMaps;
        static readonly Maps.SharedComponentsByWorld s_Entities;

        static SharedComponentMap()
        {
            s_Entities = new Maps.SharedComponentsByWorld();
            s_TrackedMaps = new Dictionary<int, SharedComponentMap>();
        }

        readonly TypeDefinition m_Type;

        protected SharedComponentMap(TypeDefinition componentType) 
        {
            m_Type = componentType;
        }

        protected static void RegisterMap(SharedComponentMap map) => s_TrackedMaps.Add(map.m_Type.HashCode, map);

        public static IReadOnlyDictionary<int, HashSet<int>> GetWorldEntitiesCollection(in int world) => s_Entities[world].Entities;
        
        #region MODIFICATIONS
        public static void ClearForWorld(in int world)
        {
            foreach (var map in s_TrackedMaps.Values)
                map.ClearForWorld_Internal(world);

            if (s_Entities.ClearForWorld(world))
                ComponentsRemovedForWorld(world);
        }

        public static void ClearForEntity(in int world, in int entity)
        {
            foreach (var map in s_TrackedMaps.Values)
                map.ClearForEntity_Internal(world, entity);

            if (s_Entities.ClearForEntity(world, entity))
                ComponentsRemovedForEntity(world, entity);
        }

        public static void CopyEntityToWorld(in int world, in int entity, in int targetWorld)
        {
            if (!s_Entities.TryGetComponenents(world, entity, out var componentTypes))
                throw new InvalidOperationException();

            foreach (var componentTypeHash in componentTypes)
                if (TryGetComponentInternal(world, entity, componentTypeHash, out var component))
                    component.Register(targetWorld, entity);
        }

        protected static bool TryRegisterComponentType(in int world, in int entity, in int typeHash)
        {
            var typeRegistred = s_Entities.TryRegisterComponentType(world, entity, typeHash);
            if (typeRegistred)
                ComponenentAdded(world, entity, typeHash);

            return typeRegistred;
        }

        protected static bool TryRemoveComponentType(in int world, in int entity, in int typeHash)
        {
            var typeRemoved = s_Entities.TryRemoveComponentType(world, entity, typeHash);
            if (typeRemoved)
                ComponenentRemoved(world, entity, typeHash);

            return typeRemoved;
        }
        #endregion

        public static IEnumerable<KeyValuePair<int, IComponent>> ComponentsForWorld(int world)
        {
            foreach (var map in s_TrackedMaps.Values)
                foreach (var c in map.CompoenentsProvider_Internal(world))
                    yield return c;
        }

        public static ComponentMapSnapshot BuildSnapshotForWorld(in int world) => 
            new ComponentMapSnapshot(world, ComponentsForWorld(world));

        public static void RestoreFromSnapshot(in ComponentMapSnapshot snapshot)
        {
            ClearForWorld(snapshot.World);
            foreach (var kvp in snapshot)
                kvp.Value.Register(snapshot.World, kvp.Key);
        }

        public static bool EntityExists(in int world, in int entity) =>
            s_Entities.EntityExists(world, entity);

        public static bool TryGetComponent<TComponent>(in int world, in int entity, out IComponent component) where TComponent : struct, IComponent =>
            TryGetComponent(world, entity, typeof(TComponent), out component);

        public static bool TryGetComponent(in int world, in int entity, in Type componentType, out IComponent component) => 
            TryGetComponentInternal(world, entity, ComponentTypeUtility.HashCodeOf(componentType), out component);

        static bool TryGetComponentInternal(in int world, in int entity, in int componentTypeHash, out IComponent component)
        {
            component = default;
            return s_TrackedMaps.TryGetValue(componentTypeHash, out var componentMap) && componentMap.ComponentProvider_Internal(world, entity, out component);
        }

        public static IComponent GetOrProduseComponent<TComponent>(in int world, in int entity) where TComponent : struct, IComponent =>
            TryGetComponent<TComponent>(world, entity, out var compoenent) ? compoenent : ProduseComponennt<TComponent>();

        public static bool CheckCollision(in int world, in int entity, HashSet<int> colliectionToCheck) =>
            s_Entities.TryGetComponenents(world, entity, out var components) && components.Overlaps(colliectionToCheck);

        public static bool CheckSubsetCollision(in int world, in int entity, HashSet<int> colliectionToCheck)
        {
            if (!s_Entities.TryGetComponenents(world, entity, out var componenetns))
                return false;

            foreach (var element in colliectionToCheck)
                if (!componenetns.Contains(element))
                    return false;

            return true;
        }

        #region GENECTIC CHILD ACCESS
        protected abstract void ClearForWorld_Internal(in int world);
        protected abstract void ClearForEntity_Internal(in int world, in int entity);
        protected abstract IEnumerable<KeyValuePair<int, IComponent>> CompoenentsProvider_Internal(int world);
        protected abstract bool ComponentProvider_Internal(in int world, in int entity, out IComponent component);
        #endregion

        public static IComponent ProduseComponennt<TComponent>() where TComponent : struct, IComponent =>
            Activator.CreateInstance<TComponent>();

        public static IComponent ProduseComponennt(in int typeHash)
        {
            if (!ComponentTypeUtility.TryGetType(typeHash, out var type))
                throw new InvalidOperationException($"'{typeHash}' is not a valid component type");

            return ProduseComponent(type);
        }

        public static IComponent ProduseComponent(in Type type)
        {
            if (!type.IsComponenentType())
                throw new InvalidOperationException($"'{type.FullName}' is not a componenent type");

            return (IComponent)Activator.CreateInstance(type);
        }

        public static Type OfType<TComponent>() where TComponent : struct, IComponent =>
            OfType(typeof(TComponent));

        public static Type OfType(in Type componentType) =>
            typeof(ComponentMap<>)
                .MakeGenericType(componentType);
    }
}

