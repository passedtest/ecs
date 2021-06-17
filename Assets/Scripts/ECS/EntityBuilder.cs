using ECS.Collections;
using ECS.Core;
using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// Allocates new entity;
    /// </summary>
    public struct EntityBuilder
    {
        readonly int m_WorldUID;
        readonly int m_EntityUID;

        EntityBuilder(ECSWorld world)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            m_WorldUID = world.UID;
            m_EntityUID = world.EntityAllocator.AllocateUID;
        }

        public void AddComponenet<TComponent>(TComponent component) where TComponent : struct, IComponent =>
            ComponentMap<TComponent>.TryAdd(m_WorldUID, m_EntityUID, component);

        public static EntityBuilder New(ECSWorld world) => new EntityBuilder(world);
    }

    /// <summary>
    /// Allocates new entity;
    /// </summary>
    public struct EntityLazyBuilder : IDisposable
    {
        readonly int m_WorldUID;
        readonly int m_EntityUID;

        PooledList<IComponent> m_Components;

        bool IsValid => m_Components != null;

        EntityLazyBuilder(ECSWorld world)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            m_WorldUID = world.UID;
            m_EntityUID = world.EntityAllocator.AllocateUID;
            m_Components = PooledList<IComponent>.Provide();
        }

        public EntityLazyBuilder AddComponents(IEnumerable<IComponent> componenetns)
        {
            if (!IsValid)
                throw new InvalidOperationException($"Entity '{m_EntityUID}' for world '{m_WorldUID}' is invalid");

            m_Components.AddRange(componenetns);
            return this;
        }

        public EntityLazyBuilder AddComponent(IComponent component)
        {
            if (!IsValid)
                throw new InvalidOperationException($"Entity '{m_EntityUID}' for world '{m_WorldUID}' is invalid");

            m_Components.Add(component);
            return this;
        }

        public void BuildNow()
        {
            if (!IsValid)
                throw new InvalidOperationException($"Entity '{m_EntityUID}' for world '{m_WorldUID}' is invalid");

            if (m_Components.Count == 0)
                throw new InvalidOperationException($"Unable to build entity with zero components");

            foreach (var c in m_Components)
                c.Register(m_WorldUID, m_EntityUID);

            Dispose();
        }

        public void BuildViaCommandBuffer(in int world, in int entity, ICommandBufferConcatenator buffer)
        {
            if (!IsValid)
                throw new InvalidOperationException($"Entity '{m_EntityUID}' for world '{m_WorldUID}' is invalid");

            foreach (var component in m_Components)
                buffer.TryAddOrSetComponent(world, entity, component);

            Dispose();
        }

        public void Dispose()
        {
            m_Components.Free();
            m_Components = null;
        }

        public static EntityLazyBuilder New(ECSWorld world) => new EntityLazyBuilder(world);

        public static EntityLazyBuilder operator +(EntityLazyBuilder builder, IComponent component)
        {
            if (!builder.IsValid)
                throw new InvalidOperationException($"Invalid builder");

            return builder.AddComponent(component);
        }
    }
}
