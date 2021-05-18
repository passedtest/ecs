using System.Collections.Generic;

namespace ECS
{
    public static class Single<TComponent> where TComponent : struct, IComponent
    {
        public delegate void TProcessorRef(ref TComponent component);

        public static bool TryGet(int world, out TComponent component) =>
            Core.ComponentMap<TComponent>.TryGet(world, 0, out component);

        public static bool TryAddOrSet(int world, TComponent component) =>
            Core.ComponentMap<TComponent>.TryAddOrSet(world, 0, component);

        public static void Change(int world, TProcessorRef processor)
        {
            Core.ComponentMap<TComponent>.TryGet(world, 0, out var component);
            processor(ref component);
            Core.ComponentMap<TComponent>.TryAddOrSet(world, 0, component);
        }
    }
}

namespace ECS.Core
{
    sealed class ComponentMap<TComponent> : SharedComponentMap where TComponent : struct, IComponent
    {
        static readonly TypeDefinition s_Type;
        static readonly Maps.ComponentsByWorld<TComponent> s_Components;

        ComponentMap(TypeDefinition type) : base(type) { }

        static ComponentMap()
        {
            s_Type = new TypeDefinition(typeof(TComponent));
            s_Components = new Maps.ComponentsByWorld<TComponent>();

            RegisterMap(new ComponentMap<TComponent>(s_Type));
        }

        public static IUnsafeComponentCollection<TComponent> ForWorldByEntityOpened(in int world) => GetWorldComponents(world);
        public static IComponentCollection<TComponent> ForWorldByEntity(in int world) => GetWorldComponents(world);
        public static IReadOnlyComponentCollection<TComponent> ForWorldByEntityReadOnly(in int world) => GetWorldComponents(world);

        static Maps.ComponentsByEntity<TComponent> GetWorldComponents(in int world) => s_Components.ForWorld(world);

        #region MODIFICATIONS
        public static bool TryAdd(in int world, in int entity, in TComponent component) =>
            TryRegisterComponentType(world, entity, s_Type.HashCode) && s_Components.TryAddOrSetComponent(world, entity, component);

        public static bool TryAddOrSet(in int world, in int entity, in TComponent component) =>
            TryRegisterComponentType(world, entity, s_Type.HashCode) & s_Components.TryAddOrSetComponent(world, entity, component);

        public static bool Remove(in int world, in int entity) =>
            TryRemoveComponentType(world, entity, s_Type.HashCode) && s_Components.RemoveComponent(world, entity);
        #endregion

        public static bool TryGet(in int world, in int entity, out TComponent component) =>
            s_Components.TryGetComponent(world, entity, out component);

        protected override void ClearForEntity_Internal(in int world, in int entity) =>
            s_Components.ClearForEntity(world, entity);

        protected override void ClearForWorld_Internal(in int world) =>
            s_Components.ClearForWorld(world);

        protected override IEnumerable<KeyValuePair<int, IComponent>> CompoenentsProvider_Internal(int world)
        {
            foreach (var kvp in GetWorldComponents(world).GetInterfaceEnumerator())
                yield return kvp;
        }

        protected override bool ComponentProvider_Internal(in int world, in int entity, out IComponent component)
        {
            component = default;

            var componentExists = s_Components.TryGetComponent(world, entity, out var actualComponent);
            if (componentExists)
                component = actualComponent;

            return componentExists;
        }
    }
}